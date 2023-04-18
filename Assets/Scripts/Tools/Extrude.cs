using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extrude : MonoBehaviour
{
    public bool faceIsSelected;
    public bool createNewPoint;
    public Vector3 normal;
    public Vector3[] oldPointPostion;
    public Vector3[] newPointPostion;
    public float extudeValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (faceIsSelected)
        {
            //create new points
            if(!createNewPoint)
            for (int i = 0; i <= oldPointPostion.Length; i++)
            {
                oldPointPostion[i] = newPointPostion[i];
            }
            createNewPoint = true;

            // make the new point move along the norm
            for (int i = 0; i < newPointPostion.Length; i++)
            {
                newPointPostion[i] = oldPointPostion[i] + (normal * extudeValue);
            }

            // keep updating the faces
            for (int i = 0; i < newPointPostion.Length; i++)
            {
                //squar
                    // triangle 1
                    //newPointPostion[i] oldPointPostion[i] newPointPostion[i+1]
                    // triangle 2
                    //oldPointPostion[i] oldPointPostion[i + 1] newPointPostion[i + 1]
            }



            //create new points at the same place as the faces points
            // make the new points follow the norm vecter
            //create new faces
        }
    }

    void SelectFace()
    {
        Vector3 vectorA;
        Vector3 vectorB;

        //normal = Vector3.Cross(vectorA, vectorB);
        faceIsSelected = true;
        
    }

    // select
        //find the norm
        //get positon of x amount of points
        // the normal have to be postion in the points local space


    // create wall from points
}
