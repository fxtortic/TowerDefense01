using UnityEngine;

/// <summary>
/// </summary>
public class CameraSetup : MonoBehaviour
{
    public float padding = 1f;

    void Start()
    {
        AdjustCamera();
    }

    public void AdjustCamera()
    {
        var config = GameManager.Instance?.config;
        if (config == null) return;

        float gridW = config.gridWidth * config.cellSize;
        float gridH = config.gridHeight * config.cellSize;

        Camera cam = Camera.main;
        float screenRatio = (float)Screen.width / Screen.height;
        float targetRatio = gridW / gridH;

        if (screenRatio >= targetRatio)
        {
            cam.orthographicSize = (gridH / 2f) + padding;
        }
        else
        {
            float diff = targetRatio / screenRatio;
            cam.orthographicSize = ((gridH / 2f) + padding) * diff;
        }

        cam.transform.position = new Vector3(0, 0, -10);
    }
}
