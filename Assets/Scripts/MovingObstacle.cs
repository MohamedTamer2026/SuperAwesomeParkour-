using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public float distance = 5f;   // how far the cloud moves
    public float speed = 1f;      // movement speed

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * distance;
        transform.position = startPos + new Vector3(offset, 0, 0);
    }
}