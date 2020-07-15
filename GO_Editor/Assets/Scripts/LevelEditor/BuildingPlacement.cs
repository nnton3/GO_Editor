using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    [SerializeField] private GameObject floorPref;

    private ObjectSelector selector;
    private IDisposable routine;

    private void Awake()
    {
        selector = GetComponent<ObjectSelector>();

        LevelInitializer.StartAddObjEvent.AddListener(() =>
        {
            if (routine != null)
                routine.Dispose();
        });
    }

    public void AddHightFloor()
    {
        LevelInitializer.StartAddObjEvent.Invoke();

        routine = Observable
            .FromCoroutine(() => selector.SelectNodeRoutine("Select node to place object"))
            .Subscribe(_ =>
            {
                selector.Reset();
                LevelInitializer.EndAddObjEvent?.Invoke();
            });
    }

    private GameObject PlaceFloor(GameObject point)
    {
        var instance = Instantiate(floorPref, point.transform.position, Quaternion.identity);
        return instance;
    }
}
