using System.Collections;
using UnityEngine;

public class EnemyMover_Sniper : EnemyMover
{
    [SerializeField] private float standTime = 1f;

    public override void MoveOneTurn()
    {
        if (state != EnemyState.Defaul)
            ToDefaultState();

        Stand();
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

    public override void ToAlarmState(Board_Node node)
    {
        if (node.transform.position.x == transform.position.x ||
            node.transform.position.z == transform.position.z)
        {
            state = EnemyState.Alarm;

            var relativePosition = node.transform.position - transform.position;
            var newRotation = Quaternion.LookRotation(relativePosition, Vector3.up);

            var newY = newRotation.eulerAngles.y;

            iTween.RotateTo(gameObject, iTween.Hash(
               "y", newY,
               "delay", 0f,
               "easetype", easeType,
               "time", rotateTime));
        }
    }
}
