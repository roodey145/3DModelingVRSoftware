using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class perspective3D : MonoBehaviour
{
    // Start is called before the first frame update
    public static Matrix4x4 PerspectiveMatrix(Vector3 position, Vector3 rotation, Vector3 scale)
    {

        // TODO create the translation matrix
        Matrix4x4 T = Matrix4x4.identity;
        T.m03 = position.x;
        T.m13 = position.y;
        T.m23 = position.z;

        // TODO create the scale matrix
        Matrix4x4 S = Matrix4x4.identity;
        S.m00 = scale.x;
        S.m11 = scale.y;
        S.m22 = scale.z;


        // TODO create three rotation matrices, one per rotation axis/euler angle
        Matrix4x4 RX = Matrix4x4.identity;
        float angleRadX = Mathf.Deg2Rad * rotation.x;
        RX.m11 = Mathf.Cos(angleRadX);
        RX.m21 = Mathf.Sin(angleRadX);
        RX.m12 = -Mathf.Sin(angleRadX);
        RX.m22 = Mathf.Cos(angleRadX);
        Matrix4x4 RY = Matrix4x4.identity;
        float angleRadY = Mathf.Deg2Rad * rotation.y;
        RY.m00 = Mathf.Cos(angleRadY);
        RY.m02 = Mathf.Sin(angleRadY);
        RY.m20 = -Mathf.Sin(angleRadY);
        RY.m22 = Mathf.Cos(angleRadY);
        Matrix4x4 RZ = Matrix4x4.identity;
        // rotation.z/RZ is given as an example
        float angleRad = Mathf.Deg2Rad * rotation.z;
        RZ.m00 = Mathf.Cos(angleRad);
        RZ.m10 = Mathf.Sin(angleRad);
        RZ.m01 = -Mathf.Sin(angleRad);
        RZ.m11 = Mathf.Cos(angleRad);

        // TODO concatenate the three matrices together
        // remember that, when using euler angles, rotation order matters!
        // by default, Unity implements the order Rotation Z -> then X -> then Y
        // notice that, since we use column vectors, transformations are applied from RIGHT TO LEFT
        Matrix4x4 R = RY * RX * RZ; // R = multiply RX, RY and RZ in the CORRECT ORDER

        // TODO concatenate the transformations into a single matrix
        // we first Scale -> then Rotate -> and then Translate
        // notice that, since we use column vectors, transformations are applied from RIGHT TO LEFT
        Matrix4x4 TRSMatrix = T * R * S; // TRSMatrix = multiply translation, scale and rotation in the CORRECT ORDER


        return TRSMatrix;
    }
}
