using UnityEngine;

[CreateAssetMenu(fileName = "MovementData", menuName = "Daggermoon/MovementData", order = 0)]
public class MovementData : ScriptableObject {
    [Header("Movement")]
    public float MoveSpeed = 5.5f;
    public float JumpRange = 2.6f;
    public float DashRange = 3.0f;
    [Header("Timings")]
    public float JumpTime = 0.25f;
    public float DashTime = 0.3f;
    [Header("Cooldowns and Buffers")]
    public float DashCooldown = 1.0f;
    public float CoyoteTime = 0.06f;
    [Header("Checks")]
    public LayerMask GroundLayer;

    internal bool IsJumping;
    internal bool IsDashing;
    internal bool IsMoving;
    internal bool IsLocked => IsDashing;
    internal bool IsGrounded => CurrentGroundCheck != null;
    internal bool IsCoyoteTime => CurrentCoyoteTime <= CoyoteTime;
    internal float CurrentJumpTime;
    internal float CurrentDashTime;
    internal float CurrentDashCooldown;
    internal float CurrentCoyoteTime;
    internal Vector2 Velocity;
    internal Vector2 Position;
    internal Vector2 Size;
    internal Vector2 Direction = Vector2.right;
    internal Collider2D PreviousGroundCheck;
    internal Collider2D CurrentGroundCheck;
    internal const float GroundCheckSize = 0.05f;

    public void Move(float axisInput) {
        if(!IsLocked) {
            if(axisInput != 0) {
                Direction = Vector2.right * Mathf.Sign(axisInput);
                Velocity.x = axisInput * MoveSpeed;
                IsMoving = true;
            } else {
                Velocity.x = 0;
                IsMoving = false;
            }
        }
    }

    public void Dash(bool dashDown) {
        if(IsDashing) {
            if(CurrentDashTime < DashTime) {
                Velocity.x = DashRange * Direction.x / DashTime;
                Velocity.y = 0;
                CurrentDashTime += Time.deltaTime;
            } else {
                Velocity.x = 0;
                IsDashing = false;
            }
        } else {
            if(dashDown && CurrentDashCooldown > DashCooldown) {
                CurrentDashTime = 0;
                CurrentDashCooldown = 0;
                IsDashing = true;
            }
        }
    }

    public void Jump(bool jumpDown, bool jumpHold, bool jumpUp) {
        if(!IsLocked) {
            if(IsJumping) {
                if(jumpHold && CurrentJumpTime < JumpTime) {
                    Velocity.y = JumpRange / JumpTime;
                    CurrentJumpTime += Time.deltaTime;
                } else {
                    IsJumping = false;
                }
            } else if(jumpDown && (IsGrounded || IsCoyoteTime)) {
                Velocity.y = JumpRange / JumpTime;
                IsJumping = true;
                CurrentJumpTime = 0;
            }
        }
    }

    public void StartCoyoteTime() {
        if(!IsJumping && !IsDashing && PreviousGroundCheck && !CurrentGroundCheck) {
            CurrentCoyoteTime = 0;
        }
    }

    public void StartUpdate(Vector2 velocity, Vector2 position, Vector2 size) {
        Velocity = velocity;
        Position = position;
        Size = size;

        var groundCheck = Physics2D.BoxCast(Position, Size, 0f, Vector2.down, GroundCheckSize, GroundLayer);
        CurrentGroundCheck = groundCheck.collider;
        StartCoyoteTime();
    }

    public void EndUpdate() {
        CurrentDashCooldown += Time.deltaTime;
        CurrentCoyoteTime += Time.deltaTime;
        PreviousGroundCheck = CurrentGroundCheck;
    }

    public void StartFixedUpdate(Vector2 velocity, Vector2 position, Vector2 size) {
        Velocity = velocity;
        Position = position;
        Size = size;
    }

    public void EndFixedUpdate() { }
}