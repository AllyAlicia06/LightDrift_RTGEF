using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBallThrower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionReference throwAction;
    [SerializeField] private Camera aimCamera;
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private MagicBall ballPrefab;
    
    [Header("Throw Settings")]
    [SerializeField] private float maxAimDistance = 20f;
    [SerializeField] private RectTransform crosshairTransform; 

    private MagicBall activeBall;
    
    private void Awake()
    {
        if (aimCamera == null)
        {
            aimCamera = Camera.main;
        }
    }
    
    private void OnEnable()
    {
        if (throwAction != null)
        {
            throwAction.action.Enable();
        }
    }
    
    private void OnDisable()
    {
        if (throwAction != null)
        {
            throwAction.action.Disable();
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (throwAction != null &&
            throwAction.action.WasPressedThisFrame())
        {
            ThrowBall();
        }
    }
    
    private void ThrowBall()
    {
        if (activeBall != null)
            return;
        
        if (aimCamera == null || ballSpawnPoint == null || ballPrefab == null)
        {
            Debug.LogWarning("Magic ball references are missing.");
            return;
        }
        
        Vector3 targetPoint = GetAimPoint();
        
        activeBall = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
        activeBall.Launch(targetPoint, ballSpawnPoint);
    }
    
    private Vector3 GetAimPoint()
    {
        Vector2 screenPoint = crosshairTransform != null
            ? RectTransformUtility.WorldToScreenPoint(
                null, crosshairTransform.position)
            : new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        
        Ray ray = aimCamera.ScreenPointToRay(screenPoint);
        
        Vector3 aimPoint = ray.GetPoint(maxAimDistance);
        float nearestDistance = maxAimDistance;
        
        RaycastHit[] hits = Physics.RaycastAll(ray, maxAimDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform))
                continue;
            
            if (hit.distance < nearestDistance)
            {
                nearestDistance = hit.distance;
                aimPoint = hit.point;
            }
        }
        
        return aimPoint;
    }
}
