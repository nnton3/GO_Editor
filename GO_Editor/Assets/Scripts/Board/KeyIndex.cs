using UnityEngine;

public class KeyIndex : MonoBehaviour
{
    [SerializeField] private int index = 1;
    public int Index { get => index; set => index = value; }
}
