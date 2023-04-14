using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public List<SoundClip> SoundClips;
    [Range(0.0f, 1.0f)] public float volume = .0f;
    public float soundSpeed = 2.0f;

    private static AudioSource audioSource;
    [System.Serializable]
    public class SoundClip
    {
        public string name;
        public AudioClip clip;
    }

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }


        audioSource = GetComponent<AudioSource>();
    }

    // Play sound with given AudioClip and sound speed
    public static void PlaySound(AudioClip clip, float speed)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: AudioClip is null.");
            return;
        }

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.clip = clip;
        audioSource.pitch = speed;
        audioSource.Play();
    }

    // Stop currently playing sound
    public static void Stop()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // Set volume of the sound
    public void SetVolume(float volume)
    {
        this.volume = volume;
        audioSource.volume = volume;
    }

    // Set speed of the sound
    public void SetSoundSpeed(float speed)
    {
        soundSpeed = speed;
        audioSource.pitch = speed;
    }
}
