using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    private BasketBallScript basketballScript;

    [SerializeField] private GameObject scorePopUpPrefab;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private RectTransform scorePanel;

    [Space, SerializeField] private AudioClip netSound; // Ball to net collision sound
    [SerializeField] private AudioClip bounceSound; // Ball bounce sound
    [SerializeField] private AudioClip scoreSound; // Ball bounce sound

    [SerializeField] private AudioSource audioSource;

    [Space, SerializeField] private Transform mainCameraTransform;
    [Space, SerializeField] private float shakeDuration = 0.45f;
    [Space, SerializeField] private float shakeMagnitude = 0.008f;

    Vector3 originalCameraPosition;
    Vector2 randomRange = new Vector2(600, 300); 

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
            
            PlaySound(scoreSound);

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

        GameObject scorePopUpInstance = Instantiate(scorePopUpPrefab, scorePanel);
        
        RectTransform rectTransform = scorePopUpInstance.GetComponent<RectTransform>();

        Vector2 randomPosition = GetRandomPositionNearCenter();
        rectTransform.anchoredPosition = randomPosition;
    }

    private Vector2 GetRandomPositionNearCenter()
    {
        
        float randomX = Random.Range(0, randomRange.x);
        float randomY = Random.Range(0, randomRange.y);

        return new Vector2(randomX, randomY);
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
