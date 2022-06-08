using UnityEngine;

public class Node
{
    // Position of the node
    public int XPos { get; set; }
    public int YPos { get; set; }

    // Whether the node is alive or dead
    public enum State
    {
        Alive,
        Dead
    }

    private State state;

    public Node(State state, int xPos = 0, int yPos = 0)
    {
        // Set the position
        XPos = xPos;
        YPos = yPos;

        // Set the state
        SetState(state);
    }

    public State GetState()
    {
        return state;
    }

    public void SetState(State newState)
    {
        state = newState;
    }
}
