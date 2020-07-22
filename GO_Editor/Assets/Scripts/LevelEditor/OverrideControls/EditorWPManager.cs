using UnityEngine;
using System.Collections;

public class EditorWPManager : WPManager
{
    public override void Initialize()
    {
        waypoints = FindObjectsOfType<Board_Node>();
        UpdateEditorGraph();
    }

    public void UpdateEditorGraph()
    {
        if (waypoints.Length > 0)
        {
            graph = new Graph();
            foreach (var wp in waypoints)
            {
                graph.AddNode(wp.gameObject);

                foreach (var link in wp.LinkedNodes)
                {
                    graph.AddEdge(wp.gameObject, link.gameObject);
                    graph.AddEdge(link.gameObject, wp.gameObject);
                }
            }
        }
    }

    void Update()
    {
        graph.debugDraw();
    }
}
