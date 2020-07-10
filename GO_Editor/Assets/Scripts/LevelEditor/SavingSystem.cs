using UnityEngine;
using System.Collections;
using System.Linq;
using System.IO;
using System;

public class SavingSystem : MonoBehaviour
{
    #region Variables
    private Board board;
    private ObstaclePlacement obstaclePlacement;
    private ObjectPlacement objectPlacement;
    private EnemyPlacement enemyPlacement;
    private LevelInitializer levelInitializer;
    private JsonLevelData levelData = new JsonLevelData();

    [Header("PRINT CONFIG PATH")]
    [SerializeField] public string ConfigPath;
    //[Header("DRAG'n'DROP LEVEL DATA IN THIS FIELD")]
    //[SerializeField] private LevelData levelData;
    #endregion

    public void Initialize()
    {
        obstaclePlacement = GetComponent<ObstaclePlacement>();
        if (obstaclePlacement == null) Debug.LogWarning("Obstacle placement is lost!");

        objectPlacement = GetComponent<ObjectPlacement>();
        if (objectPlacement == null) Debug.LogWarning("Object placement is lost");

        enemyPlacement = GetComponent<EnemyPlacement>();
        if (enemyPlacement == null) Debug.LogWarning("Enemy placement is lost!");

        board = FindObjectOfType<Board>();
        if (board == null) Debug.LogWarning("Board is lost");

        levelInitializer = GetComponent<LevelInitializer>();
        if (levelInitializer == null) Debug.Log("Level initializer is lost");
    }

    #region Saving
    public void Save()
    {
        SaveLinks();
        SavePlayer();
        SaveObjects();
        SaveObstacles();
        SaveEnemies();
        WriteLevelData();
    }

    private void SaveLinks()
    {
        levelData.nodes.Clear();
        foreach (var node in board.AllNodes)
        {
            if (node.LinkedNodes.Count == 0) continue;

            var nodeData = new NodeData(node.transform.position);
            foreach (var linkedNode in node.LinkedNodes)
                nodeData.Links.Add(linkedNode.transform.position);

            levelData.nodes.Add(nodeData);
        }
    }

    private void SavePlayer()
    {
        levelData.player = new PlayerData(FindObjectOfType<PlayerManager>().transform.position);
    }

    private void SaveObjects()
    {
        levelData.objects.Clear();
        foreach (var node in FindObjectOfType<Board>().AllNodes)
        {
            if (node.Type == NodeType.Default) continue;
            levelData.objects.Add(new ObjectData(node.Type, node.transform.position));
        }
    }

    private void SaveObstacles()
    {
        SaveBarriers();
        SaveDoors();
        SaveBarbedWires();
        SaveBushes();
    }

    private void SaveBushes()
    {
        levelData.bushes.Clear();
        //Debug.Log(GameObject.FindGameObjectsWithTag("Bush").Length);
        foreach (var bush in GameObject.FindGameObjectsWithTag("Bush"))
            levelData.bushes.Add(new BushData(bush.transform.position));
        //Debug.Log($"U save {levelData.bushes.Count} bushes");
    }

    private void SaveDoors()
    {
        levelData.doors.Clear();
        foreach (var node in board.AllNodes)
        {
            if (node.Type != NodeType.Opener) continue;
            if (!node.GetComponent<Door>()) continue;
            if (!node.GetComponent<DynamicObstacle>()) continue;
            var door = node.GetComponent<DynamicObstacle>();
            levelData.doors.Add(new DoorData(
                door.Point1.transform.position,
                door.Point2.transform.position));
        }
    }

    private void SaveBarbedWires()
    {
        levelData.barbedWires.Clear();
        foreach (var node in board.AllNodes)
        {
            if (node.Type != NodeType.Opener) continue;
            if (!node.GetComponent<BarbedWire>()) continue;
            if (!node.GetComponent<DynamicObstacle>()) continue;
            var barbedWires = node.GetComponent<DynamicObstacle>();
            levelData.barbedWires.Add(new BarbedWireData(
                barbedWires.Point1.transform.position,
                barbedWires.Point2.transform.position));
        }
    }

    private void SaveBarriers()
    {
        levelData.barriers.Clear();
        foreach (var node in board.AllNodes)
            if (node.Type == NodeType.Lever)
            {
                var obstacle = node.GetComponent<DynamicObstacle>();
                levelData.barriers.Add(new BarrierData(
                    obstacle.transform.position,
                    obstacle.Point1.transform.position,
                    obstacle.Point2.transform.position));
            }
    }

    private void SaveEnemies()
    {
        levelData.enemies.Clear();
        foreach (var enemy in FindObjectsOfType<EnemyManager>())
        {
            if (enemy.GetComponent<EnemyMover_Officer>()) continue;
            levelData.enemies.Add(new EnemyData(
                enemy.Identifier,
                enemy.transform.position, 
                enemy.transform.rotation));
        }
    }
    #endregion

    #region ClearMap
    private void ClearMap()
    {
        DeleteObstacles();
        DeleteObjects();
        DeleteEnemies();
        DeleteLinks();
    }

    private void DeleteObjects()
    {
        foreach (var node in board.AllNodes)
        {
            if (node.Type != NodeType.Default)
                objectPlacement.DeleteObject(node);
        }
    }

    private void DeleteLinks()
    {
        foreach (var target in levelData.nodes)
        {
            var node = board.FindNodeAt(target.position);
            if (node == null)
            {
                Debug.LogWarning($"Node on position{target.position} is lost");
                continue;
            }

            for (int i = 0; i < node.Links.Count; i++)
            {
                var linkInstance = node.Links.ElementAt(i);
                Destroy(linkInstance.Value.gameObject);
            }

            node.Links.Clear();
            node.LinkedNodes.Clear();
        }
    }

    private void DeleteEnemies()
    {
        foreach (var enemy in FindObjectsOfType<EnemyManager>())
            Destroy(enemy.gameObject);
    }

    private void DeleteObstacles()
    {
        DeleteBarbedWires();
        DeleteDoors();
        DeleteBarriers();
        DeleteBushes();
    }

    private void DeleteBushes()
    {
        foreach (var bush in GameObject.FindGameObjectsWithTag("Bush"))
        {
            var node = board.FindNodeAt(bush.transform.position);
            node.Type = NodeType.Default;
            Destroy(bush);
        }
    }

    private void DeleteDoors()
    {
        foreach (var node in board.AllNodes)
            if (node.GetComponent<DynamicObstacle>())
                if (node.GetComponent<Door>())
                    obstaclePlacement.DeleteObstacle(node);
    }

    private void DeleteBarriers()
    {
        foreach (var node in FindObjectsOfType<Barrier>())
            obstaclePlacement.DeleteBarrier(node.GetComponent<Board_Node>());
    }

    private void DeleteBarbedWires()
    {
        foreach (var node in board.AllNodes)
            if (node.GetComponent<DynamicObstacle>())
                if (node.GetComponent<BarbedWire>())
                    obstaclePlacement.DeleteObstacle(node);
    }
    #endregion

    #region Loading
    public void StartLoad()
    {
        StartCoroutine(LoadRoutine());
    }

    public void Load()
    {
        LoadLinks();
        LoadObjects();
        LoadObstacles();
        LoadEnemies();
        LoadPlayer();
    }

    private IEnumerator LoadRoutine()
    {
        ClearMap();
        ReadLevelData();
        yield return new WaitForSeconds(.5f);
        Load();
    }

    private void LoadLinks()
    {
        foreach (var target in levelData.nodes)
        {
            var node = board.FindNodeAt(target.position);
            if (node == null)
            {
                Debug.LogWarning($"Node on position{target.position} is lost");
                continue;
            }
            
            foreach (var nodePos in target.Links)
                node.LinkNode(board.FindNodeAt(nodePos));
        }
    }

    private void LoadPlayer()
    {
        var player = FindObjectOfType<PlayerManager>();
        if (player != null)
        {
            player.Reset();
            player.transform.position = levelData.player.position;
        }
        else levelInitializer.InstancePlayer(levelData.player.position);
    }

    private void LoadObstacles()
    {
        LoadBarriers();
        LoadDoors();
        LoadBarbedWires();
        LoadBushes();
    }

    private void LoadBushes()
    {
        foreach (var target in levelData.bushes)
            obstaclePlacement.PlaceBush(board.FindNodeAt(target.pos));
    }

    private void LoadDoors()
    {
        foreach (var target in levelData.doors)
        {
            var node1 = board.FindNodeAt(target.point1);
            var node2 = board.FindNodeAt(target.point2);
            var obstacle = node1.GetComponent<DynamicObstacle>();
            if (obstacle == null)
            {
                if (!node1.LinkedNodes.Contains(node2))
                    node1.LinkNode(node2);

                obstaclePlacement.PlaceDoor(node1.gameObject, node2.gameObject);
                obstaclePlacement.InitDoor(
                    node1.gameObject,
                    node2.gameObject,
                    node1.GetComponent<DynamicObstacle>());
            }
            else
            {
                if (obstacle.IsOpen && !target.isOpen) obstacle.OpenPath();
                if (!obstacle.IsOpen && target.isOpen) obstacle.ClosePath();
            }
        }
    }

    private void LoadBarbedWires()
    {
        foreach (var target in levelData.barbedWires)
        {
            var node1 = board.FindNodeAt(target.point1);
            var node2 = board.FindNodeAt(target.point2);

            node1.LinkNode(node2);

            if (!node1.LinkedNodes.Contains(node2))
            {
                Debug.Log("Create link");
                node1.LinkNode(node2);
            }

            obstaclePlacement.PlaceBarbedWire(node1.gameObject, node2.gameObject);
        }
    }

    private void LoadBarriers()
    {
        foreach (var target in levelData.barriers)
        {
            var node1 = board.FindNodeAt(target.point1);
            var node2 = board.FindNodeAt(target.point2);
            var node3 = board.FindNodeAt(target.point3);
            var obstacle = node1.GetComponent<DynamicObstacle>();
            if (obstacle == null)
            {
                if (!node2.LinkedNodes.Contains(node3))
                    node2.LinkNode(node3);

                obstaclePlacement.CreateBarreir(
                    node1.gameObject,
                    node2.gameObject,
                    node3.gameObject);
            }
            else
            {
                if (obstacle.IsOpen && !target.isOpen) obstacle.ClosePath();
                if (!obstacle.IsOpen && target.isOpen) obstacle.OpenPath();
            }
        }
    }

    private void LoadObjects()
    {
        foreach (var target in levelData.objects)
        {
            var node = board.FindNodeAt(target.position);
            if (node == null) continue;

            objectPlacement.PlaceObject(target.type, node.transform);
        }
    }

    private void LoadEnemies()
    {
        foreach (var target in levelData.enemies)
        {
            var node = board.FindNodeAt(target.position);
            var enemy = board.FindEnemiesAt(node);
            if (enemy.Count == 0)
                switch (target.identifier)
                {
                    case EnemyIdentifier.Spinner:
                        enemyPlacement.AddEnemySpinner(target.position, target.rotation);
                        break;
                    case EnemyIdentifier.Patrol:
                        enemyPlacement.AddEnemyPatrol(target.position, target.rotation);
                        break;
                    case EnemyIdentifier.Sniper:
                        enemyPlacement.AddEnemySniper(target.position, target.rotation);
                        break;
                    case EnemyIdentifier.Kinologist:
                        //enemyPlacement.AddEnemyKinologist(target.position, target.rotation);
                        break;
                    case EnemyIdentifier.Liquidator:
                        enemyPlacement.AddLiquidator(target.position, target.rotation);
                        break;
                    default:
                        break;
                }
            else
            {
                var targetEnemy = enemy.Find(e => e.Identifier == target.identifier);
                if (targetEnemy != null)
                    targetEnemy.transform.rotation = target.rotation;
            }
        }
    }
    #endregion

    public void ReadLevelData()
    {
        using (StreamReader stream = new StreamReader(ConfigPath))
        {
            string json = stream.ReadToEnd();
            levelData = JsonUtility.FromJson<JsonLevelData>(json);
        }
    }

    public void WriteLevelData()
    {
        using (StreamWriter stream = new StreamWriter(ConfigPath))
        {
            string json = JsonUtility.ToJson(levelData);
            stream.Write(json);
        }
    }
}
