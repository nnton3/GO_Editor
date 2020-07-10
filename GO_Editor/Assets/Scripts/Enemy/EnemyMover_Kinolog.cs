using System.Collections;
using UnityEngine;

public class EnemyMover_Kinolog : EnemyMover_Patrol
{
    private void AddPlayerStep()
    {
        if (playerLost) return;
        if (board.PlayerNode.gameObject == pathToTarget[pathToTarget.Count - 1]) return;
        if (board.PlayerNode.Type == NodeType.Bush)
        {
            playerLost = true;
            return;
        }
        pathToTarget.Add(board.PlayerNode.gameObject);
    }

    protected override IEnumerator MoveToTargetRoutine()
    {
        if (!NodeIsValid())
        {
            ToReturnToStartState();
            playerLost = true;
            MoveToTarget();
        }
        else
        {
            AddPlayerStep();
            yield return base.MoveToTargetRoutine();
        }
    }
}
