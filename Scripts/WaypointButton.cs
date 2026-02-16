using UnityEngine;
using UnityEngine.UI;

public class WaypointButton : MonoBehaviour
{
    public Vector3 targetPosition;
    public Vector3 targetPosition2;
    public Vector3 targetPosition3;
    public FlowFieldAgent agent;
    
    public Button button;
    
    void Start()
    {
        Debug.Log("WaypointButton started on: " + gameObject.name);
        
        
        if (button == null)
        {
            Debug.LogError("NO BUTTON COMPONENT on " + gameObject.name + "!");
            return;
        }
        
        Debug.Log("Button component found");
        button.onClick.AddListener(OnButtonClick);
        
        if (agent == null)
        {
            agent = FindFirstObjectByType<FlowFieldAgent>();
            Debug.Log("Agent found: " + (agent != null));
        }
    }
    
    public void OnButtonClick()
    {
        Debug.Log("BUTTON CLICKED! Target: " + targetPosition);
        
        if (agent != null)
        {
            agent.SetDestination(targetPosition);
            Debug.Log("Destination set!");
        }
        else
        {
            Debug.LogError("Agent is NULL!");
        }
    }

    public void OnButtonClick2()
    {
        Debug.Log("BUTTON CLICKED! Target: " + targetPosition2);
        
        if (agent != null)
        {
            agent.SetDestination(targetPosition2);
            Debug.Log("Destination set!");
        }
        else
        {
            Debug.LogError("Agent is NULL!");
        }
    }

    public void OnButtonClick3()
    {
        Debug.Log("BUTTON CLICKED! Target: " + targetPosition3);
        
        if (agent != null)
        {
            agent.SetDestination(targetPosition3);
            Debug.Log("Destination set!");
        }
        else
        {
            Debug.LogError("Agent is NULL!");
        }
    }

}