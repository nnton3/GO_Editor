using UnityEngine;
using System.Collections;

public class EnemyMover_Sentry : EnemieMover
{
    [SerializeField] private float standTime = 1f;

    public override void MoveOneTurn()
    {
        if (state == EnemyState.Defaul)
            Stand();
        else MoveToTarget();
    }

    private void Stand()
    {
        StartCoroutine(StandRoutine());
    }

    private IEnumerator StandRoutine()
    {
        yield return new WaitForSeconds(standTime);
        base.FinishMovementEvent.Invoke();
    }
}
