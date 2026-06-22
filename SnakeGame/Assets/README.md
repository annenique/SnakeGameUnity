# Snake Game — Unity Setup Guide

## Files included
| File | Purpose |
|---|---|
| `GridSetup.cs` | Draws the background, grid lines, and positions the camera |
| `SnakeController.cs` | Controls the snake (movement, growth, collision) |
| `GameManager.cs` | Manages food placement, game-over, and restarting |

---

## Step-by-step setup

### 1. Create a new Unity project
- Open Unity Hub → **New Project** → **2D (Core)** template.
- Name it `SnakeGame` (or anything you like).

### 2. Import the scripts
- Drag all three `.cs` files into your **Assets** folder in the Project window.

### 3. Create the scene GameObjects

Create three empty GameObjects (**right-click in the Hierarchy → Create Empty**):

| GameObject name | Script to attach |
|---|---|
| `GridSetup` | `GridSetup.cs` |
| `SnakeController` | `SnakeController.cs` |
| `GameManager` | `GameManager.cs` |

### 4. Wire up references

Select **SnakeController** in the Hierarchy:
- Drag the **GameManager** GameObject into the *Game Manager* field in the Inspector.

Select **GameManager** in the Hierarchy:
- Drag the **SnakeController** GameObject into the *Snake Controller* field in the Inspector.

*(GridSetup has no references to set.)*

### 5. Verify grid sizes match
All three scripts have a `gridSize` field — make sure they are all set to **20**.

### 6. Camera
The `GridSetup` script automatically configures the **Main Camera** in `Awake()`.
Make sure your scene has a camera tagged **MainCamera** (it does by default in new projects).

### 7. Press Play
- **Arrow keys or WASD** — steer the snake.
- Eat the **red food** square to grow.
- Hit a wall or yourself → Game Over.
- Press **Space** to restart after a game over.

---

## Customisation cheat-sheet

| What | Where | Field |
|---|---|---|
| Snake colour | `SnakeController` Inspector | *Snake Color* |
| Food colour | `GameManager` Inspector | *Food Color* |
| Move speed | `SnakeController` Inspector | *Move Interval* (seconds per tick) |
| Board size | All three scripts | *Grid Size* |
| Background colour | `GridSetup` Inspector | *Background Color* |
| Grid line colour | `GridSetup` Inspector | *Grid Line Color* |

---

## Extending the game (ideas)
- **Score display** — add a `TMP_Text` field to `GameManager` and increment a counter in `FoodEaten()`.
- **Speed scaling** — in `SnakeController.MoveSnake()`, decrease `moveInterval` each time food is eaten.
- **Sound effects** — call `AudioSource.PlayClipAtPoint()` inside `FoodEaten()` and `GameOver()`.
- **High score** — use `PlayerPrefs.SetInt("HighScore", score)` in `GameManager.GameOver()`.
