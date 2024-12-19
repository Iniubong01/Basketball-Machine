using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class BasketBallScript : MonoBehaviour
{
    public Transform ballSpawnPoint;
    public GameObject basketballPrefab, pauseUI;
    public float shootingForceMultiplier = 0.1f;
    public float upwardForce = 5f;
    public int trajectoryPoints = 20;
    public Slider timeSlider;
    public Text timeText;
    public Text currentScoreText;
    public Text easyHighScoreText, mediumHighScoreText, hardHighScoreText;

    private Vector2 startSwipePosition, endSwipePosition;
    private bool isSwiping = false;
    [HideInInspector] public bool canPlay = false, isTraining = false;
    [HideInInspector] public bool isGameOver = false; // Ensure this is declared outside the method    

    // Pause funtionality
    private bool isPaused = false; // Flag to check if Game Over has already been handled
    
    private float gameTime, remainingTime;
    private string currentDifficulty;

    private Queue<Rigidbody> basketballPool = new Queue<Rigidbody>();
    private int poolSize = 26;

    private int easyHighScore, mediumHighScore, hardHighScore;
    private int currentScore;

    private int currentEasyScore = 0, currentMediumScore = 0, currentHardScore = 0;

    private float previousTimeScale = 1f;

    private TweenManager tweenManager;

    public Camera uiCamera;        // Assign your UI Camera in the Inspector
    public Camera gameCamera;      // Assign your Game Camera in the Inspector
    public GameObject gameplayPanel;     // Gameplay UI Panel

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
            }
        }
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
        Debug.Log("GameOver triggered!");

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
        StartCoroutine(waitBrieflyBeforeEnablingSwipe());
        SetDifficulty("Easy");
        ResetCurrentScore();
       // StartGameplay();
    }

    public void OnMediumButtonPressed()
    {
        StartCoroutine(waitBrieflyBeforeEnablingSwipe());
        SetDifficulty("Medium");
        ResetCurrentScore();
       // StartGameplay();
    }

    public void OnHardButtonPressed()
    {
        StartCoroutine(waitBrieflyBeforeEnablingSwipe());
        SetDifficulty("Hard");
        ResetCurrentScore();
      //  StartGameplay();
    }

    public void OnTrainingButtonPressed()
    {
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
            Debug.Log("Game Paused my Brr!");
        }
        else
        {
            Time.timeScale = previousTimeScale;   // Resume the game by setting timeScale badck to one
            pauseUI.SetActive(false);
            StartCoroutine(waitBrieflyBeforeEnablingSwipe());
            Debug.Log("Game Resumed my Brr!");
        }
    }

    private IEnumerator waitBrieflyBeforeEnablingSwipe()
    {
        // Make the popup visible for awhile
        yield return new WaitForSeconds(0.2f);       
        canPlay = true;
    }
}
