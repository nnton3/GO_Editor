using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> menus;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(HideAllMenus());
    }

    private IEnumerator HideAllMenus()
    {
        yield return new WaitForSeconds(.1f);
        foreach (var menu in menus)
        {
            menu.SetActive(false);
        }
    }
}
