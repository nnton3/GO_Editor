using System.Collections;
using UnityEngine;

public class SpotlightManager : EnemyManager
{
    [SerializeField] float alarmDelay = 2f;

    protected override IEnumerator Kill()
    {
        Debug.Log("raise alarm");
        yield return new WaitForSeconds(alarmDelay);
        GameManager.RaiseAlarmEvent?.Invoke(enemyMover.CurrentNode);
        enemyMover.MoveOneTurn();
    }

    public override void Die() { return; }
}
