using Snapweaver;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapshotCounter : MonoBehaviour {
    List<GameObject> counters = new List<GameObject>();

    void Awake() {
        counters.Add(transform.Find("Counter0").gameObject);
        counters.Add(transform.Find("Counter1").gameObject);
        counters.Add(transform.Find("Counter2").gameObject);
        counters.Add(transform.Find("Counter3").gameObject);
        SetCounter(-1);
    }

    public void SetCounter(int value) {
        for (int i = 0; i < counters.Count; i++)
            counters[i].SetActive(i == value);
    }
}
