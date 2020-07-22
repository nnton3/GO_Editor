using System.Collections;
using UnityEngine;

public class SpotlightManager : EnemyManager
{
    [SerializeField] float alarmDelay = 2f;

    public override IEnumerator Kill()
    {
        Debug.Log("raise alarm");
        yield return new WaitForSeconds(alarmDelay);
        GameManager.RaiseAlarmEvent?.Invoke(enemyMover.CurrentNode);
        enemyMover.MoveOneTurn();
    }

    public override void Die() { return; }
}
