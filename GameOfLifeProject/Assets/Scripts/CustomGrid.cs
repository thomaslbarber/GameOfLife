using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

/// <summary>
/// Class for managing the grid and updating between the status of each cell for each generation.
/// </summary>
public class CustomGrid : MonoBehaviour
{
    // The dimensions of the grid.
    public int SIZE_X;
    public int SIZE_Y;

    // Stores all the nodes in the grid (alongside a copy).
    public Cell[,] grid, gridCopy;

    // The tilemap.
    public Tilemap tilemap;
    
    // The tiles for alive/dead cells in the tilemap.
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile deadTile;

    // The delay between generations.
    private const float DELAY = 0.15f;

    // Reference to the unity grid the tilemap uses.
    private Grid gridUnity;

    // Tracks the cursor position in the world.
    CursorTracker cursorTracker = new CursorTracker();

    // Stores a world position in grid form.
    Vector3Int gridPosition;

    /// <summary>
    /// Enum for whether the simulation is currently active or inactive.
    /// </summary>
    enum SimulationStatus
    {
        Active,
        Inactive,
    }

    // The current state of the simulation.
    SimulationStatus sStatus = SimulationStatus.Inactive;

    /// <summary>
    /// Start() is called before the first frame. Initialises necessary variables and sets the initial grid to be randomised.
    /// </summary>
    private void Start()
    {
        gridUnity = GetComponentInParent<Grid>();
        tilemap = GetComponent<Tilemap>();

        CreateGrid();
        OutputGrid();
    }


    /// <summary>
    /// Update is called every frame.
    /// </summary>
    private void Update()
    {
        CheckForKeyboardInputs();

        if (sStatus == SimulationStatus.Active) { return; }

        CheckForMouseInput();
    }

    /// <summary>
    /// Starts/stops the game via the space bar if it is pressed.
    /// </summary>
    void CheckForKeyboardInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (sStatus == SimulationStatus.Active) { StopGame(); }
            else { StartGame(); }
        }
    }

    /// <summary>
    /// Checks for mouse input, adding a alive/dead cell at the mouse cursor position.
    /// </summary>
    void CheckForMouseInput()
    {
        // Place an alive cell at the cursor position.
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) { return; }

            gridPosition = ConvertToGridPosition(cursorTracker.GetMousePosition());
            grid[gridPosition.y, gridPosition.x].SetState(Cell.State.Alive);

            grid[gridPosition.y, gridPosition.x].SetState(Cell.State.Alive);
            OutputGrid();
        }

        // Place a dead cell at the cursor position.
        if (Input.GetMouseButton(1))
        {
            if (EventSystem.current.IsPointerOverGameObject()) { return; }

            gridPosition = ConvertToGridPosition(cursorTracker.GetMousePosition());
            grid[gridPosition.y, gridPosition.x].SetState(Cell.State.Dead);

            grid[gridPosition.y, gridPosition.x].SetState(Cell.State.Dead);
            OutputGrid();
        }
    }

    /// <summary>
    /// Converts a given Vector3 position into a position on the grid (as a Vector3Int).
    /// </summary>
    /// <param name="position">The position to convert.</param>
    /// <returns>The new Vector3Int position in the grid.</returns>
    Vector3Int ConvertToGridPosition(Vector3 position)
    {
        return gridUnity.WorldToCell(position);
    }

    /// <summary>
    /// Generates a grid of the given size consisting of cells.
    /// </summary>
    public void CreateGrid()
    {
        sStatus = SimulationStatus.Inactive;

        grid = new Cell[SIZE_X, SIZE_Y];
        gridCopy = new Cell[SIZE_X, SIZE_Y];

        for (int x = 0; x < SIZE_X; x++)
        {
            for (int y = 0; y < SIZE_Y; y++)
            {
                grid[x, y] = new Cell(RandomiseCellState(), x, y);
                gridCopy[x, y] = new Cell(grid[x, y].GetState(), x, y);
            }
        }
        OutputGrid();
    }

    /// <summary>
    /// Sets every cell in the grid to be of the same state (alive/dead).
    /// </summary>
    /// <param name="isAlive">Whether to generate the grid as alive cells or dead cells.</param>
    public void CreateIdenticalGrid(bool isAlive)
    {
        sStatus = SimulationStatus.Inactive;

        Cell.State state = isAlive ? Cell.State.Alive : Cell.State.Dead;

        for (int x = 0; x < SIZE_X; x++)
        {
            for (int y = 0; y < SIZE_Y; y++)
            {
                grid[x, y].SetState(state);
            }
        }

        OutputGrid();
    }

    /// <summary>
    /// Generates a random state for a cell.
    /// </summary>
    /// <returns>Returns the random state.</returns>
    Cell.State RandomiseCellState()
    {
        int n = Random.Range(0, 16);
        if (n == 0) { return Cell.State.Alive; }
        return Cell.State.Dead;
    }

    /// <summary>
    /// Starts the simulation.
    /// </summary>
    public void StartGame()
    {
        if (sStatus == SimulationStatus.Active) { return; }

        sStatus = SimulationStatus.Active;

        StartCoroutine(GenerateGridLoop());
    }

    /// <summary>
    /// Stops the simulation.
    /// </summary>
    public void StopGame()
    {
        sStatus = SimulationStatus.Inactive;
    }

    /// <summary>
    /// Returns a list of all neighbours of a given cell.
    /// </summary>
    /// <param name="cell">The cell to get the neighbours of.</param>
    /// <returns>Returns a list of all neighbours of a cell.</returns>
    public List<Cell> Neighbours(Cell cell)
    {
        List<Cell> neighbours = new List<Cell>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) { continue; }

                int checkX = cell.XPos + x;
                int checkY = cell.YPos + y;

                if (checkX >= 0 && checkX < SIZE_X && checkY >= 0 && checkY < SIZE_Y)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    /// <summary>
    /// Outputs the current grid to the console.
    /// </summary>
    public void OutputGrid()
    {
        for (int i = 0; i < SIZE_Y; i++)
        {
            for (int j = 0; j < SIZE_X; j++)
            {
                Vector3Int p = new Vector3Int(i, j, 0);
                if (grid[j, i].GetState() == Cell.State.Alive)
                {
                    tilemap.SetTile(p, aliveTile);
                }
                else
                {
                    tilemap.SetTile(p, deadTile);
                }
            }
        }
    }

    /// <summary>
    /// Updates the grid according to the rules of the game of life.
    /// </summary>
    public void UpdateGrid()
    {
        Cell[,] newGrid = new Cell[SIZE_X, SIZE_Y];

        for (int x = 0; x < SIZE_X; x++)
        {
            for (int y = 0; y < SIZE_Y; y++)
            {
                // Count the number of alive/dead neighbours of the current cell.
                List<Cell> neighbours = Neighbours(grid[x, y]);

                int neighboursAlive = 0;
                int neighboursDead = 0;
                foreach (Cell neighbour in neighbours)
                {
                    if (neighbour.GetState() == Cell.State.Alive)
                    {
                        neighboursAlive++;
                    }
                    else
                    {
                        neighboursDead++;
                    }
                }

                /* Rules:
                 * 1. Any live cell with two or three live neighbours survives.
                 * 2. Dead cells with three live neighbours becomes a live cell.
                 * 3. Else live cell dies.
                 */

                if (grid[x, y].GetState() == Cell.State.Alive)
                {
                    if (neighboursAlive == 2 || neighboursAlive == 3)
                    {
                        // Survives.
                        newGrid[x, y] = new Cell(Cell.State.Alive, x, y);
                    }
                    else
                    {
                        // Dies.
                        newGrid[x, y] = new Cell(Cell.State.Dead, x, y);
                    }
                }
                else
                {
                    if (neighboursAlive == 3)
                    {
                        // Survives.
                        newGrid[x, y] = new Cell(Cell.State.Alive, x, y);
                    }
                    else
                    {
                        // Dies.
                        newGrid[x, y] = new Cell(Cell.State.Dead, x, y);
                    }
                }
            }
        }
        // Update the grid.
        grid = newGrid;
    }

    /// <summary>
    /// An IEnumerator that manages the running of the game, updating the cells in the grid and outputting it to the game world for every generation.
    /// </summary>
    /// <returns>Delays the coroutine before calculating a new generation (for as long as the simulation is running).</returns>
    IEnumerator GenerateGridLoop()
    {
        OutputGrid();
        while (sStatus == SimulationStatus.Active)
        {
            UpdateGrid();
            OutputGrid();

            yield return new WaitForSeconds(DELAY);
        }

        sStatus = SimulationStatus.Inactive;
        yield break;
    }
}
