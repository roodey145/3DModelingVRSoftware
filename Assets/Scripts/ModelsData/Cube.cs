using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    List<Vector3> vertices = new List<Vector3>();
    List<Face> faces = new List<Face>();
    [SerializeField]
    private bool _initialized = false;

    [Range(1e-15f, 0.9999999999999f)]
    public float cutPos = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        _Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        //draw();
    }


    Color[] colors = new Color[]
    {
        Color.red,
        Color.green,
        Color.blue
    };
    private void OnDrawGizmos()
    {
        if(!_initialized)
        {
            _Initialize();

            // Cut verticlly
            Vector2Int[] verticalLines = faces[0].GetLines(Direction.horizontal);

            // Calculate the position of the new vertices
            int topLineIndex = (int)Direction.top < (int)Direction.bottom ? 0 : 1;
            int bottomLineIndex = (int)Direction.top < (int)Direction.bottom ? 1 : 0;
            Vector3 topCut =
                Vector3.Lerp(
                    vertices[verticalLines[topLineIndex].x],
                    vertices[verticalLines[topLineIndex].y],
                    cutPos);

            Vector3 bottomCut =
                Vector3.Lerp(
                    vertices[verticalLines[bottomLineIndex].x],
                    vertices[verticalLines[bottomLineIndex].y],
                    cutPos);

            // Add the points to the vertices
            vertices.Add(topCut);
            int topVertixIndex = vertices.Count - 1;
            vertices.Add(bottomCut);
            int bottomVertixIndex = vertices.Count - 1;

            Vector2Int cutThrough = new Vector2Int();
            cutThrough.x = (int)Direction.top < (int)Direction.bottom ? topVertixIndex : bottomVertixIndex;
            cutThrough.y = (int)Direction.top > (int)Direction.bottom ? topVertixIndex : bottomVertixIndex;

            Face[] newFaces = faces[0].Split(cutThrough, 0, Direction.verctial);

            faces.Add(newFaces[0]);
            faces.Add(newFaces[1]);

            //faces[0].Split()

            print(1);
            _initialized = true;
        }


        Gizmos.color = Color.red;
        draw();
    }

    public void draw()
    {
        Vector2Int[] faceVertices;
        for(int i = 0; i < faces.Count; i++)
        {
            faceVertices = faces[i].GetLines(Direction.all);
            Gizmos.color = colors[i % colors.Length];
            for (int l = 0; l < faceVertices.Length; l++)
            {
                
                Gizmos.DrawLine(vertices[faceVertices[l].x], vertices[faceVertices[l].y]);
            }
        }
    }


    private void _Initialize()
    {
        print(1);
        vertices = new List<Vector3>();
        faces = new List<Face>();

        //--------------------------------
        // Create front face
        Face frontFace = new Face();
        // Create four new vertices
        _CreateEmptyFaceVertices();
        int faceNr = 0;
        // Initalize the new vertices
        vertices[(int)Face.VerticesPos.topLeft] = new Vector3(-0.5f, 0.5f, 0);
        vertices[(int)Face.VerticesPos.topRight] = new Vector3(0.5f, 0.5f, 0);
        vertices[(int)Face.VerticesPos.bottomRight] = new Vector3(0.5f, -0.5f, 0);
        vertices[(int)Face.VerticesPos.bottomLeft] = new Vector3(-0.5f, -0.5f, 0);
        frontFace.SetVerticesIndex(new int[]
        {
            (int)Face.VerticesPos.topLeft,
            (int)Face.VerticesPos.topRight,
            (int)Face.VerticesPos.bottomRight,
            (int)Face.VerticesPos.bottomLeft
        });

        faces.Add(frontFace);

    }

    // Create new vertices
    private void _CreateEmptyFaceVertices()
    {
        for (int i = 0; i < 4; i++)
        {
            vertices.Add(new Vector3());
        }
    }
}
