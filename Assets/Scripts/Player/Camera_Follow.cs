using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // Assign your player here in the Inspector
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10); // Keeps camera behind the scene

    void Awake()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        // Instantly move camera to player position on Awake
        if (target != null)
        {
            transform.position = target.position + offset;
        } else
        {
            Debug.LogWarning("CameraFollow: Player not found.");
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
