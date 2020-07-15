using UnityEngine;

public class Barrier : DynamicObstacle
{
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
