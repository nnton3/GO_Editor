using UnityEngine;
using System.Collections;
using UniRx;

public class EnemyMover_Spinner : EnemieMover
{
    public override void MoveOneTurn()
    {
        if (state == EnemyState.Defaul)
            Spin();
        else MoveToTarget();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("invoke");
            base.FinishMovementEvent.Invoke();
        }
    }

    private void Spin()
    {
        Observable
            .FromCoroutine(() => SpinRoutine())
            .Subscribe(_ => base.FinishMovementEvent.Invoke());
    }

    private IEnumerator SpinRoutine()
    {
        var localForward = new Vector3(0f, 0f, Board.spacing);
        destination = transform.TransformVector(localForward * -1f) + transform.position;

        FaceDestination();

        yield return new WaitForSeconds(rotateTime);
    }
}
