using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    private float h;
    public float H => h;

    private float v;
    public float V => v;

    public bool InputEnabled { get; set; } = false;

    public void GetKeyInput()
    {
        if (InputEnabled)
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        }
        else
        {
            h = 0f;
            v = 0f;
        }
    }
}
