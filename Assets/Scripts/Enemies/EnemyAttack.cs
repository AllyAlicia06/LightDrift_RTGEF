using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private float attackCooldown = 1.5f;
    
    [Header("Animation Settings")]
    [SerializeField] private string firstAttackTrigger = "AttackStab";
    [SerializeField] private string secondAttackTrigger = "AttackSlice";
    
    [Header("References")]
    [SerializeField] private PlayerHealth player;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent; 
    
    private float nextAttackTime;
    private bool isAttacking;
    private bool nextAttackIsFirst = true;
    private bool hitApplied;
    
    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            
            if (playerObject != null)
                player = playerObject.GetComponent<PlayerHealth>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || player.IsDead || isAttacking || animator == null || Time.time < nextAttackTime)
            return;
        
        Vector3 enemyPosition = transform.position + Vector3.up;
        float distance = Vector3.Distance(enemyPosition, player.transform.position);
        
        if (distance > attackRange)
            return;
        
        nextAttackTime = Time.time + attackCooldown;
        isAttacking = true;
        hitApplied = false;
        
        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;
        
        animator.SetTrigger(nextAttackIsFirst ? firstAttackTrigger : secondAttackTrigger);
        
        nextAttackIsFirst = !nextAttackIsFirst;
        
        Debug.Log($"{gameObject.name} started an attack!");
    }
    
    private void LateUpdate()
    {
        if (isAttacking && agent != null && agent.isOnNavMesh)
            agent.isStopped = true;
    }
    
    public void ApplyAttackDamage()
    {
        if (!isAttacking || hitApplied || player == null || player.IsDead)
            return;
        
        hitApplied = true;
        
        Vector3 enemyPosition = transform.position + Vector3.up;
        float distance = Vector3.Distance(enemyPosition, player.transform.position);
        
        if (distance > attackRange)
            return;
        
        player.TakeDamage(attackDamage);
        
        Debug.Log($"{gameObject.name} attacked the player!");
    }
    
    public void FinishAttack()
    {
        if (!isAttacking)
            return;
        
        isAttacking = false;
        
        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = false;
    }
}
