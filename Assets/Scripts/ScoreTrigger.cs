using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    private BasketBallScript basketballScript;
    [SerializeField] private GameObject scorePopUpPrefab;
    [SerializeField] private Canvas uiCanvas;

    private HashSet<GameObject> triggeredBalls = new HashSet<GameObject>();

    void Start()
    {
        basketballScript = GameObject.Find("Basketball Manager").GetComponent<BasketBallScript>();

        if (uiCanvas == null)
        {
            Debug.LogError("UI Canvas is not assigned to ScoreTrigger!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !triggeredBalls.Contains(other.gameObject))
        {
            triggeredBalls.Add(other.gameObject);

            basketballScript.AddScore(35); // Increment score by 35 points
            CreateScorePopUp();

            StartCoroutine(ClearTriggeredBallAfterTime(other.gameObject, 3f));
        }
    }

    private void CreateScorePopUp()
    {
        float randomX = Random.Range(Screen.width * 0.3f, Screen.width * 0.7f);
        float randomY = Random.Range(Screen.height * 0.1f, Screen.height * 0.3f);

        Vector3 randomScreenPosition = new Vector3(randomX, randomY, 0f);

        GameObject scorePopUpInstance = Instantiate(scorePopUpPrefab, uiCanvas.transform);

        RectTransform popUpRect = scorePopUpInstance.GetComponent<RectTransform>();
        if (popUpRect != null)
        {
            popUpRect.position = randomScreenPosition;
        }
        else
        {
            Debug.LogError("Score pop-up prefab is missing a RectTransform component!");
        }
    }

    private IEnumerator ClearTriggeredBallAfterTime(GameObject ball, float delay)
    {
        yield return new WaitForSeconds(delay);
        triggeredBalls.Remove(ball);
    }
}
