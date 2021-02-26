using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    float x;
    float y;
    [SerializeField] float sensitivity = 2f;
    [SerializeField] float speed = 20f;
    Vector3 pivotPosition = Vector3.zero;

    bool cursorLocked;
    //Transform target;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorLocked = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleCursorLock(!cursorLocked);
        }

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            ToggleCursorLock(true);
        if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
            ToggleCursorLock(false);


        if (cursorLocked)
        {
            x = Input.GetAxis("Mouse X");
            y = Input.GetAxis("Mouse Y");
            if (Input.GetMouseButton(1))
            {
                pivotPosition += Vector3.up * (-y * sensitivity);
                transform.position += Vector3.up * (-y * sensitivity);
            }
            if (Input.GetMouseButton(2))
            {
                transform.RotateAround(pivotPosition, Vector3.up, x * sensitivity);
                transform.RotateAround(pivotPosition, transform.right, -y * sensitivity);
            }
        }


        Vector3 pivotMoveVector = (new Vector3(transform.forward.x, 0, transform.forward.z) * Input.GetAxisRaw("Vertical")
                       + new Vector3(transform.right.x, 0, transform.right.z) * Input.GetAxisRaw("Horizontal")).normalized * speed;
        pivotPosition += pivotMoveVector;
        transform.position += pivotMoveVector;

        if (Input.GetKeyDown(KeyCode.C))
        {
            transform.position = Vector3.up * 150f;
            transform.rotation = Quaternion.Euler(90, 0, 0);
            transform.RotateAround(Vector3.zero, Vector3.up, 30);
            transform.RotateAround(Vector3.zero, transform.right, -60);
        }

        transform.position += transform.forward * Input.mouseScrollDelta.y * 5f;
    }

    void ToggleCursorLock(bool state)
    {
        if (cursorLocked != state)
        {
            if (state)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
            Cursor.visible = !state;
            cursorLocked = state;
        }
    }

}
