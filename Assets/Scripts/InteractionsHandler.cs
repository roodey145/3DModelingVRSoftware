using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionsHandler : MonoBehaviour
{
    

    public static void HandleInteraction(InteractionsEnum interaction, Vector3 start, Vector3 end, float timeUsed)
    {
        float speed = InteractionsSpeedCalculator.CalculateSpeed(interaction, start, end, timeUsed);
        // Get the interaction sound
        AudioClip sound = Sounds.GetInteractionSoundClip(interaction);
        // Play the interaction sound
        SoundManager.PlayInteractionSound(sound, speed);
        switch (interaction)
        {
            case InteractionsEnum.ScaleUp:
            case InteractionsEnum.ScaleDown:
                _handleScaling(interaction, start, end, timeUsed, speed); 
                break;
            case InteractionsEnum.Grab:
                _handleGrabing(interaction, start, end, timeUsed, speed);
                break;
            case InteractionsEnum.Move:
                _handleMovement(interaction, start, end, timeUsed, speed);
                break;
            case InteractionsEnum.Rotate:
                _handleRotation(interaction, start, end, timeUsed, speed);
                break;
            case InteractionsEnum.Extrude:
                _handleLoopCutting(interaction, start, end, timeUsed, speed);
                break;
            case InteractionsEnum.LoopCut:
                _handleExtruding(interaction, start, end, timeUsed, speed);
                break;
            case InteractionsEnum.Bevel:
                _handleBeveling(interaction, start, end, timeUsed, speed);
                break;
            case InteractionsEnum.Confirm:
                _handleConfirm(interaction, start, end, timeUsed, speed);
                break;
        }
    }


    private static void _handleScaling(InteractionsEnum interaction, Vector3 start, Vector3 end, float timeUsed, float speed)
    {
        
    }

    private static void _handleMovement(InteractionsEnum interaction, Vector3 start, Vector3 end, float timeUsed, float speed)
    {

    }

    private static void _handleGrabing(InteractionsEnum interaction, Vector3 start, Vector3 end, float timeUsed, float speed)
    {

    }


    private static void _handleRotation(InteractionsEnum interaction, Vector3 start, Vector3 end, float timeUsed, float speed)
    {

    }

    private static void _handleExtruding(InteractionsEnum interaction, Vector3 start, Vector3 end, float timeUsed, float speed)
    {

    }


    private static void _handleLoopCutting(InteractionsEnum interaction, Vector3 start, Vector3 end, float timeUsed, float speed)
    {

    }

    private static void _handleBeveling(InteractionsEnum interaction, Vector3 start, Vector3 end, float timeUsed, float speed)
    {

    }


    private static void _handleConfirm(InteractionsEnum interaction, Vector3 start, Vector3 end, float timeUsed, float speed)
    {

    }
}
