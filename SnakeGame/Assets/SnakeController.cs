using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SnakeController — attach to an empty GameObject named "SnakeController".
/// </summary>
public class SnakeController : MonoBehaviour
{
    [Header("Snake Settings")]
    public Color snakeColor = new Color(0.18f, 0.72f, 0.18f);
    public float moveInterval = 0.15f;

    [Header("Grid Settings")]
    public int gridSize = 20;

    [Header("References")]
    public GameManager gameManager;

    // ── Private state ─────────────────────────────────────────────────────────
    private List<Transform> segments = new List<Transform>();
    private Vector2Int direction     = Vector2Int.right;
    private Vector2Int nextDirection = Vector2Int.right;
    private float moveTimer = 0f;
    private bool running    = false;

    private GameObject segmentPrefab;

    // ── Unity lifecycle ───────────────────────────────────────────────────────
    private void Awake()
    {
        BuildSegmentPrefab();
    }

    private void Update()
    {
        if (!running) return;

        GatherInput();

        moveTimer += Time.deltaTime;
        if (moveTimer >= moveInterval)
        {
            moveTimer = 0f;
            MoveSnake();
        }
    }

    // ── Public API ────────────────────────────────────────────────────────────
    public void StartSnake()
    {
        ResetSnake();
        running = true;
    }

    public void StopSnake()
    {
        running = false;
    }

    // ── Input ─────────────────────────────────────────────────────────────────
    private void GatherInput()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            && direction != Vector2Int.down)
            nextDirection = Vector2Int.up;

        else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            && direction != Vector2Int.up)
            nextDirection = Vector2Int.down;

        else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            && direction != Vector2Int.right)
            nextDirection = Vector2Int.left;

        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            && direction != Vector2Int.left)
            nextDirection = Vector2Int.right;
    }

    // ── Movement ──────────────────────────────────────────────────────────────
    private void MoveSnake()
    {
        direction = nextDirection;

        Vector2Int newHead = GridPosition(segments[0].position) + direction;

        // Wall collision
        if (newHead.x < 0 || newHead.x >= gridSize ||
            newHead.y < 0 || newHead.y >= gridSize)
        {
            running = false;
            gameManager.GameOver();
            return;
        }

        // Self collision (ignore the tail tip — it moves away)
        for (int i = 0; i < segments.Count - 1; i++)
        {
            if (GridPosition(segments[i].position) == newHead)
            {
                running = false;
                gameManager.GameOver();
                return;
            }
        }

        // Move: shift tail segment to new head
        Transform tail = segments[segments.Count - 1];
        segments.RemoveAt(segments.Count - 1);
        tail.position = GridToWorld(newHead);
        segments.Insert(0, tail);

        // Food check
        if (newHead == gameManager.FoodGridPosition)
        {
            GrowSnake();
            gameManager.FoodEaten();
        }
    }

    private void GrowSnake()
    {
        Transform last = segments[segments.Count - 1];
        GameObject newSeg = Instantiate(segmentPrefab, last.position, Quaternion.identity, transform);
        newSeg.SetActive(true);
        segments.Add(newSeg.transform);
    }

    // ── Reset ─────────────────────────────────────────────────────────────────
    public void ResetSnake()
    {
        // Destroy old segments
        foreach (Transform seg in segments)
            if (seg != null) Destroy(seg.gameObject);
        segments.Clear();

        direction     = Vector2Int.right;
        nextDirection = Vector2Int.right;
        moveTimer     = 0f;

        // Spawn 3 segments at centre
        Vector2Int start = new Vector2Int(gridSize / 2, gridSize / 2);
        for (int i = 0; i < 3; i++)
        {
            Vector2Int pos = start - new Vector2Int(i, 0);
            GameObject seg = Instantiate(segmentPrefab, GridToWorld(pos), Quaternion.identity, transform);
            seg.SetActive(true);
            segments.Add(seg.transform);
        }
    }

    // ── Coordinate helpers ────────────────────────────────────────────────────
    public HashSet<Vector2Int> OccupiedCells()
    {
        var cells = new HashSet<Vector2Int>();
        foreach (Transform seg in segments)
            cells.Add(GridPosition(seg.position));
        return cells;
    }

    public Vector2Int GridPosition(Vector3 worldPos)
        => new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));

    public Vector3 GridToWorld(Vector2Int cell)
        => new Vector3(cell.x, cell.y, 0f);

    // ── Segment prefab ────────────────────────────────────────────────────────
    private void BuildSegmentPrefab()
    {
        segmentPrefab = new GameObject("SnakeSegment");
        segmentPrefab.SetActive(false);

        SpriteRenderer sr = segmentPrefab.AddComponent<SpriteRenderer>();
        sr.sprite       = CreateSquareSprite();
        sr.color        = snakeColor;
        sr.sortingOrder = 2;

        segmentPrefab.transform.localScale = Vector3.one * 0.9f;

        // Keep it hidden and alive as a prefab source
        DontDestroyOnLoad(segmentPrefab);
    }

    private Sprite CreateSquareSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}
