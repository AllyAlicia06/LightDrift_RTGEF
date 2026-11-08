using System;
using UnityEngine;
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
    
    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
    
    public bool BlocksBall(Vector3 ballDirection)
    {
        if (!onlyFromBehind)
            return false;
        
        Vector3 flatDirection = Vector3.ProjectOnPlane(ballDirection, Vector3.up);
        
        if (flatDirection.sqrMagnitude < 0.0001f)
            return false;
        
        float directionDot = Vector3.Dot(flatDirection.normalized, transform.forward);
        
        return directionDot <= -frontBlockThreshold;
    }

    public bool TryTakeHit(Vector3 ballDirection)
    {
        if (currentHealth <= 0)
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
        Debug.Log($"{gameObject.name} defeated!");
        
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
