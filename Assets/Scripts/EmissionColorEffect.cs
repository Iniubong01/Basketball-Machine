using UnityEngine;
using System.Collections;

public class EmissionColorEffect : MonoBehaviour
{
    [SerializeField] Material material;
    Color scoreColor = Color.yellow;
    float emissionIntensity = 80f;
    private bool isScoring;
    
    private void Start()
    {
        Reset();
    }

    private void Reset()
    {
        material.SetColor("_EmissionColor", scoreColor * emissionIntensity);
    }

    public void OnScoring()
    {
        if (!isScoring)
            StartCoroutine(ScoreEffect());
    }

    private IEnumerator ScoreEffect()
    {
        isScoring = true;
        for (int i = 0; i < 3; i++)
        {
            material.SetColor("_EmissionColor", scoreColor * emissionIntensity);
            yield return new WaitForSeconds(0.08f);
            material.SetColor("_EmissionColor", Color.black);
            yield return new WaitForSeconds(0.08f);
        }

        isScoring = false;
        Reset();
    }
}
