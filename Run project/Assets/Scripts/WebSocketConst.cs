using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

[System.Serializable]
public class PositionMsgGive
{
    public string id;
    public float x;
    public float y;
    public float z;

    public static PositionMsgGive FromTransform(string id, Transform t)
    {
        return new PositionMsgGive { id = id, x = t.position.x, y = t.position.y, z = t.position.z };
    }

    public Vector3 ToVector3() => new Vector3(x, y, z);
}

public class PositionMsgTake
{
    public string from;
    public float x;
    public float y;
    public float z;

    public static PositionMsgTake FromTransform(string from, Transform t)
    {
        return new PositionMsgTake { from = from, x = t.position.x, y = t.position.y, z = t.position.z };
    }

    public Vector3 ToVector3() => new Vector3(x, y, z);
}

public class Stage
{
    public string id;
}

public class Character
{
    public string id;
}

