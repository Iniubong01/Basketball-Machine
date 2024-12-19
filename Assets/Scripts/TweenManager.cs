using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TweenManager : MonoBehaviour
{
    [Header("GameObject UI to be slided in")]
    public RectTransform mainMenu, difficultyUI, gamePlayUI, recordUI, RulesUI, settingsUI;
    public GameObject background;

    private BasketBallScript basketballScript;
    private bool isAnimating = false; // Prevents multiple coroutine calls

    // Panel positions for transitions
    private readonly Vector2 onScreenPos = new Vector2(0, 0);
    private readonly Vector2 offScreenPos = new Vector2(0, 2220);

    // Start is called before the first frame update
    void Start()
    {
        // Animate the UI object with a ping-pong effect
        transform.DOLocalMove(new Vector2(0, 410), 1)
            .SetEase(Ease.InOutQuart)
            .SetLoops(-1, LoopType.Yoyo);

        // Animate buttons if needed
        StartCoroutine(AnimateButtons());

        // Find Basketball Manager script
        basketballScript = GameObject.Find("Basketball Manager").GetComponent<BasketBallScript>();
    }

    // Coroutine to animate buttons with a delay
    public IEnumerator AnimateButtons()
    {
        if (isAnimating) yield break; // Prevent multiple coroutine calls
        isAnimating = true;

        // Example button animation logic (replace with actual buttons if needed)
        yield return new WaitForSeconds(0.05f);

        isAnimating = false;
    }

    // Slide difficulty UI in and out
    public void SlideDifficultyUI()
    {
        mainMenu.DOAnchorPos(offScreenPos, 0.25f);
        difficultyUI.DOAnchorPos(onScreenPos, 0.25f).SetDelay(0.25f);
    }

    // Slide out difficulty UI and bring back main menu
    public void MenuInDiffOut()
    {
        difficultyUI.DOAnchorPos(offScreenPos, 0.25f);
        mainMenu.DOAnchorPos(onScreenPos, 0.25f).SetDelay(0.25f);
    }

    // Slide out main menu and bring in record UI
    public void MenuOutRecordIn()
    {
        mainMenu.DOAnchorPos(offScreenPos, 0.25f);
        recordUI.DOAnchorPos(onScreenPos, 0.25f).SetDelay(0.25f);
    }

    // Slide out record UI and bring back main menu
    public void MenuInRecordOut()
    {
        recordUI.DOAnchorPos(offScreenPos, 0.25f);
        mainMenu.DOAnchorPos(onScreenPos, 0.25f).SetDelay(0.25f);
    }

    // Slide out main menu and bring in rules UI
    public void MenuOutRulesIn()
    {
        mainMenu.DOAnchorPos(offScreenPos, 0.25f);
        RulesUI.DOAnchorPos(onScreenPos, 0.25f).SetDelay(0.25f);
    }

    // Slide out rules UI and bring back main menu
    public void MenuInRulesOut()
    {
        RulesUI.DOAnchorPos(offScreenPos, 0.25f);
        mainMenu.DOAnchorPos(onScreenPos, 0.25f).SetDelay(0.25f);
    }

    // Transition to gameplay UI and hide difficulty UI
    public void GamePlayInDifficultyOut()
    {
        difficultyUI.DOAnchorPos(offScreenPos, 0.25f);
        gamePlayUI.DOAnchorPos(onScreenPos, 0.25f).SetDelay(0.25f);
        ToggleBackground(false); // Hide background during gameplay
    }

    // Transition from gameplay UI back to main menu
    public void GamePlayOutMenuIn()
    {
        basketballScript.canPlay = false;
        ToggleBackground(true);

        gamePlayUI.DOAnchorPos(offScreenPos, 0.25f);
        mainMenu.DOAnchorPos(onScreenPos, 0.25f).SetDelay(0.25f);

        basketballScript.timeSlider.gameObject.SetActive(false);
        basketballScript.isTraining = false;

        DestroyAllPopups(); // Clean up any lingering popups

        Debug.Log("Returned to Main Menu.");
    }

    // Slide in settings UI
    public void SlideSettingsUI()
    {
        settingsUI.DOScale(Vector2.one, 0.25f).SetEase(Ease.OutBack);
    }

    // Slide out settings UI
    public void UndoSettingsUI()
    {
        settingsUI.DOScale(Vector2.zero, 0.25f).SetEase(Ease.InBack);
    }

    // Toggle background visibility
    private void ToggleBackground(bool isActive)
    {
        background.SetActive(isActive);
    }

    // Destroy all active popups
    private void DestroyAllPopups()
    {
        // Find all GameObjects with the ScorePopupAnim component
        ScorePopupAnim[] activePopups = FindObjectsOfType<ScorePopupAnim>();

        foreach (ScorePopupAnim popup in activePopups)
        {
            Destroy(popup.gameObject);
        }
    }
}
