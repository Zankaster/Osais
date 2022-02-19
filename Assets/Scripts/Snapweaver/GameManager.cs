using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Snapweaver {

    public class GameManager : MonoBehaviour {
        public int endLevelTileIndex = 31;
        public int currentSceneIndex;
        PixelPlayerController playerController;
        public static GameManager Instance;
        public bool inputLocked = false;
        GameObject commandsUI;
        Vector2 commandsUIstartPosition = new Vector2(16, 12);
        Animator fadeAnimator;
        bool exiting = false;
        float exitTime = 0;

        private void Awake() {
            if (GameManager.Instance == null)
                GameManager.Instance = this;
            else
                Destroy(this.gameObject);
        }

        private void Start() {
            if (currentSceneIndex == 7 || currentSceneIndex == 8) {
                inputLocked = true;
                StartCoroutine(FadeOutAndGoToNextSceneIn(currentSceneIndex == 7 ? 8 : 16));
                fadeAnimator = GameObject.Find("Fade").GetComponent<Animator>();
                return;
            }
            /*else if (currentSceneIndex == 8) {
                return;
            }*/
            playerController = FindObjectOfType<PixelPlayerController>();
            commandsUI = GameObject.Find("Commands");
            ShowHideCommands();
            commandsUI.transform.position = commandsUIstartPosition;
            StartCoroutine(CheckEndLevelReached());
        }

        private void Update() {
            if (currentSceneIndex == 8)
                return;

            if (exiting) {
                exitTime += Time.deltaTime;
                if (exitTime >= .5f)
                    exiting = false;
            }


            if (Input.GetButtonDown("Commands")) {
                ShowHideCommands();
            }

            if(Input.GetKeyDown(KeyCode.Escape)) {
                if (!exiting) {
                    exiting = true;
                    exitTime = 0f;
                }
                else {
                    Application.Quit();
                }

            }
        }

        IEnumerator CheckEndLevelReached() {
            while (true) {
                Debug.Log(PixelMapGenerator.Instance.gameMap.GetCurrentTileInGridPosition(
                    playerController.player.xTilePosition,
                    playerController.player.yTilePosition));
                if (PixelMapGenerator.Instance.gameMap.GetCurrentTileInGridPosition(
                    playerController.player.xTilePosition,
                    playerController.player.yTilePosition) == endLevelTileIndex) {
                    SceneManager.LoadScene(currentSceneIndex + 1);
                }
                yield return new WaitForSeconds(.25f);
            }
        }

        IEnumerator FadeOutAndGoToNextSceneIn(int seconds) {
            float journey = 0f;
            while (journey <= seconds) {
                journey = journey + Time.deltaTime;
                if (journey >= seconds - 2)
                    fadeAnimator.SetTrigger("FadeOut");
                yield return null;
            }
            if (currentSceneIndex == 7) {
                SceneManager.LoadScene(currentSceneIndex + 1);
            }
            else {
                Application.Quit();
            }
        }

        public void ResetLevel() {
            SceneManager.LoadScene(currentSceneIndex);
        }

        public void ShowHideCommands() {
            commandsUI.SetActive(!commandsUI.activeInHierarchy);
        }
    }
}
