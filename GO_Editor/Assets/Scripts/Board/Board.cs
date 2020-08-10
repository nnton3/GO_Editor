using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    #region Variables
    public static float spacing = 4f;

    public static Vector2[] directions =
    {
        new Vector2(spacing, 0f),
        new Vector2(-spacing, 0f),
        new Vector2(0f, spacing),
        new Vector2(0f, -spacing)
    };

    private List<Board_Node> allNodes = new List<Board_Node>();
    public List<Board_Node> AllNodes => allNodes;

    private Board_Node playerNode;
    public Board_Node PlayerNode => playerNode;

    private Board_Node goalNode;
    public Board_Node GoalNode => goalNode;

    [SerializeField] private GameObject goalPrefab;
    private float drawGoalTime = 2f;
    private float drawGoalDelay = 6f;
    private iTween.EaseType drawGoalEaseType = iTween.EaseType.easeOutExpo;

    private PlayerMover player;
    private PlayerInventory playerInventory;

    [SerializeField] private List<Transform> capturePositions;
    public List<Transform> CapturePositions => capturePositions;
    private int currentCapturePosition = 0;
    public int CurrentCapturePosition { get => currentCapturePosition; set => currentCapturePosition = value; }

    [SerializeField] private float capturePositionIconSize = 0.4f;
    [SerializeField] private Color capturePositionIconColor = Color.blue;
    #endregion

    public void Initialize()
    {
        GetNodeList();
        //GetGoalNode();
        player = FindObjectOfType<PlayerMover>();
        playerInventory = player.GetComponent<PlayerInventory>();
        Debug.Log("Board initialized");
    }

    public void GetNodeList()
    {
        Board_Node[] nList = FindObjectsOfType<Board_Node>();
        allNodes = new List<Board_Node>(nList);
    }

    public Board_Node FindNodeAt(Vector3 pos)
    {
        Vector2 boardCoord = Utility.Vector2Round(new Vector2(pos.x, pos.z));
        return allNodes.Find(n => n.Coordinate == boardCoord);
    }

    public void GetGoalNode()
    {
        goalNode = allNodes.Find(n => n.Type == NodeType.Goal);
    }

    public Board_Node FindPlayerNode()
    {
        if (player == null || player.IsMoving) return null;
        return FindNodeAt(player.transform.position);
    }

    public List<EnemyManager> FindEnemiesAt(Board_Node node)
    {
        var foundEnemies = new List<EnemyManager>();
        var enemies = FindObjectsOfType<EnemyManager>() as EnemyManager[];

        foreach (var enemy in enemies)
        {
            var mover = enemy.GetComponent<EnemyMover>();
            if (mover.CurrentNode == node)
                foundEnemies.Add(enemy);
        }
        return foundEnemies;
    }

    public void UpdatePlayerNode()
    {
        playerNode = FindPlayerNode();
        var objectPlacement = FindObjectOfType<ObjectPlacement>();

        switch (playerNode.Type)
        {
            case NodeType.Mine:
                GameManager.LoseLevelEvent?.Invoke();
                player.GetComponent<PlayerManager>().Die();
                break;
            case NodeType.Cutter:
                ClearNode();
                playerInventory.HaveCutter = true;
                break;
            case NodeType.Key:
                playerInventory.Keys.Add(playerNode.GetComponent<KeyIndex>().Index);
                ClearNode();
                break;
            case NodeType.Map:
                break;
            default:
                break;
        }

        void ClearNode()
        {
            if (objectPlacement != null) objectPlacement.DeleteObject(playerNode);
            else playerNode.Type = NodeType.Default;
        }
    }

    public void DrawGoal()
    {
        if (goalPrefab != null && goalNode != null)
        {
            var goalInstance = Instantiate(goalPrefab, goalNode.transform.position, Quaternion.identity);

            iTween.ScaleFrom(goalInstance, iTween.Hash(
                "scale", Vector3.zero,
                "time", drawGoalTime,
                "delay", drawGoalDelay,
                "easetype", drawGoalEaseType));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
        if (playerNode != null)
            Gizmos.DrawSphere(playerNode.transform.position, 2f);

        Gizmos.color = capturePositionIconColor;
        foreach (var capturePos in capturePositions)
            Gizmos.DrawCube(capturePos.position, Vector3.one * capturePositionIconSize);
    }

    public void InitBoard()
    {
        if (PlayerNode == null) return;
        PlayerNode.InitNode();
    }
}
