using UnityEngine;
using System.Collections;

public class DynamicObstacle : MonoBehaviour
{
    #region Variables
    [SerializeField] private Board_Node point1;
    public Board_Node Point1 => point1;
    [SerializeField] private Board_Node point2;
    public Board_Node Point2 => point2;
    [SerializeField] private GameObject obstaclePref;

    private GameManager gameManager;
    private WPManager wpmanager;
    private DoorRotator rotator;
    private GameObject obstacle;
    public GameObject Obstacle => obstacle;

    protected bool isOpen = false;
    public bool IsOpen=> isOpen;
    #endregion

    public void Initialize()
    {
        gameManager = FindObjectOfType<GameManager>();
        wpmanager = FindObjectOfType<WPManager>();

        InstanceObstacle();
        rotator = obstacle.GetComponent<DoorRotator>();
    }

    public void InstanceObstacle()
    {
        if (obstacle != null) return;

        var instancePos = (point1.transform.position + point2.transform.position) / 2;
        var instanceRot = Quaternion.LookRotation(point1.transform.position - point2.transform.position, Vector3.up);
        obstacle = Instantiate(obstaclePref, instancePos, instanceRot);
    }

    public virtual void OpenPath()
    {
        Debug.Log("open");
        point1.AddLink(point2);
        obstacle.GetComponent<Collider>().enabled = false;
        wpmanager.UpdateGraph();
        gameManager.BoardUpdatedEvent?.Invoke();
        isOpen = true;

        if (rotator != null)
            rotator.Open();
    }

    public virtual void ClosePath()
    {
        Debug.Log("close");
        point1.RemoveLink(point2);
        obstacle.GetComponent<Collider>().enabled = true;
        wpmanager.UpdateGraph();
        gameManager.BoardUpdatedEvent?.Invoke();
        isOpen = false;

        if (rotator != null)
            rotator.Close();
    }

    public void DeleteObstacle()
    {

    }

    // EDITOR FUNCTIONS
    public void SetPoints(GameObject _point1, GameObject _point2, GameObject _obstaclePref)
    {
        point1 = _point1.GetComponent<Board_Node>();
        point2 = _point2.GetComponent<Board_Node>();
        obstaclePref = _obstaclePref;
    }
}
