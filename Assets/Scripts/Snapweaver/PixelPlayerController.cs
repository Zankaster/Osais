using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snapweaver {
    public class PixelPlayerController : MonoBehaviour {
        [Range(2, 32)]
        public int verticalRays, horizontalRays;
        public int verticalSpacing, horizontalSpacing;
        public int[] verticalRaysPositions, horizontalRaysPositions;
        public int rightRaysXposition, leftRaysXposition, downRaysYposition, upRaysYposition;
        public PlayerStats player;
        public SoundList playerSounds;
        Dictionary<string, AudioClip> playerSoundFx = new Dictionary<string, AudioClip>();
        AudioSource audioSource;
        Map2D map;
        Animator playerAnimator;
        SpriteRenderer playerSprite;

        void Start() {
            player.xPosition = transform.position.x;
            player.yPosition = transform.position.y;
            player.xVelocity = 0f;
            player.yVelocity = 0f;
            map = FindObjectOfType<PixelMapGenerator>().gameMap;
            playerAnimator = GetComponent<Animator>();
            playerSprite = GetComponent<SpriteRenderer>();
            audioSource = GetComponent<AudioSource>();
            SoundsToDictionary();
        }

        void Update() {

            if (GameManager.Instance.inputLocked)
                return;

            //Input
            Vector2 input;
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            input.Normalize();

            if (Input.GetButtonDown("Jump")) {
                if (player.grounded) {
                    player.yVelocity = player.jumpSpeed;
                    player.grounded = false;
                    PlaySound("Jump");
                }
            }
            else {
                player.yVelocity -= player.gravity * Time.deltaTime;
            }

            player.xVelocity += (player.grounded ? (player.groundAcceleration -player.deceleration) : (player.airAcceleration - player.airDeceleration)) * input.x * Time.deltaTime;

            /*if (Mathf.Abs(player.xVelocity) < 0.01f)
                player.xVelocity = 0f;*/

            if (input.x == 0) {
                player.xVelocitySign = player.xVelocity >= 0 ? 1 : -1;
                player.xVelocity += (player.grounded ? -player.deceleration : -player.airDeceleration) * player.xVelocity * Time.deltaTime;
                if (Mathf.Abs(player.xVelocity) < 0.01f || Mathf.Sign(player.xVelocity) != player.xVelocitySign)
                    player.xVelocity = 0.0f;
            }
            else {
                player.xInputSign = input.x >= 0 ? 1 : -1;
                playerSprite.flipX = player.xInputSign == -1;
                //??? Bug velocity slide...
            }

            player.xVelocity = Mathf.Clamp(player.xVelocity, -player.maxSpeed, player.maxSpeed);
            player.yVelocity = Mathf.Clamp(player.yVelocity, -player.gravity, player.jumpSpeed);

            player.xNewPosition = player.xPosition + player.xVelocity * Time.deltaTime;
            player.yNewPosition = player.yPosition + player.yVelocity * Time.deltaTime;

            if (player.groundedLastFrame) {
                player.groundedLastFrame = false;
            }
            else {
                player.grounded = false;
            }

            //Horizontal Collisions
            if (player.xVelocity != 0) {
                int[] raycasts = new int[horizontalRays];
                for (int i = 0; i < horizontalRaysPositions.Length; i++)
                    raycasts[i] = Pixel2DPhysics.Instance.Raycast(new Vector2Int(player.xIntPosition + (player.xVelocity > 0 ? rightRaysXposition : leftRaysXposition), player.yIntPosition + horizontalRaysPositions[i]), player.xVelocity > 0 ? RaycastDirection.right : RaycastDirection.left, Mathf.Abs(player.xIntPosition - player.xIntNewPosition), true);

                var min = int.MaxValue;
                for (int i = 0; i < raycasts.Length; i++)
                    if (raycasts[i] != -1 && raycasts[i] < min)
                        min = raycasts[i];

                if (min != int.MaxValue) {
                    player.xNewPosition = player.xIntPosition + Mathf.Sign(player.xVelocity) * min - Mathf.Sign(player.xVelocity) * 1;
                    player.xVelocity = 0;
                }
            }
            else {
                //right
                bool collision = false;
                do {
                    int[] raycasts = new int[horizontalRays];
                    for (int i = 0; i < horizontalRaysPositions.Length; i++)
                        raycasts[i] = Pixel2DPhysics.Instance.Raycast(new Vector2Int(player.xIntNewPosition + rightRaysXposition-1, player.yIntPosition + horizontalRaysPositions[i]), RaycastDirection.right,1, true);

                    var min = int.MaxValue;
                    for (int i = 0; i < raycasts.Length; i++)
                        if (raycasts[i] != -1 && raycasts[i] < min)
                            min = raycasts[i];

                    if (min != int.MaxValue) {
                        collision = true;
                        player.xNewPosition = player.xIntNewPosition -1;
                        player.xVelocity = 0;
                    }
                    else {
                        collision = false;
                    }
                } while (collision);

                //left
                collision = false;
                do {
                    int[] raycasts = new int[horizontalRays];
                    for (int i = 0; i < horizontalRaysPositions.Length; i++)
                        raycasts[i] = Pixel2DPhysics.Instance.Raycast(new Vector2Int(player.xIntNewPosition + leftRaysXposition + 1, player.yIntPosition + horizontalRaysPositions[i]), RaycastDirection.left, 1, true);

                    var min = int.MaxValue;
                    for (int i = 0; i < raycasts.Length; i++)
                        if (raycasts[i] != -1 && raycasts[i] < min)
                            min = raycasts[i];

                    if (min != int.MaxValue) {
                        collision = true;
                        player.xNewPosition = player.xIntNewPosition + 1;
                        player.xVelocity = 0;
                    }
                    else {
                        collision = false;
                    }
                } while (collision);


                /*Debug.Log("Controllo xvel=0");
                int[] raycasts = new int[horizontalRays];
                for (int i = 0; i < horizontalRaysPositions.Length; i++)
                    raycasts[i] = Pixel2DPhysics.Instance.Raycast(new Vector2Int(player.xIntPosition, player.yIntPosition + horizontalRaysPositions[i]), RaycastDirection.right, player.width/2, true);

                var min = int.MaxValue;
                for (int i = 0; i < raycasts.Length; i++)
                    if (raycasts[i] != -1 && raycasts[i] < min)
                        min = raycasts[i];

                if (min != int.MaxValue) {
                    player.xNewPosition = player.xIntPosition +  min -2;
                    player.xVelocity = 0;
                }*/
            }

            //Vertical Collisions
            if (player.yVelocity <= 0) {
                int[] raycasts = new int[verticalRays];
                for (int i = 0; i < verticalRaysPositions.Length; i++)
                    raycasts[i] = Pixel2DPhysics.Instance.Raycast(new Vector2Int(player.xIntNewPosition + verticalRaysPositions[i], player.yIntPosition + downRaysYposition), RaycastDirection.down, player.grounded ? player.groundCheckLength : Mathf.Abs(player.yIntPosition  - player.yIntNewPosition), true);

                var min = int.MaxValue;
                for (int i = 0; i < raycasts.Length; i++)
                    if (raycasts[i] != -1 && raycasts[i] < min)
                        min = raycasts[i];

                if (min != int.MaxValue) {
                    player.yNewPosition = player.yIntPosition - min + 1;
                    player.yVelocity = 0;
                    player.grounded = true;
                    player.groundedLastFrame = true;
                }
            }
            else if (player.yVelocity > 0) {
                int[] raycasts = new int[verticalRays];
                int min = int.MaxValue;
                for (int i = 0; i < verticalRaysPositions.Length; i++) {
                    raycasts[i] = Pixel2DPhysics.Instance.Raycast(new Vector2Int(player.xIntNewPosition + verticalRaysPositions[i], player.yIntPosition + upRaysYposition), RaycastDirection.up, Mathf.Abs(player.yIntPosition - player.yIntNewPosition), true);
                    if (raycasts[i] != -1 && raycasts[i] < min)
                        min = raycasts[i];
                }

                if (min != int.MaxValue) {
                    player.yVelocity = 0;
                    player.yNewPosition = player.yIntPosition - min + 1;
                }
            }

            player.xPosition = player.xNewPosition;
            player.yPosition = player.yNewPosition;

            transform.position = new Vector3(
                player.xPosition,
                player.yPosition,
                transform.position.z
                );

            playerAnimator.SetFloat("xAbsVelocity",  Mathf.Abs( player.xVelocity));
            playerAnimator.SetBool("Jumping", player.yVelocity != 0);
            
        }

        public void SetTakingPhoto(bool takingPhoto) {
            playerAnimator.SetBool("TakingPhoto", takingPhoto);
        }

        private void SoundsToDictionary() {
            foreach(SoundFxClip s in playerSounds.soundList)
                playerSoundFx.Add(s.name, s.sound);
        }

        public void PlaySound(string soundName) {
            audioSource.clip = playerSoundFx[soundName];
            audioSource.Play();
        }

        public void StopSound() {
            //audioSource.clip = playerSoundFx[soundName];
            audioSource.Stop();
        }

        /*void OnValidate() {
            CalculateRays();
        }*/

        /*void CalculateRays() {
            if (verticalRays > player.width)
                verticalRays = player.width;

            if (horizontalRays > player.height)
                horizontalRays = player.height;

            //Trick to round the number of rays to the nearest power of 2
            verticalRays--;
            horizontalRays--;
            for (int i = 1; i <= 16; i *= 2) {
                verticalRays |= verticalRays >> i;
                horizontalRays |= horizontalRays >> i;
            }
            verticalRays++;
            horizontalRays++;

            verticalSpacing = player.width / Mathf.Clamp(verticalRays,1, player.width);
            horizontalSpacing = player.height / Mathf.Clamp(horizontalRays, 1, player.height);

            verticalRaysPositions = new int[verticalRays];
            horizontalRaysPositions = new int[horizontalRays];

            for (int i = 0; i < verticalRays / 2; i++) {
                verticalRaysPositions[i * 2] = -verticalSpacing * (i + 1) + player.height / 2;
                verticalRaysPositions[i * 2 + 1] = verticalSpacing * (i + 1) + player.height / 2 - 1;
            }

            for (int i = 0; i < horizontalRays / 2; i++) {
                horizontalRaysPositions[i * 2] = horizontalSpacing * i +1;
                horizontalRaysPositions[i * 2 + 1] = player.height - horizontalSpacing*i -2 ;
            }

        }*/
    }
}