using System;
using System.Collections;
using System.Collections.Generic;
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
    private bool isInstancePlayer;
    private bool point1select;
    private bool point2select;
    private bool point3select;
    private GameObject point1;
    private GameObject point2;
    private GameObject point3;
    public static UnityEvent StartAddObjEvent = new UnityEvent();
    public static UnityEvent EndAddObjEvent = new UnityEvent();
    #endregion

    private void Awake()
    {
        raycaster = GetComponent<EditorRaycaster>();
        initializer = FindObjectOfType<EditorInitializer>();
        if (initializer == null)
        {
            Debug.Log("Initializer is lost");
            Destroy(this);
        }

        StartAddObjEvent.AddListener(() =>
        {
            StopAllCoroutines();
            ResetFalgs();
        });
    }

    private void ResetFalgs()
    {
        point1select = false;
        point2select = false;
        point3select = false;
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
            Debug.Log("Map pref is lost");
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
            Debug.Log("Board pref is lost");
            return;
        }
        Instantiate(boardPref);
        Debug.Log("Board is intance");
    }

    private void InstanceNodes()
    {
        if (nodePref == null)
        {
            Debug.Log("Node pref is lost");
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
        if (playerPref == null)
        {
            Debug.Log("Player pref is lost");
            return;
        }
        if (isInstancePlayer)
        {
            Debug.Log("You have one player!");
            return;
        }
        StartAddObjEvent?.Invoke();
        StartCoroutine(InstancePlayerRoutine());
    }

    public void InstancePlayer(Vector3 pos)
    {
        var playerIntance = Instantiate(playerPref, pos, Quaternion.identity);
        Debug.Log("Player is intance");
    }

    private IEnumerator InstancePlayerRoutine()
    {
        Debug.Log("Click on point to instance");

        while (!isInstancePlayer)
        {
            if (raycaster.CheckRaycast(1024, "Node", out point1))
                isInstancePlayer = true;

            yield return null;
        }

        var playerIntance = Instantiate(playerPref, point1.transform.position, Quaternion.identity);
        Debug.Log("Player is intance");
        EndAddObjEvent?.Invoke();
        ResetFalgs();
    }

    public void AddLink()
    {
        StartAddObjEvent?.Invoke();
        StartCoroutine(AddLinkRoutine());
    }

    private IEnumerator AddLinkRoutine()
    {
        yield return SelectTwoPoint();

        CreateLink(point1, point2);
        EndAddObjEvent?.Invoke();
        ResetFalgs();
    }

    public void CreateLink(GameObject point1, GameObject point2)
    {
        var point1Pos = point1.transform.position;
        var point2Pos = point2.transform.position;

        var onAxisX = point1Pos.x == point2Pos.x;
        var onAxisZ = point1Pos.z == point2Pos.z;


        if (!onAxisX && !onAxisZ)
        {
            Debug.Log("Selected points is not valid");
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
        {
            if (pos1.x < pos2.x)
                direction = Vector3.right;
            else
                direction = Vector3.left;
        }
        else
        {
            if (pos1.z < pos2.z)
                direction = Vector3.forward;
            else
                direction = Vector3.back;
        }

        if (direction == Vector3.zero) return;

        var nodeNumber = (int)Vector3.Distance(pos1, pos2) / Board.spacing;
        var currentNode = point1.GetComponent<Board_Node>();

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
        StartCoroutine(DeleteLinkRoutine());
    }

    private IEnumerator DeleteLinkRoutine()
    {
        yield return SelectTwoPoint();

        DestroyLink(point1, point2);
        EndAddObjEvent?.Invoke();
        ResetFalgs();
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

    private IEnumerator SelectTwoPoint()
    {
        Debug.Log("Set point one");
        while (!point1select)
        {
            if (raycaster.CheckRaycast(1024, "Node", out point1))
                point1select = true;

            yield return null;
        }

        Debug.Log("Set point two");
        while (!point2select)
        {
            if (raycaster.CheckRaycast(1024, "Node", out point2))
            {
                if (point1 == point2)
                {
                    Debug.Log("Choose valid point");
                    yield break;
                }
                point2select = true;
            }
            yield return null;
        }
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
