using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PixelTile
{
    public char tileSymbol;
    public GameObject tilePrefab;
    public Color[] tilePixels;
    int size { get => (int)Mathf.Sqrt(tilePixels.Length); }

    public Color GetColorAtPosition(int x, int y) {
        if (x >= 0 && x < size && y >= 0 && y < size)
            return tilePixels[x + (size - 1 - y) * size];
        return Color.white;
    }
}
