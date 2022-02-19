using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerControllerPixel : MonoBehaviour {
    public int downRays, upRays, lateralRays;
    public PlayerStats player;
    Map2D map;

    void Start() {
        player.xPosition = transform.position.x;
        player.yPosition = transform.position.y;
        player.xVelocity = 0f;
        player.yVelocity = 0f;
        map = FindObjectOfType<PixelMapGenerator>().gameMap;
    }

    void Update() {

        //Input
        Vector2 input;
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input.Normalize();

        if (Input.GetButtonDown("Jump")) {
            if (player.grounded) {
                player.yVelocity = player.jumpSpeed;
                player.grounded = false;
            }
        }
        else {
            player.yVelocity -= player.gravity * Time.deltaTime;
        }

        player.xVelocity += (player.grounded ? player.groundAcceleration : player.airAcceleration) * input.x * Time.deltaTime;


        if (player.grounded && input.x == 0) {
            player.xVelocitySign = player.xVelocity >= 0 ? 1 : -1;
            player.xVelocity += -player.deceleration * player.xVelocity * Time.deltaTime;
            if (Mathf.Abs(player.xVelocity) < 0.01f|| Mathf.Sign(player.xVelocity) != player.xVelocitySign)
                player.xVelocity = 0.0f;
        }

        player.xVelocity = Mathf.Clamp(player.xVelocity, -player.maxSpeed, player.maxSpeed);
        player.yVelocity = Mathf.Clamp(player.yVelocity, -player.gravity, 100.0f);

        player.xNewPosition = player.xPosition + player.xVelocity * Time.deltaTime;
        player.yNewPosition = player.yPosition + player.yVelocity * Time.deltaTime;

        //Horizontal Collisions
        if (player.xVelocity != 0) {
            var g = Controller2DPhysics.Instance.Raycast(new Vector2Int(player.xIntPosition + (player.xVelocity > 0 ? map.cellSize -1 : 0), player.yIntPosition+3), player.xVelocity > 0 ? RaycastDirection.right : RaycastDirection.left, Mathf.Abs(player.xIntPosition - player.xIntNewPosition), true);
            if (g != -1) {
                player.xNewPosition = player.xIntPosition + Mathf.Sign(player.xVelocity)* g- Mathf.Sign(player.xVelocity) * 1;
                player.xVelocity = 0;
            }
            else if(player.groundedLastFrame) {
                player.groundedLastFrame = false;
            }
            else {
                player.grounded = false;
            }
        }

        //Vertical Collisions
        if (player.yVelocity <= 0) {
            int[] raycasts = new int[4];
            raycasts[0] = Controller2DPhysics.Instance.Raycast(new Vector2Int(player.xIntNewPosition +  0, player.yIntPosition), RaycastDirection.down, player.grounded ? 3 : Mathf.RoundToInt(-player.yVelocity * Time.deltaTime), true);
            raycasts[1] = Controller2DPhysics.Instance.Raycast(new Vector2Int(player.xIntNewPosition + map.cellSize - 1, player.yIntPosition), RaycastDirection.down, player.grounded ? 3 : Mathf.RoundToInt(-player.yVelocity * Time.deltaTime), true);
            raycasts[2] = Controller2DPhysics.Instance.Raycast(new Vector2Int(player.xIntNewPosition + 3, player.yIntPosition), RaycastDirection.down, player.grounded ? 3 : Mathf.RoundToInt(-player.yVelocity * Time.deltaTime), true);
            raycasts[3] = Controller2DPhysics.Instance.Raycast(new Vector2Int(player.xIntNewPosition + 4, player.yIntPosition), RaycastDirection.down, player.grounded ? 3 : Mathf.RoundToInt(-player.yVelocity * Time.deltaTime), true);
            var min = int.MaxValue;
            for(int i = 0; i < raycasts.Length; i++)
                if (raycasts[i] != -1 && raycasts[i] < min)
                    min = raycasts[i];
            if(min != int.MaxValue) { 
                player.yNewPosition = player.yIntPosition - min + 1;
                player.yVelocity = 0;
                player.grounded = true;
                player.groundedLastFrame = true;
            }
        }
        else if(player.yVelocity > 0) {
            var g1 = Controller2DPhysics.Instance.Raycast(new Vector2Int(player.xIntNewPosition, player.yIntPosition + map.cellSize-1), RaycastDirection.up, Mathf.RoundToInt(player.yVelocity * Time.deltaTime), true);
            var g2 = Controller2DPhysics.Instance.Raycast(new Vector2Int(player.xIntNewPosition + map.cellSize - 1, player.yIntPosition + map.cellSize-1), RaycastDirection.up, Mathf.RoundToInt(player.yVelocity * Time.deltaTime), true);
            if (g1 != -1 || g2 != -1) {
                int min;
                if (g1 == -1)
                    min = g2;
                else if (g2 == -1)
                    min = g1;
                else
                    min = Mathf.Min(g1, g2);
                player.yNewPosition = player.yIntPosition - min + 1;
                player.yVelocity = 0;
            }
        }

        player.xPosition = player.xNewPosition;
        player.yPosition = player.yNewPosition;

        transform.position = new Vector3(
            player.xIntPosition,
            player.yIntPosition,
            transform.position.z
            );

    }
}
