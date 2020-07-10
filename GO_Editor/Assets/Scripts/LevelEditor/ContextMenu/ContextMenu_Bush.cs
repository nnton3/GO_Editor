using UnityEngine;
using System.Collections;

public class ContextMenu_Bush : ContextMenu
{
    protected override IEnumerator SetPositionRoutine()
    {
        FindObjectOfType<Board>().FindNodeAt(transform.parent.position).Type = NodeType.Default;
        yield return base.SetPositionRoutine();
        point1.GetComponent<Board_Node>().Type = NodeType.Bush;
    }

    public override void DeleteObj()
    {
        FindObjectOfType<Board>().FindNodeAt(transform.parent.position).Type = NodeType.Default;
        base.DeleteObj();
    }
}
