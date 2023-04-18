using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bevel : MonoBehaviour
{
    public bool createNewPoint;
    public bool isEdges;
    public Vector3[] SelectedCornerPostion;
    public Vector3[] cornor1Points;
    public Vector3[] cornor2Points;

    public Vector3[] calculatedVectors;

    public Vector3[] newCorner1Points;
    public Vector3[] newCorner2Points;

    public float BevelValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isEdges) { 
        //create new points
            if (!createNewPoint)
                for (int i = 0; i < SelectedCornerPostion.Length; i++)//2
                {
                    newCorner1Points[i] = SelectedCornerPostion[i];
                    newCorner2Points[i] = SelectedCornerPostion[i];
                }
            createNewPoint = true;
            // delete old edge


            // make the new point move along the calcuated vector
            newCorner1Points[0] = SelectedCornerPostion[0] + (calculatedVectors[0] * BevelValue);
            newCorner1Points[1] = SelectedCornerPostion[1] + (calculatedVectors[1] * BevelValue);
            newCorner2Points[0] = SelectedCornerPostion[0] + (calculatedVectors[2] * BevelValue);
            newCorner2Points[1] = SelectedCornerPostion[1] + (calculatedVectors[3] * BevelValue);

            //create face from new points
            Face bevelFace = new Face();
            int[] facePoints = new int[4];

            facePoints[(int)Face.VerticesPos.topLeft] = 1;


        }

    }

    public void Beveling()
    {
        calculatedVectors[0] = cornor1Points[0] - SelectedCornerPostion[0];
        calculatedVectors[1] = cornor1Points[1] - SelectedCornerPostion[1];
        calculatedVectors[2] = cornor2Points[0] - SelectedCornerPostion[0];
        calculatedVectors[3] = cornor2Points[1] - SelectedCornerPostion[1];


            
   
    }




}
