using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    public float normalSpeed;
    public float multiplySpeed;
    public float mouseSpeed;

    private Vector3 positionVector;
    private Vector3 rotationVector;
    public bool isCursorLocked = false;

    void Start()
    {
        normalSpeed = 0.05F;
        multiplySpeed = 3;
        mouseSpeed = 1;
        Cursor.lockState = CursorLockMode.None; // Free the cursor
        Cursor.visible = true; // Show cursor

    }
    private void ToggleCursorMode()
    {
        isCursorLocked = !isCursorLocked;

        if (isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor for camera movement
            Cursor.visible = false; // Hide cursor
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; // Free the cursor
            Cursor.visible = true; // Show cursor
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleCursorMode();
        }

        // Position
        positionVector.x = Input.GetAxis("Horizontal");
        //positionVector.y = Input.GetAxis("Updown");
        positionVector.z = Input.GetAxis("Vertical");

        positionVector *= normalSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            positionVector *= multiplySpeed;
        }

        transform.Translate(positionVector.x, 0, positionVector.z, Space.Self);
        transform.Translate(0, positionVector.y, 0, Space.World);

        // Rotation
        rotationVector = transform.rotation.eulerAngles;

        //rotationVector.x -= Input.GetAxis("Rotate X"); //Input.GetAxis("Mouse Y");
        //rotationVector.y += Input.GetAxis("Rotate Y"); //Input.GetAxis("Mouse X");

        if ( rotationVector.x < 180 && rotationVector.x > 0 )
        {
            rotationVector.x = Mathf.Clamp(rotationVector.x, 0, 90);
        }

        else if ( rotationVector.x < 360 && rotationVector.x > 180 )
        {
            rotationVector.x = Mathf.Clamp(rotationVector.x, 270, 360);
        }

        transform.rotation = Quaternion.Euler(rotationVector.x, rotationVector.y, 0);
    }

}
