using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snapweaver {
    public class Pixel2DPhysics : MonoBehaviour {
        public static Pixel2DPhysics Instance;

        public List<PoolObject> objectsToPool;
        public Dictionary<string, List<GameObject>> pooledObjects;
        bool debugEnabled = false;


        private void Awake() {
            if (Pixel2DPhysics.Instance != null)
                GameObject.Destroy(this);
            Instance = this;
        }

        private void Start() {
            pooledObjects = new Dictionary<string, List<GameObject>>();
            for (int i = 0; i < objectsToPool.Count; i++) {
                List<GameObject> gos = new List<GameObject>();
                for (int j = 0; j < objectsToPool[i].poolAmount; j++) {
                    GameObject obj = (GameObject)Instantiate(objectsToPool[i].gameObj);
                    obj.name = objectsToPool[i].name + i;
                    obj.SetActive(false);
                    gos.Add(obj);
                }
                pooledObjects.Add(objectsToPool[i].name, gos);
            }
        }

        public GameObject GetPooledObject(string name) {
            for (int i = 0; i < pooledObjects[name].Count; i++)
                if (!pooledObjects[name][i].activeInHierarchy)
                    return pooledObjects[name][i];
            return null;
        }

        public int Raycast(Vector2Int from, RaycastDirection direction, int length, bool drawDebug = false) {
            if (length < 1)
                length = 1;
            GameObject ray = GetPooledObject("Raycast"); ;
            Vector2Int to;

            switch (direction) {
                case RaycastDirection.right:
                    to = Vector2Int.right;
                    if (drawDebug && debugEnabled) {
                        ray.transform.position = new Vector3(from.x, from.y, 0);
                        ray.transform.localScale = new Vector3(length, 1, 1);
                    }
                    break;
                case RaycastDirection.left:
                    to = -Vector2Int.right;
                    if (drawDebug && debugEnabled) {
                        ray.transform.position = new Vector3(from.x - length + 1, from.y, 0);
                        ray.transform.localScale = new Vector3(length, 1, 1);
                    }
                    break;
                case RaycastDirection.up:
                    to = Vector2Int.up;
                    if (drawDebug && debugEnabled) {
                        ray.transform.position = new Vector3(from.x, from.y, 0);
                        ray.transform.localScale = new Vector3(1, length, 1);
                    }
                    break;
                case RaycastDirection.down:
                    to = -Vector2Int.up;
                    if (drawDebug && debugEnabled) {
                        ray.transform.position = new Vector3(from.x, from.y - length + 1, 0);
                        ray.transform.localScale = new Vector3(1, length, 1);
                    }
                    break;
                default:
                    to = Vector2Int.zero;
                    if (drawDebug && debugEnabled) {
                        ray.transform.position = new Vector3(from.x, from.y, 0);
                    }
                    break;
            }

            if (drawDebug && debugEnabled) {
                ray.SetActive(true);
            }

            Vector2Int position = new Vector2Int();

            for (int i = 0; i <= length; i++) {
                position.x = from.x + to.x * (i);
                position.y = from.y + to.y * (i);

                if (position.x < 0 || position.x >= PixelMapGenerator.Instance.gameMap.pixelWidth ||
                    position.y < 0 || position.y >= PixelMapGenerator.Instance.gameMap.pixelHeight)
                    break;

                if (PixelMapGenerator.Instance.gameMap.GetPixelColorInGridPosition(position.x, position.y) == Color.black) {
                    if (drawDebug && debugEnabled) {
                        var raycastHit = GetPooledObject("RaycastHit");
                        raycastHit.transform.position = new Vector3(position.x, position.y, 0);
                        raycastHit.SetActive(true);
                        //Debug.Log("Raycast da " + from.x + "," + from.y + " verso " + direction.ToString() + " di " + length + ": " + (i));
                    }
                    return i;
                }
            }

            /*if (drawDebug && debugEnabled)
                Debug.Log("Raycast da " + from.x + "," + from.y + " verso " + direction.ToString() + " di " + length + ": SOLID NOT FOUND");*/
            return -1;
        }

        public void Test() {
            Debug.Log("2DPhysics Instance");
        }
    }
}