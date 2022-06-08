using UnityEngine;

public class Node : MonoBehaviour
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

    public Node(int xPos = 0, int yPos = 0)
    {
        // Set position
        XPos = xPos;
        YPos = yPos;

        // Set the state of the node
        SetState();
    }

    private void SetState()
    {
        int i = Random.Range(0, 16);
        if (i == 0) { state = State.Alive; return; }
        state = State.Dead;
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
