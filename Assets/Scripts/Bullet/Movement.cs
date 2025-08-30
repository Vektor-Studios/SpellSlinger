using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 20f; // Bullet speed
    public float lifeTime = 2f; // Time in seconds before bullet is destroyed

    void Start()
    {
        Destroy(gameObject, lifeTime);
        // move bullet out of player's position
        transform.position += transform.up * speed * 2 * Time.deltaTime;
    }

    void Update()
    {
        // Move the bullet forward in its local up direction
        transform.position += transform.up * speed * Time.deltaTime;
    }
}