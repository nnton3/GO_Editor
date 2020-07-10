using UnityEngine;
using UnityEngine.UI;

//public class Player : Unit
//{
//    [SerializeField] private Go_Node startNode;
//    private MyFollowPath followPath;

//    protected override void Start()
//    {
//        base.Start();
//        followPath = GetComponent<MyFollowPath>();
//    }

//    private void Update()
//    {
//        if (Input.GetMouseButtonDown(0))
//            SetNewTarget();
//        if (Input.GetKeyDown(KeyCode.F))
//            followPath.GoToNextWP();
//    }

//    private void SetNewTarget()
//    {
//        var wp = GetRaycastHit();
//        if (wp != null)
//            followPath.UpdatePath(wp);
//    }

//    private GameObject GetRaycastHit()
//    {
//        Ray ray;
//        RaycastHit raycastHit;

//        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        if (Physics.Raycast(ray, out raycastHit))
//        {
//            return raycastHit.transform.gameObject;
//        }
//        return null;
//    }
//}
