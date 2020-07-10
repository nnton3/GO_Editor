using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EnemyState { Defaul, Alarm, ReturnToStart }

public class EnemieMover : Mover
{
    #region Variables
    protected Vector3 directionToMove = new Vector3(0f, 0f, Board.spacing);
    protected EnemyState state = EnemyState.Defaul;

    private Board_Node startNode;
    public Board_Node StartNode => startNode;

    private Quaternion startRotation;
    public Quaternion StartRotation => startRotation;

    protected WPManager wpmanager;
    protected List<GameObject> pathToTarget = new List<GameObject>();
    protected int currentPathIndex;
    protected bool playerLost;

    #endregion

    public override void Initialize()
    {
        base.Initialize();
        startNode = board.FindNodeAt(transform.position);
        if (startNode == null)
            Debug.Log($"{gameObject.name} start node is lost");

        startRotation = transform.rotation;
        wpmanager = FindObjectOfType<WPManager>();

    }

    public virtual void MoveOneTurn() { }

    public virtual void ToAlarmState(Board_Node node)
    {
        if (node == currentNode) return;
        SetPath(node);

        if (PathIsValid())
        {
            state = EnemyState.Alarm;
            playerLost = false;
        }
    }

    protected void SetPath(Board_Node node)
    {
        pathToTarget = wpmanager.GetPath(currentNode.gameObject, node.gameObject);
        Debug.Log($"Is Enemy have valid path? {PathIsValid()}");
        if (PathIsValid())
            PrepareToMove();
    }

    protected void ToReturnToStartState()
    {
        if (startNode == currentNode) return;
        SetPath(startNode);

        if (PathIsValid())
            state = EnemyState.ReturnToStart;
    }

    protected virtual void ToDefaultState()
    {
        state = EnemyState.Defaul;

        var newY = startRotation.eulerAngles.y;

        iTween.RotateTo(gameObject, iTween.Hash(
           "y", newY,
           "delay", 0f,
           "easetype", easeType,
           "time", rotateTime));
    }

    protected void MoveToTarget()
    {
        StartCoroutine(MoveToTargetRoutine());
    }

    protected virtual IEnumerator MoveToTargetRoutine()
    {
        if (!NodeIsValid())
        {
            ToReturnToStartState();
            playerLost = true;
            MoveToTarget();
        }
        else
        {
            var startPos = new Vector3(currentNode.Coordinate.x, 0f, currentNode.Coordinate.y);
            var newDest = pathToTarget[currentPathIndex].transform.position;

            Move(newDest, 0f);

            while (isMoving)
                yield return null;

            currentPathIndex++;

            if (currentPathIndex >= pathToTarget.Count)
            {
                if (state == EnemyState.Alarm)
                    ToReturnToStartState();
                else if (state == EnemyState.ReturnToStart)
                    ToDefaultState();
                FinishMovementEvent.Invoke();
            }
            else
            {
                destination = pathToTarget[currentPathIndex].transform.position;
                FaceDestination();

                FinishMovementEvent.Invoke();
            }
        }
    }

    public void UpdatePathToTarget()
    {
        if (state == EnemyState.Defaul) return;
        if (pathToTarget.Count == 0) return;

        var targetNode = board.FindNodeAt(pathToTarget[pathToTarget.Count - 1].transform.position);
        SetPath(targetNode);

        if (pathToTarget.Count == 0)
            ToReturnToStartState();
    }

    private void PrepareToMove()
    {
        currentPathIndex = 1;
        destination = pathToTarget[currentPathIndex].transform.position;
        FaceDestination();
    }

    protected bool PathIsValid()
    {
        if (pathToTarget.Count == 0) return false;
        return board.FindNodeAt(pathToTarget[1].transform.position).LinkedNodes.Contains(currentNode);
    }

    protected bool NodeIsValid()
    {
        var nextNode = board.FindNodeAt(pathToTarget[currentPathIndex].transform.position);
        if (nextNode == null)
        {
            Debug.Log("Node is not valid");
            return false;
        }
        var link = currentNode.LinkedNodes.Find(n => n == nextNode);
        if (link == null) return false;
        else return true;
    }
}
