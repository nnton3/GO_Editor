using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NodePlaceController : MonoBehaviour
{
    //private Transform mainPos;

    //private List<Transform> reservedPositions = new List<Transform>();
    //[SerializeField] private List<Transform> freePositions = new List<Transform>();

    //private List<GameObject> enemiesOnPoint = new List<GameObject>();

    //private void Awake()
    //{
    //    foreach (var item in GetComponentsInChildren<Transform>())
    //    {
    //        if (item != transform)
    //            freePositions.Add(item);
    //    }
    //}

    //public Transform GetFreePosition()
    //{
    //    if (reservedPositions.Contains(mainPos))
    //    {
    //        var position = freePositions[0];
    //        freePositions.Remove(position);
    //        reservedPositions.Add(position);
    //        return position;
    //    }
    //    else return mainPos;
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Unit"))
    //    {
    //        if (enemiesOnPoint.Count == 0)
    //        {
    //            Debug.Log("первый посетитель");
    //            enemiesOnPoint.Add(other.gameObject);
    //        }
    //        else if (enemiesOnPoint.Count == 1)
    //        {
    //            Debug.Log("второй посетитель");
    //            enemiesOnPoint.Add(other.gameObject);
    //            SetTargetPosForAll();
    //        }
    //        else
    //        {
    //            Debug.Log("нас много");
    //            enemiesOnPoint.Add(other.gameObject);
    //            SetTargetPos();
    //        }
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Unit"))
    //    {
    //        enemiesOnPoint.Remove(other.gameObject);
    //        reservedPositions.Remove(other.GetComponent<MyFollowPath>().correctPointPos);
    //    }
    //}

    //private void SetTargetPosForAll()
    //{
    //    var poses = new List<Transform>();
    //    for (int i = 0; i < freePositions.Count; i++)
    //    {
    //        var pos = freePositions[i];
    //        enemiesOnPoint[i].GetComponent<MyFollowPath>().CorrectPosition(pos);
    //        poses.Add(pos);
    //    }
    //    foreach (var item in poses)
    //    {
    //        freePositions.Remove(item);
    //        reservedPositions.Add(item);
    //    }

    //}

    //private void SetTargetPos()
    //{

    //}
}
