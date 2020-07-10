using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameTarget_KillAllEnemies : GameTarget
{
    private List<EnemyManager> enemies = new List<EnemyManager>();

    public override void Initialize()
    {
        enemies.AddRange(GetComponent<GameManager>().Enemies);

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].GetComponent<SpotlightManager>()) enemies.Remove(enemies[i]);
        }
    }

    public override bool TargetComplete()
    {
        if (enemies.Count == 0) return false;

        foreach (var enemy in enemies)
            if (!enemy.IsDead) return false;

        return true;
    }
}
