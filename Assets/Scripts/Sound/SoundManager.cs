using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Controlling properties and variables
    private static float interactionSoundVolume = 1f;

    internal static float InteractionSoundVolume
    {
        get { return interactionSoundVolume; }
        set { 
            interactionSoundVolume = value;
            SetVolume(value);
        }
    }

    private static float backgroundSoundVolume = 1f;

    internal static float BackgroundSoundVolume
    {
        get { return backgroundSoundVolume; }
        set
        {
            backgroundSoundVolume = value;
            SetBackgroundVolume(value);
        }
    }

    private static bool interactionSoundPlaying = true;

    internal static bool InteractionSoundPlaying
    {
        get { return interactionSoundPlaying; }
        set
        {
            interactionSoundPlaying = value;
            // Set the volume zero if the sound is turned off, otherwise, returned it to normal volume.
            SetVolume(!value ? 0 : interactionSoundVolume);
        }
    }

    private static bool backgroundSoundPlaying = true;

    internal static bool BackgroundSoundPlaying
    {
        get { return backgroundSoundPlaying; }
        set
        {
            backgroundSoundPlaying = value;
            // Set the volume zero if the sound is turned off, otherwise, returned it to normal volume.
            SetBackgroundVolume(!value ? 0 : backgroundSoundVolume);
        }
    }

    #endregion

    public static SoundManager Instance { get; private set; }

    public List<AudioClip> SoundClips;
    [Range(0.0f, 1.0f)] public float volume = .0f;
    public float soundSpeed = 2.0f;

    private static AudioSource audioSource;
    private static AudioSource backgroundAudioSource;
    //[System.Serializable]
    //public class SoundClip
    //{
    //    public string name;
    //    public AudioClip clip;
    //}

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
    public static void PlayInteractionSound(AudioClip clip, float speed)
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
    public static void StopInteractionSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // Set volume of the sound
    private static void SetVolume(float volume)
    {
        //this.volume = volume;
        audioSource.volume = volume;
    }

    private static void SetBackgroundVolume(float volume)
    {
        backgroundAudioSource.volume = volume;
    }

    // Set speed of the sound
    public static void SetSoundSpeed(float speed)
    {
        //soundSpeed = speed;
        audioSource.pitch = speed;
    }
}
