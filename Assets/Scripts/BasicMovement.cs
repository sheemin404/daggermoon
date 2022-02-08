using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public MovementData Movement;
    private Rigidbody2D Rigidbody;
    private BoxCollider2D Collider;

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        Movement.Position = Collider.bounds.center;
        Movement.Size = Collider.bounds.size;
        Movement.Extents = Collider.bounds.extents;
        Movement.Velocity = Rigidbody.velocity;

        Movement.HandleMovement();

        Rigidbody.velocity = Movement.Velocity;
    }
}
