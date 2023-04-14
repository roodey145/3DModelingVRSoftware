using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionsSpeedCalculator
{

    private static Vector3 rotation;

  

    private static bool xRoationActive;
    private static bool yRoationActive;
    private static bool zRoationActive;

    public static float CalculateSpeed(InteractionsEnum actionEnum, Vector3 start, Vector3 end, float timeUsed)
    {
        float speed = 0;
        switch (actionEnum)
        {
            case InteractionsEnum.ScaleUp:
            case InteractionsEnum.ScaleDown:
                speed = CalcScalingSpeed(start, end, timeUsed);
                break;
            case InteractionsEnum.Rotate:
                speed = CalcRotationSpeed(start, end, timeUsed);
                break;
        }

        return speed;
       
    }

    private static float CalcScalingSpeed(Vector3 start, Vector3 end, float timeUsed)
    {
        //throw new NotImplementedException();

        float scalingSpeed = 0;

        return scalingSpeed = end.x - start.x;

    }

    private static float CalcRotationSpeed(Vector3 start, Vector3 end, float timeUsed)
    {
        float rotationSpeed = 0;

        if (xRoationActive)
        {
            rotationSpeed = end.x - start.x;
        }

        else if (yRoationActive)
        {
            rotationSpeed = end.y - start.y;
        }

        else if (zRoationActive)
        {
            rotationSpeed = end.z - start.z;
        }

        return rotationSpeed / timeUsed;
    }

    private static float CalcMovingpeed(Vector3 start, Vector3 end, float timeUsed)
    {
        //throw new NotImplementedException();
        Vector3 calculatedVec3;

        calculatedVec3 = (end - start);


        return (Mathf.Sqrt((calculatedVec3.x * calculatedVec3.x) + 
                                      (calculatedVec3.y * calculatedVec3.y) +
                                      (calculatedVec3.z * calculatedVec3.z)))/timeUsed;


    }
    private static float CalcExtudepeed(Vector3 start, Vector3 end, float timeUsed)
    {
        //throw new NotImplementedException();

        return MathF.Abs((end.x - start.x)/timeUsed);


    }
}
