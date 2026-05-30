using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    public static TowerPlacement Instance { get; private set; }

    [Header("Префаб вежі")]
    public GameObject towerPrefab;

    [Header("Типи веж (ScriptableObjects)")]
    public TowerData[] availableTowers;

    private GridCell selectedCell;
    private bool isActive;

    void Awake()
    {
        Instance = this;
    }

    public void SetActive(bool active)
    {
        isActive = active;
        if (!active)
        {
            selectedCell = null;
            UIManager.Instance?.HideTowerMenu();
        }
    }

    /// <summary>
    /// </summary>
    public void OnCellClicked(GridCell cell)
    {
        if (!isActive) return;

        selectedCell = cell;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(cell.transform.position);
        UIManager.Instance?.ShowTowerMenu(screenPos, availableTowers);
    }

    /// <summary>
    /// </summary>
    public void PlaceTower(int towerIndex)
    {
        if (selectedCell == null || towerIndex < 0 || towerIndex >= availableTowers.Length)
            return;

        TowerData towerData = availableTowers[towerIndex];

        if (!GameManager.Instance.SpendGold(towerData.price))
        {
            Debug.Log("Недостатньо золота!");
            return;
        }

        Vector3 pos = selectedCell.transform.position;
        GameObject towerGo = Instantiate(towerPrefab, pos, Quaternion.identity);
        Tower tower = towerGo.GetComponent<Tower>();
        tower.Initialize(towerData);
        AudioManager.Instance?.Play(AudioManager.Instance.placeTowerSound);
        GameManager.Instance.gridManager.PlaceTower(selectedCell.gridX, selectedCell.gridY, towerGo);

        selectedCell = null;
        UIManager.Instance?.HideTowerMenu();
    }
}
