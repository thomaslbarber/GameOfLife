using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    // Boundaries
    public float xMin = 5;
    public float xMax = 510;
    public float yMin = 5;
    public float yMax = 510;

    [SerializeField] private CinemachineVirtualCamera cam;
    CinemachineComponentBase camBase;
    float camDist;
    [SerializeField] float sens = 10f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            camDist = Input.GetAxis("Mouse ScrollWheel") * sens;
            cam.m_Lens.OrthographicSize -= camDist;
            if (cam.m_Lens.OrthographicSize < 0) { cam.m_Lens.OrthographicSize = 0; }
            else if (cam.m_Lens.OrthographicSize > 135) { cam.m_Lens.OrthographicSize = 135; }
        }


        float xAxisValue = Input.GetAxis("Horizontal");
        float zAxisValue = Input.GetAxis("Vertical");


        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x + xAxisValue, xMin, xMax),
            Mathf.Clamp(transform.position.y + zAxisValue, yMin, yMax),
            transform.position.z);
    }
}
