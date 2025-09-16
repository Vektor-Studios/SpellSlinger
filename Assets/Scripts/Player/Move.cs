using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Pistol : MonoBehaviour
{
    public int bulletCount = 1;
    public float bulletSpread = 5f;
}
public class Shotgun : MonoBehaviour
{
    public int bulletCount = 5;
    public float bulletSpread = 15f;
}
public class Ultra_Shotgun : MonoBehaviour
{
    public int bulletCount = 10;
    public float bulletSpread = 30f;
}

public class Move : MonoBehaviour
{
    public enum Weapons
    {
        Pistol,
        Shotgun,
        Ultra_Shotgun
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
    public int[] weaponList = new int[] { (int)Weapons.Pistol, (int)Weapons.Shotgun, (int)Weapons.Ultra_Shotgun };
    public List<int> WeaponList = new List<int>();
    public int selectedIndex = 0; // Index of the currently selected value in the array
    private int CurrentWeapon = 0;
    private int weaponBulletCount = 1;
    private float weaponBulletSpread = 5;
    void Awake()
    {
        inputActions = new PlayerInputActions();
        Bullet = Resources.Load<GameObject>("Bullet"); // Load the Bullet prefab from Resources folder
        bulletsLeft = magazineSize; // Fill magazine at start
        currentHP = maxHP; // Initialize player HP
        CurrentWeapon = weaponList[selectedIndex];
        WeaponList.Add((int)Weapons.Pistol);
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
        var _sprinting = inputActions.Player.Sprint.IsPressed();
        var _reloading = inputActions.Player.Reload.IsPressed();

        var _weapon_last = inputActions.Player.Previous.triggered;
        var _weapon_next = inputActions.Player.Next.triggered;

        // Handle reloading logic
        if (_reloading && !isReloading && bulletsLeft < magazineSize)
        {
            isReloading = true;
            reloadTimer = 0f;
        }
        if (!_reloading && isReloading)
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
                SpawnBullet(weaponBulletCount, (int) weaponBulletSpread);
            }
            else
            {
                Debug.Log("Magazine empty! Reload to shoot again.");
            }
        }
        if (_sprinting)
        {
            speed = sprintSpeed;
        }
        else
        {
            speed = walkSpeed;
        }
        if (_weapon_next)
        {
            // cycle through avaiable weapons in WeaponList list
            if (WeaponList.Count > 1)
            {
                selectedIndex = (selectedIndex + 1) % WeaponList.Count;
                CurrentWeapon = WeaponList[selectedIndex];
                Debug.Log($"Switched to weapon: {((Weapons)CurrentWeapon).ToString()}");
                SetWeaponAttributes(WeaponList[CurrentWeapon]);
            }
        }
        if (_weapon_last)
        {
            // Cycle through available weapons in WeaponList list
            if (WeaponList.Count > 1)
            {
                selectedIndex = (selectedIndex - 1 + WeaponList.Count) % WeaponList.Count;
                CurrentWeapon = WeaponList[selectedIndex];
                Debug.Log($"Switched to weapon: {((Weapons)CurrentWeapon).ToString()}");
                SetWeaponAttributes(WeaponList[CurrentWeapon]);
            }
        }

        transform.position += new Vector3(move.x, move.y, 0f) * speed * Time.deltaTime;
    }

    void Reload() // Reload the weapon
    {
        bulletsLeft = magazineSize;
        Debug.Log("Reloaded!");
    }

    void SetWeaponAttributes(int weapon = 0)
    {
        switch (weapon)
        {
            case (int)Weapons.Pistol:
                Pistol pistol = new Pistol();
                weaponBulletCount = pistol.bulletCount;
                weaponBulletSpread = pistol.bulletSpread;
                break;
            case (int)Weapons.Shotgun:
                Shotgun shotgun = new Shotgun();
                weaponBulletCount = shotgun.bulletCount;
                weaponBulletSpread = shotgun.bulletSpread;
                break;
            case (int)Weapons.Ultra_Shotgun:
                Ultra_Shotgun ultra_Shotgun = new Ultra_Shotgun();
                weaponBulletCount = ultra_Shotgun.bulletCount;
                weaponBulletSpread = ultra_Shotgun.bulletSpread;
                break;
        }
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

    void SpawnBullet(int count = 1, int spreadAngle = 5)
    {
        bulletsLeft--;
        var i = 0;
        while (i < count)
        {
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
            float padding = 10f;
            float spacing = padding / 2f;
            float x = padding;
            float y = Screen.height - padding - bulletIconHeight;

            for (int i = 0; i < bulletsLeft; i++)
            {
                //float y = yStart - i * (bulletIconHeight + spacing);
                GUI.DrawTexture(new Rect(x, y, bulletIconWidth, bulletIconHeight), bulletIcon);
                x += bulletIconWidth + spacing;
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
        //Draw current weapon text
        {
            string weaponName = ((Weapons)CurrentWeapon).ToString();
            //replace underscores with spaces
            weaponName = weaponName.Replace("_", " ");
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 15;
            style.normal.textColor = Color.gray;
            Vector2 size = style.CalcSize(new GUIContent(weaponName));
            float x = 10f; // 10 pixels from left edge
            float y = Screen.height - size.y - 20f; // 20 pixels from bottom edge
            GUI.Label(new Rect(x, y, size.x, size.y), weaponName, style);
        }
    }
}