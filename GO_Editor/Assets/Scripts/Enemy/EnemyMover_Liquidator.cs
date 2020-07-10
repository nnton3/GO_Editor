using System.Collections;
using UnityEngine;

public class EnemyMover_Liquidator : EnemieMover
{
    private bool showUnit = false;

    public override void Initialize()
    {
        base.Initialize();
        FindPathToPlayer();

        state = EnemyState.Defaul;
    }

    public override void MoveOneTurn()
    {
        MoveToTarget();
    }

    private void FindPathToPlayer()
    {
        pathToTarget = wpmanager.GetPath(currentNode.gameObject, board.PlayerNode.gameObject);

        currentPathIndex = 1;

        destination = pathToTarget[currentPathIndex].transform.position;
        FaceDestination();
    }

    protected override IEnumerator MoveToTargetRoutine()
    {
        if (state != EnemyState.Alarm)
        {
            yield return new WaitForSeconds(2f);
            FinishMovementEvent.Invoke();
            yield break;
        }

        var startPos = new Vector3(currentNode.Coordinate.x, 0f, currentNode.Coordinate.y);
        var newDest = pathToTarget[currentPathIndex].transform.position;
        
        Move(newDest, 0f);

        while (isMoving)
            yield return null;

        currentPathIndex++;

        if (currentPathIndex >= pathToTarget.Count)
            FindPathToPlayer();

        FinishMovementEvent.Invoke();
    }

    public override void ToAlarmState(Board_Node node)
    {
        if (node == currentNode) return;
        SetPath(node);

        if (PathIsValid())
        {
            state = EnemyState.Alarm;
            playerLost = false;

            if(!showUnit)
                foreach (var render in GetComponentsInChildren<Renderer>())
                    render.enabled = true;
        }
    }
}
