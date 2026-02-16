using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    public FlowFieldAgent agent;
    public LayerMask groundLayer;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 1000f, groundLayer))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}