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

    public Color strokeColor = Color.gray;

    [Range(1e-15f, 0.9999999999999f)]
    public float cutPos = 0.5f;
    public bool loopCut = false;

    //[Range(0, 5)]
    public int faceIndex = 0;
    public int numberOfFaces = 0;

    private int _lastCuttedFaceIndex = 0;

    private float _lastCutPos = 0.5f;



    private List<Face> newFaces = new List<Face>();

    LoopCutDemoInfo loopCutDemo = null;


    public bool cutHorizontally = true;
    private bool _lastCutHorizontally = true;

    [Header("Extrude")]
    public bool extrude = false;
    public float extrudeAmount = 0f;
    private ExtrudeDemoInfo extrudeDemo = null;

    [Header("Bevel")]
    public bool bevel = false;
    [Range(1e-10f, 1f)]
    public float bevelAmount = 0f;
    public Direction edgeDirection = Direction.top;
    private BevelDemoInfo bevelDemo = null;

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
    bool newFaceIndex = false;
    private void OnDrawGizmos()
    {
        if (!_initialized || faces.Count == 0)
        {
            newFaces = new List<Face>();
            _Initialize();
            _initialized = true;
        }

        if(faceIndex != _lastCuttedFaceIndex)
        {
            newFaceIndex = true;
            if (faceIndex >= faces.Count)
            {
                faceIndex = faces.Count - 1;
            }
            else if (faceIndex < 0)
            {
                faceIndex = 0;
            }
        }

        if (loopCut && (Mathf.Abs(_lastCutPos - cutPos) > 1e-10 
            || _lastCutHorizontally != cutHorizontally 
            || newFaceIndex))
        {

            


            Face faceToCut = faces[faceIndex];
            if (loopCutDemo == null)
            {
                loopCutDemo = new LoopCutDemoInfo(faceToCut, cutPos, cutHorizontally ? Direction.horizontal : Direction.verctial, vertices);
            }

            // Update the information of the loop cut 
            loopCutDemo.UpdateInfo(faceToCut, cutPos, cutHorizontally ? Direction.horizontal : Direction.verctial);
            

            _lastCutPos = cutPos;
            _lastCutHorizontally = cutHorizontally;
            _lastCuttedFaceIndex = faceIndex;
            //faces[0].Split()
        }

        if (extrude && faces.Count > faceIndex && faceIndex >= 0)
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
            //extrudeDemo.Draw(vertices);
        }

        if(bevel && faces.Count > faceIndex && faceIndex >= 0)
        {
            if (bevelDemo == null)
            {
                bevelDemo = new BevelDemoInfo(faces[faceIndex], edgeDirection, vertices);
            }
            else if (bevelDemo.selectedFace != faces[faceIndex])
            {
                bevelDemo = new BevelDemoInfo(faces[faceIndex], edgeDirection, vertices);
                //bevelDemo.selectedFace = faces[faceIndex];
            }

            if(Mathf.Abs(bevelAmount - bevelDemo.bevelAmount) > 1e-5)
            {
                bevelDemo.SetBevelAmount(bevelAmount);
            }
            
            if(edgeDirection != bevelDemo.edgeDirection)
            {
                bevelDemo = new BevelDemoInfo(faces[faceIndex], edgeDirection, vertices);

                //bevelDemo.UpdateEdgeDirection(edgeDirection);
            }

            //bevelDemo.edgeDirection = edgeDirection;
            
        } else if(bevelDemo != null)
        {
            bevelDemo = null;
        }

        if(applyChange && extrude && extrudeDemo != null)
        {
            extrudeDemo.ApplyChange(vertices, faces);
            newFaces.Clear();

            for(int i = 0; i < extrudeDemo.faces.Length; i++)
            {
                newFaces.Add(extrudeDemo.faces[i]);
            }
            extrudeDemo = null;
            applyChange = false;
        }
        else if (applyChange && loopCutDemo != null)
        {
            loopCutDemo.ApplyChange(vertices, faces);

            numberOfFaces = faces.Count;
            newFaces.Clear();

            for(int i = 0; i < loopCutDemo.faces.Count; i++)
            {
                for(int l = 0; l < loopCutDemo.faces[i].subFaces.Length; l++)
                {
                    newFaces.Add(loopCutDemo.faces[i].subFaces[l]);
                }
                
            }
            loopCutDemo = null;
            applyChange = false;
        }
        else if(applyChange && bevel && bevelDemo != null)
        {
            applyChange = false;
            bevelDemo.ApplyChange(vertices, faces);
        }

        newFaceIndex = false;

        //Gizmos.color = Color.red;
        Draw();
    }

    
    public void Draw()
    {
        Vector2Int[] faceVertices;

        // Draw the faces
        Gizmos.color = strokeColor;
        for(int i = 0; i < faces.Count; i++)
        {
            faceVertices = faces[i].GetLines(Direction.all);
            //Gizmos.color = colors[i % colors.Length];
            for (int l = 0; l < faceVertices.Length; l++)
            {
                
                Gizmos.DrawLine(vertices[faceVertices[l].x], vertices[faceVertices[l].y]);
            } 
        }

        if (faces.Count == 0) return;
        if(faceIndex >= faces.Count) faceIndex = faces.Count - 1;
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

        if (extrude && extrudeDemo != null)
        { // The user want to extrude a face
            extrudeDemo.Draw(vertices);
        }

        // Check if the demo shall be drown
        if (loopCut && loopCutDemo != null)
        {
            loopCutDemo.Draw();
        }

        // Check if the bevel shall be drown
        if(bevel && bevelDemo != null)
        {
            bevelDemo.Draw(vertices);
        }
    }

    //public void LoopCut(Face firstCaller, Face caller, Face face, float cutPositionInPercent, Direction cutDirection, LoopCutDemoInfo demo)
    //{
    //    Face newFace;
    //    Face[] newFaces;
    //    if (cutDirection == Direction.verctial)
    //    {
    //        newFaces = _CutVertically(face, cutPositionInPercent, demo);
    //        newFace = face.GetNeighbourFace(caller, Direction.top);            
    //    }
    //    else
    //    {
    //        newFaces = _CutHorizontally(face, cutPositionInPercent, demo);
    //        newFace = face.GetNeighbourFace(caller, Direction.right);
    //    }

    //    //faces.Add(newFaces[0]);
    //    //faces.Add(newFaces[1]);

    //    demo.SetNewFace(face, newFaces, cutDirection);

    //    if(demo.faces.Count > 100)
    //    {
    //        print("Reached Faces Limit");
    //        return;
    //    }


    //    print($"Move from {face.name} face to {newFace.name} face");

    //    // Make sure that the new face is not the current face
    //    if (newFace != firstCaller)
    //    { // The loop is not completed yet

    //        Direction newCutDirection = newFace.GetCutDirection(face);

    //        // Check if there is a shift in the cut direction from vertical to horizontal or the otherway around.
    //        if(cutDirection != newCutDirection)
    //        {
    //            cutPositionInPercent = 1 - cutPositionInPercent;
    //        }

    //        // Call the loop cut function to cut the next face
    //        LoopCut(firstCaller, face, newFace, cutPositionInPercent, newCutDirection, demo);
    //    }
        
    //}


    //private Face[] _CutHorizontally(Face face, float cutPositionInPercent, LoopCutDemoInfo demo)
    //{
    //    // Cut verticlly
    //    Vector2Int[] horizontalLines = face.GetLines(Direction.verctial);

    //    // Calculate the position of the new vertices
    //    int topLineIndex = (int)Direction.left < (int)Direction.right ? 0 : 1;
    //    int bottomLineIndex = (int)Direction.left < (int)Direction.right ? 1 : 0;
    //    Vector3 leftCut =
    //        Vector3.Lerp(
    //            vertices[horizontalLines[topLineIndex].x],
    //            vertices[horizontalLines[topLineIndex].y],
    //            cutPositionInPercent);

    //    Vector3 rightCut =
    //        Vector3.Lerp(
    //            vertices[horizontalLines[bottomLineIndex].x],
    //            vertices[horizontalLines[bottomLineIndex].y],
    //            cutPositionInPercent);

    //    // To understand the reason for adding the vertices.Count to the index, read the summery of the LoopCutDemoInfo class
    //    // Add the points to the vertices
    //    demo.vertices.Add(leftCut);
    //    int leftVertixIndex = demo.vertices.Count - 1 + vertices.Count;
    //    demo.vertices.Add(rightCut);
    //    int rightVertixIndex = demo.vertices.Count - 1 + vertices.Count;

        


    //    Vector2Int cutThrough = new Vector2Int();
    //    cutThrough.x = (int)Direction.left < (int)Direction.right ? leftVertixIndex : rightVertixIndex;
    //    cutThrough.y = (int)Direction.left < (int)Direction.right ? rightVertixIndex : leftVertixIndex;

    //    Face[] newFaces = face.Split(cutThrough, 0, Direction.horizontal);

    //    print($"The {face.name} face is being cut horizontally");


    //    return newFaces;
    //}


    //private Face[] _CutVertically(Face face, float cutPositionInPercent, LoopCutDemoInfo demo)
    //{
    //    // Cut verticlly
    //    Vector2Int[] verticalLines = face.GetLines(Direction.horizontal);

    //    // Calculate the position of the new vertices
    //    int topLineIndex = (int)Direction.top < (int)Direction.bottom ? 0 : 1;
    //    int bottomLineIndex = (int)Direction.top < (int)Direction.bottom ? 1 : 0;
    //    Vector3 topCut =
    //        Vector3.Lerp(
    //            vertices[verticalLines[topLineIndex].x],
    //            vertices[verticalLines[topLineIndex].y],
    //            cutPositionInPercent);

    //    Vector3 bottomCut =
    //        Vector3.Lerp(
    //            vertices[verticalLines[bottomLineIndex].x],
    //            vertices[verticalLines[bottomLineIndex].y],
    //            cutPositionInPercent);

    //    // To understand the reason for adding the vertices.Count to the index, read the summery of the LoopCutDemoInfo class
    //    // Add the points to the vertices
    //    demo.vertices.Add(topCut);
    //    int topVertixIndex = demo.vertices.Count - 1 + vertices.Count;
    //    demo.vertices.Add(bottomCut);
    //    int bottomVertixIndex = demo.vertices.Count - 1 + vertices.Count;

    //    Vector2Int cutThrough = new Vector2Int();
    //    cutThrough.x = (int)Direction.top < (int)Direction.bottom ? topVertixIndex : bottomVertixIndex;
    //    cutThrough.y = (int)Direction.top > (int)Direction.bottom ? topVertixIndex : bottomVertixIndex;

    //    Face[] newFaces = face.Split(cutThrough, 0, Direction.verctial);
    //    print($"The {face.name} face is being cut vertically");


    //    return newFaces;
    //}




    #region Initialize
    private void _Initialize()
    {
        vertices = new List<Vector3>(8);
        faces = new List<Face>();

        for(int i = 0; i < 8; i++)
        {
            vertices.Add(new Vector3());
        }

        // Create the vertices of the cube
        // Front vertices
        vertices[(int)Face.VerticesPos.topLeft] = new Vector3(-0.5f, 0.5f, -0.5f);
        vertices[(int)Face.VerticesPos.topRight] = new Vector3(0.5f, 0.5f, -0.5f);
        vertices[(int)Face.VerticesPos.bottomRight] = new Vector3(0.5f, -0.5f, -0.5f);
        vertices[(int)Face.VerticesPos.bottomLeft] = new Vector3(-0.5f, -0.5f, -0.5f);

        // Back vertices
        vertices[(int)Face.VerticesPos.topLeft + 4] = new Vector3(-0.5f, 0.5f, 0.5f);
        vertices[(int)Face.VerticesPos.topRight + 4] = new Vector3(0.5f, 0.5f, 0.5f);
        vertices[(int)Face.VerticesPos.bottomRight + 4] = new Vector3(0.5f, -0.5f, 0.5f);
        vertices[(int)Face.VerticesPos.bottomLeft + 4] = new Vector3(-0.5f, -0.5f, 0.5f);

        //--------------------------------
        // Create front face
        Face frontFace = new Face();
        // Create four new vertices
        _CreateEmptyFaceVertices();
        int faceNr = 0;
        // Initalize the new vertices
        //vertices[(int)Face.VerticesPos.topLeft] = new Vector3(-0.5f, 0.5f, -0.5f);
        //vertices[(int)Face.VerticesPos.topRight] = new Vector3(0.5f, 0.5f, -0.5f);
        //vertices[(int)Face.VerticesPos.bottomRight] = new Vector3(0.5f, -0.5f, -0.5f);
        //vertices[(int)Face.VerticesPos.bottomLeft] = new Vector3(-0.5f, -0.5f, -0.5f);
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
        //vertices[(int)Face.VerticesPos.topLeft + (4 * faceNr)] = new Vector3(0.5f, 0.5f, -0.5f);
        //vertices[(int)Face.VerticesPos.topRight + (4 * faceNr)] = new Vector3(0.5f, 0.5f, 0.5f);
        //vertices[(int)Face.VerticesPos.bottomRight + (4 * faceNr)] = new Vector3(0.5f, -0.5f, 0.5f);
        //vertices[(int)Face.VerticesPos.bottomLeft + (4 * faceNr)] = new Vector3(0.5f, -0.5f, -0.5f);
        //rightFace.SetVerticesIndex(new int[]
        //{
        //    (int)Face.VerticesPos.topLeft + (4 * faceNr),
        //    (int)Face.VerticesPos.topRight + (4 * faceNr),
        //    (int)Face.VerticesPos.bottomRight + (4 * faceNr),
        //    (int)Face.VerticesPos.bottomLeft + (4 * faceNr)
        //});

        
        int[] faceVertices = new int[4];
        faceVertices[(int)Face.VerticesPos.topLeft] = (int)Face.VerticesPos.topRight;
        faceVertices[(int)Face.VerticesPos.topRight] = (int)Face.VerticesPos.topRight + 4;
        faceVertices[(int)Face.VerticesPos.bottomRight] = (int)Face.VerticesPos.bottomRight + 4;
        faceVertices[(int)Face.VerticesPos.bottomLeft] = (int)Face.VerticesPos.bottomRight;

        //rightFace.SetVerticesIndex(new int[]
        //{
        //    (int)Face.VerticesPos.topRight, // Top left
        //    (int)Face.VerticesPos.topRight + (4 * faceNr),
        //    (int)Face.VerticesPos.bottomRight + (4 * faceNr),
        //    (int)Face.VerticesPos.bottomLeft + (4 * faceNr)
        //});

        rightFace.SetVerticesIndex((int[])faceVertices.Clone());

        faces.Add(rightFace);


        //--------------------------------
        // Create back face
        Face backFace = new Face();
        // Create four new vertices
        _CreateEmptyFaceVertices();
        faceNr = 2;
        // Initalize the new vertices
        //vertices[(int)Face.VerticesPos.topLeft + (4 * faceNr)] = new Vector3(-0.5f, 0.5f, 0.5f);
        //vertices[(int)Face.VerticesPos.topRight + (4 * faceNr)] = new Vector3(0.5f, 0.5f, 0.5f);
        //vertices[(int)Face.VerticesPos.bottomRight + (4 * faceNr)] = new Vector3(0.5f, -0.5f, 0.5f);
        //vertices[(int)Face.VerticesPos.bottomLeft + (4 * faceNr)] = new Vector3(-0.5f, -0.5f, 0.5f);

        faceVertices[(int)Face.VerticesPos.topLeft] = (int)Face.VerticesPos.topLeft + 4;
        faceVertices[(int)Face.VerticesPos.topRight] = (int)Face.VerticesPos.topRight + 4;
        faceVertices[(int)Face.VerticesPos.bottomRight] = (int)Face.VerticesPos.bottomRight + 4;
        faceVertices[(int)Face.VerticesPos.bottomLeft] = (int)Face.VerticesPos.bottomLeft + 4;

        //backFace.SetVerticesIndex(new int[]
        //{
        //    (int)Face.VerticesPos.topLeft + (4 * faceNr),
        //    (int)Face.VerticesPos.topRight + (4 * faceNr),
        //    (int)Face.VerticesPos.bottomRight + (4 * faceNr),
        //    (int)Face.VerticesPos.bottomLeft + (4 * faceNr)
        //});

        backFace.SetVerticesIndex((int[])faceVertices.Clone());

        faces.Add(backFace);



        //--------------------------------
        // Create left face
        Face leftFace = new Face();
        // Create four new vertices
        _CreateEmptyFaceVertices();
        faceNr = 3;
        // Initalize the new vertices
        //vertices[(int)Face.VerticesPos.topRight + (4 * faceNr)] = new Vector3(-0.5f, 0.5f, -0.5f);
        //vertices[(int)Face.VerticesPos.topLeft + (4 * faceNr)] = new Vector3(-0.5f, 0.5f, 0.5f);
        //vertices[(int)Face.VerticesPos.bottomLeft + (4 * faceNr)] = new Vector3(-0.5f, -0.5f, 0.5f);
        //vertices[(int)Face.VerticesPos.bottomRight + (4 * faceNr)] = new Vector3(-0.5f, -0.5f, -0.5f);

        faceVertices[(int)Face.VerticesPos.topLeft] = (int)Face.VerticesPos.topLeft;
        faceVertices[(int)Face.VerticesPos.topRight] = (int)Face.VerticesPos.topLeft + 4;
        faceVertices[(int)Face.VerticesPos.bottomRight] = (int)Face.VerticesPos.bottomLeft + 4;
        faceVertices[(int)Face.VerticesPos.bottomLeft] = (int)Face.VerticesPos.bottomLeft;

        //leftFace.SetVerticesIndex(new int[]
        //{
        //    (int)Face.VerticesPos.topLeft + (4 * faceNr),
        //    (int)Face.VerticesPos.topRight + (4 * faceNr),
        //    (int)Face.VerticesPos.bottomRight + (4 * faceNr),
        //    (int)Face.VerticesPos.bottomLeft + (4 * faceNr)
        //});

        leftFace.SetVerticesIndex((int[])faceVertices.Clone());

        faces.Add(leftFace);



        //--------------------------------
        // Create bottom face
        Face bottomFace = new Face();
        // Create four new vertices
        _CreateEmptyFaceVertices();
        faceNr = 4;
        // Initalize the new vertices
        //vertices[(int)Face.VerticesPos.topLeft + (4 * faceNr)] = new Vector3(-0.5f, -0.5f, 0.5f);
        //vertices[(int)Face.VerticesPos.topRight + (4 * faceNr)] = new Vector3(0.5f, -0.5f, 0.5f);
        //vertices[(int)Face.VerticesPos.bottomRight + (4 * faceNr)] = new Vector3(0.5f, -0.5f, -0.5f);
        //vertices[(int)Face.VerticesPos.bottomLeft + (4 * faceNr)] = new Vector3(-0.5f, -0.5f, -0.5f);

        faceVertices[(int)Face.VerticesPos.topLeft] = (int)Face.VerticesPos.bottomLeft + 4;
        faceVertices[(int)Face.VerticesPos.topRight] = (int)Face.VerticesPos.bottomRight + 4;
        faceVertices[(int)Face.VerticesPos.bottomRight] = (int)Face.VerticesPos.bottomRight;
        faceVertices[(int)Face.VerticesPos.bottomLeft] = (int)Face.VerticesPos.bottomLeft;

        //bottomFace.SetVerticesIndex(new int[]
        //{
        //    (int)Face.VerticesPos.topLeft + (4 * faceNr),
        //    (int)Face.VerticesPos.topRight + (4 * faceNr),
        //    (int)Face.VerticesPos.bottomRight + (4 * faceNr),
        //    (int)Face.VerticesPos.bottomLeft + (4 * faceNr)
        //});

        bottomFace.SetVerticesIndex((int[])faceVertices.Clone());
        faces.Add(bottomFace);


        //--------------------------------
        // Create top face
        Face topFace = new Face();
        // Create four new vertices
        _CreateEmptyFaceVertices();
        faceNr = 5;
        // Initalize the new vertices
        //vertices[(int)Face.VerticesPos.topLeft + (4 * faceNr)] = new Vector3(-0.5f, 0.5f, 0.5f);
        //vertices[(int)Face.VerticesPos.topRight + (4 * faceNr)] = new Vector3(0.5f, 0.5f, 0.5f);
        //vertices[(int)Face.VerticesPos.bottomRight + (4 * faceNr)] = new Vector3(0.5f, 0.5f, -0.5f);
        //vertices[(int)Face.VerticesPos.bottomLeft + (4 * faceNr)] = new Vector3(-0.5f, 0.5f, -0.5f);

        faceVertices[(int)Face.VerticesPos.topLeft] = (int)Face.VerticesPos.topLeft + 4;
        faceVertices[(int)Face.VerticesPos.topRight] = (int)Face.VerticesPos.topRight + 4;
        faceVertices[(int)Face.VerticesPos.bottomRight] = (int)Face.VerticesPos.topRight;
        faceVertices[(int)Face.VerticesPos.bottomLeft] = (int)Face.VerticesPos.topLeft;

        //topFace.SetVerticesIndex(new int[]
        //{
        //    (int)Face.VerticesPos.topLeft + (4 * faceNr),
        //    (int)Face.VerticesPos.topRight + (4 * faceNr),
        //    (int)Face.VerticesPos.bottomRight + (4 * faceNr),
        //    (int)Face.VerticesPos.bottomLeft + (4 * faceNr)
        //});

        topFace.SetVerticesIndex((int[])faceVertices.Clone());
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
