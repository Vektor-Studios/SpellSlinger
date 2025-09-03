using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 20f; // Bullet speed
    public float lifeTime = 2f; // Time in seconds before bullet is destroyed

    void Start()
    {
        Destroy(gameObject, lifeTime);
        var i=0;
        while (i < 10)
        {
            Update();
            i++;
        }
    }

    void Update()
    {
        // Move the bullet forward in its local up direction
        transform.position += transform.up * speed * Time.deltaTime;
    }
}