using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EditorGameManager : GameManager
{
    public override void Initialize()
    {
        base.Initialize();
        
        foreach(var enemy in Enemies)
        {
            enemy.DeathEvent.AddListener(CheckDeadEnemy);
        }
    }

    private void CheckDeadEnemy()
    {
        var deadEnemiesList = new List<EnemyManager>();
        foreach (var enemy in Enemies)
            if (enemy.IsDead) deadEnemiesList.Add(enemy);

        foreach (var enemy in deadEnemiesList)
            Enemies.Remove(enemy);
    }

    protected override IEnumerator PlayLevelRoutine()
    {
        isGamePlaying = true;
        yield return new WaitForSeconds(delay);
        player.PlayerInput.InputEnabled = true;
        PlayLevelEvent?.Invoke();

        while (!IsGameOver)
        {
            yield return null;
            if (gameTarget != null)
                isGameOver = gameTarget.TargetComplete();
        }
        Debug.Log("WIN!");
    }
}
