using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField, Min(1)] private int maxHealth = 2;
    [SerializeField] private Slider healthBar;
    
    [Header("Protection Settings")]
    [SerializeField] private bool onlyFromBehind = false;
    
    [SerializeField, Range(0f, 1f)]
    private float rearHitThreshold = 0.5f;
    
    [SerializeField, Range(0f, 1f)] private float frontBlockThreshold = 0.5f;
    
    [Header("Animation References")]
    [SerializeField] private Animator animator; 
    
    private int currentHealth;
    private EnemyAnimatorController animationController;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        
        animationController = GetComponent<EnemyAnimatorController>(); 
    }
    
    public bool BlocksBall(Vector3 ballDirection)
    {
        if (!onlyFromBehind || IsDead)
            return false;
        
        Vector3 flatDirection = Vector3.ProjectOnPlane(ballDirection, Vector3.up);
        
        if (flatDirection.sqrMagnitude < 0.0001f)
            return false;
        
        float directionDot = Vector3.Dot(flatDirection.normalized, transform.forward);
        
        bool blocked = directionDot <= -frontBlockThreshold;
        
        if (blocked && animationController != null)
            animationController.PlayImpactReaction();
        
        return blocked;
    }

    public bool TryTakeHit(Vector3 ballDirection)
    {
        if (currentHealth <= 0 || IsDead)
            return false;
        
        if (onlyFromBehind)
        {
            Vector3 flatDirection = Vector3.ProjectOnPlane(ballDirection, Vector3.up);
            
            if (flatDirection.sqrMagnitude < 0.0001f)
                return false;
            
            float directionDot = Vector3.Dot(flatDirection.normalized, transform.forward);
            
            if (directionDot < rearHitThreshold)
                return false;
        }
        
        currentHealth--;
        UpdateHealthBar();
        
        Debug.Log($"{gameObject.name} was hit! Health: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else if (onlyFromBehind && animationController != null)
        {
            animationController.PlayImpactReaction();
        }
        
        return true;
    }
    
    private void UpdateHealthBar()
    {
        if (healthBar == null)
            return;
        
        healthBar.minValue = 0;
        healthBar.maxValue = maxHealth;
        healthBar.wholeNumbers = true;
        healthBar.value = currentHealth;
    }

    private void Die()
    {
        if (IsDead)
            return;
        
        IsDead = true;
        
        Debug.Log($"{gameObject.name} defeated!");
        
        EnemyChaseAI chaseAI = GetComponent<EnemyChaseAI>();
        if (chaseAI != null)
            chaseAI.enabled = false;
        
        EnemyAttack enemyAttack = GetComponent<EnemyAttack>();
        if (enemyAttack != null)
            enemyAttack.enabled = false;
        
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
        
        foreach (Collider enemyCollider in GetComponentsInChildren<Collider>())
            enemyCollider.enabled = false;
        
        if (healthBar != null)
            healthBar.gameObject.SetActive(false);
        
        if (animator != null)
        {
            int upperBodyLayer = animator.GetLayerIndex("UpperBody");
            
            if (upperBodyLayer >= 0)
                animator.SetLayerWeight(upperBodyLayer, 0f);
            
            animator.SetTrigger("Die");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void FinishDeath()
    {
        if (!IsDead)
            return;

        Destroy(gameObject);
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
