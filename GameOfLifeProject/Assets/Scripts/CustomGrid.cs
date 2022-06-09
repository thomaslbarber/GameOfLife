using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

/// <summary>
/// Class for managing the grid of cells.
/// </summary>
public class CustomGrid : MonoBehaviour
{
    public int size_x;
    public int size_y;

    // Stores all the nodes in the grid
    public Cell[,] grid;
    Cell[,] gridCopy;

    public Tilemap tilemap;
    public Tile aliveTile;
    public Tile deadTile;

    private const float DELAY = 0.15f;

    private Grid gridUnity;

    CursorTracker ct = new CursorTracker();
    Vector3Int gridPos;

    /// <summary>
    /// Start() is called before the first frame. Initialises necessary variables and sets the initial grid to be randomised.
    /// </summary>
    private void Start()
    {
        runSimulation = false;

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

        if (runSimulation) { return; }

        CheckForMouseInput();
    }

    /// <summary>
    /// Starts/stops the game via the space bar if it is pressed.
    /// </summary>
    void CheckForKeyboardInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (runSimulation) { StopGame(); }
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

            gridPos = ConvertToGridPosition(ct.GetMousePosition());
            grid[gridPos.y, gridPos.x].SetState(Cell.State.Alive);

            OutputGrid();
        }

        // Place a dead cell at the cursor position.
        if (Input.GetMouseButton(1))
        {
            if (EventSystem.current.IsPointerOverGameObject()) { return; }

            gridPos = ConvertToGridPosition(ct.GetMousePosition());
            grid[gridPos.y, gridPos.x].SetState(Cell.State.Dead);

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
        runSimulation = false;
        grid = new Cell[size_x, size_y];
        gridCopy = new Cell[size_x, size_y];

        for (int x = 0; x < size_x; x++)
        {
            for (int y = 0; y < size_y; y++)
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
        runSimulation = false;
        Cell.State state = isAlive ? Cell.State.Alive : Cell.State.Dead;

        for (int x = 0; x < size_x; x++)
        {
            for (int y = 0; y < size_y; y++)
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
    /// Enum for whether the simulation is currently active or inactive.
    /// </summary>
    enum SimulationStatus
    {
        Active,
        Inactive,
    }

    SimulationStatus sStatus = SimulationStatus.Inactive;
    bool runSimulation = true;

    /// <summary>
    /// Starts the simulation.
    /// </summary>
    public void StartGame()
    {
        if (sStatus == SimulationStatus.Active) { return; }

        sStatus = SimulationStatus.Active;
        runSimulation = true;
        StartCoroutine(GenerateGridLoop());
    }

    /// <summary>
    /// Stops the simulation.
    /// </summary>
    public void StopGame()
    {
        runSimulation = false;
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

                if (checkX >= 0 && checkX < size_x && checkY >= 0 && checkY < size_y)
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
        for (int i = 0; i < size_y; i++)
        {
            for (int j = 0; j < size_x; j++)
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
        Cell[,] newGrid = new Cell[size_x, size_y];

        for (int x = 0; x < size_x; x++)
        {
            for (int y = 0; y < size_y; y++)
            {
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
                 * 1. Any live cell with two or three live neighbours survives
                 * 2. Dead cells with three live neighbours becomes a live cell
                 * 3. Else live cell dies
                 */

                if (grid[x, y].GetState() == Cell.State.Alive)
                {
                    if (neighboursAlive == 2 || neighboursAlive == 3)
                    {
                        // survives
                        newGrid[x, y] = new Cell(Cell.State.Alive, x, y);
                    }
                    else
                    {
                        // dies
                        newGrid[x, y] = new Cell(Cell.State.Dead, x, y);
                    }
                }
                else
                {
                    if (neighboursAlive == 3)
                    {
                        newGrid[x, y] = new Cell(Cell.State.Alive, x, y);
                    }
                    else
                    {
                        newGrid[x, y] = new Cell(Cell.State.Dead, x, y);
                    }
                }
            }
        }

        grid = newGrid;
    }

    IEnumerator GenerateGridLoop()
    {
        OutputGrid();
        while (runSimulation)
        {
            UpdateGrid();
            OutputGrid();

            yield return new WaitForSeconds(DELAY);
        }

        sStatus = SimulationStatus.Inactive;
        yield break;
    }
}
