using UnityEngine;

public class Player : MovingObject
{

    public float BaseSpeed = 8f;
    public float SprintMult = 2f;
    public static Player PlayerInstance;

    protected override void Start()
    {
        if (PlayerInstance != null)
        {
            Debug.LogError("Player instance already created.");
            return;
        }
        Debug.Log("player created");
        PlayerInstance = this;

        base.Start();

    }
    public int X
    {
        get
        {
            return (int) Mathf.Floor(transform.position.x);
        }
    }
    public int Y
    {
        get
        {
            return (int)Mathf.Floor(transform.position.y);
        }
    }

    void FixedUpdate()
    {
        float movex = Input.GetAxisRaw("Horizontal");
        float movey = Input.GetAxisRaw("Vertical");

        RigidBody2D.velocity = Input.GetKey(KeyCode.LeftShift) ? new Vector2(movex * BaseSpeed * SprintMult, movey * BaseSpeed * SprintMult) : new Vector2(movex * BaseSpeed, movey * BaseSpeed);
    }
}
