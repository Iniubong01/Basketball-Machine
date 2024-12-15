using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class JumpSimAnim : MonoBehaviour
{
    public RectTransform mainMenu;
    // Update is called once per frame
     void Start()
    {
        transform.LeanMoveLocal(new Vector2 (0, 410), 1).setEaseInOutQuart().setLoopPingPong();
        StartCoroutine(AnimateButtons());
    
    }

    public IEnumerator AnimateButtons()
    { 
        yield return new WaitForSeconds(0.05f);
        mainMenu.DOAnchorPos(new Vector2(0, 200), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f).SetDelay(0.45f);
    }
    
        
}
