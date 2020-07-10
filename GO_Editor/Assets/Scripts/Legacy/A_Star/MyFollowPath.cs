using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFollowPath : MonoBehaviour
{
    //private Transform goal;
    //[SerializeField] private float speed = 5.0f;
    //[SerializeField] private float accuracy = 1.0f;
    //[SerializeField] private GameObject wpManager;
    //private GameObject[] wps;
    //[SerializeField] private GameObject currentNode;
    //public GameObject CurrentNode => currentNode;
    //private int currentWP = 0;
    //private Graph g;
    //private bool startMove;
    //private bool needCorrect;
    //public Transform correctPointPos { get; set; }

    //void Start()
    //{
    //    wps = wpManager.GetComponent<WPManager>().waypoints;
    //    g = wpManager.GetComponent<WPManager>().graph;
    //}

    //public void UpdatePath(GameObject target)
    //{
    //    g.AStar(currentNode, target);
    //    currentWP = 0;
    //    if (currentNode == g.getPathPoint(currentWP)) currentWP++;
    //}

    //public void GoToNextWP()
    //{
    //    if (g.getPathLength() == 0 || currentWP == g.getPathLength()) return;

    //    currentNode = g.getPathPoint(currentWP);
    //    RotateToTarget();
    //    startMove = true;
    //}

    //public void CorrectPosition(Transform correctPos)
    //{
    //    correctPointPos = correctPos;
    //    needCorrect = true;
    //}

    //void LateUpdate()
    //{
    //    if (startMove)
    //    {
    //        MoveToPoint();
    //    }
    //    else if (needCorrect)
    //    {
    //        Move(correctPointPos.position);
    //    }
    //}

    //private void MoveToPoint()
    //{
    //    Move(g.getPathPoint(currentWP).transform.position);

    //    if (transform.position == g.getPathPoint(currentWP).transform.position)
    //    {
    //        startMove = false;
    //        currentWP++;
    //    }
    //}

    //private void Move(Vector3 target)
    //{
    //    transform.position = Vector3.MoveTowards(transform.position,
    //                                            target,
    //                                            speed * Time.deltaTime);

    //    if (transform.position == target) needCorrect = false;
    //}

    //private void RotateToTarget()
    //{
    //    goal = g.getPathPoint(currentWP).transform;

    //    Vector3 lookAtGoal = new Vector3(goal.position.x,
    //                                    this.transform.position.y,
    //                                    goal.position.z);
    //    Vector3 direction = lookAtGoal - this.transform.position;

    //    transform.Rotate(0f,
    //                Vector3.SignedAngle(transform.forward, direction, Vector3.up),
    //                0f);
    //}
}
