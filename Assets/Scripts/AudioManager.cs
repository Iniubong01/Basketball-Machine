using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource, musicSource;

    [SerializeField] private Toggle musicToggle, sfxToggle;    // Toggle for controlling SFX on/off

    [SerializeField] private AudioClip buttonSound;  // Sounds


    // Start is called before the first frame update
    void Start()
    {
        // Initialize toggle values based on the audio source state
        musicToggle.isOn = !musicSource.mute;
        sfxToggle.isOn = !sfxSource.mute;

        // Add listeners to toggle buttons
        musicToggle.onValueChanged.AddListener(OnMusicToggle);
        sfxToggle.onValueChanged.AddListener(OnSFXToggle);
    }
       // Called when the music toggle is changed
    public void OnMusicToggle(bool isOn)
    {
        musicSource.mute = !isOn;  // If toggle is on, unmute; if off, mute
    }

    // Called when the SFX toggle is changed
    public void OnSFXToggle(bool isOn)
    {
        sfxSource.mute = !isOn;  // If toggle is on, unmute; if off, mute
    }

    public void clickSound()
    {
        sfxSource.PlayOneShot(buttonSound);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
