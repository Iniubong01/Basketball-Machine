using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource, musicSource;
    [SerializeField] private Toggle musicToggle, sfxToggle;    // Toggle for controlling SFX on/off
    [SerializeField] private AudioClip buttonSound;  // Sounds

    public static AudioManager Instance { get; private set; } // Singleton for global access

    private void Awake()
    {
        // Set up the singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

    public bool IsSFXMuted()
    {
        return sfxSource.mute;
    }

    public void clickSound()
    {
        if (!IsSFXMuted()) // Only play sound if SFX is not muted
        {
            sfxSource.PlayOneShot(buttonSound);
        }
    }
}
