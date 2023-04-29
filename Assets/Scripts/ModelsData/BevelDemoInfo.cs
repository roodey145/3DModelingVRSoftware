using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BevelDemoInfo
{
    Color color = new Color(1f, 0.1f, 1f);
    public Vector3[] vertices = new Vector3[6]; // The lst two are for the middle points
    public float bevelAmount = 0f; // A value between 0 and 1
    private bool _oppositeLoopCut = false; // The direction of the cut 0 or 1
    public Face selectedFace;
    public Direction edgeDirection = Direction.top;

    private Vector2Int middleEdge;
    private Vector2Int bottomEdge;
    private Vector2Int topEdge;

    private LoopCutDemoInfo loopCutDemo;

    public BevelDemoInfo(Face selectedFace, Direction edgeDirection, List<Vector3> vertices)
    {
        this.selectedFace = selectedFace;
        this.edgeDirection = edgeDirection;


        // Get the neighbour face direction
        Face neighbour = selectedFace.GetNeighbourFace(selectedFace, edgeDirection);

        // Get the required edges info
        //Vector2Int middleEdge = selectedFace.GetLine(edgeDirection);
        Vector2Int middleEdge = selectedFace.GetConnectingEdge(neighbour);
        Direction connectingEdgeDirection = selectedFace.GetEdgeDirection(middleEdge);


        Direction cutDirection = Direction.horizontal;
        if(connectingEdgeDirection == Direction.left || connectingEdgeDirection == Direction.right)
        {
            cutDirection = Direction.verctial;
        }

        if(connectingEdgeDirection == Direction.top || connectingEdgeDirection == Direction.left)
        {
            _oppositeLoopCut = true;
        }
        else
        {
            _oppositeLoopCut = false;
        }

        loopCutDemo = new LoopCutDemoInfo(selectedFace, bevelAmount, cutDirection, connectingEdgeDirection, vertices);
    }

    #region Update Info
    public void SetBevelAmount(float amount)
    {
        this.bevelAmount = amount;
        //loopCutDemo.UpdateCutPosition(Mathf.Abs((_oppositeLoopCut? 1f : 0f) - amount));
        loopCutDemo.UpdateCutPosition(amount);
    }

    public void UpdateEdgeDirection(Direction edgeDirection)
    {
        _UpdateLoopCutInfo(edgeDirection);
        this.edgeDirection = edgeDirection;
    }

    private void _UpdateLoopCutInfo(Direction edgeDirection)
    {
        Direction cutDirection = Direction.horizontal;
        if (edgeDirection == Direction.left || edgeDirection == Direction.right)
        {
            cutDirection = Direction.verctial;
        }

        if (edgeDirection == Direction.top || edgeDirection == Direction.left)
        {
            _oppositeLoopCut = false;
        }
        else
        {
            _oppositeLoopCut = false;
        }

        loopCutDemo.UpdateCutDirection(cutDirection);
        SetBevelAmount(bevelAmount); // Force redraw
    }

    private void _CalcVerticesPosition(List<Vector3> originalVertices)
    {
        // Get the neighbour face direction
        Face neighbour = selectedFace.GetNeighbourFace(selectedFace, edgeDirection);
        // Get the information of the face in the opposite direction
        Direction myDirectionRelativeToNeighbour = 
            neighbour.GetNeighbourDirection(selectedFace);

        // Get the required edges info
        //Vector2Int middleEdge = selectedFace.GetLine(edgeDirection);
        /*Vector2Int*/ middleEdge = selectedFace.GetConnectingEdge(neighbour);
        Direction connectingEdgeDirection = selectedFace.GetEdgeDirection(middleEdge);

        //_UpdateLoopCutInfo(connectingEdgeDirection);

        // Get the bottom line
        //Vector2Int bottomEdge = selectedFace.GetOppositeLine(edgeDirection);
        /*Vector2Int*/ bottomEdge =
            new Vector2Int(
                selectedFace.GetOppositeVerticesIndex(middleEdge.x, edgeDirection),
                selectedFace.GetOppositeVerticesIndex(middleEdge.y, edgeDirection));

        // Get the top line
        /* To get the top line, we will take the middle line which is connecting the two faces. Take the index of the 
         * first point, and get the opposite point to it using the get opposite vertix index. The opposite of a 
         * vertix, is the vertix on the same provide axis. For instance, if the point is topLeft, and the face 
         * direction is right/left that means the opposite point is the topRight. 
         */
        //Vector2Int topEdge = neighbour.GetOppositeLine(myDirectionRelativeToNeighbour);
        /*Vector2Int*/ topEdge = 
            new Vector2Int(
                neighbour.GetOppositeVerticesIndex(middleEdge.x, myDirectionRelativeToNeighbour),
                neighbour.GetOppositeVerticesIndex(middleEdge.y, myDirectionRelativeToNeighbour) );

        int leftIndex = (int)Direction.left < (int)Direction.right ? 0 : 1;
        int rightIndex = leftIndex == 0 ? 1 : 0;

        //bool isLeft
        // Calculate the the placement of the first point
        vertices[(int)Face.VerticesPos.topLeft] =
            Vector3.Lerp(
                originalVertices[middleEdge[leftIndex]],
                originalVertices[topEdge[leftIndex]],
                bevelAmount);

        // Calculate the the placement of the second point
        vertices[(int)Face.VerticesPos.topRight] =
            Vector3.Lerp(
                originalVertices[middleEdge[rightIndex]],
                originalVertices[topEdge[rightIndex]],
                bevelAmount);


        // Calculate the the placement of the third point
        vertices[(int)Face.VerticesPos.bottomLeft] =
            Vector3.Lerp(
                originalVertices[middleEdge[leftIndex]],
                originalVertices[bottomEdge[leftIndex]],
                bevelAmount);


        // Calculate the the placement of the fourth point
        vertices[(int)Face.VerticesPos.bottomRight] =
            Vector3.Lerp(
                originalVertices[middleEdge[rightIndex]],
                originalVertices[bottomEdge[rightIndex]],
                bevelAmount);

        // Calculate the placement of the fifth point
        vertices[4 + leftIndex] =
            Vector3.Lerp(
                vertices[(int)Face.VerticesPos.topLeft],
                vertices[(int)Face.VerticesPos.bottomLeft],
                0.5f);

        // Calculate the placement of the sixth point
        vertices[4 + rightIndex] =
            Vector3.Lerp(
                vertices[(int)Face.VerticesPos.topRight],
                vertices[(int)Face.VerticesPos.bottomRight],
                0.5f);
    }
    #endregion

    private void DrawFace(Vector3[] faceVertices)
    {
        // Draw the top line
        Gizmos.DrawLine(faceVertices[(int)Face.VerticesPos.topLeft], faceVertices[(int)Face.VerticesPos.topRight]);

        // Draw the bottom line
        Gizmos.DrawLine(faceVertices[(int)Face.VerticesPos.bottomLeft], faceVertices[(int)Face.VerticesPos.bottomRight]);

        // Draw the left Line
        Gizmos.DrawLine(faceVertices[(int)Face.VerticesPos.topLeft], faceVertices[(int)Face.VerticesPos.bottomLeft]);

        // Draw the right Line
        Gizmos.DrawLine(faceVertices[(int)Face.VerticesPos.topRight], faceVertices[(int)Face.VerticesPos.bottomRight]);
    }

    public void Draw(List<Vector3> originalVertices)
    {
        _CalcVerticesPosition(originalVertices);

        loopCutDemo.Draw();

        Gizmos.color = Color.yellow;
        // Draw the top line
        Gizmos.DrawLine(vertices[(int)Face.VerticesPos.topLeft], vertices[(int)Face.VerticesPos.topRight]);
        Gizmos.color = color;
        // Draw the ceneter line
        Gizmos.DrawLine(vertices[4], vertices[5]);

        // Draw the bottom line
        Gizmos.DrawLine(vertices[(int)Face.VerticesPos.bottomLeft], vertices[(int)Face.VerticesPos.bottomRight]);

        // Draw the left Line
        Gizmos.DrawLine(vertices[(int)Face.VerticesPos.topLeft], vertices[(int)Face.VerticesPos.bottomLeft]);

        // Draw the right Line
        Gizmos.DrawLine(vertices[(int)Face.VerticesPos.topRight], vertices[(int)Face.VerticesPos.bottomRight]);


        // Draw the sub face that is to be modified
        Vector2Int selectedEdge = selectedFace.GetLine(edgeDirection);

        

        int indexOfSubFace = -1;
        for(int subFaceI = 0; subFaceI < 2; subFaceI++)
        {
            for(int verI = 0; verI < 4; verI++)
            {
                if (loopCutDemo.faces[0].subFaces[subFaceI].GetVertixIndex((Face.VerticesPos)verI) == middleEdge.x
                    || loopCutDemo.faces[0].subFaces[subFaceI].GetVertixIndex((Face.VerticesPos)verI) == middleEdge.y)
                {
                    indexOfSubFace = subFaceI;
                    break;
                } 
            }

            if (indexOfSubFace != -1) break;
        }


        if(indexOfSubFace == -1)
        {
            Gizmos.color = new Color(0.5f, 1f, 0.5f);
            Face neighbour = selectedFace.GetNeighbourFace(selectedFace, edgeDirection);
            int[] neighbourVI = neighbour._verticesIndex;
            Vector3[] neighbourVertices = new Vector3[4];
            neighbourVertices[0] = originalVertices[neighbourVI[0]];
            neighbourVertices[1] = originalVertices[neighbourVI[1]];
            neighbourVertices[2] = originalVertices[neighbourVI[2]];
            neighbourVertices[3] = originalVertices[neighbourVI[3]];
            DrawFace(neighbourVertices);
            MonoBehaviour.print("FACE ERROR");
            return;
        }

        Face subFaceToBeModified = loopCutDemo.faces[0].subFaces[indexOfSubFace];
        int[] subFaceVIndex = subFaceToBeModified._verticesIndex;

        Vector3[] subFaceVertices = new Vector3[4];

        for(int i = 0; i < subFaceVIndex.Length; i++)
        {
            if (subFaceVIndex[i] >= originalVertices.Count)
            {
                subFaceVertices[i] = loopCutDemo._vertices[subFaceVIndex[i] - originalVertices.Count];
            }
            else
            {
                subFaceVertices[i] = originalVertices[subFaceVIndex[i]];
            }
        }

        
        Gizmos.color = new Color(0f, 1f, 0f);
        DrawFace(subFaceVertices);

        // Draw the parent face
        Face parentFace = loopCutDemo.faces[0].original;

        // Get parent face vertices
        Vector3[] parentFaceVertices = new Vector3[4];
        for(int i = 0; i < parentFace._verticesIndex.Length; i++)
        {
            parentFaceVertices[i] = originalVertices[parentFace._verticesIndex[i]];
        }

        Gizmos.color = Color.black;
        //DrawFace(parentFaceVertices);

        Gizmos.color = Color.yellow;
        // Draw the edge at the selected direction
        Gizmos.DrawLine(originalVertices[middleEdge.x], originalVertices[middleEdge.y]);

    }


    public void ApplyChange(List<Vector3> originalVertices, List<Face> originalFaces)
    {
        // Print the first face subfaces info information
        MonoBehaviour.print("Face 1: \n");
        for (int i = 0; i < 4; i++)
        {
            int vertixIndex = loopCutDemo.faces[0].subFaces[0].GetVertixIndex((Face.VerticesPos)i);
            MonoBehaviour.print("Vertix Index: " + vertixIndex);
            if (vertixIndex >= originalVertices.Count)
            {
                MonoBehaviour.print(loopCutDemo._vertices[vertixIndex - originalVertices.Count]);
            }
            else
            {
                MonoBehaviour.print(originalVertices[vertixIndex]);
            }
        }
    


        // Print the first face subfaces info information
        MonoBehaviour.print("Face 2: \n");
        for (int i = 0; i < 4; i++)
        {
            int vertixIndex = loopCutDemo.faces[0].subFaces[1].GetVertixIndex((Face.VerticesPos)i);
            MonoBehaviour.print("Vertix Index: " + vertixIndex);
            if (vertixIndex >= originalVertices.Count)
            {
                MonoBehaviour.print(loopCutDemo._vertices[vertixIndex - originalVertices.Count]);
            }
            else
            {
                MonoBehaviour.print(originalVertices[vertixIndex]);
            }
        }

        // Print the bevel lines info
        //MonoBehaviour.print("Top Edge: (" + vertices[(int)Face.VerticesPos.topLeft] + ", " + vertices[(int)Face.VerticesPos.topRight] + ")");
        //MonoBehaviour.print("Bottom Edge: (" + vertices[(int)Face.VerticesPos.bottomLeft] + ", " + vertices[(int)Face.VerticesPos.bottomRight] + ")");

        Vector2Int selectedEdge = selectedFace.GetLine(edgeDirection);
        MonoBehaviour.print("Selected Edge: " + selectedEdge);
        MonoBehaviour.print("Middle Edge: " + middleEdge);
        MonoBehaviour.print("Top Edge: " + topEdge);

        // The face that shall be adjusted has the same vertices index as the line at the "edgeDirection"
        // Update the vertices which connect the line at "edgeDirection"
        originalVertices[middleEdge.x] = vertices[(int)Face.VerticesPos.topRight];
        originalVertices[middleEdge.y] = vertices[(int)Face.VerticesPos.topLeft];

        loopCutDemo.ApplyChange(originalVertices, originalFaces);

    }
}
