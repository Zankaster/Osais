using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MapGenerator : MonoBehaviour {
    public static Map2D gameMap;
    public GameObject background;
    public GameObject obstacle;

    void Start() {
        gameMap = new Map2D();
        gameMap.origin = new Vector2(0f, 0f);
        gameMap.mapWidth = 16;
        gameMap.mapHeight = 9;
        gameMap.map =   ".......##......." +
                        "........#...##.." +
                        "................" +
                        "...#.#######...." +
                        "................" +
                        "...##..####....." +
                        "################" +
                        "................" +
                        "...........##...";

        for (int y = 0; y < gameMap.mapHeight; y++) {
            for (int x = 0; x < gameMap.mapWidth; x++) {
                GameObject g;
                if (gameMap.GetTileInGridPosition(x, y) == '.') {
                    g = GameObject.Instantiate(background);
                }
                else if (gameMap.GetTileInGridPosition(x, y) == '#') {
                    g = GameObject.Instantiate(obstacle);
                }
                else {
                    continue;
                }
                if (g != null) {
                    g.transform.parent = transform;
                    g.transform.position = gameMap.origin + new Vector2(x, y);
                }
            }
        }
    }
}
