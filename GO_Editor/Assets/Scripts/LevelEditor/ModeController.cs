using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum Mode { Editor, Test}

public class ModeController : MonoBehaviour
{
    #region Variables
    public Mode CurrentMode = Mode.Editor;

    private SavingSystem savingSystem;
    private EditorInitializer initializer;
    private GameManager gameManager;
    private PlayerManager player;
    private Board board;
    private LevelInitializer levelInitializer;
    #endregion

    public void Initialize()
    {
        savingSystem = GetComponent<SavingSystem>();
        levelInitializer = GetComponent<LevelInitializer>();
        initializer = FindObjectOfType<EditorInitializer>();
        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<PlayerManager>();
        board = FindObjectOfType<Board>();
        if (initializer == null) Debug.Log("Editor initializer is lost");
    }

    public void EnableTestMode()
    {
        savingSystem.Save();
        CurrentMode = Mode.Test;
        initializer.InitializeWPManager();
        initializer.InitializeGameManager();
        initializer.InitializeGameTarget();
        initializer.InitializeEnemies();
        board.UpdatePlayerNode();
        player.GetComponent<PlayerMover>().Reset();
        initializer.StartGame();
        gameManager.PlayLevel();
    }

    public void EnableEditorMode()
    {
        CurrentMode = Mode.Editor;

        gameManager.Reset();

        savingSystem.StartLoad();
    }

    private void ResetEnemies()
    {
        foreach (var enemy in gameManager.Enemies)
            enemy.Reset();
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
