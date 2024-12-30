using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private AudioSource audioSource; // AudioSource will be added dynamically
    [SerializeField] private AudioClip bounceSound;   // Clip to play on bounce
    [SerializeField] private float minVolume = 0.2f;  // Minimum sound volume
    [SerializeField] private float maxVolume = 1f;    // Maximum sound volume
    [SerializeField] private float volumeScaleFactor = 0.01f; // Scale impulse to volume

    private void Awake()
    {
        // Ensure an AudioSource component is added dynamically
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Since the audio source is not going to be manually dragged into the slot in the inspector,
        // set the properties once it is dynamically added
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;  // Makes the sound to 3D, to make it more realistic
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (!AudioManager.Instance.IsSFXMuted() && bounceSound != null && !collision.gameObject.CompareTag("Net") && bounceSound != null)
    //     {
    //         // Calculate collision impulse
    //         float impulse = collision.relativeVelocity.magnitude * collision.rigidbody?.mass ?? 1f;

    //         // Map impulse to a volume range
    //         float volume = Mathf.Clamp(impulse * volumeScaleFactor, minVolume, maxVolume);

    //         // Play the bounce sound with calculated volume
    //         audioSource.PlayOneShot(bounceSound, volume);
    //     }
    // }
}

