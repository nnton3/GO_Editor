using UnityEngine;
using System.Collections;

public class ContextMenu_Officer : ContextMenu
{
    public void SetPatrolData() =>
        FindObjectOfType<EnemyPlacement>().AddPatrolData(transform.parent.gameObject);
}
