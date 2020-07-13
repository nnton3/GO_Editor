using UnityEngine;

public class Door : Opener
{
    private bool isOpen;
    [SerializeField] private int index = 1;
    public int Index { get => index; set => index = value; }

    public override void TryToOpen()
    {
        if (isOpen) return;

        base.TryToOpen();
        if (inventory.Keys.Find(k => k == index) != 0)
        {
            inventory.Keys.Remove(index);
            obstacle.OpenPath();
            isOpen = true;
        }
    }
}
