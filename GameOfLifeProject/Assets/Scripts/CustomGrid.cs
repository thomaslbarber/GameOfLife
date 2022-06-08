using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class CustomGrid : MonoBehaviour
{
    public int size_x;
    public int size_y;

    // stores all the nodes in the grid
    public Node[,] grid;
    Node[,] gridCopy;

    public Tilemap tilemap;
    public Tile aliveTile;
    public Tile deadTile;

    private const float DELAY = 0.15f;

    private Grid gridUnity;

    CursorTracker ct = new CursorTracker();
    Vector3Int gridPos;

    private void Start()
    {
        runSimulation = false;

        gridUnity = GetComponentInParent<Grid>();
        tilemap = GetComponent<Tilemap>();

        CreateGrid();
        OutputGrid();
    }


    private void Update()
    {
        if (runSimulation) { return; }
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) { return; }

            gridPos = ConvertToGridPosition(ct.GetMousePosition());

            grid[gridPos.y, gridPos.x].SetState(Node.State.Alive);
            OutputGrid();
        }

        if (Input.GetMouseButton(1))
        {
            if (EventSystem.current.IsPointerOverGameObject()) { return; }

            gridPos = ConvertToGridPosition(ct.GetMousePosition());

            grid[gridPos.y, gridPos.x].SetState(Node.State.Dead);
            OutputGrid();
        }
    }

    Vector3Int ConvertToGridPosition(Vector3 position)
    {
        return gridUnity.WorldToCell(position);
    }

    // generates a grid of the given size consisting of nodes
    public void CreateGrid()
    {
        runSimulation = false;
        grid = new Node[size_x, size_y];
        gridCopy = new Node[size_x, size_y];

        for (int x = 0; x < size_x; x++)
        {
            for (int y = 0; y < size_y; y++)
            {
                grid[x, y] = new Node(RandomiseNodeState(), x, y);
                gridCopy[x, y] = new Node(grid[x, y].GetState(), x, y);
            }
        }

        OutputGrid();
    }

    public void CreateIdenticalGrid(bool isAlive)
    {
        runSimulation = false;
        Node.State state = isAlive ? Node.State.Alive : Node.State.Dead;

        for (int x = 0; x < size_x; x++)
        {
            for (int y = 0; y < size_y; y++)
            {
                grid[x, y].SetState(state);
            }
        }

        OutputGrid();
    }

    Node.State RandomiseNodeState()
    {
        int i = Random.Range(0, 16);
        if (i == 0) { return Node.State.Alive; }
        return Node.State.Dead;
    }

    enum SimulationStatus
    {
        Active,
        NotActive,
    }

    SimulationStatus sStatus = SimulationStatus.NotActive;
    bool runSimulation = true;
    public void StartGame()
    {
        if (sStatus == SimulationStatus.Active) { return; }

        sStatus = SimulationStatus.Active;
        runSimulation = true;
        StartCoroutine(GenerateGridLoop());
    }

    public void StopGame()
    {
        runSimulation = false;
    }

    // returns a list of all neighbours of a node
    public List<Node> Neighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) { continue; }

                int checkX = node.XPos + x;
                int checkY = node.YPos + y;

                if (checkX >= 0 && checkX < size_x && checkY >= 0 && checkY < size_y)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    // outputs the current grid to the console
    public void OutputGrid()
    {
        for (int i = 0; i < size_y; i++)
        {
            for (int j = 0; j < size_x; j++)
            {
                Vector3Int p = new Vector3Int(i, j, 0);
                if (grid[j, i].GetState() == Node.State.Alive)
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

    // updates the grid according to the rules of the game of life
    public void UpdateGrid()
    {
        Node[,] newGrid = new Node[size_x, size_y];

        for (int x = 0; x < size_x; x++)
        {
            for (int y = 0; y < size_y; y++)
            {
                List<Node> neighbours = Neighbours(grid[x, y]);

                int neighboursAlive = 0;
                int neighboursDead = 0;
                foreach (Node neighbour in neighbours)
                {
                    if (neighbour.GetState() == Node.State.Alive)
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

                if (grid[x, y].GetState() == Node.State.Alive)
                {
                    if (neighboursAlive == 2 || neighboursAlive == 3)
                    {
                        // survives
                        newGrid[x, y] = new Node(Node.State.Alive, x, y);
                    }
                    else
                    {
                        // dies
                        newGrid[x, y] = new Node(Node.State.Dead, x, y);
                    }
                }
                else
                {
                    if (neighboursAlive == 3)
                    {
                        newGrid[x, y] = new Node(Node.State.Alive, x, y);
                    }
                    else
                    {
                        newGrid[x, y] = new Node(Node.State.Dead, x, y);
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

        sStatus = SimulationStatus.NotActive;
        yield break;
    }
}
