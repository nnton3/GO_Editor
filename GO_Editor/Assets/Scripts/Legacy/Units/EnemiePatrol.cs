using UnityEngine;
using System.Collections;

//public class EnemiePatrol : Enemie
//{
//    [SerializeField] private Vector3[] patrolWaypoints;
//    [SerializeField] private GameObject startWP;
//    [SerializeField] private GameObject endWP;

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.F))
//            Action();
//    }

//    protected override void Action()
//    {
//        if (followPath.CurrentNode == endWP)
//            followPath.UpdatePath(startWP);
//        if (followPath.CurrentNode == startWP)
//            followPath.UpdatePath(endWP);

//        followPath.GoToNextWP();
//    }

//    //private IEnumerator StartTurn()
//    //{
//    //    yield return new WaitForSeconds(0.5f);
//    //    movement.Move(patrolWaypoints[currentWaypoint]);
//    //    currentWaypoint++;
//    //    if (currentWaypoint >= patrolWaypoints.Length) currentWaypoint = 0;
//    //}
//}
