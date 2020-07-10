using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ITweenExample : MonoBehaviour
{
    private void Start()
    {
        NewAnim();
        //InvokeRepeating("RotateCube", 1f, 3f);
    }

    private void RotateCube()
    {
        iTween.RotateAdd(gameObject, iTween.Hash(
            "y", 90f,
            "time", 2f,
            "easetype", iTween.EaseType.easeInExpo));

        iTween.RotateAdd(gameObject, iTween.Hash(
            "x", 90f,
            "delay", 2f,
            "time", 1f,
            "easetype", iTween.EaseType.easeOutBounce));

        iTween.ColorTo(gameObject, iTween.Hash(
            "r", 0f,
            "g", 0.5f,
            "b", 1f,
            "delay", 4f,
            "time", 1f,
            "easetype", iTween.EaseType.easeInOutExpo));

        iTween.MoveTo(gameObject, iTween.Hash(
            "y", 0.5f,
            "delay", 6f,
            "time", 0.5f,
            "easetype", iTween.EaseType.easeOutBounce));

        iTween.ShakePosition(Camera.main.gameObject, iTween.Hash(
            "y", 0.2f,
            "delay", 6.3f,
            "time", 0.3f));
    }

    private void NewAnim()
    {
        iTween.MoveFrom(gameObject, iTween.Hash(
            "y", 0f,
            "delay", 1f,
            "time", 0.5f,
            "easetype", iTween.EaseType.easeInOutBounce));
    }
}
