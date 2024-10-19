using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 10f;  
    public Rigidbody rb;  
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get input from WASD or Arrow keys
        float moveHorizontal = Input.GetAxis("Horizontal");  
        float moveVertical = Input.GetAxis("Vertical");      

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

       
        rb.AddForce(movement * moveSpeed);
    }
}
