using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    public float lifeTime = 0.01f;
    float aliveTime;
    List<GameObject> children;
    private void Awake() {
        children = new List<GameObject>();
        foreach (Transform t in transform)
            children.Add(t.gameObject);
    }
    private void OnEnable() {
        aliveTime = 0f;
        foreach (GameObject g in children)
            g.SetActive(true);
    }

    private void OnDisable() {
        aliveTime = 0;
        foreach (GameObject g in children)
            g.SetActive(false);
    }

    void Update() {
        /*if (GameManager.instance.GetGameStatus() == GameStatus.Continue)
            return;*/
        aliveTime += Time.deltaTime;
        if (aliveTime > lifeTime) {
            gameObject.SetActive(false);
        }
    }
}
