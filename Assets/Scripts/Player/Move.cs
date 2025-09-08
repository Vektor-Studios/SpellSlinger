using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Move : MonoBehaviour
{
    public enum Weapons
    {
        Pistol,
        Shotgun,
        UltraShotgun
    }
    private PlayerInputActions inputActions;
    public float sprintSpeed = 10f; // Speed when sprinting
    public float walkSpeed = 5f; // Normal walking speed
    public float speed = 5f; // Current movement speed
    public GameObject Bullet; // Drag your Bullet prefab here in the Inspector
    public Transform firePoint;     // Optional: where the bullet spawns

    public int magazineSize = 6; // Maximum bullets per magazine
    private int bulletsLeft; // Bullets remaining in magazine

    public Texture2D bulletIcon; // Assign a bullet sprite/texture in the Inspector
    public float bulletIconWidth = 40f;
    public float bulletIconHeight = 30f;

    public float reloadTime = 0.2f; // Time in seconds to reload
    private bool isReloading = false;
    private float reloadTimer = 0f;

    public int maxHP = 5;
    public int currentHP;
    public int[] weaponList = new int[] { (int)Weapons.Pistol, (int)Weapons.Shotgun, (int)Weapons.UltraShotgun };
    public int selectedIndex = 0; // Index of the currently selected value in the array
    private int CurrentWeapon = 0;
    void Awake()
    {
        inputActions = new PlayerInputActions();
        Bullet = Resources.Load<GameObject>("Bullet"); // Load the Bullet prefab from Resources folder
        bulletsLeft = magazineSize; // Fill magazine at start
        currentHP = maxHP; // Initialize player HP
        CurrentWeapon = weaponList[selectedIndex];
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        Vector2 move = inputActions.Player.Move.ReadValue<Vector2>();
        var _attacking = inputActions.Player.Attack.triggered;
        var _interacting = inputActions.Player.Interact.triggered;
        var _sprinting = inputActions.Player.Sprint.IsPressed();
        var _reloading = inputActions.Player.Reload.IsPressed();

        // Handle reloading logic
        if (_reloading && !isReloading && bulletsLeft < magazineSize)
        {
            isReloading = true;
            reloadTimer = 0f;
        }
        if (! _reloading && isReloading)
        {
            // Cancel reload if button released early
            isReloading = false;
            reloadTimer = 0f;
        }
        if (isReloading)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer >= reloadTime)
            {
                Reload();
                isReloading = false;
                reloadTimer = 0f;
            }
        }

        if (_attacking && !isReloading)
        {
            if (bulletsLeft > 0)
            {
                switch (CurrentWeapon){
                    case (int)Weapons.Pistol:
                        Pistol();
                    break;
                    case (int)Weapons.Shotgun:
                        Shotgun();
                    break;
                    case (int)Weapons.UltraShotgun:
                        UltraShotgun();
                    break;
                }
            }
            else
                    {
                        Debug.Log("Magazine empty! Reload to shoot again.");
                    }
        }
        if (_interacting)
        {
            Debug.Log("Interacting!");
        }
        if (_sprinting)
        {
            speed = sprintSpeed;
        }
        else
        {
            speed = walkSpeed;
        }

        transform.position += new Vector3(move.x, move.y, 0f) * speed * Time.deltaTime;
    }

    void Reload() // Reload the weapon
    {
        bulletsLeft = magazineSize;
        Debug.Log("Reloaded!");
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
        {
            currentHP = 0;
            // Handle player death (e.g., respawn, game over, etc.)
            inputActions.Disable(); // Disable input on death
            Debug.Log("Player has died!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
            // Optionally, add knockback or effects here
        }
    }

    void Pistol() {
        SpawnBullet(1,5);
    }
    void Shotgun() {
        SpawnBullet(5,15);
    }
    void UltraShotgun() {
        SpawnBullet(10,30);
    }

    void SpawnBullet(int count = 1,int spreadAngle = 5)
    {
        bulletsLeft--;
        Debug.Log($"Bullets left: {bulletsLeft}");
        var i = 0;
        while (i < count) {
            Vector2 spawnPos = firePoint ? firePoint.position : transform.position;

            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0f;

            Vector2 direction = (mouseWorldPos - (Vector3)spawnPos).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            int spread = Random.Range(-spreadAngle, spreadAngle + 1) - 90;
            Quaternion rotation = Quaternion.Euler(0, 0, angle + spread);

            Instantiate(Bullet, spawnPos, rotation);
            i++;
        }
    }

    void OnGUI()
    {
        // Draw bullet icons
        if (bulletIcon != null)
        {
            float padding = 20f;
            float spacing = 10f;
            float x = padding;
            float yStart = Screen.height - padding - bulletIconHeight;

            for (int i = 0; i < bulletsLeft; i++)
            {
                float y = yStart - i * (bulletIconHeight + spacing);
                GUI.DrawTexture(new Rect(x, y, bulletIconWidth, bulletIconHeight), bulletIcon);
            }
        }

        // Draw player health bar above player
            {
            Vector3 worldPos = transform.position + Vector3.up * 1.7f;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            float barWidth = 60f;
            float barHeight = 10f;
            float progress = Mathf.Clamp01((float)currentHP / maxHP);

            float x = screenPos.x - barWidth / 2f;
            float y = Screen.height - screenPos.y - barHeight / 2f;

            // Background (dark gray)
            GUI.color = Color.gray;
            GUI.DrawTexture(new Rect(x, y, barWidth, barHeight), Texture2D.whiteTexture);

            // Foreground (red)
            GUI.color = Color.red;
            GUI.DrawTexture(new Rect(x + 1, y + 1, (barWidth - 2) * progress, barHeight - 2), Texture2D.whiteTexture);

            GUI.color = Color.white; // Reset color
        }

        // Draw reload bar below player
        if (isReloading)
        {
            Vector3 worldPos = transform.position + Vector3.down * 1.2f;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            float barWidth = 60f; // Decreased width
            float barHeight = 12f;
            float progress = Mathf.Clamp01(reloadTimer / reloadTime);

            // GUI y is from top, so invert
            float x = screenPos.x - barWidth / 2f;
            float y = Screen.height - screenPos.y - barHeight / 2f;

            // Background (light grey)
            GUI.color = Color.black;
            GUI.DrawTexture(new Rect(x, y, barWidth, barHeight), Texture2D.whiteTexture);

            // Foreground (progress, light grey)
            GUI.color = new Color(0.85f, 0.85f, 0.85f, 1f);
            GUI.DrawTexture(new Rect(x + 2, y + 2, (barWidth - 4) * progress, barHeight - 4), Texture2D.whiteTexture);

            GUI.color = Color.white; // Reset color
        }
    }
}