using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Test : MonoBehaviour
{
    public UnityEvent CheckEvent;

    private void Start()
    {
        //CheckEvent.AddListener(() => Debug.Log("hello"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            CheckEvent.Invoke();
        }
    }
}
