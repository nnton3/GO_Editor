using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectSelector : MonoBehaviour
{
    private List<GameObject> nodes = new List<GameObject>();
    public List<GameObject> Nodes => nodes;
    private GameObject node1;
    public GameObject Node1 => node1;

    private bool selected;
    private EditorRaycaster raycaster;

    private void Awake()
    {
        raycaster = GetComponent<EditorRaycaster>();
    }

    public IEnumerator SelectNodeRoutine(string message)
    {
        Debug.Log(message);
        GameObject node = null;
        while (!selected)
        {
            if (raycaster.CheckRaycast(1024, "Node", out node))
                selected = true;

            yield return null;
        }
        nodes.Add(node);
        selected = false;
    }

    public void Reset()
    {
        if (nodes.Count != 0)
            nodes.Clear();
    }
}
