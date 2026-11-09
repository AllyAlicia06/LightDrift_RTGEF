using UnityEngine;
using System.Collections.Generic;
using System;

public class MagicBall : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float travelSpeed = 15f;
    [SerializeField] private float returnSpeed = 20f;
    [SerializeField] private float catchDistance = 0.35f;
    
    [Header("Hit Detection")]
    [SerializeField] private float hitRadius = 0.18f;
    
    private Vector3 targetPoint;
    private Transform returnTarget;
    private bool isReturning;
    private bool isLaunched;
    
    private readonly HashSet<EnemyHealth> hitEnemies = new();
    
    public void Launch(Vector3 target, Transform playerCatchPoint)
    {
        targetPoint = target;
        returnTarget = playerCatchPoint;
        isReturning = false;
        isLaunched = true;
        
        hitEnemies.Clear();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLaunched)
            return;
        
        if (returnTarget == null)
        {
            Destroy(gameObject);
            return;
        }
        
        Vector3 startPosition = transform.position;
        
        if (!isReturning)
        {
            Vector3 nextPosition = Vector3.MoveTowards(startPosition, targetPoint, travelSpeed * Time.deltaTime);
            
            if (DetectEnemyHits(startPosition, nextPosition))
                return;
            
            transform.position = nextPosition;

            if (Vector3.Distance(transform.position, targetPoint) <= 0.1f)
            {
                isReturning = true;
                hitEnemies.Clear();
            }
        }
        else
        {
            Vector3 catchPosition = returnTarget.position;
            Vector3 nextPosition = Vector3.MoveTowards(startPosition, catchPosition, returnSpeed * Time.deltaTime);
            
            if (DetectEnemyHits(startPosition, nextPosition))
                return;
            
            transform.position = nextPosition;
            
            if (Vector3.Distance(transform.position, catchPosition) <= catchDistance)
            {
                Destroy(gameObject);
            }
        }
    }
    
    private bool DetectEnemyHits(Vector3 from, Vector3 to)
    {
        Vector3 movement = to - from;
        
        if (movement.sqrMagnitude < 0.000001f)
            return false;
        
        Vector3 direction = movement.normalized;
        
        RaycastHit[] hits = Physics.SphereCastAll(from, hitRadius, direction, movement.magnitude, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        
        foreach (RaycastHit hit in hits)
        {
            if (TryHitEnemy(hit.collider, direction))
                return true;
            
            TryLightTorch(hit.collider);
        }
        
        Collider[] overlaps = Physics.OverlapSphere(to, hitRadius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        
        foreach (Collider collider in overlaps)
        {
            if (TryHitEnemy(collider, direction))
                return true;
            
            TryLightTorch(collider);
        }

        return false;
    }
    
    private bool TryHitEnemy(Collider collider, Vector3 direction)
    {
        EnemyHealth enemy = collider.GetComponentInParent<EnemyHealth>();
        
        if (enemy == null || hitEnemies.Contains(enemy))
            return false;
        
        if (enemy.BlocksBall(direction))
        {
            Debug.Log($"{enemy.gameObject.name} blocked the magic ball!");
            
            isLaunched = false;
            Destroy(gameObject);
            return true;
        }
        
        if (enemy.TryTakeHit(direction))
        {
            hitEnemies.Add(enemy);
        }

        return false;
    }
    
    private void TryLightTorch(Collider collider)
    {
        RestorationTorch torch = collider.GetComponentInParent<RestorationTorch>();
        
        if (torch != null)
            torch.TryLight();
    }
}
