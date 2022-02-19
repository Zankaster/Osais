using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class Map2D {
    //The origin y corresponds to the last row y
    public Vector2 origin;
    public int mapWidth;
    public int mapHeight;
    public int cellSize;
    public int pixelWidth { get => mapWidth * cellSize; }
    public int pixelHeight { get => mapHeight * cellSize; }
    public string map;
    [SerializeField]
    public Color[,] colorMap;

    public Map2D() {}

    public Map2D(string map, int mapWidth, int mapHeight, int cellSize) {
        this.map = map;
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        this.cellSize = cellSize;
    }

    public char GetTileInGridPosition(int x, int y) {
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
            return map[x + (mapHeight - 1 - y) * mapWidth];
        return '.';
    }

    public char GetTileInGridPosition(int index) {
        if (index >= 0 && index < mapWidth * mapHeight)
            return map[index];
        return '.';
    }

    public char GetTileInWorldPosition(int x, int y) {
        x -= (int)origin.x;
        y -= (int)origin.y;
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
            return map[x + (mapHeight - 1 - y) * mapWidth];
        return '.';
    }

    public void SetColorInGridPosition(int x, int y, Color[] pixelColors) {
        int size = (int)Mathf.Sqrt(pixelColors.Length);
        for(int i = 0; i < size; i++) {
            for(int j = 0; j < size; j++) {
                colorMap[x*size + j, y*size + i] = pixelColors[j + i*size];
            }
        }
    }

    public Color GetPixelColorInGridPosition(int x, int y) {
        return colorMap[x, y];
    }
}

public enum RaycastDirection {
    right,
    left,
    up, 
    down
}