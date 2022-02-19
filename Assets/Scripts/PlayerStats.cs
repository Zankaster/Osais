using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    public float groundAcceleration = 20f;
    public float airAcceleration = 20f;
    public float deceleration = 10f;
    public float maxSpeed = 5f;
    public float gravity = 20.0f;
    public float xPosition, yPosition;
    public float xNewPosition, yNewPosition;
    public int xIntPosition { get => Mathf.RoundToInt(xPosition); }
    public int yIntPosition { get => Mathf.RoundToInt(yPosition); }
    public int xIntNewPosition { get => Mathf.RoundToInt(xNewPosition); }
    public int yIntNewPosition { get => Mathf.RoundToInt(yNewPosition); }
    public float xVelocity, yVelocity;
    public bool grounded = false;
    public bool groundedLastFrame;
    public float xVelocitySign;
    public float cellHeight = 1;
    public float jumpSpeed = 12f;
    public int xCheck1, yCheck1, xCheck2, yCheck2;
    public float playerWidth = 0.6f, playerHeight = 0.8f;
}
