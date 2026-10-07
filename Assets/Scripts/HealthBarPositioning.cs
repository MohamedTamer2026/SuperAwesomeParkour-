using UnityEngine;

public class HealthBarPositioning : MonoBehaviour
{
    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
