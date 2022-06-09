using UnityEngine;

/// <summary>
/// Each individual Cell in the game world.
/// </summary>
public class Cell
{
    // Position of the cell.
    public int XPos { get; set; }
    public int YPos { get; set; }

    /// <summary>
    /// Whether the node is alive or dead.
    /// </summary>
    public enum State
    {
        Alive,
        Dead
    }

    // The current state of the cell (alive/dead).
    private State state;

    public Cell(State state, int xPos = 0, int yPos = 0)
    {
        // Set the position of the cell.
        XPos = xPos;
        YPos = yPos;

        // Set the state of the cell.
        SetState(state);
    }

    /// <summary>
    /// Getter for the state of the cell.
    /// </summary>
    /// <returns>Whether the cell is alive/dead.</returns>
    public State GetState()
    {
        return state;
    }

    /// <summary>
    /// Setter for the state of the cell.
    /// </summary>
    /// <param name="newState">The new state of the cell.</param>
    public void SetState(State newState)
    {
        state = newState;
    }
}
