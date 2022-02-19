using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Snapweaver {
    public class PixelMapGenerator : MonoBehaviour {
        public TileAtlas tileAtlas;
        public TextAsset mapForeground;
        public TextAsset mapBackground;
        public int cellSize = 8;
        public Vector2 mapOrigin;
        int rows = 0, columns = 0;
        public Map2D gameMap;
        public AnimationCurve tilesLerpPositionCurve;
        public static PixelMapGenerator Instance;
        SpriteRenderer[,] tilesForeground;
        SpriteRenderer[,] tilesBackground;
        private Color whiteHalfTransparent = new Color(1, 1, 1, 0.5f);
        private Color white = new Color(1, 1, 1, 1);
        GameObject tilesAnimationContainer;
        PixelPlayerController player;

        private void Awake() {
            if (PixelMapGenerator.Instance != null)
                GameObject.Destroy(this);
            Instance = this;
        }

        private void Start() {
            player = FindObjectOfType<PixelPlayerController>();
            GenerateMap();
            PlaceTiles();
            tilesAnimationContainer = GameObject.Find("TilesAnimationContainer");
            /*TestPixelAtPos(1, 3);
            TestPixelAtPos(0, 8);
            TestPixelAtPos(8, 0);
            TestPixelAtPos(8, 5);
            TestPixelAtPos(0, 23);
            TestPixelAtPos(31, 23);
            TestPixelAtPos(31, 0);
            TestPixelAtPos(17, 0);
            TestPixelAtPos(17, 1);
            TestPixelAtPos(18, 0);
            TestPixelAtPos(18, 1);*/
            /*Debug.Log("1.1f -> " + Mathf.RoundToInt(1.1f));
            Debug.Log("1.5f -> " + Mathf.RoundToInt(1.5f));
            Debug.Log("1.9f -> " + Mathf.RoundToInt(1.9f));
            Debug.Log("-1.1f -> " + Mathf.RoundToInt(-1.1f));
            Debug.Log("-1.5f -> " + Mathf.RoundToInt(-1.5f));
            Debug.Log("-1.9f -> " + Mathf.RoundToInt(-1.9f));*/
        }

        public void TestPixelAtPos(int x, int y) {
            Debug.Log("Pixel at position " + x + "," + y + ": " + gameMap.GetPixelColorInGridPosition(x, y).ToString());
        }

        public void GenerateMap() {
            string mapStringForeground;
            string mapStringBackground;
            columns = mapForeground.text.Substring(0, mapForeground.text.IndexOf("\r\n")).Split(',').Count();
            mapStringForeground = mapForeground.text.Substring(0, mapForeground.text.Length - 2).Replace("\r\n", ",");
            mapStringBackground = mapBackground.text.Substring(0, mapBackground.text.Length - 2).Replace("\r\n", ",");
            List<int> mapIntForeground = mapStringForeground.Split(',').Select(int.Parse).ToList();
            List<int> mapIntBackground = mapStringBackground.Split(',').Select(int.Parse).ToList();
            rows = mapIntForeground.Count / columns;
            Dictionary<int, PixelTile> tileDictionary = new Dictionary<int, PixelTile>();
            for (int i = 0; i < tileAtlas.tiles.Count; i++)
                tileDictionary.Add(tileAtlas.tiles[i].tileIndex, tileAtlas.tiles[i]);

            gameMap = new Map2D(mapIntForeground, mapIntBackground, columns, rows, cellSize, tileDictionary);
            gameMap.origin = mapOrigin;
        }

        public void PlaceTiles() {
            tilesForeground = new SpriteRenderer[gameMap.mapWidth, gameMap.mapHeight];
            tilesBackground = new SpriteRenderer[gameMap.mapWidth, gameMap.mapHeight];
            for (int y = 0; y < gameMap.mapHeight; y++) {
                for (int x = 0; x < gameMap.mapWidth; x++) {
                    //foreground
                    GameObject f = new GameObject();
                    f.transform.parent = transform;
                    f.transform.position = gameMap.origin + new Vector2(x * cellSize, y * cellSize);
                    SpriteRenderer sf = f.AddComponent<SpriteRenderer>();
                    sf.sortingOrder = 50;
                    sf.sprite = gameMap.tileDictionary[gameMap.GetCurrentTileInGridPosition(x, y)].tileSprite;
                    tilesForeground[x, y] = sf;

                    //background
                    GameObject b = new GameObject();
                    b.transform.parent = transform;
                    b.transform.position = gameMap.origin + new Vector2(x * cellSize, y * cellSize);
                    SpriteRenderer sb = b.AddComponent<SpriteRenderer>();
                    sb.sortingOrder = 25;
                    //sb.color = new Color(1, 1, 1, .7f);
                    sb.sprite = gameMap.tileDictionary[gameMap.GetBackgroundTileInGridPosition(x, y)].tileSprite;
                    tilesBackground[x, y] = sb;
                }
            }
        }

        public void ApplyPhotoSwap(Vector2Int photoOrigin, Vector2Int photoDestination, Vector2Int photoSize) {

            //coroutine to spawn and lerp tiles
            GenerateTilesToLerp(photoOrigin, photoDestination, photoSize);
            StartCoroutine(LerpTiles(photoOrigin, photoDestination, photoSize, 1f));
        }

        IEnumerator DestroyTiles(List<GameObject> tiles, float duration) {
            float journey = 0f;
            while (journey <= duration) {
                journey = journey + Time.deltaTime;
                float percent = Mathf.Clamp01(journey / duration);

                foreach (GameObject g in tiles)
                    g.transform.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.zero, tilesLerpPositionCurve.Evaluate(percent));

                yield return null;
            }
            GameManager.Instance.inputLocked = false;
        }

        IEnumerator LerpTiles(Vector2Int photoOrigin, Vector2Int photoDestination, Vector2Int photoSize, float duration) {
            GameManager.Instance.inputLocked = true;
            Dictionary<Transform, Vector3> tilesStartPositions = new Dictionary<Transform, Vector3>();
            foreach (Transform t in tilesAnimationContainer.transform)
                tilesStartPositions.Add(t, new Vector3(t.position.x, t.position.y, t.position.z));
            float journey = 0f;
            while (journey <= duration) {
                journey = journey + Time.deltaTime;
                float percent = Mathf.Clamp01(journey / duration);

                foreach (Transform t in tilesAnimationContainer.transform)
                    t.position = Vector3.LerpUnclamped(tilesStartPositions[t], tilesStartPositions[t] + new Vector3((photoDestination.x - photoOrigin.x) * cellSize, (photoDestination.y - photoOrigin.y) * cellSize, 0), tilesLerpPositionCurve.Evaluate(percent));

                yield return null;
            }


            for (int y = 0; y < photoSize.y; y++) {
                for (int x = 0; x < photoSize.x; x++) {
                    if (gameMap.GetCurrentTileInGridPosition(photoDestination.x + x, photoDestination.y + y) == 0) {
                        gameMap.SetTempTileInGridPosition(photoDestination.x + x, photoDestination.y + y,
                            gameMap.GetCurrentTileInGridPosition(photoOrigin.x + x, photoOrigin.y + y));
                    }
                }
            }

            for (int y = 0; y < photoSize.y; y++) {
                for (int x = 0; x < photoSize.x; x++) {
                    gameMap.SetCurrentTileInGridPosition(photoDestination.x + x, photoDestination.y + y, gameMap.GetTempTileInGridPosition(photoDestination.x + x, photoDestination.y + y));
                }
            }

            RefreshTiles();
            ClearTilesToAnimate();
            var tilesToDestroy = CheckTilesTris(photoDestination, photoSize);


            RefreshTiles();
            if (tilesToDestroy != null) {
                player.PlaySound("Explosion");
                StartCoroutine(DestroyTiles(GenerateTilesToDestroy(tilesToDestroy), 1f));
            }
            else {
                GameManager.Instance.inputLocked = false;
            }
        }

        private void ClearTilesToAnimate() {
            foreach (Transform t in tilesAnimationContainer.transform)
                Destroy(t.gameObject);
        }

        private void SetTempToCurrentTiles(Vector2Int photoSize, Vector2Int photoDestination) {
            for (int y = 0; y < photoSize.y; y++) {
                for (int x = 0; x < photoSize.x; x++) {
                    gameMap.SetCurrentTileInGridPosition(photoDestination.x + x, photoDestination.y + y, gameMap.GetTempTileInGridPosition(photoDestination.x + x, photoDestination.y + y));
                }
            }
        }

        private void GenerateTilesToLerp(Vector2Int photoOrigin, Vector2Int photoDestination, Vector2Int photoSize) {
            ClearTilesToAnimate();

            for (int y = 0; y < photoSize.y; y++) {
                for (int x = 0; x < photoSize.x; x++) {
                    GameObject g = new GameObject();
                    g.transform.parent = tilesAnimationContainer.transform;
                    g.transform.position = new Vector2((photoOrigin.x + x) * PixelMapGenerator.Instance.gameMap.cellSize, (photoOrigin.y + y) * PixelMapGenerator.Instance.gameMap.cellSize);
                    SpriteRenderer s = g.AddComponent<SpriteRenderer>();
                    s.sortingOrder = 55;
                    s.sprite = PixelMapGenerator.Instance.gameMap.tileDictionary[PixelMapGenerator.Instance.gameMap.GetCurrentTileInGridPosition(photoOrigin.x + x, photoOrigin.y + y)].tileSprite;
                }
            }
        }

        private List<GameObject> GenerateTilesToDestroy(Tuple<int, List<Vector2Int>> tilesToDestroy) {
            ClearTilesToAnimate();
            List<GameObject> tilesGenerated = new List<GameObject>();
            for(int i = 0; i < tilesToDestroy.Item2.Count; i++) {
                GameObject g = new GameObject();
                g.transform.parent = tilesAnimationContainer.transform;
                g.transform.position = new Vector2(tilesToDestroy.Item2[i].x * PixelMapGenerator.Instance.gameMap.cellSize, tilesToDestroy.Item2[i].y * PixelMapGenerator.Instance.gameMap.cellSize);
                SpriteRenderer s = g.AddComponent<SpriteRenderer>();
                s.sortingOrder = 55;
                s.sprite = PixelMapGenerator.Instance.gameMap.tileDictionary[tilesToDestroy.Item1].tileSprite;
                tilesGenerated.Add(g);
            }

            return tilesGenerated;
        }


        public void RefreshTiles() {
            for (int y = 0; y < gameMap.mapHeight; y++) {
                for (int x = 0; x < gameMap.mapWidth; x++) {
                    tilesForeground[x, y].sprite = gameMap.tileDictionary[gameMap.GetCurrentTileInGridPosition(x, y)].tileSprite;
                }
            }
        }

        private Tuple<int, List<Vector2Int>> CheckTilesTris(Vector2Int photoDestination, Vector2Int photoSize) {
            Map2D map = PixelMapGenerator.Instance.gameMap;
            int sameCount;
            int lastIndex = 0, currentIndex = 0;
            //Horizontal Check
            for (int y = photoDestination.y; y < photoDestination.y + photoSize.y; y++) {
                sameCount = 0;
                for (int x = Mathf.Clamp(photoDestination.x - 2, map.minXboundTile, map.maxXboundTile); x < Mathf.Clamp(photoDestination.x + photoSize.x + 2, map.minXboundTile, map.maxXboundTile); x++) {
                    currentIndex = map.GetCurrentTileInGridPosition(x, y);
                    if (x == photoDestination.x - 2 || x == map.minXboundTile) {
                        lastIndex = currentIndex;
                        if (currentIndex != 0) {
                            sameCount = 1;
                        }
                    }
                    else {
                        if (currentIndex != 0 && currentIndex == lastIndex) {
                            sameCount++;
                            if (sameCount == 3) {

                                //tris, delete these tiles and exit
                                //(yes I have to be careful to design levels
                                //so that you can only make a tris at a time)
                                map.SetCurrentTileInGridPosition(x, y, 0);
                                map.SetCurrentTileInGridPosition(x - 1, y, 0);
                                map.SetCurrentTileInGridPosition(x - 2, y, 0);
                                return new Tuple<int, List<Vector2Int>>(currentIndex, new List<Vector2Int>() {
                                    new Vector2Int(x,y),
                                    new Vector2Int(x-1,y),
                                    new Vector2Int(x-2,y)

                                });
                            }
                        }
                        else {
                            sameCount = 1;
                        }
                        lastIndex = currentIndex;
                    }
                }
            }

            sameCount = 0;
            lastIndex = 0;
            currentIndex = 0;
            //Vertical Check
            for (int x = photoDestination.x; x < photoDestination.x + photoSize.x; x++) {
                sameCount = 0;
                for (int y = Mathf.Clamp(photoDestination.y - 2, map.minYboundTile, map.maxYboundTile); y < Mathf.Clamp(photoDestination.y + photoSize.y + 2, map.minYboundTile, map.maxYboundTile); y++) {
                    currentIndex = map.GetCurrentTileInGridPosition(x, y);
                    if (y == photoDestination.y - 2 || y == map.minYboundTile) {
                        lastIndex = currentIndex;
                        if (currentIndex != 0) {
                            sameCount = 1;
                        }
                    }
                    else {
                        if (currentIndex != 0 && currentIndex == lastIndex) {
                            sameCount++;
                            if (sameCount == 3) {

                                //tris, delete these tiles and exit
                                //(yes I have to be careful to design levels
                                //so that you can only make a tris at a time)
                                map.SetCurrentTileInGridPosition(x, y, 0);
                                map.SetCurrentTileInGridPosition(x, y- 1, 0);
                                map.SetCurrentTileInGridPosition(x, y- 2, 0);
                                return new Tuple<int, List<Vector2Int>>(currentIndex, new List<Vector2Int>() {
                                    new Vector2Int(x, y),
                                    new Vector2Int(x, y- 1),
                                    new Vector2Int(x, y- 2)

                                });
                            }
                        }
                        else {
                            sameCount = 1;
                        }
                        lastIndex = currentIndex;
                    }
                }
            }

            return null;
        }
    }
}