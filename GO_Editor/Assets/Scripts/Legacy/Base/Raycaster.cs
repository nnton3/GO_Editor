using UnityEngine;
using System.Collections;

public class Raycaster : MonoBehaviour
{
    public bool Detection(Transform origin, Vector3 movementDirection, float distance = 1f, int layerIndex = 4)
    {
        RaycastHit raycastHit;
        return Physics.Raycast(origin.position,
                            movementDirection,
                            out raycastHit,
                            distance);      
    }

    public RaycastHit GetHit(Transform origin, float distance = 1f, int layerIndex = 0)
    {
        var ray = new Ray(origin.position, origin.forward);

        RaycastHit raycastHit;
        Physics.Raycast(ray, out raycastHit, distance);
        return raycastHit;
    }
}
