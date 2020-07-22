using System.Collections;
using UnityEngine;

public class EnemyManager_Gunfire : EnemyManager
{
    public override IEnumerator Kill()
    {
        gameManager.LoseLevel();
        yield return new WaitForSeconds(0.5f);
        enemyAttack.Attack();
    }
}
