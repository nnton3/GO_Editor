using UnityEngine;
using System.Collections;

public class TestPathList : MonoBehaviour
{
    [SerializeField] private GameObject targetPos;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            var target = FindObjectOfType<Board>().AllNodes[Random.Range(0, FindObjectOfType<Board>().AllNodes.Count - 1)];
            Debug.Log($"target = {target.name}");
            var wpmanager = FindObjectOfType<WPManager>();
            //wpmanager.GetPath(FindObjectOfType<Board>().PlayerNode.gameObject, target.gameObject);
        }
    }
}
