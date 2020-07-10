using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ContextMenu_Key : ContextMenu_Obj
{
    [SerializeField] private Text inputField;

    public void SetIndex()
    {
        int index = 0;
        if (int.TryParse(inputField.text, out index))
            transform.parent.GetComponent<KeyIndex>().Index = index;
        else Debug.LogWarning("Input is not valid");
    }
}
