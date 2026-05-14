using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform targetTruck;
    
    // Scale factor for 1.5x truck size
    private float scaleFactor = 1.5f;
    
    public float distance = 4.5f;           // 3 * 1.5
    public float height = 3f;              // 2 * 1.5
    public float lookHeight = 2.25f;       // 1.5 * 1.5
    public int viewMode = 0;
    public float firstPersonHeight = 2.25f;  // 1.5 * 1.5
    public float firstPersonDistance = 3f;   // 2 * 1.5
    public BoxCollider cabinBoundary;
    
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        rb.useGravity = false;
    }
    
    void LateUpdate()
    {
        if (targetTruck != null)
        {
            Vector3 targetPosition;
            
            if (viewMode == 0)
            {
                // Third-person view - FIXED DISTANCE (no smoothing)
                targetPosition = targetTruck.position 
                    - targetTruck.forward * distance
                    + Vector3.up * height;
                
                // Set position DIRECTLY - follows exactly
                transform.position = targetPosition;
                
                Vector3 lookAtPoint = targetTruck.position + Vector3.up * lookHeight;
                transform.LookAt(lookAtPoint);
            }
            else if (viewMode == 1)
            {
                // First-person view (NO LERP - instant positioning)
                targetPosition = targetTruck.position 
                    + targetTruck.forward * firstPersonDistance
                    + Vector3.up * firstPersonHeight;
                
                // Set position DIRECTLY without Lerp
                transform.position = targetPosition;
                transform.rotation = targetTruck.rotation;
            }
        }
    }
}