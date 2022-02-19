using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snapweaver {
    [Serializable]
    public class PlayerStats {
        public int height = 8, width = 8;
        public int characterHorizontalOffset = 4;
        public float groundAcceleration = 300f;
        public float airAcceleration = 200f;
        public float deceleration = 200f;
        public float airDeceleration = 200f;
        public float maxSpeed = 80f;
        public float gravity = 300.0f;
        public float xPosition, yPosition;
        public float xNewPosition, yNewPosition;
        public int groundCheckLength = 3;
        public int xIntPosition { get => Mathf.RoundToInt(xPosition); }
        public int yIntPosition { get => Mathf.RoundToInt(yPosition); }
        public int xIntNewPosition { get => Mathf.RoundToInt(xNewPosition); }
        public int yIntNewPosition { get => Mathf.RoundToInt(yNewPosition); }
        public int xTileRoundedPosition { get => Mathf.RoundToInt((xIntPosition + characterHorizontalOffset - width/2)/width)*width; }
        public int yTileRoundedPosition { get => Mathf.RoundToInt((yIntPosition - height/2)/height)*height; }
        public int xTilePosition { get => (xIntPosition + characterHorizontalOffset - width / 2) / width +1; }
        public int yTilePosition { get => (yIntPosition - height / 2) / height +1; }
        public float xVelocity, yVelocity;
        public bool grounded = false;
        public bool groundedLastFrame;
        public float xVelocitySign;
        public float xInputSign;
        public float jumpSpeed = 120f;
    }
}