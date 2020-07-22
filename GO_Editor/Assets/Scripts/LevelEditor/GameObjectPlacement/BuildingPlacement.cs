using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    [SerializeField] private GameObject floorPref;
    [SerializeField] private GameObject stairsPref;

    private EditorSelector selector;
    private Board board;
    private IDisposable routine;

    private void Awake()
    {
        selector = GetComponent<EditorSelector>();
        LevelInitializer.StartAddObjEvent.AddListener(() =>
        {
            if (routine != null)
                routine.Dispose();
        });
    }

    #region BuildingBlock
    public void AddHightFloor()
    {
        LevelInitializer.StartAddObjEvent.Invoke();

        routine = Observable
            .FromCoroutine(() => selector.SelectNodeRoutine("Select node to place building block"))
            .Subscribe(_ =>
            {
                PlaceFloor(selector.Nodes[0]);
                selector.Reset();
                LevelInitializer.EndAddObjEvent?.Invoke();
            });
    }

    private GameObject PlaceFloor(GameObject point)
    {
        var instance = Instantiate(floorPref, point.transform.position, Quaternion.identity);
        point.transform.position += Vector3.up;

        return instance;
    }

    public void DeleteFloor(GameObject block)
    {
        var node = FindObjectOfType<Board>().FindNodeAt(block.transform.position);
        node.transform.position -= Vector3.up;

        Destroy(block);
    }
    #endregion

    #region Stairs
    public void AddStairs()
    {
        LevelInitializer.StartAddObjEvent.Invoke();

        routine = Observable
            .FromCoroutine(() => selector.SelectNodeRoutine("Select node to place building block"))
            .Subscribe(_ =>
            {
                PlaceStairs(selector.Nodes[0]);
                selector.Reset();
                LevelInitializer.EndAddObjEvent?.Invoke();
            });
    }

    private GameObject PlaceStairs(GameObject point)
    {
        var instance = Instantiate(stairsPref, point.transform.position, Quaternion.identity);
        point.transform.position += Vector3.up/2;

        return instance;
    }

    public void DeleteStairs(GameObject stairs)
    {
        var node = FindObjectOfType<Board>().FindNodeAt(stairs.transform.position);
        node.transform.position -= Vector3.up / 2;

        Destroy(stairs);
    }
    #endregion
}
