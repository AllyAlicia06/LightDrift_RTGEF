using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private float attackCooldown = 1.5f;
    
    [Header("References")]
    [SerializeField] private PlayerHealth player;
    
    private float nextAttackTime;
    
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
        if (player == null || player.IsDead || Time.time < nextAttackTime)
            return;
        
        Vector3 enemyPosition = transform.position + Vector3.up;
        float distance = Vector3.Distance(enemyPosition, player.transform.position);
        
        if (distance > attackRange)
            return;
        
        nextAttackTime = Time.time + attackCooldown;
        player.TakeDamage(attackDamage);
        
        Debug.Log($"{gameObject.name} attacked the player!");
    }
}
