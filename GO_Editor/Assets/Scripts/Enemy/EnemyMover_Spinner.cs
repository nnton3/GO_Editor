using UnityEngine;
using System.Collections;

public class EnemyMover_Spinner : EnemieMover
{
    public override void MoveOneTurn()
    {
        if (state == EnemyState.Defaul)
            Spin();
        else MoveToTarget();
    }

    private void Spin()
    {
        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        var localForward = new Vector3(0f, 0f, Board.spacing);
        destination = transform.TransformVector(localForward * -1f) + transform.position;

        FaceDestination();

        yield return new WaitForSeconds(rotateTime);

        base.FinishMovementEvent.Invoke();
    }
}
