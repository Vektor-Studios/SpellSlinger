using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
        private PlayerInputActions inputActions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //move to player transform
        transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
        //rotate to face mouse position using the player's transform as the rotation origin
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));

    }
}
