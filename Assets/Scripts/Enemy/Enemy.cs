using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        Idle,
        Chase,
        Flee
    }

    public State currentState = State.Chase;

    public float speed = 2f;
    public int maxHealth = 3;
    public int health = 0;
    // Flee settings
    public float fleeDistance = 6f;

    private Transform player;
    private Rigidbody2D rb;

    // Tracks time spent fleeing
    private float fleeTimer = 0f;

    // Tracks time spent idling
    private float idleTimer = 0f;

    void Start()
    {
        health = maxHealth; // Initialize health
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {

        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Chase:
                idleTimer = 0f; // Reset idle timer when not idling
                fleeTimer = 0f;
                Chase();
                break;
            case State.Flee:
                idleTimer = 0f; // Reset idle timer when not idling
                Flee();
                break;
        }
    }

    void Idle()
    {
        rb.linearVelocity = Vector2.zero;
        idleTimer += Time.fixedDeltaTime;

        // Heal 1 HP after idling for 2 seconds, only once per 2 seconds
        if (idleTimer >= 2f && health < maxHealth)
        {
            health += 1;
            idleTimer = 0f; // Reset timer so it only heals once per 2 seconds of idling
        }

        if (player != null && Vector2.Distance(player.position, transform.position) < fleeDistance * 2f)
            currentState = State.Chase;
    }

    void Chase()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);

            // Example transition: Flee if too close
            //if (Vector2.Distance(player.position, transform.position) < fleeDistance)
            if (health <= maxHealth / 2) // Flee if health is low
                currentState = State.Flee;
        }
    }

    void Flee()
    {
        if (player != null)
        {
            Vector2 direction = (transform.position - player.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);

            fleeTimer += Time.fixedDeltaTime;

            // Heal 1 HP after fleeing for 2 seconds, only once per flee state
            if (fleeTimer >= 2f && health < maxHealth)
            {
                health += 1;
                fleeTimer = 0f; // Reset timer so it only heals once per 2 seconds of fleeing
            }

            // Example transition: Chase if far enough away
            if (Vector2.Distance(player.position, transform.position) > fleeDistance * 2f)
            {
                currentState = State.Idle;
                fleeTimer = 0f; // Reset timer when leaving flee state
            }
        }
    }

    // Call this when hit by a bullet
    public void TakeDamage(int amount)
    {
        currentState = State.Chase; // Switch to chase state when hit
        health -= amount;
        if (health <= 0)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(1);
            Destroy(other.gameObject); // Destroy bullet
        }
    }

    void OnGUI()
    {
        // Draw health bar above enemy
        Vector3 worldPos = transform.position + Vector3.up * 0.8f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        float barWidth = 40f;
        float barHeight = 8f;
        float progress = Mathf.Clamp01((float)health / maxHealth);

        float x = screenPos.x - barWidth / 2f;
        float y = Screen.height - screenPos.y - barHeight / 2f;

        // Background (dark gray)
        GUI.color = Color.gray;
        GUI.DrawTexture(new Rect(x, y, barWidth, barHeight), Texture2D.whiteTexture);

        // Foreground (red)
        GUI.color = Color.red;
        GUI.DrawTexture(new Rect(x + 1, y + 1, (barWidth - 2) * progress, barHeight - 2), Texture2D.whiteTexture);

        GUI.color = Color.white; // Reset
    }
}
