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
        // add an offset to the gun position
        transform.position += new Vector3(0.5f, 0f, 0f);

    }
}
