using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextMenuController : MonoBehaviour
{
    private EditorRaycaster raycaster;
    private GameObject target;
    private bool canShowMenu = true;

    private void Awake()
    {
        raycaster = FindObjectOfType<EditorRaycaster>();

        LevelInitializer.StartAddObjEvent.AddListener(() => canShowMenu = false);
        LevelInitializer.EndAddObjEvent.AddListener(() => canShowMenu = true);
    }

    private void LateUpdate()
    {
        if (canShowMenu)
            CheckPlayerInput();
    }

    private void CheckPlayerInput()
    {
        if (raycaster.CheckRaycast(512, "Enemy", out target))
            target.transform.Find("ContexMenu")?.gameObject.SetActive(true);
        else if (raycaster.CheckRaycast(256, "Bush", out target))
            target.transform.Find("ContexMenu")?.gameObject.SetActive(true);
        else if (raycaster.CheckRaycast(2048, "Player", out target))
            target.transform.Find("ContexMenu_player")?.gameObject.SetActive(true);
        else if (raycaster.CheckRaycast(1024, "Node", out target))
        {
            var node = target.GetComponent<Board_Node>();
            if (node.Type != NodeType.Default)
                for (int i = 0; i < target.transform.childCount; i++)
                {
                    var child = target.transform.GetChild(i);
                    if (child.GetComponent<ContextMenu>())
                        child.gameObject.SetActive(true);
                }
        }
    }
}
