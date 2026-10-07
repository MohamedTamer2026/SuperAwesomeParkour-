using UnityEngine;

public class SimpleCameraLook : MonoBehaviour
{
    public Transform target;

    public float distance = 3f;
    public float minDistance = 2f;
    public float maxDistance = 12f;

    public float mouseSensitivity = 450f;

    public float verticalOffset = 1.5f;

    float currentX;
    float currentY = 20f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize yaw to match player
        currentX = target.eulerAngles.y;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        currentX += mouseX * mouseSensitivity * Time.deltaTime;
        currentY -= mouseY * mouseSensitivity * Time.deltaTime;

        currentY = Mathf.Clamp(currentY, -10f, 70f);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * 5f;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    void LateUpdate()
    {
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        Vector3 finalPosition = target.position + Vector3.up * verticalOffset + offset;

        transform.position = finalPosition;
        transform.rotation = rotation;
    }
}