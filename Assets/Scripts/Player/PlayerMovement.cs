using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private Transform cameraTransform;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    
    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private int maxJumps = 2;
    
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 13f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.8f;
    [SerializeField] private float dashInputBufferTime = 0.3f;
    
    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedVelocity = -2f;
    
    public event Action Jumped;
    public event Action Landed;
    public event Action Dashed;
    
    private CharacterController controller;
    private float verticalVelocity;
    private float rotationVelocity;
    private int jumpsRemaining;
    
    private float dashTimeRemaining;
    private float dashCooldownRemaining;
    private float dashInputBufferRemaining;
    private Vector3 dashDirection;
    private bool airDashUsed;
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }
    
    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }
        
        if (jumpAction != null)
        {
            jumpAction.action.Enable();
        }

        if (dashAction != null)
        {
            dashAction.action.Enable();
        }
    }
    
    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }

        if (jumpAction != null)
        {
            jumpAction.action.Disable();
        }

        if (dashAction != null)
        {
            dashAction.action.Disable();
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }
    
    private void HandleMovement()
    {
        Vector2 input = moveAction != null
            ? moveAction.action.ReadValue<Vector2>()
            : Vector2.zero;
        
        input = Vector2.ClampMagnitude(input, 1f);
        
        Vector3 forward = cameraTransform != null
            ? cameraTransform.forward
            : Vector3.forward;
        
        Vector3 right = cameraTransform != null
            ? cameraTransform.right
            : Vector3.right;
        
        forward.y = 0f;
        right.y = 0f;
        
        forward.Normalize();
        right.Normalize();
        
        Vector3 moveDirection =
            forward * input.y + right * input.x;
        
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);
        
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            float targetAngle = Mathf.Atan2(
                moveDirection.x,
                moveDirection.z
            ) * Mathf.Rad2Deg;
            
            float smoothAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref rotationVelocity,
                rotationSmoothTime
            );
            
            transform.rotation = Quaternion.Euler(
                0f,
                smoothAngle,
                0f
            );
        }
        
        bool wasGrounded = controller.isGrounded;

        if (wasGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedVelocity;
            jumpsRemaining = maxJumps;
        }
        
        if (jumpAction != null && jumpAction.action.WasPressedThisFrame() && jumpsRemaining > 0)
        {
            verticalVelocity = Mathf.Sqrt(
                jumpHeight * -2f * gravity
            );
            
            jumpsRemaining--;
            Jumped?.Invoke(); 
        }

        HandleDash(moveDirection, wasGrounded);
        
        verticalVelocity += gravity * Time.deltaTime;
        
        bool isRunning = dashAction != null &&
                         dashAction.action.IsPressed() &&
                         dashTimeRemaining <= 0f;
        
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        Vector3 velocity = moveDirection * currentSpeed;
        
        if (dashTimeRemaining > 0f)
        {
            velocity = dashDirection * dashSpeed;
        }
        
        velocity.y = verticalVelocity;

        CollisionFlags collisions = controller.Move(velocity * Time.deltaTime);
        
        if (!wasGrounded && (collisions & CollisionFlags.Below) != 0)
            Landed?.Invoke();
        
        if ((collisions & CollisionFlags.Above) != 0 && verticalVelocity > 0f)
        {
            verticalVelocity = 0f;
        }
        
        if (wasGrounded && (collisions & CollisionFlags.Below) == 0 && jumpsRemaining == maxJumps)
        {
            jumpsRemaining--;
        }
    }

    private void HandleDash(Vector3 moveDirection, bool isGrounded)
    {
        if (dashTimeRemaining > 0f)
        {
            dashTimeRemaining = Mathf.Max(0f, dashTimeRemaining - Time.deltaTime);
        }
        
        if (dashCooldownRemaining > 0f)
        { 
            dashCooldownRemaining = Mathf.Max(0f, dashCooldownRemaining - Time.deltaTime);
        }
        
        if (isGrounded)
        {
            airDashUsed = false;
        }
        
        if (dashAction != null &&
            dashAction.action.WasPressedThisFrame())
        {
            dashInputBufferRemaining = dashInputBufferTime;
        }
        
        if (dashInputBufferRemaining > 0f)
        {
            dashInputBufferRemaining -= Time.deltaTime;
        }
        
        if (dashInputBufferRemaining <= 0f || dashTimeRemaining > 0f || dashCooldownRemaining > 0f || (!isGrounded && airDashUsed))
        {
            return;
        }
        
        dashDirection = moveDirection.sqrMagnitude > 0.001f
            ? moveDirection.normalized
            : transform.forward;

        dashTimeRemaining = dashDuration;
        
        dashCooldownRemaining = dashDuration + dashCooldown;
        
        dashInputBufferRemaining = 0f;
        
        if (!isGrounded)
        {
            airDashUsed = true;
        }
        
        Dashed?.Invoke();
    }
}
