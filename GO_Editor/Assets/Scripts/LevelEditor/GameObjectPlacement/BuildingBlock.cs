using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingBlockType { Default, Elevator }

public class BuildingBlock : MonoBehaviour
{
    [SerializeField] private BuildingBlockType blockType = BuildingBlockType.Default;
    public BuildingBlockType BlockType => blockType;

    public void Initialize(Vector3 pos, BuildingBlockType type)
    {
        transform.position = pos;
        blockType = type;
    }
}
