using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{

    public float Speed = 4f;
    public float SprintMult = 2f;
    public bool Sprint = false;

    private Vector3 currentDirection = Vector3.zero;


    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            currentDirection = Vector3.left;
            transform.position += currentDirection * Speed * (Sprint ? SprintMult : 1) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            currentDirection = Vector3.right;
            transform.position += currentDirection * Speed * (Sprint ? SprintMult : 1) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            currentDirection = Vector3.up;
            transform.position += currentDirection * Speed * (Sprint ? SprintMult : 1) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            currentDirection = Vector3.down;
            transform.position += currentDirection * Speed * (Sprint ? SprintMult : 1) * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Sprint = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Sprint = false;
        }

    }
   
}
