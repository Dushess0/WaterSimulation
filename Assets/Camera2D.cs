using UnityEngine;

public class Camera2D : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float turnSpeed = 5.0f;
    public float zoomSpeed = 10.0f;

    private void Update()
    {
        // Get the input for horizontal and vertical movement (WASD keys)
        float h = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float v = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        // Calculate the forward and right direction relative to the camera's orientation
        Vector3 forward = transform.forward;
        forward.y = 0; // Keep the movement horizontal
        forward.Normalize();
        Vector3 right = transform.right;
        right.y = 0; // Keep the movement horizontal
        right.Normalize();

        // Calculate the desired movement direction
        Vector3 desiredMoveDirection = forward * v + right * h;

        // Space/Shift for vertical movement (up/down)
        float y = 0;
        if (Input.GetKey(KeyCode.Space))
        {
            y = moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            y = -moveSpeed * Time.deltaTime;
        }

        // Mouse rotation
        if (Input.GetMouseButton(1)) // Right mouse button for turning
        {
            float yaw = turnSpeed * Input.GetAxis("Mouse X");
            float pitch = turnSpeed * Input.GetAxis("Mouse Y");

            transform.eulerAngles += new Vector3(-pitch, yaw, 0);
        }

        // Mouse scroll for zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * scroll * zoomSpeed;

        // Apply movement
        transform.Translate(desiredMoveDirection, Space.World);
        transform.Translate(0, y, 0, Space.World); // Apply vertical movement separately
    }
}
