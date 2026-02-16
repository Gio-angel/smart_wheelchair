using UnityEngine;

public class FlowFieldAgent : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float arrivalDistance = 0.5f;
    public float nudgeDistance = 0.1f; // Small forward movement when stuck
    
    private FlowField flowField;
    private GridController gridController;
    private Vector3 destination;
    private bool hasDestination = false;
    private float redCellTimer = 0f;
    private float redCellThreshold = 2f; // seconds on red cell
    
    void Start()
    {
        flowField = FindFirstObjectByType<FlowField>();
        gridController = FindFirstObjectByType<GridController>();
    }
    
    void Update()
    {
        if (hasDestination)
        {
            MoveAlongFlowField();
        }
    }
    
    public void SetDestination(Vector3 targetPosition)
    {
        destination = targetPosition;
        hasDestination = true;
        flowField.CreateFlowField(targetPosition);
        redCellTimer = 0f;
    }
    
    void MoveAlongFlowField()
    {
        // Check if reached destination
        if (Vector3.Distance(transform.position, destination) < arrivalDistance)
        {
            hasDestination = false;
            return;
        }
        
        // Check if on red cell
        GridCell currentCell = gridController.GetCellFromWorldPosition(transform.position);
        
        if (currentCell.cost == 255)
        {
            redCellTimer += Time.deltaTime;
            
            if (redCellTimer >= redCellThreshold)
            {
                // Nudge forward
                transform.position += transform.forward * nudgeDistance;
                redCellTimer = 0f;
                Debug.Log("Nudged forward to escape red cell");
            }
        }
        else
        {
            redCellTimer = 0f;
        }
        
        // Get flow direction
        Vector3 flowDirection = flowField.GetFlowDirection(transform.position);
        
        if (flowDirection != Vector3.zero)
        {
            // Move
            transform.position += flowDirection * moveSpeed * Time.deltaTime;
            
            // Rotate toward movement direction
            Quaternion targetRotation = Quaternion.LookRotation(flowDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}