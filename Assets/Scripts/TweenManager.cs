 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TweenManager : MonoBehaviour
{
    [Header("GameObject UI to be slided in")]
    public RectTransform mainMenu, difficultyUI, gamePlayUI, recordUI, RulesUI, settingsUI;
    public GameObject background;

    private BasketBallScript basketballScript;

    // Update is called once per frame
     void Start()
    {
        transform.LeanMoveLocal(new Vector2 (0, 410), 1).setEaseInOutQuart().setLoopPingPong();
        StartCoroutine(AnimateButtons());
        basketballScript = GameObject.Find("Basketball Manager").GetComponent<BasketBallScript>();
    
    }

    public IEnumerator AnimateButtons()
    { 
        yield return new WaitForSeconds(0.05f);
        mainMenu.DOAnchorPos(new Vector2(0, 200), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f).SetDelay(0.45f);
    }

    public void slideMainMenu()
    {
        mainMenu.DOAnchorPos(Vector2.zero, 0.25f);
    }

    public void slidedifficultyUI()
    {
        mainMenu.DOAnchorPos(new Vector2(0, 1757), 0.25f);
        difficultyUI.DOAnchorPos(new Vector2(0, 0), 0.25f).SetDelay(0.25f);
    }

     public void menuinDiffOut()
    {
        difficultyUI.DOAnchorPos(new Vector2(0, 1757), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f).SetDelay(0.25f);        
    }

    public void menuOutRecordIn()
    {
        mainMenu.DOAnchorPos(new Vector2(0, 1757), 0.25f);
        recordUI.DOAnchorPos(new Vector2(0, 0), 0.25f).SetDelay(0.25f);
    }

     public void menuInRecordOut()
    {
        recordUI.DOAnchorPos(new Vector2(0, 1757), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f).SetDelay(0.25f);        
    }

    public void menuOutRulesIn()
    {
        mainMenu.DOAnchorPos(new Vector2(0, 1757), 0.25f);
        RulesUI.DOAnchorPos(new Vector2(0, 0), 0.25f).SetDelay(0.25f);
    }

     public void menuInRulesOut()
    {
        RulesUI.DOAnchorPos(new Vector2(0, 1757), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f).SetDelay(0.25f);        
    }

     public void GamePlayInDifficultyOut()
    {
        difficultyUI.DOAnchorPos(new Vector2(0, 1757), 0.25f);
        gamePlayUI.DOAnchorPos(new Vector2(0, 0), 0.25f).SetDelay(0.25f);
        background.SetActive(false);
          
    }

     public void GamePlayOutMenuIn()
    {
        basketballScript.canPlay = false;
        background.SetActive(true);
        gamePlayUI.DOAnchorPos(new Vector2(0, 1757), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f).SetDelay(0.25f);
        basketballScript.timeSlider.gameObject.SetActive(true);  // Reset slider active state 
        basketballScript.isTraining = false;    
        DestroyAllPopups();
    }

    private void DestroyAllPopups()
    {
        // Find all GameObjects with the ScorePopupAnim component
        ScorePopupAnim[] activePopups = FindObjectsOfType<ScorePopupAnim>();
        
        foreach (ScorePopupAnim popup in activePopups)
        {
            Destroy(popup.gameObject);
        }
    }



     public void slideSettingsUI()
    {
        settingsUI.DOScale(Vector2.one, 0.25f).SetEase(Ease.OutBack);
    }

    public void undoSettingsUI()
    {
        settingsUI.DOScale(Vector2.zero, 0.25f).SetEase(Ease.InBack);;
    }
}


