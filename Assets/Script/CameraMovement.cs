using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float Threshold = 10;
    public float speed = 30;
    public float[] border = new float[4];
    Camera cam;
    void Start()
    {
        cam = GetComponent<Camera>();   
    }
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.fieldOfView += scroll * speed;;
        //if (Input.mousePosition.x >= Screen.width - Threshold && this.transform.position.x <= border[0])
        //{
        //    transform.position += Vector3.right * Time.deltaTime * speed;
        //}
        //if (Input.mousePosition.x <= Threshold && this.transform.position.x >= border[1])
        //{
        //    transform.position += Vector3.left * Time.deltaTime * speed;
        //}
        //if (Input.mousePosition.y >= Screen.height - Threshold && this.transform.position.y <= border[2])
        //{
        //    transform.position += Vector3.up * Time.deltaTime * speed;
        //}
        //if (Input.mousePosition.y <= Threshold && this.transform.position.y >= border[3])
        //{
        //    transform.position += Vector3.down * Time.deltaTime * speed;
        //}
    }
}
