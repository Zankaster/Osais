using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snapweaver {
    [Serializable]
    public class PixelTile {
        public int tileIndex;
        public Sprite tileSprite;
        public Sprite tileCollider;
        public Color[] tilePixels;
        int size { get => (int)Mathf.Sqrt(tilePixels.Length); }

        public Color GetColorAtPosition(int x, int y) {
            if (x >= 0 && x < size && y >= 0 && y < size)
                return tilePixels[x + (size - 1 - y) * size];
            return Color.white;
        }

    }
}