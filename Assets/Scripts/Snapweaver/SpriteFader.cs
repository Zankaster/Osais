using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFader : MonoBehaviour
{
    public float fadeInAfterSeconds = 6;
    public float fadeInDuration = 1;
    float elapsedTime = 0;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= fadeInAfterSeconds) {
            StartCoroutine(FadeInSprite());
        }
    }

    IEnumerator FadeInSprite() {
        float journey = 0f;
        while (journey <= fadeInDuration) {
            journey = journey + Time.deltaTime;
            if (journey >= fadeInDuration - 2)
                spriteRenderer.color = new Color(1, 1, 1, Mathf.Lerp(0,1,journey/fadeInDuration));
            yield return null;
        }
    }
}
