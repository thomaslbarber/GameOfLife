using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Class for handling the camera behaviour. Allows for moving (WASD) and zooming (mouse wheel).
/// </summary>
public class CameraController : MonoBehaviour
{
    // Movement variables.
    public float xMin = 5;
    public float xMax = 510;
    public float yMin = 5;
    public float yMax = 510;
    float moveSpeed = 2f;

    // Zoom variables.
    [SerializeField] private CinemachineVirtualCamera cinemachineCamera;
    private float camDist;
    private readonly float sens = 50f;

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        Zoom();
    }

    /// <summary>
    /// FixedUpdate is called at a rate of the physics timestep.
    /// </summary>
    void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// Zooms the camera using the mouse scrollwheel.
    /// </summary>
    private void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            camDist = Input.GetAxis("Mouse ScrollWheel") * sens;
            cinemachineCamera.m_Lens.OrthographicSize -= camDist;
            if (cinemachineCamera.m_Lens.OrthographicSize < 0) { cinemachineCamera.m_Lens.OrthographicSize = 0; }
            else if (cinemachineCamera.m_Lens.OrthographicSize > 135) { cinemachineCamera.m_Lens.OrthographicSize = 135; }
            cinemachineCamera.m_Lens.OrthographicSize -= camDist;
            if (cinemachineCamera.m_Lens.OrthographicSize < 0) { cinemachineCamera.m_Lens.OrthographicSize = 0; }
            else if (cinemachineCamera.m_Lens.OrthographicSize > 135) { cinemachineCamera.m_Lens.OrthographicSize = 135; }
        }
    }

    /// <summary>
    /// Moves the camera using WASD.
    /// </summary>
    private void Move()
    {
        float xAxisValue = Input.GetAxis("Horizontal") * moveSpeed;
        float zAxisValue = Input.GetAxis("Vertical") * moveSpeed;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x + xAxisValue, xMin, xMax),
            Mathf.Clamp(transform.position.y + zAxisValue, yMin, yMax),
            transform.position.z);
    }
}
