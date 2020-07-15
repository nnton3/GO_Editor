 using System.Collections;
using UnityEngine;

public class SpotlightMover : EnemieMover
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

        CheckPlayer();
        
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

    private void CheckPlayer()
    {
        if (board == null) return;

        if (transform.position == board.PlayerNode.transform.position)
            GameManager.RaiseAlarmEvent?.Invoke(currentNode);
    }

    // EDITOR
    public void SetMovementParams(Vector3 _start, Vector3 _end)
    {
        startPoint = _start;
        endPoint = _end;
    }
}
