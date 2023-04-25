using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LoopCutDemoInfo;

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

    private Face _originalFace;
    private float _originalCutPosition;
    private Direction _originalDirection;
    public Direction referenceDirection = Direction.top;
    private bool _flippedCutPosition = false;

    public Color color = Color.white;
    private List<Vector3> _vertices = new List<Vector3>();
    public List<Vector3> originalVertices = new List<Vector3>();
    public List<CutInfo> faces = new List<CutInfo>();
    public List<Direction> direction = new List<Direction>();

    public LoopCutDemoInfo(Face originalFace, float cutPosition, Direction cutDirection, List<Vector3> vertices)
    {
        _originalFace = originalFace;
        _originalCutPosition = cutPosition;
        _originalDirection = cutDirection;

        this.originalVertices = vertices;
    }

    public LoopCutDemoInfo(
        Face originalFace, float cutPosition, 
        Direction cutDirection, Direction referenceDirection, 
        List<Vector3> vertices)
    {
        _originalFace = originalFace;
        _originalCutPosition = cutPosition;
        _originalDirection = cutDirection;
        this.referenceDirection = referenceDirection;

        if (cutDirection == Direction.horizontal)
        {
            // Cut verticlly
            Vector2Int[] horizontalLines = originalFace.GetLines(Direction.verctial);

            // Get the edge of the reference direction
            Vector2Int referenceEdge = originalFace.GetLine(referenceDirection);

            // Calculate the position of the new vertices
            int leftLineIndex = (int)Direction.left < (int)Direction.right ? 0 : 1;

            if (horizontalLines[leftLineIndex].y == referenceEdge.x ||
                horizontalLines[leftLineIndex].y == referenceEdge.y)
            { // Flipped direction
                _flippedCutPosition = true;
            }
        }
        else
        {
            // Cut verticlly
            Vector2Int[] verticalLines = originalFace.GetLines(Direction.horizontal);

            // Calculate the position of the new vertices
            int topLineIndex = (int)Direction.top < (int)Direction.bottom ? 0 : 1;

            // Get the edge of the reference direction
            Vector2Int referenceEdge = originalFace.GetLine(referenceDirection);

            if (verticalLines[topLineIndex].y == referenceEdge.x ||
                verticalLines[topLineIndex].y == referenceEdge.y)
            { // Flipped direction
                _flippedCutPosition = true;
            }
        }

        this.originalVertices = vertices;
    }

    public void SetNewFace(Face original, Face[] splittedPart, Direction cutDirection)
    {
        CutInfo cutInfo = new CutInfo();
        cutInfo.original = original;
        cutInfo.subFaces = splittedPart;
        // Add the info
        faces.Add(cutInfo);
        direction.Add(cutDirection); // The cut direction (horizontal, vertical)

        //for(int i = 0; i < newVertices.Length; i++)
        //{
        //    vertices.Add(newVertices[i]);
        //}
    }

    public void UpdateInfo(Face originalFace, float cutPosition, Direction cutDirection)
    {
        _Reset();
        _originalFace = originalFace;
        _originalCutPosition = cutPosition;
        _originalDirection = cutDirection;
        // Call the loop cut method to update the loop cut
        _LoopCut(_originalFace, _originalFace, _originalFace, _CalcCutPosition(),
                    _originalDirection);
    }

    public void UpdateCutDirection(Direction newCutDirection)
    {
        _Reset();
        _originalDirection = newCutDirection;
        // Call the loop cut method to update the loop cut
        _LoopCut(_originalFace, _originalFace, _originalFace, _CalcCutPosition(),
                    _originalDirection);
    }

    public void UpdateReferenceDirection(Direction newReferenceDirection, Direction cutDirection)
    {
        _Reset();
        _originalDirection = cutDirection;
        referenceDirection = newReferenceDirection;

        if (cutDirection == Direction.horizontal)
        {
            // Cut verticlly
            Vector2Int[] horizontalLines = _originalFace.GetLines(Direction.verctial);

            // Get the edge of the reference direction
            Vector2Int referenceEdge = _originalFace.GetLine(referenceDirection);

            // Calculate the position of the new vertices
            int leftLineIndex = (int)Direction.left < (int)Direction.right ? 0 : 1;

            if (horizontalLines[leftLineIndex].y == referenceEdge.x ||
                horizontalLines[leftLineIndex].y == referenceEdge.y)
            { // Flipped direction
                _flippedCutPosition = true;
            }
        }
        else
        {
            // Cut verticlly
            Vector2Int[] verticalLines = _originalFace.GetLines(Direction.horizontal);

            // Calculate the position of the new vertices
            int topLineIndex = (int)Direction.top < (int)Direction.bottom ? 0 : 1;

            // Get the edge of the reference direction
            Vector2Int referenceEdge = _originalFace.GetLine(referenceDirection);

            if (verticalLines[topLineIndex].y == referenceEdge.x ||
                verticalLines[topLineIndex].y == referenceEdge.y)
            { // Flipped direction
                _flippedCutPosition = true;
            }
        }

        UpdateCutPosition(_originalCutPosition);

        // Call the loop cut method to update the loop cut
        //_LoopCut(_originalFace, _originalFace, _originalFace, _originalCutPosition,
        //            _originalDirection);
    }

    public void UpdateFace(Face newFaceToCut)
    {
        _Reset();
        _originalFace = newFaceToCut;
        // Call the loop cut method to update the loop cut
        _LoopCut(_originalFace, _originalFace, _originalFace, _CalcCutPosition(),
                    _originalDirection);
    }

    public void UpdateCutPosition(float newCutPosition)
    {
        _Reset();
        _originalCutPosition = newCutPosition;
        // Call the loop cut method to update the loop cut
        _LoopCut(_originalFace, _originalFace, _originalFace, _CalcCutPosition(),
                    _originalDirection);
    }

    private float _CalcCutPosition()
    {
        return _flippedCutPosition ? 1f - _originalCutPosition : _originalCutPosition;
    }

    public void Draw()
    {
        // Draw the demo
        Gizmos.color = color;

        // Connect the vertices
        for(int i = 0; i < _vertices.Count/2; i++)
        {
            Gizmos.DrawLine(_vertices[(i * 2)], _vertices[(i * 2) + 1]);

        }
    }

    private void _Reset()
    {
        _vertices = new List<Vector3>();
        faces = new List<CutInfo>();
        direction = new List<Direction>();
    }

    public void ApplyChange(List<Vector3> originalVertices, List<Face> originalFaces)
    {
        for (int i = 0; i < faces.Count; i++)
        {
            // Remove the old face from the orignal faces list
            originalFaces.Remove(faces[i].original);
        }

        int nextFaceIndex;
        int previousFaceIndex;
        int leftFaceIndex = (int)Direction.left < (int)Direction.right ? 0 : 1;
        int rightFaceIndex = leftFaceIndex == 0 ? 1 : 0;
        int topFaceIndex = (int)Direction.top < (int)Direction.bottom ? 0 : 1;
        int bottomFaceIndex = topFaceIndex == 0 ? 1 : 0;

        Face[] neighbours = new Face[4];

        for (int i = 0; i < _vertices.Count / 2; i++)
        {
            originalVertices.Add(_vertices[(i * 2)]);
            originalVertices.Add(_vertices[(i * 2) + 1]);

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

                neighbours[(int)Direction.left] = faces[i].original.GetNeighbourFace(faces[i].original, Direction.left);
                neighbours[(int)Direction.right] = faces[i].original.GetNeighbourFace(faces[i].original, Direction.right);

                // Assign the preserved faces - 1 face assigned
                faces[i].subFaces[leftFaceIndex].SetFace(
                    neighbours[(int)Direction.left],
                    Direction.left);
                faces[i].subFaces[rightFaceIndex].SetFace(
                    neighbours[(int)Direction.right],
                    Direction.right);

                // Assign the subfaces to the neighbour faces
                neighbours[(int)Direction.left].ReplaceNeighbourFace(faces[i].original, faces[i].subFaces[leftFaceIndex]);
                neighbours[(int)Direction.right].ReplaceNeighbourFace(faces[i].original, faces[i].subFaces[rightFaceIndex]);

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
                neighbours[(int)Direction.top] = faces[i].original.GetNeighbourFace(faces[i].original, Direction.top);
                neighbours[(int)Direction.bottom] = faces[i].original.GetNeighbourFace(faces[i].original, Direction.bottom);

                // Assign the preserved faces - 1 face assigned
                faces[i].subFaces[topFaceIndex].SetFace(
                    faces[i].original.GetNeighbourFace(faces[i].original, Direction.top),
                    Direction.top);
                faces[i].subFaces[bottomFaceIndex].SetFace(
                    faces[i].original.GetNeighbourFace(faces[i].original, Direction.bottom),
                    Direction.bottom);


                // Assign the subfaces to the neighbour faces
                neighbours[(int)Direction.top].ReplaceNeighbourFace(faces[i].original, faces[i].subFaces[topFaceIndex]);
                neighbours[(int)Direction.bottom].ReplaceNeighbourFace(faces[i].original, faces[i].subFaces[bottomFaceIndex]);

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



    private void _LoopCut(Face firstCaller, Face caller, Face face, float cutPositionInPercent, Direction cutDirection)
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

        //faces.Add(newFaces[0]);
        //faces.Add(newFaces[1]);

        SetNewFace(face, newFaces, cutDirection);

        if (faces.Count > 100)
        {
            MonoBehaviour.print("Reached Faces Limit");
            return;
        }


        MonoBehaviour.print($"Move from {face.name} face to {newFace.name} face");

        // Make sure that the new face is not the current face
        if (newFace != firstCaller)
        { // The loop is not completed yet

            Direction newCutDirection = newFace.GetCutDirection(face);

            // Check if there is a shift in the cut direction from vertical to horizontal or the otherway around.
            if (cutDirection != newCutDirection)
            {
                cutPositionInPercent = 1 - cutPositionInPercent;
            }

            // Call the loop cut function to cut the next face
            _LoopCut(firstCaller, face, newFace, cutPositionInPercent, newCutDirection);
        }

    }


    private Face[] _CutHorizontally(Face face, float cutPositionInPercent)
    {
        // Cut verticlly
        Vector2Int[] horizontalLines = face.GetLines(Direction.verctial);

        // Calculate the position of the new vertices
        int leftLineIndex = (int)Direction.left < (int)Direction.right ? 0 : 1;
        int rightLineIndex = (int)Direction.left < (int)Direction.right ? 1 : 0;
        Vector3 leftCut =
            Vector3.Lerp(
                originalVertices[horizontalLines[leftLineIndex].x],
                originalVertices[horizontalLines[leftLineIndex].y],
                cutPositionInPercent);

        Vector3 rightCut =
            Vector3.Lerp(
                originalVertices[horizontalLines[rightLineIndex].x],
                originalVertices[horizontalLines[rightLineIndex].y],
                cutPositionInPercent);

        // To understand the reason for adding the vertices.Count to the index, read the summery of the LoopCutDemoInfo class
        // Add the points to the vertices
        _vertices.Add(leftCut);
        int leftVertixIndex = _vertices.Count - 1 + originalVertices.Count;
        _vertices.Add(rightCut);
        int rightVertixIndex = _vertices.Count - 1 + originalVertices.Count;




        Vector2Int cutThrough = new Vector2Int();
        cutThrough.x = (int)Direction.left < (int)Direction.right ? leftVertixIndex : rightVertixIndex;
        cutThrough.y = (int)Direction.left < (int)Direction.right ? rightVertixIndex : leftVertixIndex;

        Face[] newFaces = face.Split(cutThrough, 0, Direction.horizontal);

        MonoBehaviour.print($"The {face.name} face is being cut horizontally");


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
                originalVertices[verticalLines[topLineIndex].x],
                originalVertices[verticalLines[topLineIndex].y],
                cutPositionInPercent);

        Vector3 bottomCut =
            Vector3.Lerp(
                originalVertices[verticalLines[bottomLineIndex].x],
                originalVertices[verticalLines[bottomLineIndex].y],
                cutPositionInPercent);

        // To understand the reason for adding the vertices.Count to the index, read the summery of the LoopCutDemoInfo class
        // Add the points to the vertices
        _vertices.Add(topCut);
        int topVertixIndex = _vertices.Count - 1 + originalVertices.Count;
        _vertices.Add(bottomCut);
        int bottomVertixIndex = _vertices.Count - 1 + originalVertices.Count;

        Vector2Int cutThrough = new Vector2Int();
        cutThrough.x = (int)Direction.top < (int)Direction.bottom ? topVertixIndex : bottomVertixIndex;
        cutThrough.y = (int)Direction.top > (int)Direction.bottom ? topVertixIndex : bottomVertixIndex;

        Face[] newFaces = face.Split(cutThrough, 0, Direction.verctial);
        MonoBehaviour.print($"The {face.name} face is being cut vertically");


        return newFaces;
    }
}
