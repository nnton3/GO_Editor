using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PatrolData
{
    public Vector3 Position;
    public GameObject Enemy;

    public PatrolData(Vector3 _position, GameObject _enemy)
    {
        Position = _position;
        Enemy = _enemy;
    }
}

public class EnemyMover_Officer : EnemieMover
{
    [SerializeField] private bool loop;
    [SerializeField] private List<Vector3> waypoints;
    public List<Vector3> Waypoints => waypoints;
    [SerializeField] private PatrolData checkPatrol;
    public PatrolData PatrolDataValue => checkPatrol;
    private int currentTargetWaypoint = 1;

    public override void MoveOneTurn()
    {
        if (state == EnemyState.Defaul)
            SquarePatrol();
        else MoveToTarget();
    }

    private void SquarePatrol()
    {
        StartCoroutine(SquarePatrolRoutine());
    }

    private IEnumerator SquarePatrolRoutine()
    {
        var startPos = transform.position;
        Debug.Log($"{startPos} = {waypoints[currentTargetWaypoint]}? {waypoints[currentTargetWaypoint] == startPos}");
        if (Vector3.Distance(waypoints[currentTargetWaypoint], startPos) < .1f)
        {
            Debug.Log("work");
            SetTargetPoint();
            sensor.UpdateSensor();
            yield return new WaitForSeconds(rotateTime);
        }
        Debug.Log($"{waypoints[currentTargetWaypoint]} current target");

        var newDest = startPos + transform.TransformVector(directionToMove);

        if (sensor.FoundPlayer)
            yield return StartCoroutine(GetComponent<EnemyManager>().Kill());
        else
        {
            Move(newDest, 0f);

            while (isMoving)
                yield return null;

            CheckPatrol();

            base.FinishMovementEvent.Invoke();
        }
    }

    private void SetTargetPoint()
    {
        currentTargetWaypoint++;
        if (currentTargetWaypoint >= waypoints.Count)
        {
            if (loop)
            {
                currentTargetWaypoint = 0;
            }
            else
            {
                var tempList = new List<Vector3>();

                for (int i = waypoints.Count - 1; i >= 0; i--)
                {
                    tempList.Add(waypoints[i]);
                }
                waypoints = tempList;
                currentTargetWaypoint = 1;
            }
        }

        destination = waypoints[currentTargetWaypoint];
        FaceDestination();
    }

    private void CheckPatrol()
    {
        if (checkPatrol.Enemy == null) return;
        if (transform.position != checkPatrol.Position) return;
        
        var node = board.FindNodeAt(checkPatrol.Position);
        var enemies = board.FindEnemiesAt(node);
        foreach (var item in enemies)
            Debug.Log(item.name);

        foreach (var enemy in enemies)
            if (enemy.gameObject == checkPatrol.Enemy) return;
       
        Debug.Log("ALARM");
        GameManager.RaiseAlarmEvent?.Invoke(currentNode);
    }

    // EDITOR FUNCTIONS
    public void SetPatrolParams(List<Vector3> _waypoints, PatrolData _checkPatrol = new PatrolData(), bool _loop = false)
    {
        waypoints.Clear();
        waypoints.AddRange(_waypoints);
        checkPatrol = _checkPatrol;
        loop = _loop;
    }

    public void SetPatrolParams(PatrolData _checkPatrol) =>
        checkPatrol = _checkPatrol;
}
