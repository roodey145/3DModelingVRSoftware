using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LoopCutDemoInfo;

public class ExtrudeDemoInfo
{
    public Color color = Color.black;
    public Vector3[] vertices = new Vector3[4];
    public Face face;
    public Face[] faces = new Face[4];
    public float extrudeAmount = 0f;

    public ExtrudeDemoInfo(Face faceToExtrude)
    {
        face = faceToExtrude;
    }

    public void SetExtrudeAmount(float extrudeAmount)
    {
        this.extrudeAmount = extrudeAmount;
    }


    private void _CalcVerticesPosition(List<Vector3> vertices)
    {
        // Calculate the normal of the selected face
        int baseVertixIndex = face.GetVertixIndex(Face.VerticesPos.bottomRight);
        Vector3 a = vertices[face.GetVertixIndex(Face.VerticesPos.topRight)] - vertices[baseVertixIndex];
        Vector3 b = vertices[face.GetVertixIndex(Face.VerticesPos.bottomLeft)] - vertices[baseVertixIndex];
        Vector3 normal = Vector3.Cross(a, b).normalized; // TODO: This is probably in the opposite direction

        // Create a copy of the vertices and move them along the face normal vector
        this.vertices[(int)Face.VerticesPos.topLeft] =
            vertices[face.GetVertixIndex(Face.VerticesPos.topLeft)] + normal * extrudeAmount;

        this.vertices[(int)Face.VerticesPos.topRight] =
            vertices[face.GetVertixIndex(Face.VerticesPos.topRight)] + normal * extrudeAmount;

        this.vertices[(int)Face.VerticesPos.bottomRight] =
            vertices[face.GetVertixIndex(Face.VerticesPos.bottomRight)] + normal * extrudeAmount;

        this.vertices[(int)Face.VerticesPos.bottomLeft] =
            vertices[face.GetVertixIndex(Face.VerticesPos.bottomLeft)] + normal * extrudeAmount;
    }

    public void draw(List<Vector3> vertices)
    {
        _CalcVerticesPosition(vertices);
        // Draw the new faces
        Gizmos.color = color;
        // The front face
        Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.topLeft], this.vertices[(int)Face.VerticesPos.topRight]);
        Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.topRight], this.vertices[(int)Face.VerticesPos.bottomRight]);
        Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.bottomRight], this.vertices[(int)Face.VerticesPos.bottomLeft]);
        Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.bottomLeft], this.vertices[(int)Face.VerticesPos.topLeft]);

        // The top face relative to the selected face
        Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.topLeft], vertices[face.GetVertixIndex(Face.VerticesPos.topLeft)]);
        Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.topRight], vertices[face.GetVertixIndex(Face.VerticesPos.topRight)]);

        // The bottom face relative to the selected face
        Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.bottomLeft], vertices[face.GetVertixIndex(Face.VerticesPos.bottomLeft)]);
        Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.bottomRight], vertices[face.GetVertixIndex(Face.VerticesPos.bottomRight)]);

        // The left face relative to the selected face
        //Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.bottomLeft], vertices[face.GetVertixIndex(Face.VerticesPos.bottomLeft)]);
        //Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.bottomLeft], vertices[face.GetVertixIndex(Face.VerticesPos.bottomLeft)]);

        //// The right face relative to the selected face
        //Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.bottomLeft], vertices[face.GetVertixIndex(Face.VerticesPos.bottomLeft)]);
        //Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.bottomLeft], vertices[face.GetVertixIndex(Face.VerticesPos.bottomLeft)]);
    }

    public void ApplyChange(List<Vector3> vertices, List<Face> originalFaces)
    {
        // Calculate the position of the vertices
        _CalcVerticesPosition(vertices);
        int verticesCount = vertices.Count;
        // Add the new calculated vertices to the main vertices array
        for(int i = 0; i < this.vertices.Length; i++)
        {
            vertices.Add(this.vertices[i]);
        }

        //---------------------------------
        // Create the new faces
        // Create the faces array
        faces = new Face[4]; // It is 5 because we have the front face as well

        // Create the face vertices index array
        int[] verticesIndex = new int[4];
        //******
        // Create the top face
        faces[(int)Direction.top] = new Face();

        // Create the vertices of the top face
        verticesIndex[(int)Face.VerticesPos.topLeft] = (int)Face.VerticesPos.topLeft + verticesCount;
        verticesIndex[(int)Face.VerticesPos.topRight] = (int)Face.VerticesPos.topRight + verticesCount;
        verticesIndex[(int)Face.VerticesPos.bottomLeft] = face.GetVertixIndex(Face.VerticesPos.topLeft);
        verticesIndex[(int)Face.VerticesPos.bottomRight] = face.GetVertixIndex(Face.VerticesPos.topRight);

        // Assign the vertices indeces to the top face
        faces[(int)Direction.top].SetVerticesIndex((int[])verticesIndex.Clone());


        //******
        // Create the bottom face
        faces[(int)Direction.right] = new Face();

        // Create the vertices of the top face
        verticesIndex[(int)Face.VerticesPos.topRight] = (int)Face.VerticesPos.topRight + verticesCount;
        verticesIndex[(int)Face.VerticesPos.bottomRight] = (int)Face.VerticesPos.bottomRight + verticesCount;
        verticesIndex[(int)Face.VerticesPos.bottomLeft] = face.GetVertixIndex(Face.VerticesPos.bottomRight);
        verticesIndex[(int)Face.VerticesPos.topLeft] = face.GetVertixIndex(Face.VerticesPos.topRight);

        // Assign the vertices indeces to the top face
        faces[(int)Direction.right].SetVerticesIndex((int[])verticesIndex.Clone());


        //******
        // Create the bottom face
        faces[(int)Direction.bottom] = new Face();

        // Create the vertices of the top face
        verticesIndex[(int)Face.VerticesPos.topLeft] = (int)Face.VerticesPos.bottomLeft + verticesCount;
        verticesIndex[(int)Face.VerticesPos.topRight] = (int)Face.VerticesPos.bottomRight + verticesCount;
        verticesIndex[(int)Face.VerticesPos.bottomLeft] = face.GetVertixIndex(Face.VerticesPos.bottomLeft);
        verticesIndex[(int)Face.VerticesPos.bottomRight] = face.GetVertixIndex(Face.VerticesPos.bottomRight);

        // Assign the vertices indeces to the top face
        faces[(int)Direction.bottom].SetVerticesIndex((int[])verticesIndex.Clone());


        //******
        // Create the left face
        faces[(int)Direction.left] = new Face();

        // Create the vertices of the top face
        verticesIndex[(int)Face.VerticesPos.topRight] = (int)Face.VerticesPos.topLeft + verticesCount;
        verticesIndex[(int)Face.VerticesPos.bottomRight] = (int)Face.VerticesPos.bottomLeft + verticesCount;
        verticesIndex[(int)Face.VerticesPos.topLeft] = face.GetVertixIndex(Face.VerticesPos.topLeft);
        verticesIndex[(int)Face.VerticesPos.bottomLeft] = face.GetVertixIndex(Face.VerticesPos.bottomLeft);

        // Assign the vertices indeces to the top face
        faces[(int)Direction.left].SetVerticesIndex((int[])verticesIndex.Clone());


        //-----------------------
        // Assign the neighbour of the faces

        //******
        // The top face
        faces[(int)Direction.top].SetFace(face.GetNeighbourFace(face, Direction.top), Direction.top); // Top
        faces[(int)Direction.top].SetFace(face, Direction.bottom); // Bottom
        faces[(int)Direction.top].SetFace(faces[(int)Direction.right], Direction.right); // Right
        faces[(int)Direction.top].SetFace(faces[(int)Direction.left], Direction.left); // Left

        //******
        // The bottom face
        faces[(int)Direction.bottom].SetFace(face, Direction.top); // Top
        faces[(int)Direction.bottom].SetFace(face.GetNeighbourFace(face, Direction.bottom), Direction.bottom); // Bottom
        faces[(int)Direction.bottom].SetFace(faces[(int)Direction.right], Direction.right); // Right
        faces[(int)Direction.bottom].SetFace(faces[(int)Direction.left], Direction.left); // Left


        //******
        // The left face
        faces[(int)Direction.left].SetFace(faces[(int)Direction.top], Direction.top); // Top
        faces[(int)Direction.left].SetFace(face.GetNeighbourFace(face, Direction.left), Direction.left); // Left
        faces[(int)Direction.left].SetFace(faces[(int)Direction.bottom], Direction.bottom); // Bottom
        faces[(int)Direction.left].SetFace(face, Direction.right); // Right


        //******
        // The right face
        faces[(int)Direction.right].SetFace(faces[(int)Direction.top], Direction.top); // Top
        faces[(int)Direction.right].SetFace(face, Direction.left); // Left
        faces[(int)Direction.right].SetFace(faces[(int)Direction.bottom], Direction.bottom); // Bottom
        faces[(int)Direction.right].SetFace(face.GetNeighbourFace(face, Direction.right), Direction.right); // Right

        // Assign the new neighbour to the face old neighbour
        face.GetNeighbourFace(face, Direction.top).ReplaceNeighbourFace(face, faces[(int)Direction.top]);
        face.GetNeighbourFace(face, Direction.right).ReplaceNeighbourFace(face, faces[(int)Direction.right]);
        face.GetNeighbourFace(face, Direction.bottom).ReplaceNeighbourFace(face, faces[(int)Direction.bottom]);
        face.GetNeighbourFace(face, Direction.left).ReplaceNeighbourFace(face, faces[(int)Direction.left]);

        // Update the selected face to the front face
        face.SetFace(faces[(int)Direction.top], Direction.bottom);
        face.SetFace(faces[(int)Direction.left], Direction.right);
        face.SetFace(faces[(int)Direction.bottom], Direction.top);
        face.SetFace(faces[(int)Direction.right], Direction.left);

        verticesIndex[(int)Face.VerticesPos.topLeft] = (int)Face.VerticesPos.topLeft + verticesCount;
        verticesIndex[(int)Face.VerticesPos.topRight] = (int)Face.VerticesPos.topRight + verticesCount;
        verticesIndex[(int)Face.VerticesPos.bottomRight] = (int)Face.VerticesPos.bottomRight + verticesCount;
        verticesIndex[(int)Face.VerticesPos.bottomLeft] = (int)Face.VerticesPos.bottomLeft + verticesCount;

        face.SetVerticesIndex(verticesIndex);

        // Add the new created faces to the faces list
        for(int i = 0; i < faces.Length; i++)
        {
            originalFaces.Add(faces[i]);
        }
    }
}
