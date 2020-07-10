using UnityEngine;

public class Services : MonoBehaviour
{
    private Board board;
    private WPManager wpmanager;
    private GameManager gameManager;

    protected virtual void Awake()
    {
        InitializeGame();
        StartGame();
    }

    public void InitializeGame()
    {
        InitializeBoard();
        InitializeNodes();
        InitializeWPManager();
        InitializeGameManager();
        InitializeGameTarget();
        InitializePlayer();
        InitializeEnemies();
        InitializeDynamicObstacles();
        InitializeOpeners();
    }

    public void InitializeBoard()
    {
        board = FindObjectOfType<Board>();
        if (board != null)
            board.Initialize();
        else Debug.Log("Board is lost");
    }

    public void InitializeNodes()
    {
        if (board.AllNodes.Count != 0)
        {
            foreach (var node in board.AllNodes)
                node.Initialize();

            foreach (var node in board.AllNodes)
                node.FindNeighbors(board.AllNodes);
        }
        else Debug.Log("You haven't nodes");
    }

    public void InitializeWPManager()
    {
        wpmanager = FindObjectOfType<WPManager>();
        if (wpmanager != null)
            wpmanager.Initialize();
        else Debug.Log("WPManager is lost");
    }

    public void InitializeGameManager()
    {
        gameManager = GetComponent<GameManager>();
        if (gameManager != null)
            gameManager.Initialize();
        else Debug.Log("GameManager is lost");
    }

    public void InitializeGameTarget()
    {
        GetComponent<GameTarget>()?.Initialize();

        if (!GetComponent<GameTarget>())
            Debug.Log("Mission haven't target");
    }

    public void InitializePlayer()
    {
        var player = FindObjectOfType<PlayerManager>();
        if (player != null)
        {
            player.Initialize();
            player.GetComponent<Mover>()?.Initialize();
        }
        else Debug.Log("Player is lost");
    }

    public void InitializeEnemies()
    {
        if (gameManager.Enemies.Count != 0)
            foreach (var enemy in gameManager.Enemies)
            {
                enemy.Initialize();
                enemy.GetComponent<Mover>()?.Initialize();
                enemy.GetComponent<EnemySensor>()?.Initialize();
                enemy.GetComponent<EnemyAttack>()?.Initialize();
                enemy.GetComponent<EnemyDeath>()?.Initialize();
            }
        else Debug.Log("You haven't enemies");
    }

    public void InitializeDynamicObstacles()
    {
        var obstacles = FindObjectsOfType<DynamicObstacle>();
        if (obstacles.Length != 0)
            foreach (var obstacle in obstacles)
                obstacle.Initialize();
        else Debug.Log("You haven't doors, barriers and barbed wires");
    }

    public void InitializeOpeners()
    {
        var openers = FindObjectsOfType<Opener>();
        if (openers.Length != 0)
            foreach (var opener in openers)
                opener.Initialize();
        else Debug.Log("You haven't nodes that can open doors or barriers");
    }

    public void StartGame()
    {
        if (gameManager != null)
            gameManager.StartLoop();
        else Debug.Log("GameManager is lost");
    }
}
