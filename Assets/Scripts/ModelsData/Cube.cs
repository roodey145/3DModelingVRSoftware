using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    List<Vector3> vertices = new List<Vector3>();
    public Face[] facesPreview;
    List<Face> faces = new List<Face>();
    [SerializeField]
    private bool _initialized = false;

    [Range(1e-15f, 0.9999999999999f)]
    public float cutPos = 0.5f;

    //[Range(0, 5)]
    public int faceIndex = 0;
    public int numberOfFaces = 0;

    private int _lastCuttedFaceIndex = 0;

    private float _lastCutPos = 0.5f;



    private List<Face> newFaces = new List<Face>();

    LoopCutDemoInfo demo = null;


    public bool cutHorizontally = true;
    private bool _lastCutHorizontally = true;

    public bool extrude = false;
    public float extrudeAmount = 0f;
    private ExtrudeDemoInfo extrudeDemo = null;

    public bool applyChange = false;

    public int faceToFillIndex = 0;
    public Color fillColor = Color.white;


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
        if(!_initialized || Mathf.Abs(_lastCutPos - cutPos) > 1e-10 
            || _lastCutHorizontally != cutHorizontally || faceIndex != _lastCuttedFaceIndex)
        {
            if(!_initialized)
            {
                newFaces = new List<Face>();
                _Initialize();
                _initialized = true;
            }

            if(faceIndex >= faces.Count)
            {
                faceIndex = faces.Count - 1;
            }


            Face faceToCut = faces[faceIndex];
            demo = new LoopCutDemoInfo();
            LoopCut(faceToCut, faceToCut, faceToCut, cutPos, 
                cutHorizontally ? Direction.horizontal : Direction.verctial, demo);

            _lastCutPos = cutPos;
            _lastCutHorizontally = cutHorizontally;
            _lastCuttedFaceIndex = faceIndex;
            //faces[0].Split()
        }

        if (extrude && faces.Count >= faceIndex && faceIndex >= 0)
        { // The user want to extrude a face
            if (extrudeDemo == null)
            {
                extrudeDemo = new ExtrudeDemoInfo(faces[faceIndex]);
            }
            else if(extrudeDemo.face != faces[faceIndex])
            {
                extrudeDemo.face = faces[faceIndex];
            }

            extrudeDemo.SetExtrudeAmount(extrudeAmount);
            extrudeDemo.draw(vertices);
        }

        if (applyChange && demo != null)
        {
            demo.ApplyChange(vertices, faces);

            numberOfFaces = faces.Count;

            for(int i = 0; i < demo.faces.Count; i++)
            {
                for(int l = 0; l < demo.faces[i].subFaces.Length; l++)
                {
                    newFaces.Add(demo.faces[i].subFaces[l]);
                }
                
            }
            demo = null;
            applyChange = false;
        }

        //Gizmos.color = Color.red;
        draw();
    }

    
    public void draw()
    {
        Vector2Int[] faceVertices;
        // Draw the faces
        for(int i = 0; i < faces.Count; i++)
        {
            faceVertices = faces[i].GetLines(Direction.all);
            Gizmos.color = colors[i % colors.Length];
            for (int l = 0; l < faceVertices.Length; l++)
            {
                
                Gizmos.DrawLine(vertices[faceVertices[l].x], vertices[faceVertices[l].y]);
            }
        }

        if (faces.Count == 0) return;
        faceVertices = faces[faceIndex].GetLines(Direction.all);
        Gizmos.color = Color.cyan;
        for (int l = 0; l < faceVertices.Length; l++)
        {

            Gizmos.DrawLine(vertices[faceVertices[l].x], vertices[faceVertices[l].y]);
        }

        if (newFaces.Count > faceToFillIndex)
        {
            faceVertices = newFaces[faceToFillIndex].GetLines(Direction.all);
            Gizmos.color = fillColor;
            for (int l = 0; l < faceVertices.Length; l++)
            {
                Gizmos.DrawLine(vertices[faceVertices[l].x], vertices[faceVertices[l].y]);
            }
        }

        if (extrude)
        { // The user want to extrude a face
            extrudeDemo.draw(vertices);
        }

        // Check if the demo shall be drown
        if (demo == null) return; // The demo shall not be drown
        demo.draw();
    }

    public void LoopCut(Face firstCaller, Face caller, Face face, float cutPositionInPercent, Direction cutDirection, LoopCutDemoInfo demo)
    {
        Face newFace;
        Face[] newFaces;
        if (cutDirection == Direction.verctial)
        {
            newFaces = _CutVertically(face, cutPositionInPercent, demo);
            newFace = face.GetNeighbourFace(caller, Direction.top);            
        }
        else
        {
            newFaces = _CutHorizontally(face, cutPositionInPercent, demo);
            newFace = face.GetNeighbourFace(caller, Direction.right);
        }

        //faces.Add(newFaces[0]);
        //faces.Add(newFaces[1]);

        demo.SetNewFace(face, newFaces, cutDirection);

        if(demo.faces.Count > 100)
        {
            print("Reached Faces Limit");
            return;
        }


        print($"Move from {face.name} face to {newFace.name} face");

        // Make sure that the new face is not the current face
        if (newFace != firstCaller)
        { // The loop is not completed yet

            Direction newCutDirection = newFace.GetCutDirection(face);

            // Check if there is a shift in the cut direction from vertical to horizontal or the otherway around.
            if(cutDirection != newCutDirection)
            {
                cutPositionInPercent = 1 - cutPositionInPercent;
            }

            // Call the loop cut function to cut the next face
            LoopCut(firstCaller, face, newFace, cutPositionInPercent, newCutDirection, demo);
        }
        
    }


    private Face[] _CutHorizontally(Face face, float cutPositionInPercent, LoopCutDemoInfo demo)
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

        // To understand the reason for adding the vertices.Count to the index, read the summery of the LoopCutDemoInfo class
        // Add the points to the vertices
        demo.vertices.Add(leftCut);
        int leftVertixIndex = demo.vertices.Count - 1 + vertices.Count;
        demo.vertices.Add(rightCut);
        int rightVertixIndex = demo.vertices.Count - 1 + vertices.Count;

        


        Vector2Int cutThrough = new Vector2Int();
        cutThrough.x = (int)Direction.left < (int)Direction.right ? leftVertixIndex : rightVertixIndex;
        cutThrough.y = (int)Direction.left < (int)Direction.right ? rightVertixIndex : leftVertixIndex;

        Face[] newFaces = face.Split(cutThrough, 0, Direction.horizontal);

        print($"The {face.name} face is being cut horizontally");


        return newFaces;
    }


    private Face[] _CutVertically(Face face, float cutPositionInPercent, LoopCutDemoInfo demo)
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

        // To understand the reason for adding the vertices.Count to the index, read the summery of the LoopCutDemoInfo class
        // Add the points to the vertices
        demo.vertices.Add(topCut);
        int topVertixIndex = demo.vertices.Count - 1 + vertices.Count;
        demo.vertices.Add(bottomCut);
        int bottomVertixIndex = demo.vertices.Count - 1 + vertices.Count;

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
        bottomFace.SetFace(leftFace, Direction.right);
        bottomFace.SetFace(backFace, Direction.bottom);
        bottomFace.SetFace(rightFace, Direction.left);


        // Assign the top face neighbour faces
        topFace.SetFace(backFace, Direction.top);
        topFace.SetFace(rightFace, Direction.left);
        topFace.SetFace(frontFace, Direction.bottom);
        topFace.SetFace(leftFace, Direction.right);



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
