using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;

public class EnemyPlacement : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject enemySpinnerPref;
    [SerializeField] private GameObject enemyPatrolPref;
    [SerializeField] private GameObject enemySniperPref;
    [SerializeField] private GameObject enemyOfficerPref;
    [SerializeField] private GameObject enemyLiquidatorPref;
    [SerializeField] private GameObject enemyKinologistPref;

    private EditorRaycaster raycaster;
    private ObjectSelector selector;
    private IDisposable routine;
    #endregion

    private void Awake()
    {
        raycaster = GetComponent<EditorRaycaster>();
        selector = GetComponent<ObjectSelector>();

        LevelInitializer.StartAddObjEvent.AddListener(() =>
        {
            if (routine != null)
                routine.Dispose();
        });
    }

    public void AddEnemy(int enemyType)
    {
        LevelInitializer.StartAddObjEvent?.Invoke();
        routine = Observable
            .FromCoroutine(() => selector.SelectNodeRoutine("Select node to place enemy"))
            .Subscribe(_ =>
            {
                InstanceEnemy(GetEnemyPref(enemyType), selector.Nodes[0].transform.position);
                selector.Reset();
                LevelInitializer.EndAddObjEvent?.Invoke();
            });
    }

    public void AddEnemy(int enemyType, Vector3 position, Quaternion rotation)
    {
        var instance = InstanceEnemy(GetEnemyPref(enemyType), position);
        instance.transform.rotation = rotation;
    }

    private GameObject GetEnemyPref(int enemyType)
    {
        switch ((EnemyIdentifier)enemyType)
        {
            case EnemyIdentifier.Spinner:
                return enemySpinnerPref;
            case EnemyIdentifier.Patrol:
                return enemyPatrolPref;
            case EnemyIdentifier.Sniper:
                return enemySniperPref;
            case EnemyIdentifier.Liquidator:
                return enemyLiquidatorPref;
            case EnemyIdentifier.Kinologist:
                return enemyKinologistPref;
            default:
                break;
        }
        Debug.LogWarning("Not valid enemy type");
        return null;
    }

    public void AddEnemyOfficer()
    {
        LevelInitializer.StartAddObjEvent?.Invoke();
        routine = Observable
            .FromCoroutine(() => selector.SelectNodeRoutine("Select patrol point1"))
            .SelectMany(() => selector.SelectNodeRoutine("Select patrol point2"))
            .SelectMany(() => selector.SelectNodeRoutine("Select patrol point3"))
            .SelectMany(() => selector.SelectNodeRoutine("Select patrol point4"))
            .SelectMany(() => selector.SelectNodeRoutine("Select start point"))
            .SelectMany(() => selector.SelectNodeRoutine("Select check setry point"))
            .SelectMany(() => selector.SelectEnemyRoutine("Select enemy for check"))
            .Subscribe(_ =>
            {
                var points = new List<Vector3>();
                selector.Nodes.GetRange(0, 4).ForEach(node => points.Add(node.transform.position));
                InstanceOfficer(
                    selector.Nodes[4].transform.position, 
                    points, 
                    new PatrolData(selector.Nodes[5].transform.position, selector.Enemy));
                selector.Reset();
                LevelInitializer.EndAddObjEvent?.Invoke();
            });
    }

    public void AddEnemyOfficer(Vector3 startPos, List<Vector3> waypoints, PatrolData patrolData, Quaternion rotation)
    {
        InstanceOfficer(startPos, waypoints, patrolData);
    }

    private void InstanceOfficer(Vector3 startPos, List<Vector3> waypoints, PatrolData patrolData)
    {
        if (InputParamsValid(startPos, waypoints, patrolData))
        {
            var officer = Instantiate(enemyOfficerPref, startPos, Quaternion.identity);
            officer.GetComponent<EnemyManager>().Initialize();
            officer.GetComponent<EnemyMover_Officer>().SetPatrolParams(waypoints, patrolData);
        }
    }

    private bool InputParamsValid(Vector3 startPos, List<Vector3> waypoints, PatrolData patrolData)
    {
        bool startposValid = false;
        bool checkPatrolPositionValid = false;

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i].x == waypoints[i + 1].x)
            {
                if (startPos.x == waypoints[i].x) startposValid = true;
                if (patrolData.Position.x == waypoints[i].x) checkPatrolPositionValid = true;
            }
            else if (waypoints[i].z == waypoints[i + 1].z)
            {
                if (startPos.z == waypoints[i].z) startposValid = true;
                if (patrolData.Position.z == waypoints[i].z) checkPatrolPositionValid = true;
            }
            else
            {
                Debug.Log("Not valid patrol path");
                return false;
            }

            if (waypoints[i] == waypoints[i + 1])
            {
                Debug.Log("Not valid patrol path");
                return false;
            }
        }

        if (!startposValid)
        {
            Debug.Log("Start position is not valid");
            return false;
        }
        if (!checkPatrolPositionValid)
        {
            Debug.Log("Position for check sentry is not valid");
            return false;
        }

        return true;
    }

    private GameObject InstanceEnemy(GameObject enemyPref, Vector3 pos)
    {
        var enemy = Instantiate(enemyPref, pos, Quaternion.identity);
        enemy.GetComponent<EnemyManager>().Initialize();
        return enemy;
    }
}
