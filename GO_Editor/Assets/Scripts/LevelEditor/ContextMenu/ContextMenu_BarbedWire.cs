using UnityEngine;
using System.Collections;

public class ContextMenu_BarbedWire : ContextMenu
{
    public override void DeleteObj()
    {
        FindObjectOfType<ObstaclePlacement>().DeleteObstacle(transform.parent.GetComponent<Board_Node>());
        Destroy(gameObject);
    }
}
