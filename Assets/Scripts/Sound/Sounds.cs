using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public enum Scene
    {
        
    }

    private static AudioClip[] interactionSounds;
    private static AudioClip[] backgroundSounds;
    // Start is called before the first frame update


    private void Awake()
    {
        // Loading the audio from the resources folder
        interactionSounds = Resources.LoadAll<AudioClip>("Sounds/Interaction");
        backgroundSounds = Resources.LoadAll<AudioClip>("Sounds/Background");

    }

    /// <summary>
    /// Uses the interaction type to retrieve its sound.
    /// </summary>
    /// <param name="InteractionEnum">The interaction type.</param>
    /// <returns>The sound which shall play.</returns>
    public static AudioClip GetInteractionSoundClip(InteractionsEnum InteractionEnum)
    {
        //AudioClip audio = _GetInteractionAudioClip(InteractionEnum.name());
        // Incase we want to edit the audio, we can do it here
        return _GetInteractionAudioClip(InteractionEnum.name());
    }
    public static AudioClip GetBackgroundSoundClip(Scene SceneEnum)
    {
        AudioClip audio = null;
        switch (SceneEnum)
        {

            /*case Scene.textHere:
                return interactionSounds[x];
                break;
            */
            default:
                audio = backgroundSounds[0];
                break;
        } 
        return audio;
    }


    // Internal help functions

    private static AudioClip _GetInteractionAudioClip(string audioName)
    {
        AudioClip audio = null;

        foreach(AudioClip clip in interactionSounds)
        {
            if(clip.name == audioName)
            { // The target audio has been found
                audio = clip;
                break;
            }
        }
        
        return audio;
    }

}
