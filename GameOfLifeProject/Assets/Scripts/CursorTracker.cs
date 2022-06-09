using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Small class for tracking the position of the mouse.
/// </summary>
public class CursorTracker
{
    /// <summary>
    /// Converts the mouse position from screen coordinates to the world position and returns it.
    /// </summary>
    /// <returns>The current position of the mouse in the world.</returns>
    public Vector3 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
    }
}
