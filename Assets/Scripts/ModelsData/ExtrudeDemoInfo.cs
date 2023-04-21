using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LoopCutDemoInfo;

public class ExtrudeDemoInfo
{
    public Color color = Color.black;
    public Vector3[] vertices = new Vector3[4];
    public Face face;
    public float extrudeAmount = 0f;

    public ExtrudeDemoInfo(Face faceToExtrude)
    {
        face = faceToExtrude;
    }

    public void SetExtrudeAmount(float extrudeAmount)
    {
        this.extrudeAmount = extrudeAmount;
    }

    public void draw(List<Vector3> vertices)
    {
        // Calculate the normal of the selected face
        int baseVertixIndex = face.GetVertixIndex(Face.VerticesPos.bottomRight);
        Vector3 a = vertices[face.GetVertixIndex(Face.VerticesPos.topRight)] - vertices[baseVertixIndex];
        Vector3 b = vertices[face.GetVertixIndex(Face.VerticesPos.bottomLeft)] - vertices[baseVertixIndex];
        Vector3 normal = Vector3.Cross(a, b).normalized;

        // Create a copy of the vertices and move them along the face normal vector
        this.vertices[(int)Face.VerticesPos.topLeft] =
            vertices[face.GetVertixIndex(Face.VerticesPos.topLeft)] + normal * extrudeAmount;

        this.vertices[(int)Face.VerticesPos.topRight] = 
            vertices[face.GetVertixIndex(Face.VerticesPos.topRight)] + normal * extrudeAmount;

        this.vertices[(int)Face.VerticesPos.bottomRight] = 
            vertices[face.GetVertixIndex(Face.VerticesPos.bottomRight)] + normal * extrudeAmount;

        this.vertices[(int)Face.VerticesPos.bottomLeft] = 
            vertices[face.GetVertixIndex(Face.VerticesPos.bottomLeft)] + normal * extrudeAmount;
        // Draw the new faces
        Gizmos.color = color;
        Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.topLeft], this.vertices[(int)Face.VerticesPos.topRight]);
        Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.topRight], this.vertices[(int)Face.VerticesPos.bottomRight]);
        Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.bottomRight], this.vertices[(int)Face.VerticesPos.bottomLeft]);
        Gizmos.DrawLine(this.vertices[(int)Face.VerticesPos.bottomLeft], this.vertices[(int)Face.VerticesPos.topLeft]);
    }

    public void ApplyChanges(List<Vector3> vertices)
    {

    }
}
