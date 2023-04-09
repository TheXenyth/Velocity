using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;
    public Transform car;
    public Transform[] cams = new Transform[3];
    public float lerpTime = 0.01f;
    int i = 0;

    Quaternion rotGoal;
    Vector3 direction;
    void Start()
    {
        cam.transform.position = cams[0].transform.position;
        i = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            i++;
        }
        if (i == cams.Length)
        {
            i= 0;
        }
        cam.transform.position = cams[i].transform.position;
        
        direction = (car.position - cam.transform.position).normalized;
        rotGoal = Quaternion.LookRotation(direction);
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, rotGoal, lerpTime);
    }
}
