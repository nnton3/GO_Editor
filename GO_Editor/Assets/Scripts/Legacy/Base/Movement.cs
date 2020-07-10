using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Transform model;
    private Raycaster raycaster;
    private Vector3 movementDirection;

    private void Awake()
    {
        raycaster = GetComponent<Raycaster>();
    }

    public bool Move(Vector3 direction)
    {
        movementDirection = direction;
        if (!CanMove()) return false;
        transform.position += direction;
        transform.Rotate(0f, 
                        Vector3.SignedAngle(transform.forward, direction, Vector3.up),
                        0f);
        return true;
    }

    private bool CanMove()
    {
        var result = raycaster.Detection(model, movementDirection);
        return !result;
    }
}
