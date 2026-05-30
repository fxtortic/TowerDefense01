using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum GameState { Menu, Preparation, Battle, RoundEnd, GameOver }
public enum GameMode { PvE, PvP }
public enum GameResult { DefenderWin, AttackerWin }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Конфігурація")]
    public GameConfig config;

    [Header("Посилання")]
    public GridManager gridManager;
    public WaypointPath waypointPath;
    public EnemySpawner enemySpawner;
    public TowerPlacement towerPlacement;
    public UIManager uiManager;
    public AIAttacker aiAttacker;

    public GameState CurrentState { get; private set; } = GameState.Menu;
    public GameMode CurrentMode { get; private set; } = GameMode.PvE;
    public int CurrentRound { get; private set; } = 0;
    public int Gold { get; private set; }
    public int BaseHP { get; private set; }
    public int AttackBudget { get; private set; }
    public float PreparationTimer { get; private set; }

    public event Action<GameState> OnStateChanged;
    public event Action<int> OnGoldChanged;
    public event Action<int> OnBaseHPChanged;
    public event Action<int> OnRoundChanged;

    private int enemiesAliveCount;
    private int enemiesToSpawn;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartGame(GameMode mode)
    {
        CurrentMode = mode;
        CurrentRound = 0;
        Gold = config.startingGold;
        BaseHP = config.baseHP;
        AttackBudget = config.startingAttackBudget;

        OnGoldChanged?.Invoke(Gold);
        OnBaseHPChanged?.Invoke(BaseHP);

        gridManager.InitializeGrid(config.gridWidth, config.gridHeight, config.cellSize);
        ChangeState(GameState.Preparation);
        StartNextRound();
    }

    void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
        Debug.Log($"[GameState] -> {newState}");
    }

    void StartNextRound()
    {
        CurrentRound++;
        OnRoundChanged?.Invoke(CurrentRound);

        if (CurrentRound > 1)
        {
            Gold += config.goldPerRound;
            AttackBudget += config.budgetIncreasePerRound;
            OnGoldChanged?.Invoke(Gold);
        }

        PreparationTimer = config.preparationTime;
        ChangeState(GameState.Preparation);
        towerPlacement.SetActive(true);
    }

    void Update()
    {
        if (CurrentState == GameState.Preparation)
        {
            PreparationTimer -= Time.deltaTime;
            if (PreparationTimer <= 0f)
            {
                StartBattle();
            }
        }
    }

    public void StartBattle()
    {
        towerPlacement.SetActive(false);
        ChangeState(GameState.Battle);

        List<EnemyData> wave;
        if (CurrentMode == GameMode.PvE)
        {
            wave = aiAttacker.GenerateWave(AttackBudget, CurrentRound);
        }
        else
        {
            wave = PvPWavePanel.Instance != null
                ? PvPWavePanel.Instance.GetSelectedWave()
                : aiAttacker.GenerateWave(AttackBudget, CurrentRound);
        }

        enemiesToSpawn = wave.Count;
        enemiesAliveCount = wave.Count;
        enemySpawner.SpawnWave(wave, config.spawnInterval);
    }

    public void ForceStartBattle()
    {
        if (CurrentState == GameState.Preparation)
            StartBattle();
    }

    public void OnEnemyKilled(EnemyData data)
    {
        Gold += config.goldPerKill;
        OnGoldChanged?.Invoke(Gold);
        EnemyDefeated();
    }

    public void OnEnemyReachedBase(EnemyData data)
    {
        BaseHP -= 1;
        OnBaseHPChanged?.Invoke(BaseHP);

        if (BaseHP <= 0)
        {
            ChangeState(GameState.GameOver);
            uiManager.ShowGameOver(GameResult.AttackerWin);
            return;
        }

        EnemyDefeated();
    }

    void EnemyDefeated()
    {
        enemiesAliveCount--;
        if (enemiesAliveCount <= 0 && CurrentState == GameState.Battle)
        {
            ChangeState(GameState.RoundEnd);
            StartCoroutine(RoundEndSequence());
        }
    }

    IEnumerator RoundEndSequence()
    {
        yield return new WaitForSeconds(1.5f);

        if (CurrentRound >= config.totalRounds)
        {
            ChangeState(GameState.GameOver);
            uiManager.ShowGameOver(GameResult.DefenderWin);
        }
        else
        {
            StartNextRound();
        }
    }

    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            OnGoldChanged?.Invoke(Gold);
            return true;
        }
        return false;
    }

    public void StartPvE() { StartGame(GameMode.PvE); }
    public void StartPvP() { StartGame(GameMode.PvP); }

    public void RestartGame()
    {
        StopAllCoroutines();
        enemySpawner.ClearAll();
        gridManager.ClearTowers();
        ChangeState(GameState.Menu);
        uiManager.ShowMainMenu();
    }
}