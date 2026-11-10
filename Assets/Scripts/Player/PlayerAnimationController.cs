using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    
    [Header("Animation Settings")]
    [SerializeField] private float animationSmoothTime = 0.1f;
    
    private CharacterController controller;
    private PlayerMovement movement;
    private PlayerBallThrower ballThrower;
    private PlayerHealth health;
    private bool jumpAnimationActive;
    private bool nextHitA = true;
    private bool isDead;
    private int upperBodyLayerIndex;
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        movement = GetComponent<PlayerMovement>();
        ballThrower = GetComponent<PlayerBallThrower>();
        health = GetComponent<PlayerHealth>();
        
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        
        upperBodyLayerIndex = animator != null ? animator.GetLayerIndex("UpperBody") : -1;
    }
    
    private void OnEnable()
    {
        if (movement == null)
            return;
        
        movement.Jumped += PlayJump;
        movement.Landed += PlayLand;
        movement.Dashed += PlayDash;
        
        if (ballThrower != null)
            ballThrower.BallThrown += PlayAttack;
        
        if (health != null)
        {
            health.Damaged += PlayHit;
            health.Died += PlayDeath;
        }
    }
    
    private void OnDisable()
    {
        if (movement == null)
            return;
        
        movement.Jumped -= PlayJump;
        movement.Landed -= PlayLand;
        movement.Dashed -= PlayDash;
        
        if (ballThrower != null)
            ballThrower.BallThrown -= PlayAttack;
        
        if (health != null)
        {
            health.Damaged -= PlayHit;
            health.Died -= PlayDeath;
        }
    }
    
    private void LateUpdate()
    {
        if (animator == null || isDead)
            return;
        
        Vector3 velocity = controller.velocity;
        velocity.y = 0f;
        
        animator.SetFloat("Speed", velocity.magnitude, animationSmoothTime, Time.deltaTime);
        animator.SetBool("IsGrounded", controller.isGrounded); 
    }
    
    private void PlayJump()
    {
        if (animator == null || isDead)
            return;
        
        jumpAnimationActive = true;
        
        animator.ResetTrigger("Land");
        animator.SetTrigger("Jump");
    }
    
    private void PlayLand()
    {
        if (animator == null || !jumpAnimationActive || isDead)
            return;
        
        jumpAnimationActive = false;
        
        animator.ResetTrigger("Jump");
        animator.SetTrigger("Land");
    }
    
    private void PlayDash()
    {
        if (animator == null || isDead)
            return;
        
        animator.SetTrigger("Dash");
    }
    
    private void PlayAttack()
    {
        if (animator == null ||isDead)
            return;
        
        animator.SetTrigger("Attack");
    }
    
    private void PlayHit()
    {
        if (animator == null || isDead)
            return;
        
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("HitA");
        animator.ResetTrigger("HitB");
        
        animator.SetTrigger(nextHitA ? "HitA" : "HitB");
        nextHitA = !nextHitA;
    }
    
    private void PlayDeath()
    {
        if (animator == null || isDead)
            return;
        
        isDead = true;
        
        animator.ResetTrigger("Jump");
        animator.ResetTrigger("Land");
        animator.ResetTrigger("Dash");
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("HitA");
        animator.ResetTrigger("HitB");
        
        if (upperBodyLayerIndex >= 0)
            animator.SetLayerWeight(upperBodyLayerIndex, 0f);
        
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        
        animator.SetTrigger("Die");
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
