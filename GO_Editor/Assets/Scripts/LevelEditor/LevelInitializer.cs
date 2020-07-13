using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class LevelInitializer : MonoBehaviour
{
    #region Variables
    [SerializeField] private int sizeX;
    public int SizeX { get => sizeX; set => sizeX = value; }

    [SerializeField] private int sizeZ;
    public int SizeZ { get => sizeZ; set => sizeZ = value; }

    [SerializeField] private GameObject mapPref;
    [SerializeField] private GameObject boardPref;
    [SerializeField] private GameObject nodePref;
    [SerializeField] private GameObject playerPref;
    [SerializeField] private GameObject playerContextMenuPref;

    private EditorInitializer initializer;
    private EditorRaycaster raycaster;
    private ObjectSelector selector;
    private IDisposable routine;

    public static UnityEvent StartAddObjEvent = new UnityEvent();
    public static UnityEvent EndAddObjEvent = new UnityEvent();
    private bool isInstancePlayer;
    #endregion

    private void Awake()
    {
        raycaster = GetComponent<EditorRaycaster>();
        initializer = FindObjectOfType<EditorInitializer>();
        selector = GetComponent<ObjectSelector>();

        if (initializer == null)
        {
            Debug.Log("Initializer is lost");
            Destroy(this);
        }

        StartAddObjEvent.AddListener(() =>
        {
            if (routine != null)
                routine.Dispose();
        });
    }

    public void SetMapSize()
    {
        if (sizeX == 0 || sizeZ == 0)
        {
            Debug.Log("Set map size. Map size must not equale 0!");
            return;
        }

        InstanceMap();
        InstanceBoardComponent();
        InstanceNodes();
    }

    private void InstanceMap()
    {
        if (mapPref == null)
        {
            Debug.LogWarning("Map pref is lost");
            return;
        }
        var map = Instantiate(mapPref, new Vector3(-2f, 0f, -2f), Quaternion.identity);
        map.transform.localScale = new Vector3(
            Board.spacing * .1f * sizeX,
            1f,
            Board.spacing * .1f * sizeZ);
        Debug.Log("Map is intance");
    }

    private void InstanceBoardComponent()
    {
        if(boardPref == null)
        {
            Debug.LogWarning("Board pref is lost");
            return;
        }
        Instantiate(boardPref);
        Debug.Log("Board is intance");
    }

    private void InstanceNodes()
    {
        if (nodePref == null)
        {
            Debug.LogWarning("Node pref is lost");
            return;
        }

        var nodeParent = Instantiate(new GameObject("Nodes"));

        for (int i = 0; i < sizeX; i++)
        {
            var posX = Board.spacing * i;

            for (int j = 0; j < sizeZ; j++)
            {
                var posZ = Board.spacing * j;

                var node = Instantiate(
                    nodePref,
                    new Vector3(posX, 0, posZ),
                    Quaternion.identity,
                    nodeParent.transform);
                node.name = $"Node_{i}_{j}";
            }
        }
        Debug.Log("Nodes is intance");
    }

    public void InstancePlayer()
    {
        if (!CheckPlayer()) return;

        StartAddObjEvent?.Invoke();

        routine = Observable
            .FromCoroutine(() => selector.SelectNodeRoutine("Select node to place object"))
            .Subscribe(_ =>
            {
                Instantiate(playerPref, selector.Nodes[0].transform.position, Quaternion.identity);
                isInstancePlayer = true;
                selector.Reset();
                EndAddObjEvent?.Invoke();
            });
    }

    public void InstancePlayer(Vector3 pos)
    {
        if (!CheckPlayer()) return;

        Instantiate(playerPref, pos, Quaternion.identity);
        isInstancePlayer = true;
        Debug.Log("Player is intance");
    }

    private bool CheckPlayer()
    {
        if (playerPref == null)
        {
            Debug.LogWarning("Player pref is lost");
            return false;
        }
        if (isInstancePlayer)
        {
            Debug.LogWarning("You have one player!");
            return false;
        }
        return true;
    }

    public void AddLink()
    {
        StartAddObjEvent?.Invoke();


        routine = Observable
            .FromCoroutine(() => selector.SelectNodeRoutine("Select point 1"))
            .SelectMany(() => selector.SelectNodeRoutine("Select point 2"))
            .Subscribe(_ =>
            {
                CreateLink(selector.Nodes[0], selector.Nodes[1]);
                selector.Reset();
                EndAddObjEvent?.Invoke();
            });
    }

    public void CreateLink(GameObject point1, GameObject point2)
    {
        var point1Pos = point1.transform.position;
        var point2Pos = point2.transform.position;

        var onAxisX = point1Pos.x == point2Pos.x;
        var onAxisZ = point1Pos.z == point2Pos.z;


        if (!onAxisX && !onAxisZ)
        {
            Debug.LogWarning("Selected points is not valid");
            return;
        }
        if (Vector3.Distance(point1.transform.position, point2.transform.position) <= Board.spacing)
            point1.GetComponent<Board_Node>().LinkNode(point2.GetComponent<Board_Node>());
        else
            MultipleLink(point1Pos, point2Pos, onAxisZ);
    }

    private void MultipleLink(Vector3 pos1, Vector3 pos2, bool onAxisZ)
    {
        Vector3 direction = Vector3.zero;
        var board = FindObjectOfType<Board>();

        if (onAxisZ)
            direction = (pos1.x < pos2.x) ? Vector3.right : Vector3.left;
        else
            direction = (pos1.z < pos2.z) ? Vector3.forward : Vector3.back;

        if (direction == Vector3.zero) return;

        var nodeNumber = (int)Vector3.Distance(pos1, pos2) / Board.spacing;
        var currentNode = board.FindNodeAt(pos1);

        for (int i = 1; i < nodeNumber + 1; i++)
        {
            var targetNode = board.FindNodeAt(direction * i * Board.spacing + pos1);
            currentNode.LinkNode(targetNode);
            currentNode = targetNode;
        }
    }

    public void DeleteLink()
    {
        StartAddObjEvent?.Invoke();

        routine = Observable
            .FromCoroutine(() => selector.SelectNodeRoutine("Select point 1"))
            .SelectMany(() => selector.SelectNodeRoutine("Select point 2"))
            .Subscribe(_ =>
            {
                DestroyLink(selector.Nodes[0], selector.Nodes[1]);
                selector.Reset();
                EndAddObjEvent?.Invoke();
            });
    }

    private void DestroyLink(GameObject point1, GameObject point2)
    {
        var node1 = point1.GetComponent<Board_Node>();
        var node2 = point2.GetComponent<Board_Node>();

        if (node1.Links.ContainsKey(node2))
            node1.DeleteLink(node2);
        else if (node2.Links.ContainsKey(node1))
            node2.DeleteLink(node1);
        else
            Debug.Log("Nodes haven't link");
    }

    public void InitializeBoard()
    {
        if (!FindObjectOfType<PlayerManager>())
        {
            Debug.Log("You must place player");
            return;
        }
        if (!FindObjectOfType<Board>())
        {
            Debug.Log("You must place board");
            return;
        }
        initializer.InitializeBoard();
        initializer.InitializeNodes();
        initializer.InitializePlayer();
        GetComponent<ModeController>()?.Initialize();
        GetComponent<SavingSystem>()?.Initialize();

        foreach (var node in FindObjectOfType<Board>().AllNodes)
            node.ShowGeometry();
    }

    public void FastStart()
    {
        SetMapSize();
        InstancePlayer(Vector3.zero);
        InitializeBoard();
    }

    // PLAYER INPUT (HOT KEYS)
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) AddLink();
        if (Input.GetKeyDown(KeyCode.Alpha2)) DeleteLink();
    }
}
