using System.Collections;
using UnityEngine;

public class ScorePopupAnim : MonoBehaviour
{
    private CanvasGroup canvasGroup; // To control transparency
    private float fadeDuration = 2.5f; // Duration of the fade-out

    private void Start()
    {
        // Ensure the GameObject has a CanvasGroup component
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Start the fade-out coroutine
        StartCoroutine(FadeOutAndDestroy());
    }

    private void Update()
    {
        // Upward motion during the fade-out
        transform.Translate(Vector3.up * Time.deltaTime * 50f);
    }

    private IEnumerator FadeOutAndDestroy()
    {
        // Make the popup visible for awhile
        yield return new WaitForSeconds(0.2f);

        float timer = 0f;

        // Gradually decrease the alpha value
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        // Ensure the alpha is fully 0
        canvasGroup.alpha = 0f;

        // Then destroy the GameObject
        Destroy(gameObject);
    }
}
