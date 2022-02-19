using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTile : MonoBehaviour {
    public float groundAcceleration = 20f;
    public float airAcceleration = 20f;
    public float deceleration = 0f;
    public float maxSpeed = 5f;
    public float gravity = 20.0f;
    public float xPosition, yPosition;
    public float xVelocity, yVelocity;
    public bool grounded = false;
    float xVelocitySign;
    public float cellHeight = 1;
    public float jumpSpeed = 12f;
    int xCheck1, yCheck1, xCheck2, yCheck2;
    public float playerWidth = 0.6f, playerHeight = 0.8f;

    private void Start() {
        xPosition = transform.position.x;
        yPosition = transform.position.y;
        xVelocity = 0f;
        yVelocity = 0f;
        //Time.timeScale = 0.1f;
    }

    void Update() {
        //Input
        Vector2 input;
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input.Normalize();

        if (Input.GetButtonDown("Jump")) {
            if (grounded) {
                yVelocity = jumpSpeed;
            }
        }

        xVelocity += (grounded ? groundAcceleration : airAcceleration) * input.x * Time.deltaTime;
        yVelocity -= gravity * Time.deltaTime;

        if (grounded && input.x == 0) {
            xVelocitySign = xVelocity >= 0 ? 1 : -1;
            xVelocity += -deceleration * xVelocity * Time.deltaTime;
            if (Mathf.Abs(xVelocity) < 0.01f)//|| Mathf.Sign(xVelocity) != xVelocitySign
                xVelocity = 0.0f;
        }

        xVelocity = Mathf.Clamp(xVelocity, -maxSpeed, maxSpeed);
        yVelocity = Mathf.Clamp(yVelocity, -100.0f, 100.0f);

        float xNewPosition = xPosition + xVelocity * Time.deltaTime;
        float yNewPosition = yPosition + yVelocity * Time.deltaTime;


        // Check for Collisions
        if (xVelocity != 0)
        {
            xVelocitySign = Mathf.Sign(xVelocity);
            xCheck1 = Mathf.RoundToInt(xNewPosition + xVelocitySign * playerWidth / 2f);
            yCheck1 = Mathf.RoundToInt(yPosition - 0.49f);
            xCheck2 = Mathf.RoundToInt(xNewPosition + xVelocitySign * playerWidth / 2f);
            yCheck2 = Mathf.RoundToInt(yPosition + playerHeight - 0.5f - .1f);
            DrawDebugRectangle(new Vector2(xCheck1, yCheck1), Color.red);
            DrawDebugRectangle(new Vector2(xCheck2, yCheck2), Color.red);
            if (MapGenerator.gameMap.GetTileInWorldPosition(xCheck1, yCheck1) != '.' || MapGenerator.gameMap.GetTileInWorldPosition(xCheck2, yCheck2) != '.') {
                xNewPosition = Mathf.RoundToInt(xNewPosition) + xVelocitySign * (0.5f - playerWidth / 2f);
                xVelocity = 0;
            }
        }

        grounded = false;
        if (yVelocity < 0) // Moving Down
        {
            xCheck1 = Mathf.RoundToInt(xNewPosition - (playerWidth / 2f - 0.01f));
            yCheck1 = Mathf.RoundToInt(yNewPosition - 0.5f);
            xCheck2 = Mathf.RoundToInt(xNewPosition + (playerWidth / 2f - 0.01f));
            yCheck2 = Mathf.RoundToInt(yNewPosition - 0.5f);
            DrawDebugRectangle(new Vector2(xCheck1, yCheck1), Color.blue, .9f);
            DrawDebugRectangle(new Vector2(xCheck2, yCheck2), Color.blue, .9f);
            if (MapGenerator.gameMap.GetTileInWorldPosition(xCheck1, yCheck1) != '.' || MapGenerator.gameMap.GetTileInWorldPosition(xCheck2, yCheck2) != '.') {
                yNewPosition = Mathf.RoundToInt(yNewPosition);
                yVelocity = 0;
                grounded = true;
            }
        }
        else if(yVelocity > 0) // Moving Up
        {
            xCheck1 = Mathf.RoundToInt(xNewPosition - (playerWidth / 2f - 0.01f));
            yCheck1 = Mathf.RoundToInt(yNewPosition + playerHeight-0.5f);
            xCheck2 = Mathf.RoundToInt(xNewPosition + (playerWidth / 2f - 0.01f));
            yCheck2 = Mathf.RoundToInt(yNewPosition + playerHeight - 0.5f);
            DrawDebugRectangle(new Vector2(xCheck1, yCheck1), Color.blue, .9f);
            DrawDebugRectangle(new Vector2(xCheck2, yCheck2), Color.blue, .9f);
            if (MapGenerator.gameMap.GetTileInWorldPosition(xCheck1, yCheck1) != '.' || MapGenerator.gameMap.GetTileInWorldPosition(xCheck2, yCheck2) != '.') {
                yNewPosition = Mathf.RoundToInt(yNewPosition) + 1f-playerHeight;
                yVelocity = 0;
            }
        }

        xPosition = xNewPosition;
        yPosition = yNewPosition;

        transform.position = new Vector3(
            xPosition,
            yPosition,
            transform.position.z
            );
        DrawDebugPlayer();

    }

    private void DrawDebugRectangle(Vector2 position, Color col, float size = 1f) {
        Debug.DrawLine(
            position - Vector2.right * size / 2 - Vector2.up * cellHeight / 2,
            position + Vector2.right * size / 2 - Vector2.up * cellHeight / 2, col);
        Debug.DrawLine(
            position - Vector2.right * size / 2 + Vector2.up * cellHeight / 2,
            position + Vector2.right * size / 2 + Vector2.up * cellHeight / 2, col);
        Debug.DrawLine(
            position - Vector2.right * size / 2 - Vector2.up * cellHeight / 2,
            position - Vector2.right * size / 2 + Vector2.up * cellHeight / 2, col);
        Debug.DrawLine(
            position + Vector2.right * size / 2 - Vector2.up * cellHeight / 2,
            position + Vector2.right * size / 2 + Vector2.up * cellHeight / 2, col);
    }

    private void DrawDebugPlayer() {
        Debug.DrawLine(
            new Vector2(xPosition - playerWidth / 2f, yPosition - 0.5f),
            new Vector2(xPosition + playerWidth / 2f, yPosition - 0.5f), Color.green);
        Debug.DrawLine(
            new Vector2(xPosition - playerWidth / 2f, yPosition + playerHeight - 0.5f),
            new Vector2(xPosition + playerWidth / 2f, yPosition + playerHeight - 0.5f), Color.green);
        Debug.DrawLine(
            new Vector2(xPosition - playerWidth / 2f, yPosition - 0.5f),
            new Vector2(xPosition - playerWidth / 2f, yPosition + playerHeight - 0.5f), Color.green);
        Debug.DrawLine(
            new Vector2(xPosition + playerWidth / 2f, yPosition - 0.5f),
            new Vector2(xPosition + playerWidth / 2f, yPosition + playerHeight - 0.5f), Color.green);
    }
}
