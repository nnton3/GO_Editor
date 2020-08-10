using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public enum Turn { Player, Enemy}
public class NodeEvent : UnityEvent<Board_Node> { }

public class GameManager : MonoBehaviour
{
    #region Variables
    protected Board board;
    protected PlayerManager player;
    protected GameTarget gameTarget;

    private List<EnemyManager> enemies;
    public List<EnemyManager> Enemies { get => enemies; set => enemies = value; }
    
    private Turn currentTurn = Turn.Player;
    public Turn CurrentTurn => currentTurn;

    private bool hasLevelStarted = false;
    public bool HasLevelStarted => hasLevelStarted;
    protected bool isGamePlaying = false;
    public bool IsGamePlaying => isGamePlaying;
    protected bool isGameOver = false;
    public bool IsGameOver => isGameOver;
    private bool hasLevelFinishing = false;
    public bool HasLevelFinishing => hasLevelFinishing;

    [SerializeField] protected float delay = 1f;

    [HideInInspector] public static UnityEvent LoseLevelEvent;
    [HideInInspector] public UnityEvent StartLevelEvent;
    [HideInInspector] public UnityEvent PlayLevelEvent;
    [HideInInspector] public UnityEvent EndLevelEvent;
    [HideInInspector] public UnityEvent BoardUpdatedEvent;
    [HideInInspector] public static NodeEvent RaiseAlarmEvent = new NodeEvent();
    #endregion

    private void Awake()
    {
        RaiseAlarmEvent.AddListener(RaiseAlarm);
        BoardUpdatedEvent.AddListener(UpdateEnemiesPath);
    }

    public virtual void Initialize()
    {
        board = FindObjectOfType<Board>();
        player = FindObjectOfType<PlayerManager>();
        gameTarget = GetComponent<GameTarget>();

        EnemyManager[] enemiesArray = FindObjectsOfType<EnemyManager>() as EnemyManager[];
        enemies = enemiesArray.ToList();
    }

    public void StartLoop()
    {
        if (player != null && board != null)
            StartCoroutine("RunGameLoop");
        else Debug.LogError("GAMEMANAGER Error");
    }

    private void UpdateEnemiesPath()
    {
        foreach (var enemy in enemies)
            if (!enemy.IsDead)
                enemy.GetComponent<EnemyMover>().UpdatePathToTarget();
    }

    private void RaiseAlarm(Board_Node alarmSource)
    {
        Debug.Log("ALARM");
        foreach (var enemy in enemies)
            if (!enemy.IsDead)
                enemy.GetComponent<EnemyMover>().ToAlarmState(alarmSource);
    }

    private IEnumerator RunGameLoop()
    {
        yield return StartCoroutine("StartLevelRoutine");
        yield return StartCoroutine("PlayLevelRoutine");
        yield return StartCoroutine("EndLevelRoutine");
    }

    private IEnumerator StartLevelRoutine()
    {
        player.PlayerInput.InputEnabled = false;

        while (!hasLevelStarted)
            yield return null;

        StartLevelEvent?.Invoke();
    }

    protected virtual IEnumerator PlayLevelRoutine()
    {
        isGamePlaying = true;
        yield return new WaitForSeconds(delay);
        player.PlayerInput.InputEnabled = true;
        board.DrawGoal();
        board.InitBoard();
        PlayLevelEvent?.Invoke();

        while (!IsGameOver)
        {
            yield return null;
            if (gameTarget != null)
                isGameOver = gameTarget.TargetComplete();
        }
        Debug.Log("WIN!");
    }

    public void LoseLevel()
    {
        StartCoroutine(LoseLevelRoutine());
    }

    private IEnumerator LoseLevelRoutine()
    {
        isGameOver = true;
        yield return new WaitForSeconds(3f);
        LoseLevelEvent?.Invoke();
        yield return new WaitForSeconds(2f);

        Debug.Log("LOOSE");
    }

    private IEnumerator EndLevelRoutine()
    {
        Debug.Log("End level");
        player.PlayerInput.InputEnabled = false;

        EndLevelEvent?.Invoke();

        while (!hasLevelFinishing)
        {
            yield return null;
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayLevel()
    {
        hasLevelStarted = true;
    }

    private void PlayPlayerTurn()
    {
        currentTurn = Turn.Player;
        player.IsTurnComplete = false;
        player.PlayerInput.InputEnabled = true;
    }

    private void PlayEnemiesTurn()
    {
        currentTurn = Turn.Enemy;

        foreach (var enemy in enemies)
        {
            if (enemy == null) return;
            enemy.IsTurnComplete = false;
            enemy.PlayTurn();
        }
    }

    private bool IsEnemyTurnCpmplete()
    {
        foreach (var enemy in enemies)
            if (!enemy.IsTurnComplete) return false;

        return true;
    }

    public void UpdateTurn()
    {
        if (currentTurn == Turn.Player && player != null)
        {
            if (player.IsTurnComplete)
                PlayEnemiesTurn();
        }
        else if (currentTurn == Turn.Enemy)
        {
            if (IsEnemyTurnCpmplete())
                PlayPlayerTurn();
        }
    }

    public void Reset()
    {
        enemies.Clear();
        StopAllCoroutines();
        hasLevelStarted = false;
        isGamePlaying = false;
        isGameOver = false;
        hasLevelFinishing = false;
        player.PlayerInput.InputEnabled = false;
        currentTurn = Turn.Player;
    }
}
