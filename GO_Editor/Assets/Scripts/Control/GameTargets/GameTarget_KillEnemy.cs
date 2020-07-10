using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTarget_KillEnemy : GameTarget
{
    private EnemyManager target;

    public override void Initialize()
    {
        target = GetComponent<GameManager>()?.Enemies.Find(e => e.IsMissionTarget);
    }

    public override bool TargetComplete()
    {
        if (target == null) return false;
        return target.IsDead;
    }
}
