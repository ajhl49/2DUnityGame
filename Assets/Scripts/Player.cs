using UnityEngine;

public class Player : MovingObject
{

    public float BaseSpeed = 8f;
    public float SprintMult = 2f;

    void FixedUpdate()
    {
        float movex = Input.GetAxisRaw("Horizontal");
        float movey = Input.GetAxisRaw("Vertical");

        RigidBody2D.velocity = Input.GetKey(KeyCode.LeftShift) ? new Vector2(movex * BaseSpeed * SprintMult, movey * BaseSpeed * SprintMult) : new Vector2(movex * BaseSpeed, movey * BaseSpeed);
    }
}
