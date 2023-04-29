using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedFaceInfo
{
    public Face face;
    public Face neighbourFace;
    public Vector2Int connectingEdge;
    public int connectingEdgeMidVertixIndex = -1;

    public ConnectedFaceInfo() { }
}
