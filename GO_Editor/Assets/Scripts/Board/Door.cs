using UnityEngine;

public class Door : Opener
{
    [SerializeField] private int index = 1;
    public int Index { get => index; set => index = value; }

    public override void TryToOpen()
    {
        base.TryToOpen();
        if (inventory.Keys.Find(k => k == index) != 0)
        {
            inventory.Keys.Remove(index);
            obstacle.OpenPath();
            Destroy(obstacle);
        }
    }
}
