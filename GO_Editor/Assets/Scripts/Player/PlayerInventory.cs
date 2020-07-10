using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] bool haveCutter = false;
    public bool HaveCutter { get => haveCutter; set => haveCutter = value; }

    [SerializeField] List<int> keys = new List<int>();
    public List<int> Keys { get => keys; set => keys = value; }

    public void Reset()
    {
        haveCutter = false;
        keys.Clear();
    }
}
