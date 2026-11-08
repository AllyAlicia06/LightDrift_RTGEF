using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChaseAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 12f;
    
    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed = 240f;
    [SerializeField] private float pathUpdateInterval = 0.2f;
    
    private NavMeshAgent agent;
    private float nextPathUpdate;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (target == null)
        {
            GameObject player =  GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null || !agent.isOnNavMesh)
            return;
        
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;
        
        if (direction.sqrMagnitude > detectionRange * detectionRange)
        {
            agent.isStopped = true;
            return;
        }
        
        agent.isStopped = false;
        
        if (Time.time >= nextPathUpdate)
        {
            if (NavMesh.SamplePosition(target.position, out NavMeshHit navHit, 3f, NavMesh.AllAreas))
            {
                agent.SetDestination(navHit.position);
            }
            
            nextPathUpdate = Time.time + pathUpdateInterval;
        }
        
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
