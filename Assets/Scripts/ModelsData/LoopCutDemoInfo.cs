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

        }
    }

    public void ApplyChange(List<Vector3> originalVertices, List<Face> originalFaces)
    {
        for (int i = 0; i < faces.Count; i++)
        {
            // Remove the old face from the orignal faces list
            originalFaces.Remove(faces[i].original);
        }

        int nextFaceIndex = -1;
        int previousFaceIndex = -1;
        int leftFaceIndex = (int)Direction.left < (int)Direction.right ? 0 : 1;
        int rightFaceIndex = leftFaceIndex == 0 ? 1 : 0;
        int topFaceIndex = (int)Direction.top < (int)Direction.bottom ? 0 : 1;
        int bottomFaceIndex = topFaceIndex == 0 ? 1 : 0;


        for (int i = 0; i < vertices.Count / 2; i++)
        {
            originalVertices.Add(vertices[(i * 2)]);
            originalVertices.Add(vertices[(i * 2) + 1]);

            // Get the index of the next face
            nextFaceIndex = i == faces.Count - 1 ? 0 : i + 1;
            // Get the index of the previous face
            previousFaceIndex = i == 0 ? faces.Count - 1 : i - 1;

            // Get the direction of the next face
            Direction nextFaceDirection = 
                faces[i].original.GetNeighbourDirection(faces[nextFaceIndex].original);
            Direction previousFaceDirection =
                faces[i].original.GetNeighbourDirection(faces[previousFaceIndex].original);

            //------------------------------------
            // Assign the subfaces neighbour faces

            // Check if this face was cut verticlly
            if (direction[i] == Direction.verctial)
            { // This face was cut verticlly thus right and left faces are preserved

                // Assign the preserved faces - 1 face assigned
                faces[i].subFaces[leftFaceIndex].SetFace(
                    faces[i].original.GetNeighbourFace(faces[i].original, Direction.left),
                    Direction.left);
                faces[i].subFaces[rightFaceIndex].SetFace(
                    faces[i].original.GetNeighbourFace(faces[i].original, Direction.right),
                    Direction.right);

                // Assign the faces to them selve - 2 faces assigned
                faces[i].subFaces[leftFaceIndex].SetFace(
                    faces[i].subFaces[rightFaceIndex],
                    Direction.right);
                faces[i].subFaces[rightFaceIndex].SetFace(
                    faces[i].subFaces[leftFaceIndex]
                    , Direction.left);

                // Assign the subfaces of the next face at their respective directions - 3 faces assigned
                faces[i].subFaces[leftFaceIndex].SetFace(
                    faces[nextFaceIndex].subFaces[leftFaceIndex],
                    nextFaceDirection);
                faces[i].subFaces[rightFaceIndex].SetFace(
                    faces[nextFaceIndex].subFaces[rightFaceIndex],
                    nextFaceDirection);

                // Assign the subfaces of the previous face at their respective directions - 4 faces assigned
                faces[i].subFaces[leftFaceIndex].SetFace(
                    faces[previousFaceIndex].subFaces[leftFaceIndex],
                    previousFaceDirection);
                faces[i].subFaces[rightFaceIndex].SetFace(
                    faces[previousFaceIndex].subFaces[rightFaceIndex],
                    previousFaceDirection);

            }
            else if (direction[i] == Direction.horizontal)
            {
                // Assign the preserved faces - 1 face assigned
                faces[i].subFaces[topFaceIndex].SetFace(
                    faces[i].original.GetNeighbourFace(faces[i].original, Direction.top),
                    Direction.top);
                faces[i].subFaces[bottomFaceIndex].SetFace(
                    faces[i].original.GetNeighbourFace(faces[i].original, Direction.bottom),
                    Direction.bottom);

                // Assign the faces to them selve - 2 faces assigned
                faces[i].subFaces[topFaceIndex].SetFace(
                    faces[i].subFaces[bottomFaceIndex],
                    Direction.bottom);
                faces[i].subFaces[bottomFaceIndex].SetFace(
                    faces[i].subFaces[topFaceIndex]
                    , Direction.top);

                // Assign the subfaces of the next face at their respective directions - 3 faces assigned
                faces[i].subFaces[topFaceIndex].SetFace(
                    faces[nextFaceIndex].subFaces[topFaceIndex],
                    nextFaceDirection);
                faces[i].subFaces[bottomFaceIndex].SetFace(
                    faces[nextFaceIndex].subFaces[bottomFaceIndex],
                    nextFaceDirection);

                // Assign the subfaces of the previous face at their respective directions - 4 faces assigned
                faces[i].subFaces[topFaceIndex].SetFace(
                    faces[previousFaceIndex].subFaces[topFaceIndex],
                    previousFaceDirection);
                faces[i].subFaces[bottomFaceIndex].SetFace(
                    faces[previousFaceIndex].subFaces[bottomFaceIndex],
                    previousFaceDirection);
            }
            else
            {
                throw new System.Exception($"Expected cut direction of vertical or horizontal but got {direction[i]}");
            }

            // Change the sub faces points
            for (int subFace = 0; subFace < faces[i].subFaces.Length; subFace++)
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
