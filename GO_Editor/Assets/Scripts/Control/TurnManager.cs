using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    protected GameManager gameManager;

    private bool isTurnComplete = false;
    public bool IsTurnComplete { get => isTurnComplete; set => isTurnComplete = value; }

    public virtual void Initialize()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public virtual void FinishTurn()
    {
        //Debug.Log($"{gameObject.name} is complete turn");
        isTurnComplete = true;
        if (gameManager == null) return;
        gameManager.UpdateTurn();
    }
}
