using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextMenu_Obj : ContextMenu
{
    private GameObject indicator;
    public GameObject Indicator => indicator;

    public void Initialize(GameObject _indicator)
    {
        indicator = _indicator;
    }

    protected override IEnumerator SetPositionRoutine()
    {
        Debug.Log("Select new point for this object");
        while (!point1select)
        {
            if (raycaster.CheckRaycast(1024, "Node", out point1))
                point1select = true;

            yield return null;
        }

        SetNewParent();
        LevelInitializer.EndAddObjEvent?.Invoke();
        ResetFlags();
    }

    private void SetNewParent()
    {
        if (!PosIsValid())
        {
            Debug.LogWarning("This point is not valid!");
            return;
        }

        gameObject.SetActive(false);
        iTween.MoveTo(indicator, iTween.Hash(
            "x", point1.transform.position.x,
            "z", point1.transform.position.z,
            "time", 1f,
            "easetype", iTween.EaseType.easeInOutSine));

        indicator.transform.SetParent(point1.transform);
        var type = transform.parent.GetComponent<Board_Node>().Type;
        transform.parent.GetComponent<Board_Node>().Type = NodeType.Default;
        point1.GetComponent<Board_Node>().Type = type;
        transform.SetParent(point1.transform);
        transform.localPosition = Vector3.up * 2;
    }

    private bool PosIsValid()
    {
        if (point1.GetComponent<Board_Node>().Type == NodeType.Default) return true;
        else return false;
    }

    public override void DeleteObj()
    {
        transform.parent.GetComponent<Board_Node>().Type = NodeType.Default;
        Destroy(indicator.gameObject);
        Destroy(gameObject);
    }
}
