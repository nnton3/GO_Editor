using System.Collections.Generic;
using UnityEngine;

public class WPManager : MonoBehaviour
{
	protected Board_Node[] waypoints;
    protected Graph graph = new Graph();

    public virtual void Initialize()
    {
        waypoints = FindObjectsOfType<Board_Node>();
        UpdateGraph();
    }

    public void UpdateGraph()
    {
        if (waypoints.Length > 0)
        {
            graph = new Graph();
            foreach (var wp in waypoints)
            {
                graph.AddNode(wp.gameObject);

                foreach (var neighbor in wp.NeighborNodes)
                {
                    if (wp.FindObstacle(neighbor) == null)
                    {
                        graph.AddEdge(neighbor.gameObject, wp.gameObject);
                        graph.AddEdge(wp.gameObject, neighbor.gameObject);
                    }
                }
            }
        }
    }

    public List<GameObject> GetPath(GameObject start, GameObject end)
    {
        var pathList = new List<GameObject>();
        graph.AStar(start, end);

        foreach (var node in graph.PathList)
        {
            pathList.Add(node.id);
        }

        return pathList;
    }

    void Update()
    {
        graph.debugDraw();
    }
}
