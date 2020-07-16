using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextMenu_Building : ContextMenu
{
    public void DeleteBlock()
    {
        FindObjectOfType<BuildingPlacement>().DeleteFloor(transform.parent.gameObject);
    }

    public void DeleteStairs()
    {
        FindObjectOfType<BuildingPlacement>().DeleteStairs(transform.parent.gameObject);
    }

    public void DeleteLadder() { }
}
