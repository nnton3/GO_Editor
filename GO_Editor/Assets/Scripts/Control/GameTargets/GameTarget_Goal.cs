using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTarget_Goal : GameTarget
{
    private Board board;

    public override void Initialize()
    {
        board = FindObjectOfType<Board>();
        if (board != null) board.GetGoalNode();
    }

    public override bool TargetComplete()
    {
        if (board.PlayerNode == null) return false;
        return board.PlayerNode == board.GoalNode;
    }
}
