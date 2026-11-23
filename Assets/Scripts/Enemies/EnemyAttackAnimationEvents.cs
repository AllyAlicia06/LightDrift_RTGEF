using UnityEngine;

public class EnemyAttackAnimationEvents : MonoBehaviour
{
    [SerializeField] private EnemyAttack enemyAttack;
    [SerializeField] private EnemyHealth enemyHealth; 
    
    private void Awake()
    {
        if (enemyAttack == null)
            enemyAttack = GetComponentInParent<EnemyAttack>();
        
        if (enemyHealth == null)
            enemyHealth = GetComponentInParent<EnemyHealth>();
    }
    
    public void DealDamage()
    {
        if (enemyHealth != null && enemyHealth.IsDead)
            return;
        
        if (enemyAttack != null)
            enemyAttack.ApplyAttackDamage();
    }
    
    public void EndAttack()
    {
        if (enemyHealth != null && enemyHealth.IsDead)
            return;
        
        if (enemyAttack != null)
            enemyAttack.FinishAttack();
    }
    
    public void EndDeath()
    {
        if (enemyHealth != null)
            enemyHealth.FinishDeath();
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
