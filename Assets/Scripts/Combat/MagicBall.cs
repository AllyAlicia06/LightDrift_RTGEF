using UnityEngine;

public class MagicBall : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float travelSpeed = 15f;
    [SerializeField] private float returnSpeed = 20f;
    [SerializeField] private float catchDistance = 0.35f;
    
    private Vector3 targetPoint;
    private Transform returnTarget;
    private bool isReturning;
    private bool isLaunched;
    
    public void Launch(Vector3 target, Transform playerCatchPoint)
    {
        targetPoint = target;
        returnTarget = playerCatchPoint;
        isReturning = false;
        isLaunched = true;
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
        
        if (!isReturning)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPoint,
                travelSpeed * Time.deltaTime
            );
            
            if (Vector3.Distance(transform.position, targetPoint) <= 0.1f)
            {
                isReturning = true;
            }
        }
        else
        {
            Vector3 catchPosition = returnTarget.position;
            
            transform.position = Vector3.MoveTowards(
                transform.position,
                catchPosition,
                returnSpeed * Time.deltaTime
            );
            
            if (Vector3.Distance(transform.position, catchPosition) <= catchDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}
