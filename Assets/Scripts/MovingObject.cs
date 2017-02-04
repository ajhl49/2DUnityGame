using UnityEngine;

public class MovingObject : MonoBehaviour
{
    
    public LayerMask BlockingLayer;

    protected BoxCollider2D BoxCollider;
    protected Rigidbody2D RigidBody2D;

    protected virtual void Start()
    {
        BoxCollider = GetComponent<BoxCollider2D>();
        RigidBody2D = GetComponent<Rigidbody2D>();
    }
}
