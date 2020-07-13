﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private Animator anim;
    public Animator Anim => anim;

    public void Die()
    {
        if (anim == null) return;
        Debug.Log("Is dead");
        anim.ResetTrigger("reset");
        anim.SetTrigger("isDead");
    }
}