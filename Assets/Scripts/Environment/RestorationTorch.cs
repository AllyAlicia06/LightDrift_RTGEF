using UnityEngine;
using System;

public class RestorationTorch : MonoBehaviour
{
    [Header("Visual References")]
    [SerializeField] private GameObject flameVisual;
    [SerializeField] private Light flameLight;
    [SerializeField] private GameObject readyIndicator;
    
    private bool isUnlocked;
    public bool IsLit { get; private set; }
    
    public event Action<RestorationTorch> OnLit;
    
    private void Awake()
    {
        if (flameVisual != null)
            flameVisual.SetActive(false);
        
        if (flameLight != null)
            flameLight.enabled = false;
        
        if (readyIndicator != null)
            readyIndicator.SetActive(false);
    }
    
    public void Unlock()
    {
        if (isUnlocked)
            return;
        
        isUnlocked = true;
        
        if (readyIndicator != null)
            readyIndicator.SetActive(true);
        
        Debug.Log($"{gameObject.name} is ready to be lit!");
    }
    
    public void TryLight()
    {
        if (!isUnlocked || IsLit)
            return;
        
        IsLit = true;
        
        if (readyIndicator != null)
            readyIndicator.SetActive(false);
        
        if (flameVisual != null)
            flameVisual.SetActive(true);
        
        if (flameLight != null)
            flameLight.enabled = true;
        
        Debug.Log($"{gameObject.name} has been lit!");
        
        OnLit?.Invoke(this); 
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
