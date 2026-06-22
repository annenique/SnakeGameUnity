using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameManager — attach to an empty GameObject named "GameManager".
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Food Settings")]
    public Color foodColor = new Color(0.92f, 0.25f, 0.20f);

    [Header("Grid Settings")]
    public int gridSize = 20;

    [Header("References")]
    public SnakeController snakeController;

    // ── Private state ─────────────────────────────────────────────────────────
    private GameObject foodObject;
    private Vector2Int foodGridPosition;
    private bool isGameOver = false;

    public Vector2Int FoodGridPosition => foodGridPosition;

    // ── Unity lifecycle ───────────────────────────────────────────────────────
    private void Start()
    {
        BuildFoodObject();
        StartGame();
    }

    private void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
            StartGame();
    }

    // ── Game flow ─────────────────────────────────────────────────────────────
    private void StartGame()
    {
        isGameOver = false;
        snakeController.StartSnake();   // resets the snake, then enables movement
        PlaceFood();
    }

    public void GameOver()
    {
        isGameOver = true;
        snakeController.StopSnake();
        Debug.Log("Game Over! Press SPACE to restart.");
    }

    public void FoodEaten()
    {
        PlaceFood();
    }

    // ── Food ──────────────────────────────────────────────────────────────────
    private void PlaceFood()
    {
        HashSet<Vector2Int> occupied = snakeController.OccupiedCells();

        List<Vector2Int> free = new List<Vector2Int>();
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                if (!occupied.Contains(cell))
                    free.Add(cell);
            }

        if (free.Count == 0)
        {
            Debug.Log("You win! The board is full.");
            return;
        }

        foodGridPosition = free[Random.Range(0, free.Count)];
        foodObject.transform.position = snakeController.GridToWorld(foodGridPosition);
        foodObject.SetActive(true);
    }

    // ── Food object ───────────────────────────────────────────────────────────
    private void BuildFoodObject()
    {
        foodObject = new GameObject("Food");

        SpriteRenderer sr = foodObject.AddComponent<SpriteRenderer>();
        sr.sprite       = CreateSquareSprite();
        sr.color        = foodColor;
        sr.sortingOrder = 1;

        foodObject.transform.localScale = Vector3.one * 0.85f;
        foodObject.SetActive(false);
    }

    private Sprite CreateSquareSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}
