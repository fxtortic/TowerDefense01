using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Панелі")]
    public GameObject mainMenuPanel;
    public GameObject hudPanel;
    public GameObject towerMenuPanel;
    public GameObject gameOverPanel;
    public GameObject pvpWavePanel;

    [Header("HUD елементи")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI stateText;
    public Button startBattleButton;

    [Header("Меню веж")]
    public Transform towerButtonsParent;
    public GameObject towerButtonPrefab;

    [Header("Кінець гри")]
    public TextMeshProUGUI gameOverText;
    public Button restartButton;

    [Header("Головне меню")]
    public Button pveModeButton;
    public Button pvpModeButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("GameManager.Instance is null!");
            return;
        }

        gm.OnGoldChanged += UpdateGold;
        gm.OnBaseHPChanged += UpdateHP;
        gm.OnRoundChanged += UpdateRound;
        gm.OnStateChanged += UpdateState;

        if (pveModeButton != null)
            pveModeButton.onClick.AddListener(() => gm.StartGame(GameMode.PvE));
        if (pvpModeButton != null)
            pvpModeButton.onClick.AddListener(() => gm.StartGame(GameMode.PvP));
        if (startBattleButton != null)
            startBattleButton.onClick.AddListener(() => gm.ForceStartBattle());
        if (restartButton != null)
            restartButton.onClick.AddListener(() => gm.RestartGame());

        ShowMainMenu();
    }

    void Update()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.CurrentState == GameState.Preparation &&
            timerText != null)
        {
            timerText.text = "Час: " + Mathf.CeilToInt(GameManager.Instance.PreparationTimer) + "с";
        }
    }

    public void ShowMainMenu()
    {
        SetPanel(mainMenuPanel, true);
        SetPanel(hudPanel, false);
        SetPanel(gameOverPanel, false);
        SetPanel(towerMenuPanel, false);
        SetPanel(pvpWavePanel, false);
    }

    void UpdateGold(int gold)
    {
        if (goldText != null) goldText.text = "Золото: " + gold;
    }

    void UpdateHP(int hp)
    {
        if (hpText != null) hpText.text = "База HP: " + hp;
    }

    void UpdateRound(int round)
    {
        if (roundText != null && GameManager.Instance != null)
            roundText.text = "Раунд: " + round + "/" + GameManager.Instance.config.totalRounds;
    }

    void UpdateState(GameState state)
    {
        switch (state)
        {
            case GameState.Preparation:
                SetPanel(mainMenuPanel, false);
                SetPanel(hudPanel, true);
                SetPanel(gameOverPanel, false);
                SetPanel(towerMenuPanel, false);
                if (startBattleButton != null)
                    startBattleButton.gameObject.SetActive(true);
                if (stateText != null) stateText.text = "ПІДГОТОВКА";

                if (GameManager.Instance != null &&
                    GameManager.Instance.CurrentMode == GameMode.PvP)
                    SetPanel(pvpWavePanel, true);
                break;

            case GameState.Battle:
                if (startBattleButton != null)
                    startBattleButton.gameObject.SetActive(false);
                if (stateText != null) stateText.text = "БІЙ!";
                SetPanel(pvpWavePanel, false);
                if (timerText != null) timerText.text = "";
                break;

            case GameState.RoundEnd:
                if (stateText != null) stateText.text = "КІНЕЦЬ РАУНДУ";
                break;
        }
    }

    public void ShowTowerMenu(Vector3 screenPos, TowerData[] towers)
    {
        if (towerMenuPanel == null) return;

        towerMenuPanel.SetActive(true);
        towerMenuPanel.transform.position = screenPos + Vector3.up * 60f;

        if (towerButtonsParent != null)
        {
            foreach (Transform child in towerButtonsParent)
                Destroy(child.gameObject);
        }

        if (towerButtonPrefab == null || towerButtonsParent == null) return;

        for (int i = 0; i < towers.Length; i++)
        {
            int index = i;
            TowerData td = towers[i];

            GameObject btn = Instantiate(towerButtonPrefab, towerButtonsParent);
            TextMeshProUGUI txt = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = td.towerName + "\n" + td.price + "g";

            Button button = btn.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => TowerPlacement.Instance.PlaceTower(index));

                if (GameManager.Instance != null && GameManager.Instance.Gold < td.price)
                {
                    var colors = button.colors;
                    colors.normalColor = Color.gray;
                    button.colors = colors;
                }
            }
        }
    }

    public void HideTowerMenu()
    {
        SetPanel(towerMenuPanel, false);
    }

    public void ShowGameOver(GameResult result)
{
    SetPanel(gameOverPanel, true);
    SetPanel(hudPanel, false);
    SetPanel(towerMenuPanel, false);

    if (result == GameResult.DefenderWin)
        AudioManager.Instance?.Play(AudioManager.Instance.winSound);
    else
        AudioManager.Instance?.Play(AudioManager.Instance.loseSound);

    if (gameOverText != null)
    {
        gameOverText.text = result == GameResult.DefenderWin
            ? "ПЕРЕМОГА ЗАХИСНИКА!\nБаза вистояла!"
            : "ПЕРЕМОГА АТАКУЮЧОГО!\nБазу знищено!";
    }
}

    void SetPanel(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }
}