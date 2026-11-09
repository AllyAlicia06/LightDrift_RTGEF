using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private RestorationTorch[] torches;
    
    [Header("UI References")]
    [SerializeField] private TMP_Text torchCounterText;
    [SerializeField] private GameUIManager gameUIManager;
    
    [Header("Completion Settings")]
    [SerializeField, Min(0f)] private float completionDelay = 3f;
    
    private readonly HashSet<RestorationTorch> trackedTorches = new();
    private readonly HashSet<RestorationTorch> litTorches = new();
    private bool levelCompleted;
    
    private void Awake()
    {
        foreach (RestorationTorch torch in torches)
        {
            if (torch != null)
                trackedTorches.Add(torch);
        }
        
        UpdateTorchCounter();
    }
    
    private void OnEnable()
    {
        foreach (RestorationTorch torch in trackedTorches)
            torch.OnLit += HandleTorchLit;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (trackedTorches.Count == 0)
            Debug.LogWarning("LevelManager has no torches assigned!");
    }
    
    private void OnDisable()
    {
        foreach (RestorationTorch torch in trackedTorches)
        {
            if (torch != null)
                torch.OnLit -= HandleTorchLit;
        }
    }

    private void HandleTorchLit(RestorationTorch torch)
    {
        if (!trackedTorches.Contains(torch) || !litTorches.Add(torch))
            return;
        
        UpdateTorchCounter();
        
        Debug.Log($"Torches lit: {litTorches.Count}/{trackedTorches.Count}");
        
        if (!levelCompleted && litTorches.Count == trackedTorches.Count)
        {
            levelCompleted = true;
            StartCoroutine(CompleteLevel());
        }
    }
    
    private void UpdateTorchCounter()
    {
        if (torchCounterText != null)
            torchCounterText.text = $"Light Restored: {litTorches.Count} / {trackedTorches.Count}";
    }
    
    private IEnumerator CompleteLevel()
    {
        yield return new WaitForSeconds(completionDelay);
        
        if (gameUIManager != null)
            gameUIManager.ShowVictory();
        else
            Debug.LogError("LevelManager: GameUIManager is not assigned!");
        
        Debug.Log("The light has returned! Level complete.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
