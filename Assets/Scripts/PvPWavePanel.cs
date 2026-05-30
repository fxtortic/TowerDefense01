using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PvPWavePanel : MonoBehaviour
{
    public static PvPWavePanel Instance { get; private set; }

    [Header("Посилання на EnemyData")]
    public EnemyData goblinData;
    public EnemyData orcData;
    public EnemyData ghostData;

    [Header("UI елементи")]
    public TextMeshProUGUI budgetText;
    public TextMeshProUGUI wavePreviewText;
    public Button addGoblinButton;
    public Button addOrcButton;
    public Button addGhostButton;
    public Button clearWaveButton;

    private List<EnemyData> currentWave = new List<EnemyData>();
    private int spentBudget;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        currentWave.Clear();
        spentBudget = 0;
        UpdateUI();

        addGoblinButton?.onClick.AddListener(() => AddEnemy(goblinData));
        addOrcButton?.onClick.AddListener(() => AddEnemy(orcData));
        addGhostButton?.onClick.AddListener(() => AddEnemy(ghostData));
        clearWaveButton?.onClick.AddListener(ClearWave);
    }

    void OnDisable()
    {
        addGoblinButton?.onClick.RemoveAllListeners();
        addOrcButton?.onClick.RemoveAllListeners();
        addGhostButton?.onClick.RemoveAllListeners();
        clearWaveButton?.onClick.RemoveAllListeners();
    }

    void AddEnemy(EnemyData data)
    {
        if (data == null) return;

        int budget = GameManager.Instance.AttackBudget;
        int maxEnemies = GameManager.Instance.config.maxEnemiesPerWave;

        if (spentBudget + data.cost > budget)
        {
            Debug.Log("Недостатньо бюджету!");
            return;
        }
        if (currentWave.Count >= maxEnemies)
        {
            Debug.Log("Максимум ворогів у хвилі!");
            return;
        }

        currentWave.Add(data);
        spentBudget += data.cost;
        UpdateUI();
    }

    void ClearWave()
    {
        currentWave.Clear();
        spentBudget = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        int budget = GameManager.Instance != null ? GameManager.Instance.AttackBudget : 0;

        if (budgetText != null)
            budgetText.text = $"Бюджет: {spentBudget}/{budget}";

        if (wavePreviewText != null)
        {
            int goblins = 0, orcs = 0, ghosts = 0;
            foreach (var e in currentWave)
            {
                if (e == goblinData) goblins++;
                else if (e == orcData) orcs++;
                else if (e == ghostData) ghosts++;
            }
            wavePreviewText.text = $"Гобліни: {goblins}  Орки: {orcs}  Привиди: {ghosts}\nВсього: {currentWave.Count}";
        }
    }

    public List<EnemyData> GetSelectedWave()
    {
        if (currentWave.Count == 0)
            return GameManager.Instance.aiAttacker.GenerateWave(
                GameManager.Instance.AttackBudget,
                GameManager.Instance.CurrentRound);

        return new List<EnemyData>(currentWave);
    }
}
