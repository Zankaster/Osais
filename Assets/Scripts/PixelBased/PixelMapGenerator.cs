using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PixelMapGenerator : MonoBehaviour {
    public int cellSize = 8;
    public Vector2 mapOrigin;
    public TextAsset mapAsset;
    public List<PixelTile> tiles;
    int rows = 0, columns = 0;
    string mapString;
    public Map2D gameMap;
    public static PixelMapGenerator Instance;

    private void Awake() {
        if (PixelMapGenerator.Instance != null)
            GameObject.Destroy(this);
        Instance = this;
    }

    void Start() {
        GenerateTilesPixels();
        GenerateMap();
        PlaceTiles();

        /*TestPixelAtPos(0, 0);
        TestPixelAtPos(6, 0);
        TestPixelAtPos(7, 0);
        TestPixelAtPos(0, 1);
        TestPixelAtPos(0, 15);
        TestPixelAtPos(0, 7);
        TestPixelAtPos(0, 8);*/
        /*Controller2DPhysics.Instance.Raycast(new Vector2Int(13, 6), RaycastDirection.left, 15, true);
        Controller2DPhysics.Instance.Raycast(new Vector2Int(11, 5), RaycastDirection.right, 15, true);*/

    }

    public void TestPixelAtPos(int x, int y) {
        Debug.Log("Pixel at position " + x + "," + y + ": " + gameMap.GetPixelColorInGridPosition(x, y).ToString());
    }

    public void GenerateTilesPixels() {
        for(int i = 0; i < tiles.Count(); i++) {
            var sprite = tiles[i].tilePrefab.GetComponent<SpriteRenderer>().sprite;
            var texture = sprite.texture;
            int xPos = (int)sprite.rect.x, yPos = (int)sprite.rect.y;
            int width = (int)sprite.rect.width, height = (int)sprite.rect.height;
            tiles[i].tilePixels = new Color[width * height];
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    tiles[i].tilePixels[x + y * width] = texture.GetPixel(xPos + x, yPos + y);
                }
            }
        }
    }

    public void GenerateMap() {
        columns = mapAsset.text.IndexOf("\r\n");
        mapString = mapAsset.text.Replace("\r\n", "");
        rows = mapString.Length / columns;
        /*Debug.Log(rows + "x" + columns);
        Debug.Log(mapString);*/
        gameMap = new Map2D(mapString, columns, rows, cellSize);
        gameMap.origin = mapOrigin;
        gameMap.colorMap = new Color[cellSize * gameMap.mapWidth, cellSize * gameMap.mapHeight];
        for (int y = 0; y < gameMap.mapHeight; y++) {
            for (int x = 0; x < gameMap.mapWidth; x++) {
                gameMap.SetColorInGridPosition(x, y, tiles.Where(t => t.tileSymbol == gameMap.GetTileInGridPosition(x, y)).FirstOrDefault().tilePixels);
            }
        }
    }

    public void PlaceTiles() {
        for (int y = 0; y < gameMap.mapHeight; y++) {
            for (int x = 0; x < gameMap.mapWidth; x++) {
                //Debug.Log("map " + x + "x" + y);
                GameObject g;
                g = GameObject.Instantiate(tiles.Where(m => m.tileSymbol == gameMap.GetTileInGridPosition(x, y)).FirstOrDefault().tilePrefab);
                if (g != null) {
                    g.transform.parent = transform;
                    g.transform.position = gameMap.origin + new Vector2(x*cellSize, y * cellSize);
                }
            }
        }
    }

}
