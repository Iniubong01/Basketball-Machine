using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class BasketBallScript : MonoBehaviour
{
    [Space, SerializeField] EmissionColorEffect emissionColorEffect; // to control machine lights

    [SerializeField] private  Transform ballSpawnPoint;
    [SerializeField] private GameObject basketballPrefab, pauseUI;
    [SerializeField] private float shootingForceMultiplier = 0.1f;

    [SerializeField] private float upwardForce = 5f;
    private float gameTime, remainingTime;
    private float previousTimeScale = 1f;

    [SerializeField] private int trajectoryPoints = 20;
    private int poolSize = 26;
    private int easyHighScore, mediumHighScore, hardHighScore;
    private int currentScore;
    private int currentEasyScore = 0, currentMediumScore = 0, currentHardScore = 0;
    private int SwipeAnimCount = 0;
    private int maxSwipeAnimCount = 2;

    [SerializeField] public Slider timeSlider;
    [SerializeField] private Text timeText;
    [SerializeField] private Text currentScoreText;
    [SerializeField] private Text easyHighScoreText, mediumHighScoreText, hardHighScoreText;

    private Vector2 startSwipePosition, endSwipePosition;

    private bool isSwiping = false;
    [HideInInspector] public bool canPlay = false, isTraining = false;
    [HideInInspector] public bool isGameOver = false;   
    // Pause funtionality
    private bool isPaused = false; // Flag to check if Game Over has already been handled
    [SerializeField] private AudioSource swipeSound; // Reference to the AudioSource for the swipe sound
    [SerializeField] private float maxSwipeForce = 100f; // Maximum expected swipe force for volume scaling

    
    private string currentDifficulty;

    private Queue<Rigidbody> basketballPool = new Queue<Rigidbody>();

    private TweenManager tweenManager;

    [SerializeField] RectTransform swipeUpHandGesture; 


    private void Start()
    {
        // Initialize object pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject basketball = Instantiate(basketballPrefab);
            basketball.SetActive(false);
            basketballPool.Enqueue(basketball.GetComponent<Rigidbody>());
        }

        SetDifficulty("Easy");

        // Load high scores
        easyHighScore = PlayerPrefs.GetInt("EasyHighScore", 0);
        mediumHighScore = PlayerPrefs.GetInt("MediumHighScore", 0);
        hardHighScore = PlayerPrefs.GetInt("HardHighScore", 0);
        SwipeAnimCount = PlayerPrefs.GetInt("SwipeAnimCount", 0); 

        UpdateHighScoreUI();
        tweenManager = GameObject.Find("Tween Manager").GetComponent<TweenManager>();

        UpdateScoreUI();
    }

    
    private void Update()
    {
        HandleSwipeInput();
        UpdateTimer();
    }

    public void SetDifficulty(string difficulty)
    {
        if (isTraining)
            return;

        currentDifficulty = difficulty;

        gameTime = difficulty switch
        {
            "Easy" => 300f,
            "Medium" => 180f,
            "Hard" => 60f,
            _ => 300f
        };

        remainingTime = gameTime;
        timeSlider.maxValue = gameTime;
        timeSlider.value = remainingTime;
        timeSlider.gameObject.SetActive(true);  // Ensure the slider is visible and active
    }


    private void HandleSwipeInput()
    {
        if (!canPlay)
            return;

        if (isTraining)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startSwipePosition = Input.mousePosition;
                isSwiping = true;
            }

            if (Input.GetMouseButtonUp(0) && isSwiping)
            {
                endSwipePosition = Input.mousePosition;
                isSwiping = false;

                Vector2 swipeDirection = endSwipePosition - startSwipePosition;
                float swipeForce = swipeDirection.magnitude * shootingForceMultiplier;

                ShootBasketball(swipeDirection, swipeForce);

                // Play the swipe sound
                PlaySwipeSound(swipeForce);
            }
        }
        else if (remainingTime > 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startSwipePosition = Input.mousePosition;
                isSwiping = true;
            }

            if (Input.GetMouseButtonUp(0) && isSwiping)
            {
                endSwipePosition = Input.mousePosition;
                isSwiping = false;

                Vector2 swipeDirection = endSwipePosition - startSwipePosition;
                float swipeForce = swipeDirection.magnitude * shootingForceMultiplier;

                ShootBasketball(swipeDirection, swipeForce);

                // Play the swipe sound depending on the swipe force, the higher the swipe force,
                // the higher the volume on the swipe Audio Source
                PlaySwipeSound(swipeForce);
            }
        }
    }

    private void PlaySwipeSound(float swipeForce)
    {
        if (swipeSound == null)
        {
            Debug.LogWarning("Swipe sound AudioSource is not assigned!");
            return;
        }

        // Debug the swipe force
        Debug.Log("Swipe Force :" + swipeForce);
        float normalizedForce = Mathf.Clamp01(swipeForce / maxSwipeForce);

        float volumeMultiplier = 4;

        swipeSound.volume = normalizedForce * volumeMultiplier;
        swipeSound.Play();
    }


    private void ShootBasketball(Vector2 swipeDirection, float swipeForce)
    {
        Rigidbody basketball = basketballPool.Dequeue();
        basketball.transform.position = ballSpawnPoint.position;
        basketball.gameObject.SetActive(true);

        Vector3 shootDirection = new Vector3(swipeDirection.x, upwardForce, swipeDirection.y).normalized;
        basketball.AddForce(shootDirection * swipeForce, ForceMode.Impulse);

        StartCoroutine(DisableBasketballAfterTime(basketball, 3f));
        basketballPool.Enqueue(basketball);
    }

    private IEnumerator DisableBasketballAfterTime(Rigidbody basketball, float delay)
    {
        yield return new WaitForSeconds(delay);
        basketball.gameObject.SetActive(false);
    }

    private void UpdateTimer()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            timeSlider.value = remainingTime;
        }
        else
        {
            GameOver();
        }
    }

    private void UpdateScoreUI()
    {
        
        switch (currentDifficulty)
        {
            case "Easy":
                currentScoreText.text = currentEasyScore.ToString();
                break;
            case "Medium":
                currentScoreText.text = currentMediumScore.ToString();
                break;
            case "Hard":
                currentScoreText.text = currentHardScore.ToString();
                break;
        }
    }

    public void AddScore(int points)
    {
       
        emissionColorEffect.OnScoring(); // light blink after player scorees a basket

        switch (currentDifficulty)
        {
            case "Easy":
                currentEasyScore += points;
                break;
            case "Medium":
                currentMediumScore += points;
                break;
            case "Hard":
                currentHardScore += points;
                break;
        }

        UpdateScoreUI();
    }

    private void SaveHighScore()
    {
        if (currentDifficulty == "Easy" && currentEasyScore > easyHighScore)
        {
            easyHighScore = currentEasyScore;
            PlayerPrefs.SetInt("EasyHighScore", easyHighScore);
        }
        else if (currentDifficulty == "Medium" && currentMediumScore > mediumHighScore)
        {
            mediumHighScore = currentMediumScore;
            PlayerPrefs.SetInt("MediumHighScore", mediumHighScore);
        }
        else if (currentDifficulty == "Hard" && currentHardScore > hardHighScore)
        {
            hardHighScore = currentHardScore;
            PlayerPrefs.SetInt("HardHighScore", hardHighScore);
        }
        
        PlayerPrefs.SetInt("SwipeAnimCount", SwipeAnimCount);

        PlayerPrefs.Save();
        UpdateHighScoreUI();
    }

    public void ResetCurrentScore()
    {
        currentEasyScore = 0;
        currentMediumScore = 0;
        currentHardScore = 0;
        currentScore = 0;
        isGameOver = false;

        UpdateScoreUI();
    }

    private void UpdateHighScoreUI()
    {
        easyHighScoreText.text = easyHighScore.ToString();
        mediumHighScoreText.text = mediumHighScore.ToString();
        hardHighScoreText.text = hardHighScore.ToString();
    }

        private void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // Save high score
        SaveHighScore();

        // Transition to the main menu only if not in training
        if (isGameOver)
        {
            tweenManager.GamePlayOutMenuIn();
        }
    }

    private IEnumerator ResetGameOverState()
    {
        // Add a short delay to ensure GameOver actions complete
        yield return new WaitForSeconds(1f);

        // Reset the game over flag, if needed for replays
        isGameOver = false;
    }

    public void OnEasyButtonPressed()
    {
        ShowHandAnim();
        StartCoroutine(waitBrieflyBeforeEnablingSwipe());
        SetDifficulty("Easy");
        ResetCurrentScore();
    }

    public void OnMediumButtonPressed()
    {
        ShowHandAnim();
        StartCoroutine(waitBrieflyBeforeEnablingSwipe());
        SetDifficulty("Medium");
        ResetCurrentScore();
    }

    public void OnHardButtonPressed()
    {
        ShowHandAnim();
        StartCoroutine(waitBrieflyBeforeEnablingSwipe());
        SetDifficulty("Hard");
        ResetCurrentScore();
    }

    public void OnTrainingButtonPressed()
    {
        ShowHandAnim();
        isTraining = true;  // Enable Training Mode
        StartCoroutine(waitBrieflyBeforeEnablingSwipe());
        ResetCurrentScore();
       /// StartGameplay();

        if(isTraining == true) {
            {
                // No timer or scoring setup needed for Training
                remainingTime = float.PositiveInfinity;
                timeSlider.gameObject.SetActive(false);  // Optionally hide the timer slider
            }
        }
    }

    public void ShowHandAnim()
    { 
        CanvasGroup myAnimCanvasGroup = swipeUpHandGesture.GetComponent<CanvasGroup>();
        if(SwipeAnimCount < maxSwipeAnimCount)
        {
            // Code to simulate hand gesture animation
            swipeUpHandGesture.anchoredPosition = new Vector2(200, 450); 
            swipeUpHandGesture.DOAnchorPos(new Vector2(200, 140), 1f).SetLoops(-1, LoopType.Yoyo);
            swipeUpHandGesture.gameObject.SetActive(true);
            SwipeAnimCount++;
            // Fade out with DOTween
            myAnimCanvasGroup.alpha = 1;
            StartCoroutine(disableSwipeAnim());  
        } 
        else
        {
            Debug.LogWarning("Will not play Anim");
            Debug.Log("AnimCount "+ SwipeAnimCount);
            return;
        }                      
    }

     private void FadeOutHandGesture()
    {
        CanvasGroup myAnimCanvasGroup = swipeUpHandGesture.GetComponent<CanvasGroup>();
        if (myAnimCanvasGroup != null)
        {
            // Fade out with DOTween
            myAnimCanvasGroup.DOFade(0, 1f).OnComplete(()=>
            {
                swipeUpHandGesture.gameObject.SetActive(false);
            });
        }
    }

    public IEnumerator disableSwipeAnim()
    {
        yield return new WaitForSeconds(5f);
        FadeOutHandGesture();
    }

    public void OnHighScoreButtonPressed() => SaveHighScore();

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            
            canPlay = false;
            previousTimeScale = Time.timeScale;  // Save the current TimeScale which is one
            Time.timeScale = 0;   // Pause game
            pauseUI.SetActive(true); 
        }
        else
        {
            Time.timeScale = previousTimeScale;   // Resume the game by setting timeScale badck to one
            pauseUI.SetActive(false);
            StartCoroutine(waitBrieflyBeforeEnablingSwipe());
        }
    }

    private IEnumerator waitBrieflyBeforeEnablingSwipe()
    {
        // Make the popup visible for awhile
        yield return new WaitForSeconds(0.2f);       
        canPlay = true;
    }
}
