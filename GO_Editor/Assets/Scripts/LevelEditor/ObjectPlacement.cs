using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject stoneIndicatorPref;
    [SerializeField] private GameObject mineIndicatorPref;
    [SerializeField] private GameObject keyIndicatorPref;
    [SerializeField] private GameObject mapIndicatorPref;
    [SerializeField] private GameObject cutterIndicatorPref;
    [SerializeField] private GameObject contextMenuPref;
    [SerializeField] private GameObject keyContextMenuPref;

    private EditorRaycaster raycaster;
    private bool point1select;
    private GameObject point1;
    #endregion

    private void Awake()
    {
        raycaster = GetComponent<EditorRaycaster>();

        LevelInitializer.StartAddObjEvent.AddListener(() =>
        {
            StopAllCoroutines();
            ResetFlags();
        });
    }

    private void ResetFlags()
    {
        point1select = false;
    }

    public void PlaceStone()
    {
        LevelInitializer.StartAddObjEvent.Invoke();
        StartCoroutine(PlaceObjectRoutine(stoneIndicatorPref, NodeType.Stone));
    }

    public void PlaceMine()
    {
        LevelInitializer.StartAddObjEvent.Invoke();
        StartCoroutine(PlaceObjectRoutine(mineIndicatorPref, NodeType.Mine));
    }

    public void PlaceKey()
    {
        LevelInitializer.StartAddObjEvent.Invoke();
        StartCoroutine(PlaceObjectRoutine(keyIndicatorPref, NodeType.Key));
    }

    public void PlaceMap()
    {
        LevelInitializer.StartAddObjEvent.Invoke();
        StartCoroutine(PlaceObjectRoutine(mapIndicatorPref, NodeType.Map));
    }

    public void PlaceCutter()
    {
        LevelInitializer.StartAddObjEvent.Invoke();
        StartCoroutine(PlaceObjectRoutine(cutterIndicatorPref, NodeType.Cutter));
    }

    private IEnumerator PlaceObjectRoutine(GameObject objIndicatorPref, NodeType type)
    {
        Debug.Log("Select point");
        while (!point1select)
        {
            if (raycaster.CheckRaycast(1024, "Node", out point1))
            {
                var indicator = Instantiate(objIndicatorPref, point1.transform);
                GameObject contextMenu = null;

                if (type == NodeType.Key) contextMenu = Instantiate(keyContextMenuPref, point1.transform);
                else contextMenu = Instantiate(contextMenuPref, point1.transform);

                contextMenu.GetComponent<ContextMenu_Obj>().Initialize(indicator);
                point1.GetComponent<Board_Node>().Type = type;
                point1select = true;
            }
            yield return null;
        }
        LevelInitializer.EndAddObjEvent?.Invoke();
        ResetFlags();
    }
}
