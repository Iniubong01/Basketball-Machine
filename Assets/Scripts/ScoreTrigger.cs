using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    private BasketBallScript basketballScript;

    [SerializeField] private GameObject scorePopUpPrefab;
    [SerializeField] private Canvas uiCanvas;

    [SerializeField] private AudioClip netSound; // Ball to net collision sound
    [SerializeField] private AudioClip bounceSound; // Ball bounce sound

    [SerializeField] private AudioSource audioSource;

    [Space, SerializeField] private Transform mainCameraTransform;
    [Space, SerializeField] private float shakeDuration = 0.5f;
    private float shakeMagnitude = 0.01f;

    Vector3 originalCameraPosition;

    public ParticleSystem scoreEffect;

    private HashSet<GameObject> triggeredBalls = new HashSet<GameObject>();

    void Start()
    {
        basketballScript = GameObject.Find("Basketball Manager").GetComponent<BasketBallScript>();

        if (uiCanvas == null)
        {
            Debug.LogError("UI Canvas is not assigned to ScoreTrigger!");
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned to ScoreTrigger!");
        }

        if (mainCameraTransform == null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        originalCameraPosition = mainCameraTransform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !triggeredBalls.Contains(other.gameObject))
        {
            // Handle net collision, so that a single ball does not collide twice with the net
            triggeredBalls.Add(other.gameObject);

            basketballScript.AddScore(35); // Increment score by 35 points
            CreateScorePopUp();

            scoreEffect.Play();

            StartCoroutine(CameraShakeRoutine());

            PlaySound(netSound); // Play net collision sound

            StartCoroutine(ClearTriggeredBallAfterTime(other.gameObject, 3f));
        }
    }


    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

        private void CreateScorePopUp()
    {
        // Generate random screen position
        float randomX = Random.Range(Screen.width * 0.3f, Screen.width * 0.7f);
        float randomY = Random.Range(Screen.height * 0.1f, Screen.height * 0.3f);
        Vector3 randomScreenPosition = new Vector3(randomX, randomY, 0f);

        // Convert screen position to world position in the canvas
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(
            randomScreenPosition.x,
            randomScreenPosition.y,
            uiCanvas.planeDistance // Ensure the correct distance from the camera
        ));

        // Instantiate the pop-up prefab and set its position
        GameObject scorePopUpInstance = Instantiate(scorePopUpPrefab, uiCanvas.transform);
        scorePopUpInstance.transform.position = worldPosition;

        RectTransform popUpRect = scorePopUpInstance.GetComponent<RectTransform>();
        if (popUpRect == null)
        {
            Debug.LogError("Score pop-up prefab is missing a RectTransform component!");
        }
    }

    private IEnumerator CameraShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            mainCameraTransform.position = originalCameraPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCameraTransform.position = originalCameraPosition;
    }


    private IEnumerator ClearTriggeredBallAfterTime(GameObject ball, float delay)
    {
        yield return new WaitForSeconds(delay);
        triggeredBalls.Remove(ball);
    }
}
