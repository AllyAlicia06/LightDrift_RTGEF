using System;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    private void Awake()
    {
        if  (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (targetCamera != null)
            transform.rotation = targetCamera.transform.rotation;
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
