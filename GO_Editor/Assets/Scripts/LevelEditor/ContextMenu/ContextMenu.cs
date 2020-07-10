using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextMenu : MonoBehaviour
{
    protected EditorRaycaster raycaster;
    protected GameObject point1;
    protected bool point1select;

    private void Awake()
    {
        raycaster = FindObjectOfType<EditorRaycaster>();

        LevelInitializer.StartAddObjEvent.AddListener(() =>
        {
            StopAllCoroutines();
            ResetFlags();
        });
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
    }

    protected void ResetFlags()
    {
        point1select = false;
    }

    public void RotateObj(bool clockwise)
    {
        LevelInitializer.StartAddObjEvent?.Invoke();

        if (clockwise)
            iTween.RotateAdd(transform.parent.gameObject, iTween.Hash(
                "y", 90f,
                "time", 0.5f,
                "easetype", iTween.EaseType.linear));
        else
            iTween.RotateAdd(transform.parent.gameObject, iTween.Hash(
                "y", -90f,
                "time", 0.5f,
                "easetype", iTween.EaseType.linear));
    }

    public void SetPosition()
    {
        LevelInitializer.StartAddObjEvent?.Invoke();
        StartCoroutine(SetPositionRoutine());
    }

    protected virtual IEnumerator SetPositionRoutine()
    {
        Debug.Log("Select new point for this object");
        while (!point1select)
        {
            if (raycaster.CheckRaycast(1024, "Node", out point1))
            {
                iTween.MoveTo(transform.parent.gameObject, iTween.Hash(
                    "x", point1.transform.position.x,
                    "z", point1.transform.position.z,
                    "time", 1f,
                    "easetype", iTween.EaseType.easeInOutSine));

                point1select = true;
            }
            yield return null;
        }
        LevelInitializer.EndAddObjEvent?.Invoke();
        ResetFlags();
    }

    public virtual void DeleteObj()
    {
        Destroy(transform.parent.gameObject);
    }

    public void HideContextMenu()
    {
        LevelInitializer.EndAddObjEvent?.Invoke();
        gameObject.SetActive(false);
    }
}
