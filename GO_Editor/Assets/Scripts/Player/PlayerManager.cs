using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerDeath))]
public class PlayerManager : TurnManager
{
    #region Variables
    [SerializeField] private bool stoneRelised;
    [SerializeField] private int patrons = 2;
    private bool playerFire;
    private Board board;
    private PlayerMover playerMover;
    private PlayerDeath playerDeath;
    private bool isInitialized;
    private PlayerInput playerInput;
    public PlayerInput PlayerInput => playerInput;

    [HideInInspector] public UnityEvent DeathEvent;
    #endregion

    public override void Initialize()
    {
        base.Initialize();
        playerMover = GetComponent<PlayerMover>();
        playerInput = GetComponent<PlayerInput>();
        playerDeath = GetComponent<PlayerDeath>();

        if (playerMover != null) playerMover.FinishMovementEvent.AddListener(() =>
        {
            FinishTurn();
        });
        if (playerDeath != null) DeathEvent.AddListener(playerDeath.Die);
        board = FindObjectOfType<Board>();
        isInitialized = true;
    }

    private void Update()
    {
        if (isInitialized)
            CheckPlayerInput();
    }

    private void CheckPlayerInput()
    {
        if (playerMover.IsMoving || gameManager.CurrentTurn == Turn.Enemy) return;
        playerInput.GetKeyInput();

        if (playerInput.V == 0)
        {
            if (playerInput.H < 0) playerMover.MoveLeft();
            else if (playerInput.H > 0) playerMover.MoveRight();
        }
        else if (playerInput.H == 0)
        {
            if (playerInput.V < 0) playerMover.MoveBackward();
            else if (playerInput.V > 0) playerMover.MoveForward();
        }

        if (playerFire)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycast;
                if (Physics.Raycast(ray, out raycast))
                {
                    var enemy = raycast.transform.GetComponent<EnemyManager>();
                    if (TargetIsValid(enemy))
                        StartCoroutine(KillEnemy(enemy));
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                playerFire = false;
                playerInput.InputEnabled = true;
            }
        }
    }

    private IEnumerator KillEnemy(EnemyManager enemy)
    {
        playerInput.InputEnabled = false;

        patrons--;
        enemy.Die();
        playerFire = false;
        GameManager.RaiseAlarmEvent?.Invoke(playerMover.CurrentNode);

        yield return new WaitForSeconds(1f);
        base.FinishTurn();
        playerInput.InputEnabled = true;
    }

    public void Die()
    {
        DeathEvent?.Invoke();
    }

    private IEnumerator CaptureEnemies()
    {
        playerInput.InputEnabled = false;

        if (playerMover.CurrentNode.Type != NodeType.Bush)
            if (board != null)
            {
                var enemies = board.FindEnemiesAt(board.PlayerNode);

                foreach (var enemy in enemies)
                {
                    if (!enemy.GetComponent<EnemyMover_Liquidator>())
                        enemy.Die();
                    else
                    {
                        Debug.Log("Liquidator");
                        Die();
                    }
                }
                yield return new WaitForSeconds(1.5f);
            }

        Debug.Log("player turn complete, capture enemies");
        base.FinishTurn();
        playerInput.InputEnabled = true;
    }

    private bool HaveEnemieOnWay()
    {
        var enemies = board.FindEnemiesAt(board.PlayerNode);
        if (enemies.Count == 0)
            return false;
        else return true;
    }

    public override void FinishTurn()
    {
        if (playerMover.CurrentNode.Type == NodeType.Stone)
        {
            playerInput.InputEnabled = false;
            StartCoroutine(ReliseStone());
        }
        else if (HaveEnemieOnWay())
        {
            playerInput.InputEnabled = false;
            StartCoroutine(CaptureEnemies());
        }
        else
            base.FinishTurn();
    }

    private IEnumerator ReliseStone()
    {
        while (!stoneRelised)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycast;
                if (Physics.Raycast(ray, out raycast))
                {
                    var node = raycast.transform.GetComponent<Board_Node>();
                    if (node != null)
                    {
                        var targetNode = playerMover.CurrentNode.NeighborNodes.Find(n => n == node);
                        if (targetNode != null)
                            ThrowStone(targetNode);
                    }
                }
            }
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        playerMover.CurrentNode.Type = NodeType.Default;
        stoneRelised = false;

        Debug.Log("player turn complete, stone");

        base.FinishTurn();
        playerInput.InputEnabled = true;
    }

    private void ThrowStone(Board_Node node)
    {
        Debug.Log("THROW STONE");
        foreach (var n in node.LinkedNodes)
        {
            var enemies = board.FindEnemiesAt(n);
            foreach (var enemy in enemies)
                enemy.GetComponent<EnemieMover>().ToAlarmState(node);
        }

        stoneRelised = true;
    }

    public void TryToFire()
    {
        if (patrons == 0) return;
        playerInput.InputEnabled = false;

        playerFire = true;
    }

    private bool TargetIsValid(EnemyManager enemy)
    {
        if (enemy == null) return false;
        if (enemy.GetComponent<SpotlightManager>()) return false;
        if (enemy.GetComponent<EnemieMover>().CurrentNode.Type == NodeType.Bush) return false;
        if (Vector3.Distance(playerMover.CurrentNode.transform.position,
                            enemy.GetComponent<EnemieMover>().CurrentNode.transform.position) > (Board.spacing * 2))
            return false;
        
        RaycastHit raycastHit;
        var direction = enemy.transform.position - transform.position;
        
        if (Physics.Raycast(
            transform.position + new Vector3(0f, .6f, 0f),
            direction, 
            out raycastHit, 
            Board.spacing * 2))
            if (raycastHit.transform.gameObject == enemy.gameObject)
                return true;

        return false;
    }

    // EDITOR
    public void Reset()
    {
        StopAllCoroutines();
        DeathEvent.RemoveAllListeners();
        patrons = 2;
        transform.position = playerMover.StartPos;
        transform.rotation = playerMover.StartRot;
        GetComponent<PlayerInventory>().Reset();
        playerDeath.Anim.SetTrigger("reset");
    }

    private void OnDestroy()
    {
        DeathEvent.RemoveAllListeners();
    }
}
