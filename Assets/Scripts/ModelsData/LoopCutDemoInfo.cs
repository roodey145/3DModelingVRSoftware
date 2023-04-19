using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopCutDemoInfo
{
    public class CutInfo
    {
        public Face original;
        public Face[] subFaces;
    }

    public Color color = Color.white;
    public List<Vector3> vertices = new List<Vector3>();
    public List<CutInfo> faces = new List<CutInfo>();
    public List<Direction> direction = new List<Direction>();

    public LoopCutDemoInfo()
    {

    }

    public void SetNewFace(Face original, Face[] splittedPart, Direction cutDirection)
    {
        CutInfo cutInfo = new CutInfo();
        cutInfo.original = original;
        cutInfo.subFaces = splittedPart;
        faces.Add(
            cutInfo
         );

        direction.Add(cutDirection); // The cut direction (horizontal, vertical)

        //for(int i = 0; i < newVertices.Length; i++)
        //{
        //    vertices.Add(newVertices[i]);
        //}
    }


    public void draw()
    {
        // Draw the demo
        Gizmos.color = color;

        // Connect the vertices
        for(int i = 0; i < vertices.Count/2; i++)
        {
            Gizmos.DrawLine(vertices[(i * 2)], vertices[(i * 2) + 1]);

            //if (i < vertices.Count - 1)
            //{
            //    Gizmos.DrawLine(vertices[i], vertices[i + 1]);
            //}
            //else
            //{
            //    Gizmos.DrawLine(vertices[i], vertices[0]);
            //}
        }

        //for (int i = 0; i < faces.Count; i++)
        //{
        //    for (int subFaces = 0; subFaces < faces[i][1].Length; subFaces++)
        //    {
        //        faceVertices = faces[i][1][subFaces].GetLines(Direction.all);

        //        for (int l = 0; l < faceVertices.Length; l++)
        //        {

        //            Gizmos.DrawLine(vertices[faceVertices[l].x], vertices[faceVertices[l].y]);
        //        }
        //    }

        //}
    }

    public void ApplyChange(List<Vector3> originalVertices, List<Face> originalFaces)
    {
        for (int i = 0; i < faces.Count; i++)
        {
            // Remove the old face from the orignal faces list
            originalFaces.Remove(faces[i].original);
        }


        for (int i = 0; i < vertices.Count / 2; i++)
        {
            originalVertices.Add(vertices[(i * 2)]);
            originalVertices.Add(vertices[(i * 2) + 1]);


            // Change the sub faces points
            for(int subFace = 0; subFace < faces[i].subFaces.Length; subFace++)
            {
                faces[i].subFaces[subFace].UpdateVerticesIndex(
                new Vector2Int((i * 2), (i * 2) + 1),
                new Vector2Int(originalVertices.Count - 2, originalVertices.Count - 1));

                // Add the subfaces to the list
                originalFaces.Add(faces[i].subFaces[subFace]);

            }
            
        }
    }
}
