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

    private float _lastCutPos = 0.5f;


    public bool cutHorizontally = true;
    private bool _lastCutHorizontally = true;

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
        if(!_initialized || Mathf.Abs(_lastCutPos - cutPos) > 1e-10 || _lastCutHorizontally != cutHorizontally)
        {
            _Initialize();

            //// Cut verticlly
            //Vector2Int[] verticalLines = faces[4].GetLines(Direction.horizontal);

            //// Calculate the position of the new vertices
            //int topLineIndex = (int)Direction.top < (int)Direction.bottom ? 0 : 1;
            //int bottomLineIndex = (int)Direction.top < (int)Direction.bottom ? 1 : 0;
            //Vector3 topCut =
            //    Vector3.Lerp(
            //        vertices[verticalLines[topLineIndex].x],
            //        vertices[verticalLines[topLineIndex].y],
            //        cutPos);

            //Vector3 bottomCut =
            //    Vector3.Lerp(
            //        vertices[verticalLines[bottomLineIndex].x],
            //        vertices[verticalLines[bottomLineIndex].y],
            //        cutPos);

            //// Add the points to the vertices
            //vertices.Add(topCut);
            //int topVertixIndex = vertices.Count - 1;
            //vertices.Add(bottomCut);
            //int bottomVertixIndex = vertices.Count - 1;

            //Vector2Int cutThrough = new Vector2Int();
            //cutThrough.x = (int)Direction.top < (int)Direction.bottom ? topVertixIndex : bottomVertixIndex;
            //cutThrough.y = (int)Direction.top > (int)Direction.bottom ? topVertixIndex : bottomVertixIndex;

            //Face[] newFaces = faces[4].Split(cutThrough, 0, Direction.verctial);

            //faces.Add(newFaces[0]);
            //faces.Add(newFaces[1]);

            Face faceToCut = faces[0];
            LoopCut(faceToCut, faceToCut, faceToCut, cutPos, cutHorizontally ? Direction.horizontal : Direction.verctial);

            _lastCutPos = cutPos;
            _lastCutHorizontally = cutHorizontally;
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

    public void LoopCut(Face firstCaller, Face caller, Face face, float cutPositionInPercent, Direction cutDirection)
    {
        Face newFace;
        Face[] newFaces;
        if (cutDirection == Direction.verctial)
        {
            newFaces = _CutVertically(face, cutPositionInPercent);
            newFace = face.GetNeighbourFace(caller, Direction.top);            
        }
        else
        {
            newFaces = _CutHorizontally(face, cutPositionInPercent);
            newFace = face.GetNeighbourFace(caller, Direction.right);
        }

        faces.Add(newFaces[0]);
        faces.Add(newFaces[1]);

        if(faces.Count > 100)
        {
            print("Reached Faces Limit");
            return;
        }

        //// Cut verticlly
        //Vector2Int[] verticalLines = face.GetLines(Direction.horizontal);

        //// Calculate the position of the new vertices
        //int topLineIndex = (int)Direction.top < (int)Direction.bottom ? 0 : 1;
        //int bottomLineIndex = (int)Direction.top < (int)Direction.bottom ? 1 : 0;
        //Vector3 topCut =
        //    Vector3.Lerp(
        //        vertices[verticalLines[topLineIndex].x],
        //        vertices[verticalLines[topLineIndex].y],
        //        cutPositionInPercent);

        //Vector3 bottomCut =
        //    Vector3.Lerp(
        //        vertices[verticalLines[bottomLineIndex].x],
        //        vertices[verticalLines[bottomLineIndex].y],
        //        cutPositionInPercent);

        //// Add the points to the vertices
        //vertices.Add(topCut);
        //int topVertixIndex = vertices.Count - 1;
        //vertices.Add(bottomCut);
        //int bottomVertixIndex = vertices.Count - 1;

        //Vector2Int cutThrough = new Vector2Int();
        //cutThrough.x = (int)Direction.top < (int)Direction.bottom ? topVertixIndex : bottomVertixIndex;
        //cutThrough.y = (int)Direction.top > (int)Direction.bottom ? topVertixIndex : bottomVertixIndex;

        //Face[] newFaces = face.Split(cutThrough, 0, Direction.verctial);

        //faces.Add(newFaces[0]);
        //faces.Add(newFaces[1]);


        print($"Move from {face.name} face to {newFace.name} face");

        // Make sure that the new face is not the current face
        if(newFace != firstCaller)
        { // The loop is not completed yet
            // Call the loop cut function to cut the next face
            LoopCut(firstCaller, face, newFace, cutPositionInPercent, newFace.GetCutDirection(face));
        }
        
    }


    private Face[] _CutHorizontally(Face face, float cutPositionInPercent)
    {
        // Cut verticlly
        Vector2Int[] horizontalLines = face.GetLines(Direction.verctial);

        // Calculate the position of the new vertices
        int topLineIndex = (int)Direction.left < (int)Direction.right ? 0 : 1;
        int bottomLineIndex = (int)Direction.left < (int)Direction.right ? 1 : 0;
        Vector3 leftCut =
            Vector3.Lerp(
                vertices[horizontalLines[topLineIndex].x],
                vertices[horizontalLines[topLineIndex].y],
                cutPositionInPercent);

        Vector3 rightCut =
            Vector3.Lerp(
                vertices[horizontalLines[bottomLineIndex].x],
                vertices[horizontalLines[bottomLineIndex].y],
                cutPositionInPercent);

        // Add the points to the vertices
        vertices.Add(leftCut);
        int leftVertixIndex = vertices.Count - 1;
        vertices.Add(rightCut);
        int rightVertixIndex = vertices.Count - 1;

        Vector2Int cutThrough = new Vector2Int();
        cutThrough.x = (int)Direction.left < (int)Direction.right ? leftVertixIndex : rightVertixIndex;
        cutThrough.y = (int)Direction.left < (int)Direction.right ? rightVertixIndex : leftVertixIndex;

        Face[] newFaces = face.Split(cutThrough, 0, Direction.horizontal);

        print($"The {face.name} face is being cut horizontally");


        return newFaces;
    }


    private Face[] _CutVertically(Face face, float cutPositionInPercent)
    {
        // Cut verticlly
        Vector2Int[] verticalLines = face.GetLines(Direction.horizontal);

        // Calculate the position of the new vertices
        int topLineIndex = (int)Direction.top < (int)Direction.bottom ? 0 : 1;
        int bottomLineIndex = (int)Direction.top < (int)Direction.bottom ? 1 : 0;
        Vector3 topCut =
            Vector3.Lerp(
                vertices[verticalLines[topLineIndex].x],
                vertices[verticalLines[topLineIndex].y],
                cutPositionInPercent);

        Vector3 bottomCut =
            Vector3.Lerp(
                vertices[verticalLines[bottomLineIndex].x],
                vertices[verticalLines[bottomLineIndex].y],
                cutPositionInPercent);

        // Add the points to the vertices
        vertices.Add(topCut);
        int topVertixIndex = vertices.Count - 1;
        vertices.Add(bottomCut);
        int bottomVertixIndex = vertices.Count - 1;

        Vector2Int cutThrough = new Vector2Int();
        cutThrough.x = (int)Direction.top < (int)Direction.bottom ? topVertixIndex : bottomVertixIndex;
        cutThrough.y = (int)Direction.top > (int)Direction.bottom ? topVertixIndex : bottomVertixIndex;

        Face[] newFaces = face.Split(cutThrough, 0, Direction.verctial);
        print($"The {face.name} face is being cut vertically");


        return newFaces;
    }




    #region Initialize
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
        vertices[(int)Face.VerticesPos.topLeft] = new Vector3(-0.5f, 0.5f, -0.5f);
        vertices[(int)Face.VerticesPos.topRight] = new Vector3(0.5f, 0.5f, -0.5f);
        vertices[(int)Face.VerticesPos.bottomRight] = new Vector3(0.5f, -0.5f, -0.5f);
        vertices[(int)Face.VerticesPos.bottomLeft] = new Vector3(-0.5f, -0.5f, -0.5f);
        frontFace.SetVerticesIndex(new int[]
        {
            (int)Face.VerticesPos.topLeft,
            (int)Face.VerticesPos.topRight,
            (int)Face.VerticesPos.bottomRight,
            (int)Face.VerticesPos.bottomLeft
        });
        
        faces.Add(frontFace);


        //--------------------------------
        // Create right face
        Face rightFace = new Face();
        // Create four new vertices
        _CreateEmptyFaceVertices();
        faceNr = 1;
        // Initalize the new vertices
        vertices[(int)Face.VerticesPos.topLeft + (4 * faceNr)] = new Vector3(0.5f, 0.5f, -0.5f);
        vertices[(int)Face.VerticesPos.topRight + (4 * faceNr)] = new Vector3(0.5f, 0.5f, 0.5f);
        vertices[(int)Face.VerticesPos.bottomRight + (4 * faceNr)] = new Vector3(0.5f, -0.5f, 0.5f);
        vertices[(int)Face.VerticesPos.bottomLeft + (4 * faceNr)] = new Vector3(0.5f, -0.5f, -0.5f);
        rightFace.SetVerticesIndex(new int[]
        {
            (int)Face.VerticesPos.topLeft + (4 * faceNr),
            (int)Face.VerticesPos.topRight + (4 * faceNr),
            (int)Face.VerticesPos.bottomRight + (4 * faceNr),
            (int)Face.VerticesPos.bottomLeft + (4 * faceNr)
        });

        faces.Add(rightFace);


        //--------------------------------
        // Create back face
        Face backFace = new Face();
        // Create four new vertices
        _CreateEmptyFaceVertices();
        faceNr = 2;
        // Initalize the new vertices
        vertices[(int)Face.VerticesPos.topLeft + (4 * faceNr)] = new Vector3(-0.5f, 0.5f, 0.5f);
        vertices[(int)Face.VerticesPos.topRight + (4 * faceNr)] = new Vector3(0.5f, 0.5f, 0.5f);
        vertices[(int)Face.VerticesPos.bottomRight + (4 * faceNr)] = new Vector3(0.5f, -0.5f, 0.5f);
        vertices[(int)Face.VerticesPos.bottomLeft + (4 * faceNr)] = new Vector3(-0.5f, -0.5f, 0.5f); ;
        backFace.SetVerticesIndex(new int[]
        {
            (int)Face.VerticesPos.topLeft + (4 * faceNr),
            (int)Face.VerticesPos.topRight + (4 * faceNr),
            (int)Face.VerticesPos.bottomRight + (4 * faceNr),
            (int)Face.VerticesPos.bottomLeft + (4 * faceNr)
        });

        faces.Add(backFace);



        //--------------------------------
        // Create left face
        Face leftFace = new Face();
        // Create four new vertices
        _CreateEmptyFaceVertices();
        faceNr = 3;
        // Initalize the new vertices
        vertices[(int)Face.VerticesPos.topLeft + (4 * faceNr)] = new Vector3(-0.5f, 0.5f, -0.5f);
        vertices[(int)Face.VerticesPos.topRight + (4 * faceNr)] = new Vector3(-0.5f, 0.5f, 0.5f);
        vertices[(int)Face.VerticesPos.bottomRight + (4 * faceNr)] = new Vector3(-0.5f, -0.5f, 0.5f);
        vertices[(int)Face.VerticesPos.bottomLeft + (4 * faceNr)] = new Vector3(-0.5f, -0.5f, -0.5f);
        leftFace.SetVerticesIndex(new int[]
        {
            (int)Face.VerticesPos.topLeft + (4 * faceNr),
            (int)Face.VerticesPos.topRight + (4 * faceNr),
            (int)Face.VerticesPos.bottomRight + (4 * faceNr),
            (int)Face.VerticesPos.bottomLeft + (4 * faceNr)
        });

        faces.Add(leftFace);



        //--------------------------------
        // Create bottom face
        Face bottomFace = new Face();
        // Create four new vertices
        _CreateEmptyFaceVertices();
        faceNr = 4;
        // Initalize the new vertices
        vertices[(int)Face.VerticesPos.topLeft + (4 * faceNr)] = new Vector3(-0.5f, -0.5f, 0.5f);
        vertices[(int)Face.VerticesPos.topRight + (4 * faceNr)] = new Vector3(0.5f, -0.5f, 0.5f);
        vertices[(int)Face.VerticesPos.bottomRight + (4 * faceNr)] = new Vector3(0.5f, -0.5f, -0.5f);
        vertices[(int)Face.VerticesPos.bottomLeft + (4 * faceNr)] = new Vector3(-0.5f, -0.5f, -0.5f);
        bottomFace.SetVerticesIndex(new int[]
        {
            (int)Face.VerticesPos.topLeft + (4 * faceNr),
            (int)Face.VerticesPos.topRight + (4 * faceNr),
            (int)Face.VerticesPos.bottomRight + (4 * faceNr),
            (int)Face.VerticesPos.bottomLeft + (4 * faceNr)
        });

        faces.Add(bottomFace);


        //--------------------------------
        // Create top face
        Face topFace = new Face();
        // Create four new vertices
        _CreateEmptyFaceVertices();
        faceNr = 5;
        // Initalize the new vertices
        vertices[(int)Face.VerticesPos.topLeft + (4 * faceNr)] = new Vector3(-0.5f, 0.5f, 0.5f);
        vertices[(int)Face.VerticesPos.topRight + (4 * faceNr)] = new Vector3(0.5f, 0.5f, 0.5f);
        vertices[(int)Face.VerticesPos.bottomRight + (4 * faceNr)] = new Vector3(0.5f, 0.5f, -0.5f);
        vertices[(int)Face.VerticesPos.bottomLeft + (4 * faceNr)] = new Vector3(-0.5f, 0.5f, -0.5f);
        topFace.SetVerticesIndex(new int[]
        {
            (int)Face.VerticesPos.topLeft + (4 * faceNr),
            (int)Face.VerticesPos.topRight + (4 * faceNr),
            (int)Face.VerticesPos.bottomRight + (4 * faceNr),
            (int)Face.VerticesPos.bottomLeft + (4 * faceNr)
        });

        faces.Add(topFace);



        //---------------------------------------------------
        // Assign the front face neighbour faces
        frontFace.SetFace(topFace, Direction.top);
        frontFace.SetFace(rightFace, Direction.right);
        frontFace.SetFace(bottomFace, Direction.bottom);
        frontFace.SetFace(leftFace, Direction.left);


        // Assign the right face neighbour faces
        rightFace.SetFace(topFace, Direction.top);
        rightFace.SetFace(backFace, Direction.right);
        rightFace.SetFace(bottomFace, Direction.bottom);
        rightFace.SetFace(frontFace, Direction.left);

        // Assign the back face neighbour faces
        backFace.SetFace(topFace, Direction.top);
        backFace.SetFace(leftFace, Direction.right);
        backFace.SetFace(bottomFace, Direction.bottom);
        backFace.SetFace(rightFace, Direction.left);


        // Assign the left face neighbour faces
        leftFace.SetFace(topFace, Direction.top);
        leftFace.SetFace(frontFace, Direction.right);
        leftFace.SetFace(bottomFace, Direction.bottom);
        leftFace.SetFace(backFace, Direction.left);


        // Assign the bottom face neighbour faces
        bottomFace.SetFace(frontFace, Direction.top);
        bottomFace.SetFace(leftFace, Direction.left);
        bottomFace.SetFace(backFace, Direction.bottom);
        bottomFace.SetFace(rightFace, Direction.right);


        // Assign the top face neighbour faces
        topFace.SetFace(backFace, Direction.top);
        topFace.SetFace(rightFace, Direction.right);
        topFace.SetFace(frontFace, Direction.bottom);
        topFace.SetFace(leftFace, Direction.left);



        // Give the faces a name. Shall be deleted later
        frontFace.name = "Front";
        rightFace.name = "Right";
        backFace.name = "Back";
        leftFace.name = "Left";
        topFace.name = "Top";
        bottomFace.name = "Bottom";
    }

    // Create new vertices
    private void _CreateEmptyFaceVertices()
    {
        for (int i = 0; i < 4; i++)
        {
            vertices.Add(new Vector3());
        }
    }

    #endregion
}
