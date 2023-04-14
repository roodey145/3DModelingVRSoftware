using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundUIController : MonoBehaviour
{
    private Slider interactionSoundSlider;
    private Slider backgroundSoundSlider;

    private void Awake()
    {
        interactionSoundSlider = GetComponent<Slider>();
        backgroundSoundSlider = GetComponent<Slider>();
        interactionSoundSlider.onValueChanged.AddListener(ChangeInteractionSoundVolume);
        backgroundSoundSlider.onValueChanged.AddListener(ChangeBackgroundSoundVolume);
    }

    private void ChangeInteractionSoundVolume(float value)
    {
        SoundManager.InteractionSoundVolume = value;

        if (SoundManager.InteractionSoundVolume > 0f)
        {
            SoundManager.InteractionSoundPlaying = true;
        }
        else
        {
            SoundManager.InteractionSoundPlaying = !SoundManager.InteractionSoundPlaying;
        }
    }

    private void ChangeBackgroundSoundVolume(float value)
    {
        SoundManager.BackgroundSoundVolume = value;

        if (SoundManager.BackgroundSoundVolume > 0f)
        {
            SoundManager.BackgroundSoundPlaying = true;
        }
        else
        {
            SoundManager.BackgroundSoundPlaying = !SoundManager.BackgroundSoundPlaying;
        }
    }


    public void ToggleInteractionSound()
    {
        SoundManager.InteractionSoundPlaying = !SoundManager.InteractionSoundPlaying;
    }

    public void ToggleBackgroundSound()
    {
        SoundManager.BackgroundSoundPlaying = !SoundManager.BackgroundSoundPlaying;
    }
}
