using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharactorCamera : MonoBehaviour
{

    private Camera mainCamera_;

    private float xRotation_;
    private float yRotation_;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera_ = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        mainCamera_.transform.position = transform.position;
        mainCamera_.transform.rotation = transform.rotation;

        xRotation_ += Input.GetAxis("Mouse X") * 0.8f;
        yRotation_ += Input.GetAxis("Mouse Y") * 0.8f;

        transform.rotation = Quaternion.Euler(-yRotation_, xRotation_, 0);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
