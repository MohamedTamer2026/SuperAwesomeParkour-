using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}