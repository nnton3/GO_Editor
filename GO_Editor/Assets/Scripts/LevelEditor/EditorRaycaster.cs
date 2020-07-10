using UnityEngine;
using System.Collections;
using System;

public class EditorRaycaster : MonoBehaviour
{
    public bool CheckRaycast(int layer, string tag, out GameObject target)
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;

            if (Physics.Raycast(ray, out raycast, Mathf.Infinity, layer))
                if (raycast.transform.CompareTag(tag))
                {
                    target = raycast.transform.gameObject;
                    return true;
                }
        }
        target = null;
        return false;
    }
}
