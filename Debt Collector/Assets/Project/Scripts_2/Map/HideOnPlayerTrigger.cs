using System.Collections;
using UnityEngine;

public class HideOnPlayerTrigger : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private LayerMask Player;
    [SerializeField] private float fadeDuration = 0.2f;
    private bool isFading = false;

    private int playerLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & Player) != 0)
        {
            StartCoroutine(FadeOut());
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & Player) != 0)
        {
            StartCoroutine(FadeIn());
        }
    }
    private IEnumerator FadeOut()
    {
        isFading = true;
        SpriteRenderer sr = targetObject.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("SpriteRenderer не найден!");
            yield break;
        }

        Color color = sr.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, 0f, t);
            sr.color = color;
            yield return null;
        }

        color.a = 0f;
        sr.color = color;
        targetObject.SetActive(false);
    }
    private IEnumerator FadeIn()
    {
        isFading = true;
        SpriteRenderer sr = targetObject.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("SpriteRenderer не найден!");
            yield break;
        }

        Color color = sr.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, 1f, t);
            sr.color = color;
            yield return null;
        }

        color.a = 1f;
        sr.color = color;
        targetObject.SetActive(true);
    }

}
