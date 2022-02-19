using Snapweaver;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

namespace Snapweaver {
    public class SnapshotController : MonoBehaviour {

        GameObject frameGameObject;
        GameObject focusGameObject;
        GameObject pictureGameObject;
        GameObject blurPanel;
        Material blurMaterial;
        Animator focusAnimator;
        public int maxPhotos = 1;
        public bool mouseControl = false;
        private int currentPhotos = 0;
        public float focusTime;
        public float elapsedTime;
        public Vector2Int snapshotSize = new Vector2Int(3, 3);
        PixelPlayerController player;
        bool snapShotCaptured = false;
        bool focused = false;
        Vector2Int snapshotPosition;
        SpriteRenderer[,] tilesPhantom;
        public AnimationCurve blurFocusCurve;
        bool justSnappedShot = false;
        SnapshotCounter snapshotCounter;
        public Vector2 mousePosition;
        public Vector2Int mouseSnapshotPosition;

        private void Start() {
            player = FindObjectOfType<PixelPlayerController>();
            frameGameObject = GameObject.Find("Frame").gameObject;
            focusGameObject = frameGameObject.transform.Find("Focus").gameObject;
            focusAnimator = focusGameObject.GetComponent<Animator>();
            pictureGameObject = frameGameObject.transform.Find("Picture").gameObject;
            tilesPhantom = new SpriteRenderer[snapshotSize.x, snapshotSize.y];
            blurPanel = GameObject.Find("Blur");
            blurMaterial = blurPanel.GetComponent<Image>().material;
            snapshotCounter = FindObjectOfType<SnapshotCounter>();
            snapshotCounter.SetCounter(maxPhotos-currentPhotos);
        }

        private void Update() {
            
            if (GameManager.Instance.inputLocked)
                return;

            if (Input.GetButtonDown("ResetLevel")) {
                Debug.Log("ResetLevel");
                GameManager.Instance.ResetLevel();
                return;
            }

            if (Input.GetButton("Frame") && Input.GetButtonDown("Snap")) {
                if (focused) {
                    if (snapShotCaptured) {
                        if (currentPhotos >= maxPhotos) {
                            //click sound, no photos can be taken
                            player.PlaySound("NoPhotos");
                            return;
                        }
                        player.PlaySound("Snapshot");
                        justSnappedShot = true;
                        snapShotCaptured = true;
                        elapsedTime = 0;
                        focused = false;

                        //hide frame
                        SetFrameVisible(false);

                        SwapTiles();
                        //reset captured frame
                        snapShotCaptured = false;
                        ClearPhantomShot();
                        currentPhotos++;
                        player.SetTakingPhoto(false);
                        snapshotCounter.SetCounter(maxPhotos - currentPhotos);
                    }
                    else {

                        if (currentPhotos >= maxPhotos) {
                            //click sound
                            player.PlaySound("NoPhotos");
                            return;
                        }

                        Debug.Log("Snapshot!");
                        player.PlaySound("Snapshot");

                        justSnappedShot = true;
                        //capture frame
                        snapShotCaptured = true;
                        elapsedTime = 0;
                        focused = false;

                        //hide frame
                        SetFrameVisible(false);
                        focusAnimator.SetBool("Focusing", false);
                        player.SetTakingPhoto(false);
                        CaptureShot();
                        GeneratePhantomShot();
                        
                    }
                    
                }
            }
            else if (Input.GetButton("ResetPhoto") && snapShotCaptured) {
                Debug.Log("Reset");
                player.PlaySound("ResetPhoto");
                //reset captured frame
                snapShotCaptured = false;
                ClearPhantomShot();

            }
            else if (Input.GetButtonDown("Frame")) {
                player.SetTakingPhoto(true);
                focused = false;
                elapsedTime = 0f;
                SetFrameToPlayerPosition();
                //show frame
                SetFrameVisible(true);
                focusAnimator.SetBool("Focusing", true);
                justSnappedShot = false;
            }
            else if (Input.GetButtonUp("Frame")) {
                focused = false;
                elapsedTime = 0;
                justSnappedShot = false;

                //hide frame
                SetFrameVisible(false);
                focusAnimator.SetBool("Focusing", false);
                player.SetTakingPhoto(false);
            }
            else if (Input.GetButton("Frame")) {

                if (justSnappedShot) 
                    return;
                
                if(snapShotCaptured)
                    if(CheckPhotoOverlaps(snapshotPosition, GetPhotoPosition(), snapshotSize)) {
                        elapsedTime = 0f;
                        focused = false;
                    }

                if (player.player.xVelocity != 0 || player.player.yVelocity != 0) {
                    elapsedTime = 0;
                    focusAnimator.SetBool("Focusing", false);
                    if (focused) {
                        focused = false;
                        player.SetTakingPhoto(false);
                    }
                }
                else {
                    if (elapsedTime == 0)
                        player.PlaySound("Focus");
                    elapsedTime += Time.deltaTime;
                    player.SetTakingPhoto(true);
                    focusAnimator.SetBool("Focusing", true);
                }
                Debug.Log("Framing");
                if (elapsedTime >= focusTime && !focused) {
                    focused = true;
                    player.PlaySound("Focused");
                }
                SetFrameToPlayerPosition();
                SetBlurValue(elapsedTime);
            }
        }

        private void SetFrameVisible(bool visible) {
            //set frame visibility
            if (frameGameObject.activeInHierarchy != visible)
                frameGameObject.SetActive(visible);
            if (blurPanel.activeInHierarchy != visible)
                blurPanel.SetActive(visible);
        }

        private void SetFocusVisible(bool visible) {
            //set focus visibility
            if (focusGameObject.activeInHierarchy != visible)
                focusGameObject.SetActive(visible);
            
        }

        private void SetBlurValue(float value) {
            value = blurFocusCurve.Evaluate(value / focusTime);
            blurMaterial.SetFloat("_BlurVal", value);
        }

        private void SetFrameToPlayerPosition() {
            if (mouseControl) {
                var mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                var tmp = Camera.main.ScreenToWorldPoint(mousePos);
                var mouseSnapPos = new Vector2Int((int)tmp.x/ PixelMapGenerator.Instance.gameMap.cellSize, (int)tmp.y / PixelMapGenerator.Instance.gameMap.cellSize)* PixelMapGenerator.Instance.gameMap.cellSize - Vector2Int.one*PixelMapGenerator.Instance.cellSize;
                Vector3 pos = new Vector3(Mathf.Clamp(mouseSnapPos.x, PixelMapGenerator.Instance.gameMap.minXboundPixels, PixelMapGenerator.Instance.gameMap.maxXboundPixels - PixelMapGenerator.Instance.gameMap.cellSize * (snapshotSize.x - 1)), Mathf.Clamp(mouseSnapPos.y + PixelMapGenerator.Instance.gameMap.cellSize, PixelMapGenerator.Instance.gameMap.minYboundPixels, PixelMapGenerator.Instance.gameMap.maxYboundPixels - PixelMapGenerator.Instance.gameMap.cellSize * (snapshotSize.y - 1)), 0);
                frameGameObject.transform.position = pos;
                blurPanel.transform.position = pos;
            }
            else {
                Vector3 pos = new Vector3(Mathf.Clamp(player.player.xTileRoundedPosition, PixelMapGenerator.Instance.gameMap.minXboundPixels, PixelMapGenerator.Instance.gameMap.maxXboundPixels - PixelMapGenerator.Instance.gameMap.cellSize * (snapshotSize.x - 1)), Mathf.Clamp(player.player.yTileRoundedPosition + PixelMapGenerator.Instance.gameMap.cellSize, PixelMapGenerator.Instance.gameMap.minYboundPixels, PixelMapGenerator.Instance.gameMap.maxYboundPixels - PixelMapGenerator.Instance.gameMap.cellSize * (snapshotSize.y - 1)), 0);
                frameGameObject.transform.position = pos;
                blurPanel.transform.position = pos;
            }
        }

        private void CaptureShot() {
            snapshotPosition = GetPhotoPosition();
        }

        private Vector2Int GetPhotoPosition() {
            if (mouseControl) {
                var mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                var tmp = Camera.main.ScreenToWorldPoint(mousePos);
                var mouseSnapPos = new Vector2Int((int)tmp.x, (int)tmp.y)/PixelMapGenerator.Instance.gameMap.cellSize;
                return new Vector2Int(
                    Mathf.Clamp((int)mouseSnapPos.x - 1, PixelMapGenerator.Instance.gameMap.minXboundTile, PixelMapGenerator.Instance.gameMap.maxXboundTile - (snapshotSize.x - 1)),
                    Mathf.Clamp((int)mouseSnapPos.y, PixelMapGenerator.Instance.gameMap.minYboundTile, PixelMapGenerator.Instance.gameMap.maxYboundTile - (snapshotSize.y - 1)));
            }
            else {

                return new Vector2Int(
                    Mathf.Clamp(player.player.xTilePosition - 1, PixelMapGenerator.Instance.gameMap.minXboundTile, PixelMapGenerator.Instance.gameMap.maxXboundTile - (snapshotSize.x - 1)),
                    Mathf.Clamp(player.player.yTilePosition, PixelMapGenerator.Instance.gameMap.minYboundTile, PixelMapGenerator.Instance.gameMap.maxYboundTile - (snapshotSize.y - 1)));
            }
        }


        private void GeneratePhantomShot() {
            ClearPhantomShot();

            for (int y = 0; y < snapshotSize.y; y++) {
                for (int x = 0; x < snapshotSize.x; x++) {
                    GameObject g = new GameObject();
                    g.transform.parent = pictureGameObject.transform;
                    g.transform.localPosition = new Vector2(x * PixelMapGenerator.Instance.gameMap.cellSize, y * PixelMapGenerator.Instance.gameMap.cellSize);
                    SpriteRenderer s = g.AddComponent<SpriteRenderer>();
                    s.sortingOrder = 55;
                    s.sprite = PixelMapGenerator.Instance.gameMap.tileDictionary[PixelMapGenerator.Instance.gameMap.GetCurrentTileInGridPosition(snapshotPosition.x + x, snapshotPosition.y + y)].tileSprite;
                    tilesPhantom[x, y] = s;
                }
            }
        }

        private void ClearPhantomShot() {
            foreach (Transform t in pictureGameObject.transform)
                Destroy(t.gameObject);
        }


        private void SwapTiles() {
            Debug.Log(snapshotPosition + " --> " + new Vector2Int(player.player.xTilePosition-1, player.player.yTilePosition));
            PixelMapGenerator.Instance.ApplyPhotoSwap(
                snapshotPosition, 
                GetPhotoPosition(), 
                snapshotSize);
        }


        private bool CheckPhotoOverlaps(Vector2Int photoOrigin, Vector2Int photoDestination, Vector2Int photoSize) {
            bool overlaps = false;
            for (int y = 0; y < photoSize.y; y++) {
                for (int x = 0; x < photoSize.x; x++) {
                    if (PixelMapGenerator.Instance.gameMap.GetCurrentTileInGridPosition(photoDestination.x + x, photoDestination.y + y) != 0 &&
                        PixelMapGenerator.Instance.gameMap.GetCurrentTileInGridPosition(photoOrigin.x + x, photoOrigin.y + y) != 0) {
                        tilesPhantom[x, y].color = Color.red;
                        overlaps = true;
                    }
                    else if (PixelMapGenerator.Instance.gameMap.GetCurrentTileInGridPosition(photoOrigin.x + x, photoOrigin.y + y) != 0 &&
                        (player.player.xTilePosition == photoDestination.x + x && player.player.yTilePosition == photoDestination.y + y)) {
                        tilesPhantom[x, y].color = Color.red;
                        overlaps = true;
                    }
                    else {
                        tilesPhantom[x, y].color = Color.white;
                    }
                }
            }
            return overlaps;
        }
    }
}