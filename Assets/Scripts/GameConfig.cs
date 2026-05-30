using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "TowerDefense/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Сітка")]
    public int gridWidth = 12;
    public int gridHeight = 8;
    public float cellSize = 1f;

    [Header("Захисник")]
    public int startingGold = 300;
    public int baseHP = 20;
    public int goldPerKill = 10;
    public int goldPerRound = 50;

    [Header("Атакуючий")]
    public int startingAttackBudget = 200;
    public int budgetIncreasePerRound = 50;

    [Header("Раунди")]
    public int totalRounds = 10;
    public float spawnInterval = 1.0f;
    public int maxEnemiesPerWave = 50;

    [Header("Час підготовки")]
    public float preparationTime = 30f;
}
