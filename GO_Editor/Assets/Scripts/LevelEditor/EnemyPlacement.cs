using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemyPlacement : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject enemySpinnerPref;
    [SerializeField] private GameObject enemyPatrolPref;
    [SerializeField] private GameObject enemySniperPref;
    [SerializeField] private GameObject enemyOfficerPref;
    [SerializeField] private GameObject enemyLiquidatorPref;

    private EditorRaycaster raycaster;
    private bool point1select;
    private bool point2select;
    private bool point3select;
    private bool point4select;
    private bool point5select;
    private bool point6select;
    private GameObject point1;
    private GameObject point2;
    private GameObject point3;
    private GameObject point4;
    private GameObject point5;
    private GameObject point6;
    private GameObject enemy;
    private bool enemySelect;
    #endregion

    private void Awake()
    {
        raycaster = GetComponent<EditorRaycaster>();

        LevelInitializer.StartAddObjEvent.AddListener(() =>
        {
            StopAllCoroutines();
            ResetFlags();
        });
    }

    private void ResetFlags()
    {
        point1select = false;
        point2select = false;
        point3select = false;
        point4select = false;
        point5select = false;
        point6select = false;
    }

    public void AddEnemySpinner()
    {
        LevelInitializer.StartAddObjEvent?.Invoke();
        StartCoroutine(AddEnemyRoutine(enemySpinnerPref));
    }

    public void AddEnemySpinner(Vector3 position, Quaternion rotation)
    {
        var instance = InstanceEnemy(enemySpinnerPref, position);
        instance.transform.rotation = rotation;
    }

    public void AddEnemyPatrol()
    {
        LevelInitializer.StartAddObjEvent?.Invoke();
        StartCoroutine(AddEnemyRoutine(enemyPatrolPref));
    }

    public void AddEnemyPatrol(Vector3 position, Quaternion rotation)
    {
        var instance = InstanceEnemy(enemyPatrolPref, position);
        instance.transform.rotation = rotation;
    }

    public void AddEnemySniper()
    {
        LevelInitializer.StartAddObjEvent?.Invoke();
        StartCoroutine(AddEnemyRoutine(enemySniperPref));
    }

    public void AddEnemySniper(Vector3 position, Quaternion rotation)
    {
        var instance = InstanceEnemy(enemySniperPref, position);
        instance.transform.rotation = rotation;
    }

    public void AddEnemyOfficer()
    {
        LevelInitializer.StartAddObjEvent?.Invoke();
        StartCoroutine(AddOfficerRoutine());
    }

    public void AddEnemyOfficer(Vector3 startPos, List<Vector3> waypoints, PatrolData patrolData, Quaternion rotation)
    {
        InstanceOfficer(startPos, waypoints, patrolData);
    }

    public void AddLiquidator()
    {
        LevelInitializer.StartAddObjEvent?.Invoke();
        StartCoroutine(AddEnemyRoutine(enemyLiquidatorPref));
    }

    public void AddLiquidator(Vector3 position, Quaternion rotation)
    {
        var instance = InstanceEnemy(enemyLiquidatorPref, position);
        instance.transform.rotation = rotation;
    }

    private IEnumerator AddOfficerRoutine()
    {
        var waypoints = new List<Vector3>();

        Debug.Log("Select patrol point1");
        while (!point1select)
        {
            if (raycaster.CheckRaycast(1024, "Node", out point1))
                point1select = true;

            yield return null;
        }
        waypoints.Add(point1.transform.position);

        Debug.Log("Select patrol point2");
        while (!point2select)
        {
            if (raycaster.CheckRaycast(1024, "Node", out point2))
                point2select = true;

            yield return null;
        }
        waypoints.Add(point2.transform.position);

        Debug.Log("Select patrol point3");
        while (!point3select)
        {
            if (raycaster.CheckRaycast(1024, "Node", out point3))
                point3select = true;

            yield return null;
        }
        waypoints.Add(point3.transform.position);

        Debug.Log("Select patrol point4");
        while (!point4select)
        {
            if (raycaster.CheckRaycast(1024, "Node", out point4))
                point4select = true;

            yield return null;
        }
        waypoints.Add(point4.transform.position);

        Debug.Log("Select start point");
        while (!point5select)
        {
            if (raycaster.CheckRaycast(1024, "Node", out point5))
                point5select = true;

            yield return null;
        }

        Debug.Log("Select check setry point");
        while (!point6select)
        {
            if (raycaster.CheckRaycast(1024, "Node", out point6))
                point6select = true;

            yield return null;
        }

        Debug.Log("Select enemy for check");
        while (!enemySelect)
        {
            if (raycaster.CheckRaycast(512, "Enemy", out enemy))
                enemySelect = true;

            yield return null;
        }

        InstanceOfficer(point5.transform.position, waypoints, new PatrolData(point6.transform.position, enemy));
        LevelInitializer.EndAddObjEvent?.Invoke();
        ResetFlags();
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

    private IEnumerator AddEnemyRoutine(GameObject enemyPref)
    {
        Debug.Log("Select point");
        while (!point1select)
        {
            if (raycaster.CheckRaycast(1024, "Node", out point1))
                point1select = true;

            yield return null;
        }
        InstanceEnemy(enemyPref, point1.transform.position);
        LevelInitializer.EndAddObjEvent?.Invoke();
        ResetFlags();
    }
}
