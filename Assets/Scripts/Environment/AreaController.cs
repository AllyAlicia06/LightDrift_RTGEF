using UnityEngine;

public class AreaController : MonoBehaviour
{
    [Header("Area References")]
    [SerializeField] private EnemyHealth[] enemies;
    [SerializeField] private RestorationTorch torch;
    
    private bool areaCleared;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (torch == null)
            Debug.LogWarning($"{gameObject.name}: No torch assigned!");
        
        if (enemies == null || enemies.Length == 0)
            Debug.LogWarning($"{gameObject.name}: No enemies assigned!");
    }

    // Update is called once per frame
    void Update()
    {
        if (areaCleared || torch == null || enemies == null || enemies.Length == 0)
            return;
        
        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy != null)
                return;
        }
        
        areaCleared = true;
        torch.Unlock();
        
        Debug.Log($"{gameObject.name} has been cleared!");
    }
}
