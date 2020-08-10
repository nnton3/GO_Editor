 using System.Collections;
using UnityEngine;

public class SpotlightMover : EnemyMover
{
    [SerializeField] private Vector3 startPoint;
    public Vector3 StartPoint => startPoint;
    [SerializeField] private Vector3 endPoint;
    public Vector3 EndPoint => endPoint;

    public override void ToAlarmState(Board_Node node) { return; }

    public override void MoveOneTurn()
    {
        StartCoroutine(PatrolRoutine());
    }

    private IEnumerator PatrolRoutine()
    {
        var startPos = transform.position;
        var newDest = startPos + transform.TransformVector(directionToMove);

        Move(newDest, 0f);

        while (isMoving)
            yield return null;

        if (newDest == startPoint || newDest == endPoint)
        {
            destination = startPos;
            FaceDestination();

            yield return new WaitForSeconds(rotateTime);
        }

        sensor.UpdateSensor();
        if (sensor.FoundPlayer)
            GameManager.RaiseAlarmEvent?.Invoke(currentNode);
        
        base.FinishMovementEvent.Invoke();
    }

    public override void Move(Vector3 destinationPos, float delayTime = 0.025F)
    {
        if (board == null) return;
        var targetNode = board.FindNodeAt(destinationPos);
        if (targetNode == null) return;
        if (currentNode == null) return;
        StartCoroutine(MoveRoutine(destinationPos, delayTime));
    }

    // EDITOR
    public void SetMovementParams(Vector3 _start, Vector3 _end)
    {
        startPoint = _start;
        endPoint = _end;
    }
}
