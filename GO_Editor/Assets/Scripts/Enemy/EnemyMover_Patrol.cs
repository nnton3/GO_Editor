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
        var nextDest = startPos + transform.TransformVector(directionToMove * 2);

        Move(newDest, 0f);

        while (isMoving)
            yield return null;

        if (board != null)
        {
            var newDestNode = board.FindNodeAt(newDest);
            var nextDestNode = board.FindNodeAt(nextDest);

            if (nextDestNode == null)
            {
                destination = startPos;
                FaceDestination();

                yield return new WaitForSeconds(rotateTime);
            }
            else if (newDestNode.LinkedNodes.Find(n => n.Coordinate == nextDestNode.Coordinate) == null)
            {
                destination = startPos;
                FaceDestination();

                yield return new WaitForSeconds(rotateTime);
            }
        }
        base.FinishMovementEvent.Invoke();
    }
}
