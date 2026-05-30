using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab;       
    public Color pathColor = new Color(0.6f, 0.4f, 0.2f);
    public Sprite pathSprite;
    public Color freeColor = new Color(0.3f, 0.7f, 0.3f);
    public Color occupiedColor = new Color(0.5f, 0.5f, 0.5f);

    private GridCell[,] cells;
    private int width, height;
    private float cellSize;
    private Vector3 origin;

    public void InitializeGrid(int w, int h, float size)
    {
        ClearGrid();
        width = w;
        height = h;
        cellSize = size;

        origin = new Vector3(-w * size / 2f, -h * size / 2f, 0);
        cells = new GridCell[w, h];

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                Vector3 pos = origin + new Vector3(x * size + size / 2f, y * size + size / 2f, 0);
                GameObject go = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                go.name = $"Cell_{x}_{y}";
                go.transform.localScale = Vector3.one * size * 1.5f;

                GridCell cell = go.GetComponent<GridCell>();
                if (cell == null) cell = go.AddComponent<GridCell>();
                cell.Init(x, y, this);
                cells[x, y] = cell;
            }
        }

        MarkPathCells();
    }

    void MarkPathCells()
    {
        if (GameManager.Instance.waypointPath == null) return;
        var waypoints = GameManager.Instance.waypointPath.waypoints;

        foreach (var wp in waypoints)
        {
            Vector2Int gridPos = WorldToGrid(wp.position);
            if (IsInBounds(gridPos.x, gridPos.y))
            {
                cells[gridPos.x, gridPos.y].SetAsPath();
            }
        }

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            MarkCellsBetween(waypoints[i].position, waypoints[i + 1].position);
        }
    }

    void MarkCellsBetween(Vector3 from, Vector3 to)
    {
        float dist = Vector3.Distance(from, to);
        int steps = Mathf.CeilToInt(dist / (cellSize * 0.5f));
        for (int s = 0; s <= steps; s++)
        {
            Vector3 point = Vector3.Lerp(from, to, (float)s / steps);
            Vector2Int gp = WorldToGrid(point);
            if (IsInBounds(gp.x, gp.y))
                cells[gp.x, gp.y].SetAsPath();
        }
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - origin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(int x, int y)
    {
        return origin + new Vector3(x * cellSize + cellSize / 2f, y * cellSize + cellSize / 2f, 0);
    }

    public bool CanPlaceTower(int x, int y)
    {
        if (!IsInBounds(x, y)) return false;
        return cells[x, y].cellType == CellType.Free;
    }

    public void PlaceTower(int x, int y, GameObject tower)
    {
        cells[x, y].SetOccupied(tower);
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public GridCell GetCell(int x, int y)
    {
        if (!IsInBounds(x, y)) return null;
        return cells[x, y];
    }

    public void ClearTowers()
    {
        if (cells == null) return;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (cells[x, y].cellType == CellType.Occupied)
                    cells[x, y].ClearTower();
    }

    void ClearGrid()
    {
        if (cells == null) return;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (cells[x, y] != null)
                    Destroy(cells[x, y].gameObject);
        cells = null;
    }
}
