using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAnimatorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    
    [Header("Animation Settings")]
    [SerializeField] private float animationSmoothTime = 0.1f;
    
    private NavMeshAgent agent;
    private EnemyHealth health;
    private int upperBodyLayerIndex = -1;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        
        health = GetComponent<EnemyHealth>(); 
        
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        
        if (animator != null)
            upperBodyLayerIndex = animator.GetLayerIndex("UpperBody");
    }
    
    private void LateUpdate()
    {
        if (animator == null || (health != null && health.IsDead))
            return;
        
        float speed = agent.velocity.magnitude;
        
        animator.SetFloat("Speed", speed, animationSmoothTime, Time.deltaTime);
    }
    
    public void PlayImpactReaction()
    {
        if (animator == null || upperBodyLayerIndex < 0 || (health != null && health.IsDead))
            return;
        
        if (animator.IsInTransition(upperBodyLayerIndex))
            return;
        
        if (!animator.GetCurrentAnimatorStateInfo(upperBodyLayerIndex).IsName("ShieldBlock"))
            return;
        
        animator.SetTrigger("BlockHit");
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
