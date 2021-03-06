﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EnemyState { Defaul, Alarm, ReturnToStart }

public class EnemyMover : Mover
{
    #region Variables
    protected Vector3 directionToMove = new Vector3(0f, 0f, Board.spacing);
    protected EnemyState state = EnemyState.Defaul;

    private Board_Node startNode;
    public Board_Node StartNode => startNode;

    private Quaternion startRotation;
    public Quaternion StartRotation => startRotation;

    protected WPManager wpmanager;
    protected EnemySensor sensor;
    protected List<GameObject> pathToTarget = new List<GameObject>();
    protected int currentPathIndex;
    protected bool playerLost;
    #endregion

    public override void Initialize()
    {
        base.Initialize();
        startNode = board.FindNodeAt(transform.position);
        if (startNode == null)
            Debug.LogWarning($"{gameObject.name} start node is lost");

        startRotation = transform.rotation;
        wpmanager = FindObjectOfType<WPManager>();
        sensor = GetComponent<EnemySensor>();
        GetComponent<EnemyManager>().DeathEvent.AddListener(() => currentNode = null);
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
        pathToTarget.Clear();
        pathToTarget = wpmanager.GetPath(currentNode.gameObject, node.gameObject);

        if (PathIsValid())
            PrepareToMove();
    }

    protected virtual void ReturnToStartState()
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
        if (!HaveLink(pathToTarget[currentPathIndex].transform.position))
            LostPlayer();
        else
        {
            var startPos = new Vector3(currentNode.Coordinate.x, 0f, currentNode.Coordinate.y);
            var newDest = pathToTarget[currentPathIndex].transform.position;

            destination = newDest;
            FaceDestination();

            yield return new WaitForSeconds(rotateTime + 0.1f);

            sensor.UpdateSensor();

            if (sensor.FoundPlayer)
                yield return StartCoroutine(GetComponent<EnemyManager>().Kill());
            else
            {
                Move(newDest, 0f);

                while (isMoving)
                    yield return null;

                IncrementPathIndex();

                base.FinishMovementEvent.Invoke();
            }
        }
    }

    protected void IncrementPathIndex()
    {
        if (currentNode.Type == NodeType.Lever) return;

        currentPathIndex++;
        if (currentPathIndex >= pathToTarget.Count)
        {
            if (state == EnemyState.Alarm)
                ReturnToStartState();
            else if (state == EnemyState.ReturnToStart)
                ToDefaultState();
        }
    }

    protected void LostPlayer()
    {
        ReturnToStartState();
        playerLost = true;
        MoveToTarget();
    }

    public void UpdatePathToTarget()
    {
        if (state == EnemyState.Defaul) return;
        if (pathToTarget.Count == 0) return;

        var targetNode = board.FindNodeAt(pathToTarget[pathToTarget.Count - 1].transform.position);
        SetPath(targetNode);

        if (pathToTarget.Count == 0)
            ReturnToStartState();
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

    protected bool HaveLink(Vector3 nodePos)
    {
        var nextNode = board.FindNodeAt(nodePos);
        if (nextNode == null)
        {
            Debug.LogWarning("Node is not valid");
            return false;
        }
        return currentNode.LinkedNodes.Find(n => n == nextNode);
    }
}
