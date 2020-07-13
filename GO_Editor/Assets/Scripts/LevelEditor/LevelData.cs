using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ObjectData
    {
        public NodeType type;
        public Vector3 position;

        public ObjectData(NodeType _type, Vector3 _position)
        {
            type = _type;
            position = _position;
        }
    }

[System.Serializable]
public struct BarbedWireData
{
    public Vector3 point1;
    public Vector3 point2;
    public bool isOpen;

    public BarbedWireData (Vector3 _point1, Vector3 _point2, bool _isOpen = false)
    {
        point1 = _point1;
        point2 = _point2;
        isOpen = _isOpen;
    }
}

[System.Serializable]
public struct DoorData
{
    public Vector3 point1;
    public Vector3 point2;
    public bool isOpen;

    public DoorData(Vector3 _point1, Vector3 _point2, bool _isOpen = false)
    {
        point1 = _point1;
        point2 = _point2;
        isOpen = _isOpen;
    }
}

[System.Serializable]
public struct BarrierData
{
    public Vector3 point1;
    public Vector3 point2;
    public Vector3 point3;
    public bool isOpen;

    public BarrierData(Vector3 _point1, Vector3 _point2, Vector3 _point3, bool _isOpen = false)
    {
        point1 = _point1;
        point2 = _point2;
        point3 = _point3;
        isOpen = _isOpen;
    }
}

[System.Serializable]
public struct EnemyData
{
    public EnemyIdentifier identifier;
    public Vector3 position;
    public Quaternion rotation;

    public EnemyData(EnemyIdentifier _identifier, Vector3 _position, Quaternion _rotation)
    {
        identifier = _identifier;
        position = _position;
        rotation = _rotation;
    }
}

[System.Serializable]
public struct OfficerData
{
    public EnemyIdentifier identifier;
    public Vector3 position;
    public Quaternion rotation;
    public List<Vector3> patrolPath;
    public Vector3 checkPosition;

    public OfficerData(Vector3 _pos, Quaternion _rot, List<Vector3> _path, Vector3 _checkPosition)
    {
        identifier = EnemyIdentifier.Officer;
        position = _pos;
        rotation = _rot;
        patrolPath = _path;
        checkPosition = _checkPosition;
    }
}

[System.Serializable]
public struct PlayerData
{
    public Vector3 position;

    public PlayerData(Vector3 pos) => position = pos;
}

[System.Serializable]
public struct NodeData
{
    public List<Vector3> Links;
    public Vector3 position;

    public NodeData(Vector3 _pos)
    {
        Links = new List<Vector3>();
        position = _pos;
    }

    public NodeData(List<Vector3> _links, Vector3 pos)
    {
        Links = _links;
        position = pos;
    }
}

[System.Serializable]
public struct BushData
{
    public Vector3 pos;

    public BushData(Vector3 _pos)
    {
        pos = _pos;
    }
}

[System.Serializable]
public struct SpotlightData
{
    public Vector3 startPos;
    public Vector3 endPos;
    public Quaternion rotation;

    public SpotlightData(Vector3 _startPos, Vector3 _endPos, Quaternion _rotation)
    {
        startPos = _startPos;
        endPos = _endPos;
        rotation = _rotation;
    }
}
