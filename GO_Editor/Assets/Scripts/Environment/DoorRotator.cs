using UnityEngine;

public class DoorRotator : MonoBehaviour
{
    [SerializeField] private GameObject pivot;

    public void Open()
    {
        iTween.RotateAdd(pivot, iTween.Hash(
            "y", 90f,
            "time", 0.5f,
            "easetype", iTween.EaseType.linear));
    }

    public void Close()
    {
        iTween.RotateAdd(pivot, iTween.Hash(
            "y", -90f,
            "time", 0.5f,
            "easetype", iTween.EaseType.linear));
    }
}
