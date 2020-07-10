using UnityEngine;

public class Barrier : DynamicObstacle
{
    private bool isOpen;

    public void SwapBarrier()
    {
        if (isOpen)
        {
            isOpen = false;
            ClosePath();
        }
        else
        {
            isOpen = true;
            OpenPath();
        }
    }
}
