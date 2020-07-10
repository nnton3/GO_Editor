using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public enum NodeType { Default, Bush, Mine, Stone, Goal, Cutter, Key, Map, Lever, Opener}

public class Board_Node : MonoBehaviour
{
    #region Variables
    [SerializeField] private float scaleTime = 0.5f;
    [SerializeField] private float delay = 1f;
    [SerializeField] private GameObject linkPrefab;
    [SerializeField] private LayerMask obstacleLayer;
    private GameObject geometry;
    private iTween.EaseType easeType = iTween.EaseType.easeInExpo;
    private Board board;
    private bool isInitialized;

    private Vector2 coordinate;
    public Vector2 Coordinate => Utility.Vector2Round(coordinate);

    private List<Board_Node> linkedNodes = new List<Board_Node>();
    public List<Board_Node> LinkedNodes => linkedNodes;

    private Dictionary<Board_Node, GameObject> links = new Dictionary<Board_Node, GameObject>();
    public Dictionary<Board_Node, GameObject> Links { get => links; set => links = value; }

    private List<Board_Node> neighborNodes = new List<Board_Node>();
    public List<Board_Node> NeighborNodes => neighborNodes;

    [SerializeField] private NodeType type = NodeType.Default;
    public NodeType Type { get => type; set => type = value; }
    #endregion

    public void Initialize()
    {
        geometry = gameObject;
        board = FindObjectOfType<Board>();
        coordinate = new Vector2(transform.position.x, transform.position.z);

        if (geometry != null)
            geometry.transform.localScale = Vector3.zero;
    }

    public void ShowGeometry()
    {
        if (geometry != null)
        {
            iTween.ScaleTo(geometry, iTween.Hash(
                "time", scaleTime,
                "scale", Vector3.one,
                "easetype", easeType,
                "delay", delay));
        }
    }

    public void FindNeighbors(List<Board_Node> nodes)
    {
        List<Board_Node> nList = new List<Board_Node>();

        foreach (var dir in Board.directions)
        {
            var foundNeighbor = nodes.Find(n => n.Coordinate == coordinate + dir);

            if (foundNeighbor != null && !nList.Contains(foundNeighbor))
                nList.Add(foundNeighbor);
        }
        neighborNodes = nList;
    }

    public void InitNode()
    {
        if (!isInitialized)
        {
            ShowGeometry();
            InitNeighbors();
            isInitialized = true;
        }
    }

    private void InitNeighbors()
    {
        StartCoroutine(InitNeighborsRoutine());
    }

    private IEnumerator InitNeighborsRoutine()
    {
        yield return new WaitForSeconds(delay);

        foreach (var n in neighborNodes)
        {
            if (!linkedNodes.Contains(n))
            {
                var obstacle = FindObstacle(n);
                if (obstacle == null || obstacle.CompareTag("Barrier"))
                {
                    LinkNode(n);
                    n.InitNode();
                }
            }
        }
    }

    public void LinkNode(Board_Node target)
    {
        if (linkPrefab == null) return;
        if (links.ContainsKey(target) || target.Links.ContainsKey(target))
        {
            Debug.LogWarning("These nodes is linked");
            return;
        }

        if (!target.Links.ContainsKey(this) && !links.ContainsKey(target))
        {
            var linkInstance = Instantiate(linkPrefab, transform.position, Quaternion.identity);
            linkInstance.transform.parent = transform;
            links.Add(target, linkInstance);

            var link = linkInstance.GetComponent<BoardLink>();
            if (link == null) return;
            link.DrawLink(transform.position, target.transform.position);
        }

        if (FindObstacle(target) != null)
            if(FindObstacle(target).CompareTag("Barrier")) return;

        AddLink(target);
    }

    public void RemoveLink(Board_Node target)
    {
        if (linkedNodes.Contains(target))
            linkedNodes.Remove(target);

        if (target.LinkedNodes.Contains(this))
            target.linkedNodes.Remove(this);
    }

    public void AddLink(Board_Node target)
    {
        if (!linkedNodes.Contains(target))
            linkedNodes.Add(target);

        if (!target.LinkedNodes.Contains(this))
            target.linkedNodes.Add(this);
    }

    public Obstacle FindObstacle(Board_Node targetNode)
    {
        var checkDirection = targetNode.transform.position - transform.position;
        RaycastHit raycastHit;

        if (Physics.Raycast(transform.position, checkDirection, 
            out raycastHit, Board.spacing + 0.1f, obstacleLayer))
        {
            return raycastHit.collider.GetComponent<Obstacle>();
        }
        return null;
    }

    // EDITOR FUNCTIONS
    public void DeleteLink(Board_Node target)
    {
        var link = links[target];
        if (link != null)
        {
            RemoveLink(target);
            links.Remove(target);
            Destroy(link.gameObject);
        }
    }
}
