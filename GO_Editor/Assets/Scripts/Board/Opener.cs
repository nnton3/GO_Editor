using UnityEngine;

public class Opener : MonoBehaviour
{
    #region Variables
    [SerializeField] protected DynamicObstacle obstacle;

    protected PlayerInventory inventory;
    #endregion

    public void Initialize()
    {
        inventory = FindObjectOfType<PlayerInventory>();
    }

    public virtual void TryToOpen()
    {
        if (obstacle == null) return;
    }

    // EDITOR FUNCTIONS
    public void SetObstacle(DynamicObstacle _obstacle)
    {
        obstacle = _obstacle;
    }
}
