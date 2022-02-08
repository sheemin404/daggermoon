using UnityEngine;

[CreateAssetMenu(fileName = "MovementData", menuName = "Daggermoon/MovementData", order = 0)]
public class MovementData : ScriptableObject {
    [HideInInspector]
    public Vector2 Velocity = Vector2.zero;
    [HideInInspector]
    public Vector2 Position = Vector2.zero;
    [HideInInspector]
    public Vector2 Size = Vector2.zero;
    [HideInInspector]
    public Vector2 Extents = Vector2.zero;
    public float MoveSpeed = 5.5f;
    public LayerMask GroundLayer;
    public float JumpRange = 2.0f;
    public float DashRange = 3.0f;
    public float MaxJumpTime = 0.25f;
    public float DashTime = 0.1f;
    public float DashCooldown = 1f;
    public float CoyoteTime = 0.1f;

    private Vector2 Direction = Vector2.right;
    private bool IsJumping = false;
    private bool PreviousGroundCheck = false;
    private bool CurrentGroundCheck = false;
    private bool IsDashing = false;
    // Timers
    private float CurrentJumpTime = 0.0f;
    private float CurrentDashTime = 0.0f;
    private float CurrentDashCooldown = 0.0f;
    private float CurrentCoyoteTime = 0.0f;
    // Constants
    public const float GroundCheckSize = 0.05f;

    public bool IsLocked() {
        return IsDashing;
    }

    public void HandleMovement() {
        UpdateGroundCheck();
        CurrentDashCooldown += Time.deltaTime;

        if(IsLocked()) {
            UpdateDash();
        } else {
            HandleHorizontalMovement();
            HandleJump();
            HandleDash();
        }
        PreviousGroundCheck = CurrentGroundCheck;
    }

    public void HandleDash() {
        var dashDown = Input.GetAxis("Dash") > 0;
        if(dashDown && CurrentDashCooldown > DashCooldown) {
            CurrentDashTime = 0;
            IsDashing = true;
            CurrentDashCooldown = 0;
        }
    }

    public void UpdateDash() {
        if(IsDashing) {
            if(CurrentDashTime < DashTime) {
                Velocity.x = DashRange * Direction.x / DashTime;
                Velocity.y = 0;
                CurrentDashTime += Time.deltaTime;
            } else {
                Velocity.x = 0;
                IsDashing = false;
            }
        }
    }

    public void HandleHorizontalMovement() {
        var horizontalInput = Input.GetAxisRaw("Horizontal");

        if(horizontalInput != 0) {
            Direction = Vector2.right * Mathf.Sign(horizontalInput);
            Velocity.x = horizontalInput * MoveSpeed;
        } else {
            Velocity.x = 0;
        }
    }

    public void HandleJump() {
        var jumpDown = Input.GetButtonDown("Jump");
        var jumpHold = Input.GetButton("Jump");
        var jumpUp = Input.GetButtonUp("Jump");

        CheckCoyoteTime();
        if((IsGrounded() || IsCoyoteTime()) && jumpDown) {
            Velocity.y = JumpRange / MaxJumpTime;
            IsJumping = true;
            CurrentJumpTime = 0;
        }
        
        if(jumpHold && IsJumping) {
            if(CurrentJumpTime < MaxJumpTime) {
                Velocity.y = JumpRange / MaxJumpTime;
                CurrentJumpTime += Time.deltaTime;
            } else {
                IsJumping = false;
            }
        }

        if(jumpUp) {
            IsJumping = false;
        }  
    }

    void CheckCoyoteTime() {
        if(!IsJumping && !IsDashing && PreviousGroundCheck && !CurrentGroundCheck) {
            CurrentCoyoteTime = 0;
        } else {
            CurrentCoyoteTime += Time.deltaTime;
        }
    }

    bool IsCoyoteTime() {
        return CurrentCoyoteTime <= CoyoteTime; 
    }

    bool IsGrounded() {
        return CurrentGroundCheck;
    }
    
    void UpdateGroundCheck() {
        var groundCheck = Physics2D.BoxCast(Position, Size, 0f, Vector2.down, GroundCheckSize, GroundLayer);

        CurrentGroundCheck = groundCheck.collider != null;
    }
}