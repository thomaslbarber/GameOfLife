using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float xMin = 5;
    public float xMax = 510;
    public float yMin = 5;
    public float yMax = 510;


    // Update is called once per frame
    void Update()
    {
        float xAxisValue = Input.GetAxis("Horizontal");
        float zAxisValue = Input.GetAxis("Vertical");


        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x + xAxisValue, xMin, xMax),
            Mathf.Clamp(transform.position.y + zAxisValue, yMin, yMax),
            transform.position.z);
    }
}
