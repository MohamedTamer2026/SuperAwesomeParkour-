using UnityEngine;

public class BarkBullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifeSpan = 0.4f;
    public float expandSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifeSpan);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        Vector3 expand = new Vector3(expandSpeed, expandSpeed, 0);
        transform.localScale += expand * Time.deltaTime;

        if (transform.localScale.x > 5f)
        {
            transform.localScale = new Vector3(5f, 5f, transform.localScale.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyFSM ghost = other.GetComponentInParent<EnemyFSM>();

            if (ghost != null)
            {
                float damage = ghost.maxHealth / 2f;

                ghost.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
