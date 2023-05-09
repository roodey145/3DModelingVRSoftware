using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundUIController : MonoBehaviour
{
    private Slider interactionSoundSlider;
    private Slider backgroundSoundSlider;
    public TextMeshProUGUI interactionVolumeProcent;
    public TextMeshProUGUI backgroundVolumeProcent;
    public Image interactionImg;
    public Image backgroundImg;
    public Image[] muteUnmute;
    


    private void Awake()
    {
        interactionSoundSlider = GetComponent<Slider>();
        backgroundSoundSlider = GetComponent<Slider>();
        interactionSoundSlider.onValueChanged.AddListener(ChangeInteractionSoundVolume);
        backgroundSoundSlider.onValueChanged.AddListener(ChangeBackgroundSoundVolume);
        interactionVolumeProcent.text = "Tool volume: " + interactionSoundSlider.value + "%";
        backgroundVolumeProcent.text = "Envirement volume: " + backgroundSoundSlider.value + "%";
    }

    private void ChangeInteractionSoundVolume(float value)
    {
        interactionVolumeProcent.text = "Tool volume: " + value + "%";


        SoundManager.InteractionSoundVolume = value;

        if (SoundManager.InteractionSoundVolume > 0f)
        {
            SoundManager.InteractionSoundPlaying = true;
            interactionImg = muteUnmute[1];
        }
        else
        {
            SoundManager.InteractionSoundPlaying = !SoundManager.InteractionSoundPlaying;
            interactionImg = muteUnmute[0];
        }
    }

    private void ChangeBackgroundSoundVolume(float value)
    {
        backgroundVolumeProcent.text = "Envirement volume: " + value + "%";

        SoundManager.BackgroundSoundVolume = value;

        if (SoundManager.BackgroundSoundVolume > 0f)
        {
            SoundManager.BackgroundSoundPlaying = true;
            backgroundImg = muteUnmute[1];
        }
        else
        {
            SoundManager.BackgroundSoundPlaying = !SoundManager.BackgroundSoundPlaying;
            backgroundImg = muteUnmute[0];
        }
    }


    public void ToggleInteractionSound()
    {
        SoundManager.InteractionSoundPlaying = !SoundManager.InteractionSoundPlaying;
        interactionImg = muteUnmute[0];
    }

    public void ToggleBackgroundSound()
    {
        SoundManager.BackgroundSoundPlaying = !SoundManager.BackgroundSoundPlaying;
        backgroundImg = muteUnmute[0];
    }
}
