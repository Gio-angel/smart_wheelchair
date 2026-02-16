using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Your agent/character
    public Vector3 offset = new Vector3(0, 10, -8); // Camera position relative to target
    public float followSpeed = 5f; // How smoothly camera follows
    public float lookAtHeight = 1f; // Height offset for camera to look at
    
    void Start()
    {
        // Auto-find agent if not assigned
        if (target == null)
        {
            FlowFieldAgent agent = FindFirstObjectByType<FlowFieldAgent>();
            if (agent != null)
            {
                target = agent.transform;
            }
        }
        
        // Set initial position
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Smoothly move camera to follow target
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        
        // Look at the target (with slight height offset)
        Vector3 lookAtPosition = target.position + Vector3.up * lookAtHeight;
        transform.LookAt(lookAtPosition);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        offset[1] -= scroll * 5f;
        offset[1] = Mathf.Clamp(offset[1], 1, 6);
    }
}
