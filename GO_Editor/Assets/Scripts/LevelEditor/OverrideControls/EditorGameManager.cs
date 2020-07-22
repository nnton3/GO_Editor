using UnityEngine;
using System.Collections;

public class EditorGameManager : GameManager
{
    protected override IEnumerator PlayerLevelRoutine()
    {
        isGamePlaying = true;
        player.PlayerInput.InputEnabled = true;
        //board.DrawGoal();
        //board.InitBoard();
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
