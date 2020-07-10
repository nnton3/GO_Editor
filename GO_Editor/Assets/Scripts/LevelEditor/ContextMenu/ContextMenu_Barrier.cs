using UnityEngine;
using System.Collections;

public class ContextMenu_Barrier : ContextMenu_Obj
{
    public override void DeleteObj()
    {
        FindObjectOfType<ObstaclePlacement>().DeleteBarrier(transform.parent.GetComponent<Board_Node>());
        Destroy(gameObject);
    }
}
