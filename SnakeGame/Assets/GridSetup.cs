using UnityEngine;

/// <summary>
/// GridSetup — attach this to an empty GameObject called "GridSetup".
/// Draws the dark background quad and centres the camera on the 20×20 grid.
/// Run this once in the scene; it fires in Awake so everything is ready before
/// SnakeController and GameManager Start().
/// </summary>
[DefaultExecutionOrder(-100)] // run before other scripts
public class GridSetup : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("Must match SnakeController.gridSize and GameManager.gridSize")]
    public int gridSize = 20;

    [Header("Colours")]
    public Color backgroundColor = new Color(0.08f, 0.08f, 0.10f); // near-black
    public Color gridLineColor   = new Color(0.18f, 0.18f, 0.22f); // subtle dark lines

    private void Awake()
    {
        SetupCamera();
        DrawBackground();
        DrawGridLines();
    }

    // ── Camera ────────────────────────────────────────────────────────────────
    private void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        cam.orthographic = true;
        cam.backgroundColor = backgroundColor;

        // Centre on the grid. Cells are 1 unit each, origin is at (0,0).
        // Grid spans x: 0..(gridSize-1), y: 0..(gridSize-1).
        float halfGrid = gridSize / 2f;
        cam.transform.position = new Vector3(halfGrid - 0.5f, halfGrid - 0.5f, -10f);

        // Add half a cell of padding on each side
        cam.orthographicSize = halfGrid + 0.5f;
    }

    // ── Background quad ───────────────────────────────────────────────────────
    private void DrawBackground()
    {
        GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Quad);
        bg.name = "Background";
        Destroy(bg.GetComponent<Collider>());

        // Size it to cover the grid plus half-cell border
        float size = gridSize + 1f;
        bg.transform.localScale  = new Vector3(size, size, 1f);
        bg.transform.position    = new Vector3(gridSize / 2f - 0.5f, gridSize / 2f - 0.5f, 1f);

        // Use the built-in Unlit/Color shader so no lighting is needed
        Renderer rend = bg.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Unlit/Color"));
        rend.material.color = backgroundColor;
    }

    // ── Grid lines ────────────────────────────────────────────────────────────
    private void DrawGridLines()
    {
        GameObject linesRoot = new GameObject("GridLines");

        for (int i = 0; i <= gridSize; i++)
        {
            // Vertical line
            CreateLine(linesRoot,
                new Vector3(i - 0.5f, -0.5f, 0f),
                new Vector3(i - 0.5f, gridSize - 0.5f, 0f));

            // Horizontal line
            CreateLine(linesRoot,
                new Vector3(-0.5f,         i - 0.5f, 0f),
                new Vector3(gridSize - 0.5f, i - 0.5f, 0f));
        }
    }

    private void CreateLine(GameObject parent, Vector3 start, Vector3 end)
    {
        GameObject go = new GameObject("Line");
        go.transform.parent = parent.transform;

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.positionCount  = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = lr.endWidth = 0.02f;
        lr.useWorldSpace = true;
        lr.sortingOrder  = 0; // behind snake and food

        Material mat = new Material(Shader.Find("Unlit/Color"));
        mat.color    = gridLineColor;
        lr.material  = mat;
    }
}
