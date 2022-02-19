using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snapweaver {
    [Serializable]
    public class Map2D {
        //The origin y corresponds to the last row y
        public Vector2 origin;
        public int mapWidth;
        public int mapHeight;
        public int cellSize;
        public int pixelWidth { get => mapWidth * cellSize; }
        public int pixelHeight { get => mapHeight * cellSize; }

        public int minXboundTile { get => 1; }
        public int minYboundTile { get => 1; }
        public int maxXboundTile { get => mapWidth-2; }
        public int maxYboundTile { get => mapHeight-2; }
        public int minXboundPixels { get => cellSize; }
        public int minYboundPixels { get => cellSize; }
        public int maxXboundPixels { get => (mapWidth-2)*cellSize; }
        public int maxYboundPixels { get => (mapHeight-2)*cellSize; }
        public List<int> baseMap = new List<int>();
        public List<int> currentMap = new List<int>();
        public List<int> tempMap = new List<int>();
        public List<int> backgroundMap = new List<int>();
        public Dictionary<int, PixelTile> tileDictionary = new Dictionary<int, PixelTile>();


        public Map2D() { }

        public Map2D(List<int> mapIntForeground, List<int> mapIntBackground, int mapWidth, int mapHeight, int cellSize, Dictionary<int, PixelTile> tileDictionary) {
            this.baseMap = mapIntForeground.ToList();
            this.backgroundMap = mapIntBackground.ToList();
            this.currentMap = mapIntForeground.ToList();
            this.tempMap = mapIntForeground.ToList();
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.cellSize = cellSize;
            this.tileDictionary = tileDictionary;
        }

        public int GetCurrentTileInGridPosition(int x, int y) {
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                return currentMap[x + (mapHeight - 1 - y) * mapWidth];
            return -1;
        }

        public int GetBaseTileInGridPosition(int x, int y) {
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                return baseMap[x + (mapHeight - 1 - y) * mapWidth];
            return -1;
        }

        public int GetBackgroundTileInGridPosition(int x, int y) {
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                return backgroundMap[x + (mapHeight - 1 - y) * mapWidth];
            return -1;
        }

        public int GetTempTileInGridPosition(int x, int y) {
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                return tempMap[x + (mapHeight - 1 - y) * mapWidth];
            return -1;
        }

        public int GetCurrentTileInGridPosition(int index) {
            if (index >= 0 && index < mapWidth * mapHeight)
                return currentMap[index];
            return -1;
        }

        public void SetCurrentTileInGridPosition(int x, int y, int value) {
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                currentMap[x + (mapHeight - 1 - y) * mapWidth] = value;
        }
        public void SetTempTileInGridPosition(int x, int y, int value) {
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                tempMap[x + (mapHeight - 1 - y) * mapWidth] = value;
        }

        public int GetTileInWorldPosition(int x, int y) {
            x -= (int)origin.x;
            y -= (int)origin.y;
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                return currentMap[x + (mapHeight - 1 - y) * mapWidth];
            return -1;
        }

        public Color GetPixelColorInGridPosition(int x, int y) {
            int gridX = x/cellSize, gridY = y/cellSize;
            int cellX = x % cellSize, cellY = y % cellSize;
            int cell = GetCurrentTileInGridPosition(gridX, gridY);
            return tileDictionary[cell].tilePixels[cellX + cellY*cellSize];
        }
    }

    public enum RaycastDirection {
        right,
        left,
        up,
        down
    }
}
