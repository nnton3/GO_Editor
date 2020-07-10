using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private PlayerManager player;

    public void Initialize()
    {
        player = FindObjectOfType<PlayerManager>();   
    }

    public void Attack()
    {
        if (player == null) return;
        player.Die();
    }
}
