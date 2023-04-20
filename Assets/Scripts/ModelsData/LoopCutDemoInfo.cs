using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Note:
/// // The vertices.Count/originalVertices.Count part, why are we plusing it to index?
///     // The vertices are saved in the main vertices list, however, the demo vertices
///     // are saved in a new list, since the face which we are cutting has indeces of the
///     // vertices which are stored in the main vertices list, the subfaces which will be
///     // created when cutting a face will also contain the indeces of the vertices whcih
///     // are stored in the main vertices list. This cause a problem to the new created
///     // subfaces in contrary to the main faces, because the main faces contains only
///     // the indeces of the vertices which are contained in the main vertices list, however
///     // the subfaces contains the indeces of the vertices which are contained in the 
///     // demo vertices list, therefore, some of the indeces will be duplicated. This problem
///     // can be solved in many different ways, however, the chosen way her is to add the 
///     // number of vertices stored in the main vertices list to ensure non of the indeces 
///     // will be duplicated.
///     // The vertices.Count is first added inside the _CutVertically and _CutHorizontally 
///         // methods which are inside the model(Cube) class.
/// </summary>

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
                // To understand why the originalVertices.Count is added, read the summery
                faces[i].subFaces[subFace].UpdateVerticesIndex(
                new Vector2Int((i * 2) + originalVertices.Count, (i * 2) + 1 + originalVertices.Count),
                new Vector2Int(originalVertices.Count - 2, originalVertices.Count - 1));

                // Add the subfaces to the list
                originalFaces.Add(faces[i].subFaces[subFace]);

            }
            
        }
    }
}
