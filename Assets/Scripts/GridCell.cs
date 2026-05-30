using UnityEngine;

public enum CellType { Free, Path, Occupied }

public class GridCell : MonoBehaviour
{
    public CellType cellType = CellType.Free;
    public int gridX, gridY;
    public GameObject placedTower;

    private SpriteRenderer sr;
    private GridManager gridManager;

    public void Init(int x, int y, GridManager manager)
    {
        gridX = x;
        gridY = y;
        gridManager = manager;
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 0;
        UpdateVisual();
    }

    public void SetAsPath()
{
    cellType = CellType.Path;
    if (gridManager != null)
    {
        GridManager gm = gridManager;
        if (gm.pathSprite != null && sr != null)
        {
            sr.sprite = gm.pathSprite;
            sr.color = Color.white;
            return;
        }
    }
    UpdateVisual();
}
    public void SetOccupied(GameObject tower)
    {
        cellType = CellType.Occupied;
        placedTower = tower;
        UpdateVisual();
    }

    public void ClearTower()
    {
        if (placedTower != null)
            Destroy(placedTower);
        placedTower = null;
        cellType = CellType.Free;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (sr == null) return;
        switch (cellType)
        {
            case CellType.Free:
                sr.color = gridManager != null ? gridManager.freeColor : Color.green;
                break;
            case CellType.Path:
                sr.color = gridManager != null ? gridManager.pathColor : Color.yellow;
                break;
            case CellType.Occupied:
                sr.color = gridManager != null ? gridManager.occupiedColor : Color.gray;
                break;
        }
    }

    void OnMouseDown()
    {
        if (GameManager.Instance.CurrentState != GameState.Preparation) return;
        if (cellType != CellType.Free) return;

        TowerPlacement.Instance?.OnCellClicked(this);
    }
}
