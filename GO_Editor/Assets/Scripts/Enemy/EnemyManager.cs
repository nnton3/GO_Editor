using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum EnemyIdentifier
{
    Setry,
    Spinner,
    Patrol,
    Sniper,
    Officer,
    Kinologist,
    Liquidator
}

[RequireComponent(typeof(EnemieMover))]
[RequireComponent(typeof(EnemySensor))]
[RequireComponent(typeof(EnemyAttack))]
public class EnemyManager : TurnManager
{
    #region Variables
    protected EnemieMover enemyMover;
    private EnemySensor enemieSensor;
    protected EnemyAttack enemyAttack;
    private Board board;
    private bool isDead;
    public bool IsDead => isDead;

    [SerializeField] private bool isMissionTarget;
    public bool IsMissionTarget => isMissionTarget;
    [HideInInspector] public UnityEvent DeathEvent;
    #endregion

    public override void Initialize()
    {
        base.Initialize();
        board = FindObjectOfType<Board>();
        enemyMover = GetComponent<EnemieMover>();
        enemieSensor = GetComponent<EnemySensor>();
        enemyAttack = GetComponent<EnemyAttack>();

        if (enemyMover != null) enemyMover.FinishMovementEvent.AddListener(FinishTurn);
    }

    public void PlayTurn()
    {
        if (isDead)
        {
            FinishTurn();
            return;
        }
        StartCoroutine(PlayTurnRoutine());
    }

    private IEnumerator PlayTurnRoutine()
    {
        if (gameManager != null && !gameManager.IsGameOver)
        {
            enemieSensor.UpdateSensor();
            
            yield return new WaitForSeconds(0f);
            
            if (enemieSensor.FoundPlayer)
                yield return StartCoroutine(Kill());
            else
                enemyMover.MoveOneTurn();
        }
    }

    protected virtual IEnumerator Kill()
    {
        gameManager.LoseLevel();

        var playerPos = new Vector3(board.PlayerNode.Coordinate.x, 0f,
            board.PlayerNode.Coordinate.y);
        enemyMover.Move(playerPos, 0f);

        while (enemyMover.IsMoving) yield return null;

        enemyAttack.Attack();
    }

    public virtual void Die()
    {
        if (isDead) return;

        isDead = true;
        DeathEvent?.Invoke();        
    }

    #region EDITOR
    public virtual void Reset()
    {
        isDead = false;
        transform.position = enemyMover.StartNode.transform.position;
        transform.rotation = enemyMover.StartRotation;
    }

    [SerializeField] private EnemyIdentifier identifier = EnemyIdentifier.Setry;
    public EnemyIdentifier Identifier => identifier;
    #endregion
}
