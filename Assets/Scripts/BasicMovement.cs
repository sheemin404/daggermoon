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

    void FixedUpdate() {
        var axisInput = Input.GetAxisRaw("Horizontal");

        Movement.StartFixedUpdate(Rigidbody.velocity, Collider.bounds.center, Collider.bounds.size); 
        {
            Movement.Move(axisInput);
        }
        Movement.EndFixedUpdate();

        Rigidbody.velocity = Movement.Velocity;
    }

    void Update()
    {
        var jumpDown = Input.GetButtonDown("Jump");
        var jumpHold = Input.GetButton("Jump");
        var jumpUp = Input.GetButtonUp("Jump");
        var dashDown = Input.GetAxis("Dash") > 0;

        Movement.StartUpdate(Rigidbody.velocity, Collider.bounds.center, Collider.bounds.size); 
        {
            Movement.Dash(dashDown);
            Movement.Jump(jumpDown, jumpHold, jumpUp);
        }
        Movement.EndUpdate();

        Rigidbody.velocity = Movement.Velocity;
    }
}
