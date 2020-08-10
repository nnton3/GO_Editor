using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;

public class ObstaclePlacement : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject barrierObstaclePref;
    [SerializeField] private GameObject bushPref;
    [SerializeField] private GameObject spotlightPref;
    [SerializeField] private GameObject leverIndicatorPref;
    [SerializeField] private GameObject barrierContextMenuPref;
    [SerializeField] private GameObject doorContextMenuPref;
    [SerializeField] private GameObject barbedWireContextMenuPref;

    private EditorRaycaster raycaster;
    private EditorSelector selector;
    private IDisposable routine;

    private GameObject _point1;
    private GameObject _point2;
    #endregion

    private void Awake()
    {
        raycaster = GetComponent<EditorRaycaster>();
        selector = GetComponent<EditorSelector>();

        LevelInitializer.StartAddObjEvent.AddListener(() =>
        {
            if (routine != null)
                routine.Dispose();
        });
    }

    #region Barrier
    public void AddBarrier()
    {
        LevelInitializer.StartAddObjEvent?.Invoke();
        routine = Observable
            .FromCoroutine(() => selector.SelectNodeRoutine("Select point for lever"))
            .SelectMany(() => selector.SelectNodeRoutine("Select point 1"))
            .SelectMany(() => selector.SelectNodeRoutine("Select point 2"))
            .Subscribe(_ =>
            {
                CreateBarreir(selector.Nodes[0], selector.Nodes[1], selector.Nodes[2]);
                selector.Reset();
                LevelInitializer.EndAddObjEvent?.Invoke();
            });
    }

    public void CreateBarreir(GameObject _point1, GameObject _point2, GameObject _point3)
    {
        if (!CheckLink(_point2, _point3)) return;

        var obstacle = _point1.AddComponent<Barrier>();
        _point1.GetComponent<Board_Node>().Type = NodeType.Lever;
        _point2.GetComponent<Board_Node>().RemoveLink(_point3.GetComponent<Board_Node>());

        var indicator = Instantiate(leverIndicatorPref, _point1.transform);
        var contexMenu = Instantiate(barrierContextMenuPref, _point1.transform);
        contexMenu.GetComponent<ContextMenu_Barrier>().Initialize(indicator);

        obstacle.SetPoints(_point2, _point3, barrierObstaclePref);
        _point1.GetComponent<DynamicObstacle>().Initialize();
    }

    public void DeleteBarrier(Board_Node mainNode)
    {
        for (int i = 0; i < mainNode.transform.childCount; i++)
        {
            var child = mainNode.transform.GetChild(i);
            if (child.GetComponent<ContextMenu>())
                Destroy(child.GetComponent<ContextMenu_Obj>().Indicator);
        }
        DeleteObstacle(mainNode);
    }
    #endregion

    #region Door
    public void AddDoor()
    {
        LevelInitializer.StartAddObjEvent?.Invoke();

        routine = Observable
            .FromCoroutine(() => selector.SelectNodeRoutine("Select point 1"))
            .SelectMany(() => selector.SelectNodeRoutine("Select point 2"))
            .Subscribe(_ =>
            {
                PlaceObstacle(selector.Nodes[0], selector.Nodes[1], doorContextMenuPref);
                InitDoor(selector.Nodes[0], selector.Nodes[1], selector.Nodes[0].GetComponent<DynamicObstacle>());
                selector.Reset();
                LevelInitializer.EndAddObjEvent?.Invoke();
            });
    }

    public void PlaceDoor(GameObject _point1, GameObject _point2)
    {
        PlaceObstacle(_point1, _point2, doorContextMenuPref);
        InitDoor(_point1, _point2, _point1.GetComponent<DynamicObstacle>());
    }

    public void InitDoor(GameObject _point1, GameObject _point2, DynamicObstacle obstacle)
    {
        var openers = new List<Opener>();
        if (!_point1.GetComponent<Opener>())
            openers.Add(_point1.AddComponent<Door>());
        if (!_point2.GetComponent<Opener>())
            openers.Add(_point2.AddComponent<Door>());

        _point1.GetComponent<Board_Node>().Type = NodeType.Opener;
        _point2.GetComponent<Board_Node>().Type = NodeType.Opener;

        foreach (var opener in openers)
        {
            opener.SetObstacle(obstacle);
            opener.Initialize();
        }
    }
    #endregion

    #region BarbedWire
    public void AddBarbedWire()
    {
        LevelInitializer.StartAddObjEvent?.Invoke();
        routine = Observable
            .FromCoroutine(() => selector.SelectNodeRoutine("Select point 1"))
            .SelectMany(() => selector.SelectNodeRoutine("Select point 2"))
            .Subscribe(_ =>
            {
                PlaceObstacle(selector.Nodes[0], selector.Nodes[1], barbedWireContextMenuPref);
                InitBarbedWire(selector.Nodes[0], selector.Nodes[1], selector.Nodes[0].GetComponent<DynamicObstacle>());
                selector.Reset();
                LevelInitializer.EndAddObjEvent?.Invoke();
            });
    }

    public void PlaceBarbedWire(GameObject _point1, GameObject _point2)
    {
        PlaceObstacle(_point1, _point2, barbedWireContextMenuPref);
        InitBarbedWire(_point1, _point2, _point1.GetComponent<DynamicObstacle>());
    }

    public void InitBarbedWire(GameObject _point1, GameObject _point2, DynamicObstacle obstacle)
    {
        var openers = new List<Opener>();
        if (!_point1.GetComponent<Opener>())
            openers.Add(_point1.AddComponent<BarbedWire>());
        if (!_point2.GetComponent<Opener>())
            openers.Add(_point2.AddComponent<BarbedWire>());

        _point1.GetComponent<Board_Node>().Type = NodeType.Opener;
        _point2.GetComponent<Board_Node>().Type = NodeType.Opener;

        foreach (var opener in openers)
        {
            opener.SetObstacle(obstacle);
            opener.Initialize();
        }
    }
    #endregion

    #region Bush
    public void AddBush()
    {
        LevelInitializer.StartAddObjEvent?.Invoke();

        routine = Observable
            .FromCoroutine(() => selector.SelectNodeRoutine("Select node to place bush"))
            .Subscribe(_ =>
            {
                PlaceBush(selector.Nodes[0].GetComponent<Board_Node>());
                selector.Reset();
            });
    }

    public void PlaceBush(Board_Node node)
    {
        node.Type = NodeType.Bush;
        Instantiate(bushPref, node.transform.position, Quaternion.identity);
    }
    #endregion

    #region Spotlight
    public void AddSpotlight()
    {
        LevelInitializer.StartAddObjEvent?.Invoke();

        routine = Observable
            .FromCoroutine(() => selector.SelectNodeRoutine("Select start point"))
            .SelectMany(() => selector.SelectNodeRoutine("Select end point"))
            .Subscribe(_ =>
            {
                _point1 = selector.Nodes[0];
                _point2 = selector.Nodes[1];
                PlaceSpotlight(_point1, _point2);
                selector.Reset();
                LevelInitializer.EndAddObjEvent?.Invoke();
            });
    }

    public void PlaceSpotlight(GameObject point1, GameObject point2, Quaternion rotation = new Quaternion())
    {
        var spotlightInstance = Instantiate(spotlightPref, point1.transform.position, rotation);
        spotlightInstance.GetComponent<SpotlightMover>().SetMovementParams(point1.transform.position, point2.transform.position);
        spotlightInstance.transform.rotation = rotation;
    }
    #endregion

    public void DeleteObstacle(Board_Node mainNode)
    {
        var obstacle = mainNode.GetComponent<DynamicObstacle>();
        obstacle.Point1.GetComponent<Board_Node>().AddLink(obstacle.Point2.GetComponent<Board_Node>());
        mainNode.Type = NodeType.Default;
        obstacle.Point2.GetComponent<Board_Node>().Type = NodeType.Default;

        for (int i = 0; i < mainNode.transform.childCount; i++)
        {
            var child = mainNode.transform.GetChild(i);
            if (child.GetComponent<ContextMenu>())
                Destroy(child.gameObject);
        }

        Destroy(obstacle.Obstacle.gameObject);
        Destroy(obstacle.Point1.GetComponent<Opener>());
        Destroy(obstacle.Point2.GetComponent<Opener>());
        Destroy(obstacle);
    }

    public void PlaceObstacle(GameObject _point1, GameObject _point2, GameObject contextMenuPref = null)
    {
        if (!CheckLink(_point1, _point2)) return;

        var obstacle = _point1.AddComponent<DynamicObstacle>();
        obstacle.SetPoints(_point1, _point2, barrierObstaclePref);
        obstacle.Initialize();

        var mainNode = _point1.GetComponent<Board_Node>();
        mainNode.RemoveLink(_point2.GetComponent<Board_Node>());
        if (contextMenuPref != null)
        {
            if (_point1.transform.Find("ContexMenu_barbedWire(Clone)")) return;
            Instantiate(contextMenuPref, _point1.transform);
        }
    }

    private bool CheckLink(GameObject _point1, GameObject _point2)
    {
        var node1 = _point1.GetComponent<Board_Node>();
        var node2 = _point2.GetComponent<Board_Node>();

        if (node1.LinkedNodes.Contains(node2)) return true;
        if (node2.LinkedNodes.Contains(node1)) return true;
        Debug.Log("Nodes haven't link");
        return false;
    }
}
