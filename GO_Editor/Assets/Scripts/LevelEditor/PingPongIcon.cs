using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongIcon : MonoBehaviour
{
    [SerializeField] private iTween.EaseType easeType1 = iTween.EaseType.easeInSine;
    [SerializeField] private iTween.EaseType easeType2 = iTween.EaseType.easeOutSine;

    private void Start()
    {
        StartCoroutine(PingPongRoutine());
    }

    private IEnumerator PingPongRoutine()
    {
        while (true)
        {
            iTween.MoveAdd(gameObject, iTween.Hash(
                "y", 0.5f,
                "time", 1f,
                "easetype", easeType1));

            yield return new WaitForSeconds(1f);

            iTween.MoveAdd(gameObject, iTween.Hash(
               "y", -0.5f,
               "time", 1f,
               "easetype", easeType2));

            yield return new WaitForSeconds(1f);
        }
    }
}
