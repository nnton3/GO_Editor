using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ContextMenu_Door : ContextMenu
{
    [SerializeField] private Text inputField;

    public void SetIndex()
    {
        int index = 0;
        if (int.TryParse(inputField.text, out index))
        {
            var obstacle = transform.parent.GetComponent<DynamicObstacle>();
            obstacle.Point1.GetComponent<Door>().Index = index;
            obstacle.Point2.GetComponent<Door>().Index = index;
        }
        else Debug.LogWarning("Input is not valid");
    }

    public override void DeleteObj()
    {
        FindObjectOfType<ObstaclePlacement>().DeleteObstacle(transform.parent.GetComponent<Board_Node>());
        Destroy(gameObject);
    }
}
