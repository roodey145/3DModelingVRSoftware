using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is intended to get the data of four points and create a face out of them.
/// The Face class does not actually have access to the vertices position, but to the 
/// indeces of the vertices, thus, to get access to the vertices coordinate, the vertices
/// array in the model class shall be used. 
/// </summary>

public class Face
{
    public enum VerticesPos : int
    {
        topLeft = 0,
        topRight = 1,
        bottomRight = 2,
        bottomLeft = 3
    }

    private int[] _verticesIndex; // TopLeft, topRight, bottomRight, bottomLeft
    private Vector2Int[] _lines;
    private Face[] _faces; // Neighbour faces

    private static byte _linesCount = 4;
    private static byte _facesCount = 4;

    public string name = "";

    private Direction _lastFaceCutDirection = Direction.verctial;

    /// <summary>
    /// An empty face that has data. The faces and lines of this face shall be assigned
    /// manually
    /// </summary>
    public Face()
    {
        _lines = new Vector2Int[_linesCount];
        _faces = new Face[_facesCount];
    }


    public Face(Face[] neighbourFaces, int[] verticesIndex)
    {
        // Make sure there is enough faces and lines
        if (neighbourFaces.Length < _facesCount)
        {
            throw new Exception($"Expecting Faces array of length {_facesCount}, " +
                                $"but got a Faces array of length {neighbourFaces.Length}");
        }
        if (verticesIndex.Length < _linesCount)
        {
            throw new Exception($"Expecting int[] array of length {_linesCount}, " +
                                $"but got a int[] array of length {verticesIndex.Length}");
        }
        _faces = neighbourFaces;
        _verticesIndex = verticesIndex;
    }

    /// <summary>
    /// Create a face.
    /// </summary>
    /// <param name="neighbourFaces">
    /// The faces which are connected to this one, ordered as follows:
    ///     Top, right, bottom, left.
    /// </param>
    /// <param name="lines">
    /// The lines of this face ordered as follows:
    ///     Top, right, bottom, left.
    /// </param>
    public Face(Face[] neighbourFaces, Vector2Int[] lines)
    {
        // Make sure there is enough faces and lines
        if(neighbourFaces.Length < _facesCount)
        {
            throw new Exception($"Expecting Faces array of length {_facesCount}, " +
                                $"but got a Faces array of length {neighbourFaces.Length}");
        }
        if(lines.Length < _linesCount)
        {
            throw new Exception($"Expecting Vector2Int array of length {_linesCount}, " +
                                $"but got a Vector2Int array of length {lines.Length}");
        }
        _faces = neighbourFaces;
        _lines = lines;
    }


    public Vector2Int[] GetLines(Direction dir)
    {
        Vector2Int[] lines = null;
        switch(dir)
        {
            case Direction.horizontal:
                //lines = new Vector2Int[]
                //{
                //    _lines[(int)Direction.left], _lines[(int)Direction.right]
                //};

                lines = _GetHorizontalLines();
                break;

            case Direction.verctial:
                lines = _GetVerticalLines();
                break;

            case Direction.all:
                Vector2Int[] horizontalLines = _GetHorizontalLines();
                Vector2Int[] verticalLines = _GetVerticalLines();

                lines = new Vector2Int[_linesCount];

                lines[(int)Direction.left] = 
                    horizontalLines[(int)Direction.left < (int)Direction.right ? 0 : 1];

                lines[(int)Direction.right] =
                    horizontalLines[(int)Direction.left < (int)Direction.right ? 1 : 0];

                lines[(int)Direction.top] =
                    verticalLines[(int)Direction.top < (int)Direction.bottom ? 0 : 1];

                lines[(int)Direction.bottom] =
                    verticalLines[(int)Direction.top < (int)Direction.bottom ? 1 : 0];

                break;
        }

        return lines;
    }


    private Vector2Int[] _GetVerticalLines()
    {
        Vector2Int[] lines = new Vector2Int[2];

        // Get the right line
        Vector2Int rightLine =
            new Vector2Int(_verticesIndex[(int)VerticesPos.topLeft],
                            _verticesIndex[(int)VerticesPos.bottomLeft]);

        // Get the left line
        Vector2Int leftLine =
            new Vector2Int(_verticesIndex[(int)VerticesPos.topRight],
                            _verticesIndex[(int)VerticesPos.bottomRight]);

        lines[(int)Direction.left < (int)Direction.right ? 0 : 1] = leftLine;
        lines[(int)Direction.left < (int)Direction.right ? 1 : 0] = rightLine;


        return lines;
    }


    private Vector2Int[] _GetHorizontalLines()
    {
        Vector2Int[] lines = new Vector2Int[2];

        // Get the top line
        Vector2Int topLine =
            new Vector2Int(_verticesIndex[(int)VerticesPos.topLeft],
                            _verticesIndex[(int)VerticesPos.topRight]);

        // Get the bottom line
        Vector2Int bottomLine =
            new Vector2Int(_verticesIndex[(int)VerticesPos.bottomLeft],
                            _verticesIndex[(int)VerticesPos.bottomRight]);

        lines[(int)Direction.top < (int)Direction.bottom ? 0 : 1] = topLine;
        lines[(int)Direction.top < (int)Direction.bottom ? 1 : 0] = bottomLine;


        return lines;
    }


    public Direction GetLastFaceCutDirection()
    {
        return _lastFaceCutDirection;
    }


    public Direction GetCutDirection(Face previoselyCuttedFace)
    {
        Direction dir = Direction.horizontal;

        dir = _GetCallerFaceOppositeDirection(previoselyCuttedFace, dir);

        if (dir == Direction.right || dir == Direction.left)
        { // The cut shall be horizontal
            dir = Direction.horizontal;
        }
        else
        { // The cut shall be vertical
            dir = Direction.verctial;
        }

        _lastFaceCutDirection = dir;

        return dir;
    }

    /// <summary>
    /// Get a neighbour face using one of the following directions only:
    ///     top, right, bottom, left
    /// </summary>
    /// <param name="dir">The direction of the face to be fetched.</param>
    /// <returns>Return the face connected to this face in the specified direction.</returns>
    public Face GetNeighbourFace(Face caller, Direction dir)
    {
        Face face;

        // Convert the wanted face to the face relative to the caller
        dir = _GetCallerFaceOppositeDirection(caller, dir);
        
        // The reason this if statement works is, the face that the expected directions
            // top, right, bottom, left has the numerical values 0, 1, 2, 3 respectively. 
        if((int)dir < _facesCount)
        {
            face = _faces[(int)dir];
        }
        else
        {
            throw new Exception($"Expected top, right, bottom, left directions, " +
                                $"but got {dir}");
        }

        return face;
    }

    public Direction GetNeighbourDirection(Face neighbourFace)
    {
        Direction dir = Direction.all;
        if(neighbourFace != null)
        {
            // Get the placement of the face inside the faces array
            for (int i = 0; i < _faces.Length; i++)
            {
                if (_faces[i] == neighbourFace)
                {
                    dir = (Direction)i;
                    break;
                }
            }
        }

        return dir;
    }

    private Direction _GetCallerFaceOppositeDirection(Face face, Direction dir)
    {
        // Initialize the top direction relative to this face
        //Direction dir = Direction.top;

        if(face != this)
        { // The top direction shall be relative to the caller face
            // Get the placement of the face inside the faces array
            for (int i = 0; i < _faces.Length; i++)
            {
                if (_faces[i] == face)
                {
                    dir = _GetTheOppositeDirection((Direction)i);
                    break;
                }
            }
        }

        return dir;
    }

    private Direction _GetTheOppositeDirection(Direction dir)
    {
        switch(dir)
        {
            case Direction.left:
                dir = Direction.right;
                break;
            case Direction.right:
                dir = Direction.left;
                break;
            case Direction.bottom:
                dir = Direction.top;
                break;
            case Direction.top:
                dir = Direction.bottom;
                break;
        }

        return dir;
    }

    public void SetVerticesIndex(int[] verticesIndex)
    {
        if(verticesIndex.Length == _linesCount)
        {
            _verticesIndex = verticesIndex;
        }
    }

    public void UpdateVerticesIndex(Vector2Int oldVerticesIndex, Vector2Int newVerticesIndex)
    {
        for(int i = 0; i < _verticesIndex.Length; i++)
        {
            if(oldVerticesIndex.x == _verticesIndex[i])
            {
                _verticesIndex[i] = newVerticesIndex.x;
            }
            else if(oldVerticesIndex.y == _verticesIndex[i])
            {
                _verticesIndex[i] = newVerticesIndex.y;
            }
        }
    }

    /// <summary>
    /// Assign a line to this face.
    /// </summary>
    /// <param name="line">
    /// A vector2int that contains the indeces of the vertices which connects this line.
    /// </param>
    /// <param name="dir">
    /// Where the line shall be placed. 
    /// Expected values: top, right, bottom, left
    /// </param>
    public void SetLine(Vector2Int line, Direction dir)
    {
        if((int)dir < _linesCount)
        {
            _lines[(int)dir] = line;
        }
    }

    /// <summary>
    /// Assign a line to this face.
    /// </summary>
    /// <param name="face">
    /// A face that shares a line with this face.
    /// </param>
    /// <param name="dir">
    /// Where the face shall be placed. 
    /// Expected values: top, right, bottom, left
    /// </param>
    public void SetFace(Face face, Direction dir)
    {
        if ((int)dir < _facesCount)
        {
            _faces[(int)dir] = face;
        }
    }


    public void ReplaceNeighbourFace(Face oldFace, Face newFace)
    {
        for(int i = 0; i < _faces.Length; i++)
        {
            if (_faces[i] == oldFace)
            {
                _faces[i] = newFace;
                break;
            }
        }
    }

    /// <summary>
    /// Takes the indeces of two points and uses them to split this face in two.
    /// It is expected that the index of the first vertix shall match the right/top edge
    /// and the index of the second vertix shall match the left/bottom edge.
    /// </summary>
    /// <param name="verticesIndex">
    /// The index of the vertices which are stored in the model's vertices array.
    /// </param>
    /// <param name="splitPlacement">
    /// The placement of the split between the vertices. Value between zero and one. 
    /// A value of 0.01 means one of the new two faces will have 1% of the original
    /// face size and the other will have 99%. A value of 0.5 means the new faces 
    /// will be identical since the split will happen in the center.
    /// </param>
    /// <param name="dir">
    /// The splitting direction. Expected values are: verticle or horizontal
    /// </param>
    /// <returns>Two faces that are connected to the rest of the model.</returns>
    public Face[] Split(Vector2Int verticesIndex, float splitPlacement, Direction dir)
    {
        Face[] faces;

        if(dir == Direction.verctial)
        {
            faces = _cutVerticlly(verticesIndex);
        }
        else if(dir == Direction.horizontal)
        {
            faces = _cutHorizontally(verticesIndex);
        }
        else
        {
            throw new Exception($"Expected vertical or horizontal directions, " +
                                $"but got {dir}");
        }




        return faces;
    }


    private Face[] _cutVerticlly(Vector2Int verticesIndex, bool demo = true)
    {
        Face[] faces = new Face[2];

        //-------------------------------------------
        // Create the Left face
        faces[(int)Direction.left < (int)Direction.right? 0 : 1] = new Face();

        int[] leftFaceVerticesIndex = new int[4];
        //{
        //    verticesIndex.x, // TopLeft
        //    _verticesIndex[(int)VerticesPos.topRight], // TopRight
        //    _verticesIndex[(int)VerticesPos.bottomRight], // BottomRight
        //    verticesIndex.y, // BottomLeft
        //};

        leftFaceVerticesIndex[(int)VerticesPos.topLeft] = _verticesIndex[(int)VerticesPos.topLeft];
        leftFaceVerticesIndex[(int)VerticesPos.topRight] = verticesIndex.x;
        leftFaceVerticesIndex[(int)VerticesPos.bottomRight] = verticesIndex.y;
        leftFaceVerticesIndex[(int)VerticesPos.bottomLeft] = _verticesIndex[(int)VerticesPos.bottomLeft];

        faces[(int)Direction.left < (int)Direction.right ? 0 : 1].SetVerticesIndex(leftFaceVerticesIndex);


        //// Create and Set the left line
        //Vector2Int leftFaceLeftLine = new Vector2Int();
        //leftFaceLeftLine.x = _lines[(int)Direction.top].x;
        //leftFaceLeftLine.y = _lines[(int)Direction.bottom].x;
        //faces[0].SetLine(leftFaceLeftLine, Direction.left);

        //// Create and Set the top line
        //Vector2Int leftFaceTopLine = new Vector2Int();
        //leftFaceTopLine.x = _lines[(int)Direction.top].x;
        //leftFaceTopLine.y = verticesIndex.x;
        //faces[0].SetLine(leftFaceTopLine, Direction.top);

        //// Create and Set the right line
        //Vector2Int leftFaceRightLine = new Vector2Int();
        //leftFaceRightLine.x = verticesIndex.x;
        //leftFaceRightLine.y = verticesIndex.y;
        //faces[0].SetLine(leftFaceRightLine, Direction.right);

        //// Create and Set the bottom line
        //Vector2Int leftFaceBottomLine = new Vector2Int();
        //leftFaceBottomLine.x = _lines[(int)Direction.bottom].x;
        //leftFaceBottomLine.y = verticesIndex.y;
        //faces[0].SetLine(leftFaceBottomLine, Direction.bottom);



        //-------------------------------------------
        // Create the right face
        faces[(int)Direction.left < (int)Direction.right ? 1 : 0] = new Face(); // TODO: I have switched position between this face and the other one. This is supposed to be right face, but it is the left face....
        int[] rightFaceVerticesIndex = new int[]
        {
            _verticesIndex[(int)VerticesPos.topLeft], // TopLeft
            verticesIndex.x, // TopRight
            verticesIndex.y, // BottomRight
            _verticesIndex[(int)VerticesPos.bottomLeft] // BottomLeft
        };

        rightFaceVerticesIndex[(int)VerticesPos.topLeft] = verticesIndex.x;
        rightFaceVerticesIndex[(int)VerticesPos.topRight] = _verticesIndex[(int)VerticesPos.topRight];
        rightFaceVerticesIndex[(int)VerticesPos.bottomRight] = _verticesIndex[(int)VerticesPos.bottomRight];
        rightFaceVerticesIndex[(int)VerticesPos.bottomLeft] = verticesIndex.y;

        faces[(int)Direction.left < (int)Direction.right ? 1 : 0].SetVerticesIndex(rightFaceVerticesIndex);


        if(!demo)
        {
            //-------------------------------------------
            // Assign the neighbour faces

            // The left face
            faces[(int)Direction.left < (int)Direction.right ? 0 : 1].SetFace(
                _faces[(int)Direction.top], Direction.top
                );
            faces[(int)Direction.left < (int)Direction.right ? 0 : 1].SetFace(
               _faces[(int)Direction.bottom], Direction.bottom
               );

            faces[(int)Direction.left < (int)Direction.right ? 0 : 1].SetFace(
                faces[(int)Direction.left < (int)Direction.right ? 1 : 0], Direction.right
                );
            faces[(int)Direction.left < (int)Direction.right ? 0 : 1].SetFace(
               _faces[(int)Direction.left], Direction.left
               );


            // The right face
            faces[(int)Direction.left < (int)Direction.right ? 1 : 0].SetFace(
                _faces[(int)Direction.top], Direction.top
                );
            faces[(int)Direction.left < (int)Direction.right ? 1 : 0].SetFace(
               _faces[(int)Direction.bottom], Direction.bottom
               );

            faces[(int)Direction.left < (int)Direction.right ? 1 : 0].SetFace(
               _faces[(int)Direction.right], Direction.right
               );
            faces[(int)Direction.left < (int)Direction.right ? 1 : 0].SetFace(
                faces[(int)Direction.left < (int)Direction.right ? 0 : 1], Direction.left
                );
        }



        return faces;
    }


    private Face[] _cutHorizontally(Vector2Int verticesIndex)
    {
        Face[] faces = new Face[2];

        //-------------------------------------------
        // Create the Left face
        faces[0] = new Face();

        int[] leftFaceVerticesIndex = new int[]
        {
            _verticesIndex[(int)VerticesPos.topLeft], // TopLeft
            _verticesIndex[(int)VerticesPos.topRight], // TopRight
            verticesIndex.y, // BottomRight
            verticesIndex.x, // BottomLeft
        };

        faces[0].SetVerticesIndex(leftFaceVerticesIndex);



        //-------------------------------------------
        // Create the right face
        faces[1] = new Face();
        int[] rightFaceVerticesIndex = new int[]
        {
            verticesIndex.x, // TopLeft
            verticesIndex.y, // TopRight
            _verticesIndex[(int)VerticesPos.bottomRight], // BottomRight
            _verticesIndex[(int)VerticesPos.bottomLeft] // BottomLeft
        };

        faces[1].SetVerticesIndex(rightFaceVerticesIndex);

        return faces;
    }
}
