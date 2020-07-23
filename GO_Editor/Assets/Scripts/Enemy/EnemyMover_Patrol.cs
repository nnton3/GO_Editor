using UnityEngine;
using System.Collections;

public class EnemyMover_Patrol : EnemieMover
{
    public override void MoveOneTurn()
    {
        if (state == EnemyState.Defaul)
            Patrol();
        else MoveToTarget();
    }

    private void Patrol()
    {
        StartCoroutine(PatrolRoutine());
    }

    private IEnumerator PatrolRoutine()
    {
        var startPos = new Vector3(currentNode.Coordinate.x, 0f, currentNode.Coordinate.y);
        var newDest = startPos + transform.TransformVector(directionToMove);

        var startNode = board.FindNodeAt(startPos);
        var nextNode = board.FindNodeAt(newDest);
        if (nextNode == null || !startNode.LinkedNodes.Find(n => n.Coordinate == nextNode.Coordinate))
        {
            newDest = startPos - transform.TransformVector(directionToMove);
            destination = newDest;
            FaceDestination();

            yield return new WaitForSeconds(rotateTime + 0.1f);

            sensor.UpdateSensor();
        }

        if (sensor.FoundPlayer)
            yield return StartCoroutine(GetComponent<EnemyManager>().Kill());
        else
        {
            Move(newDest, 0f);

            while (isMoving)
                yield return null;

            base.FinishMovementEvent?.Invoke();
        }
    }
}
