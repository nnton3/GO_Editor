using UnityEngine;
using System.Collections;

public class TestEvent : MonoBehaviour
{
    private void Start()
    {
        FindObjectOfType<Test>().CheckEvent.AddListener(() => Debug.Log("hello"));
    }
}
