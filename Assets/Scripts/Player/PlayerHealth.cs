using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField, Min(1)] private int maxHealth = 5;
    [SerializeField] private float invulnerabilityDuration = 0.75f;
    
    [Header("UI")]
    [SerializeField] private Slider healthBar;
    
    [Header("References")]
    [SerializeField] private GameUIManager gameUIManager;
    
    private int currentHealth;
    private float nextAllowedDamageTime;
    
    public bool IsDead { get; private set; }
    
    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
    
    public void TakeDamage(int damage)
    {
        if (IsDead || damage <= 0 || Time.time < nextAllowedDamageTime)
            return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        nextAllowedDamageTime = Time.time + invulnerabilityDuration;
        
        UpdateHealthBar();
        
        Debug.Log($"Player received {damage} damage. Health: {currentHealth}");
        
        if (currentHealth <= 0)
            Die();
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
        IsDead = true;
        
        Debug.Log("Player defeated!");
        
        if (gameUIManager != null)
            gameUIManager.ShowDeath();
        else
            Debug.LogError("PlayerHealth: GameUIManager is not assigned!");
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
