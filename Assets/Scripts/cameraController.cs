using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    private Transform myTransform;
    public float cameraSpeed = 15f;


    void Awake()
    {
        myTransform = this.transform;
    }
    // Update is called once per frame
    void Update()
    {
        float initCamSpeed = cameraSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            cameraSpeed = cameraSpeed * 2;
        }
        if (Input.GetKey(KeyCode.W))
        {
            myTransform.localPosition += Vector3.forward * Time.deltaTime * cameraSpeed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            myTransform.localPosition += Vector3.left * Time.deltaTime * cameraSpeed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            myTransform.localPosition += Vector3.back * Time.deltaTime * cameraSpeed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            myTransform.localPosition += Vector3.right * Time.deltaTime * cameraSpeed;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            myTransform.localPosition += Vector3.up * Time.deltaTime * cameraSpeed;
        }

        if (Input.GetKey(KeyCode.E))
        {
            myTransform.localPosition += Vector3.down * Time.deltaTime * cameraSpeed;
        }

        cameraSpeed = initCamSpeed;

    }
}
