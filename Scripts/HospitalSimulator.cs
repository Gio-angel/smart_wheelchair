using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class HospitalSimulator : MonoBehaviour
{
    [Header("Locations")]
    public HospitalLocation emergencyRoom;
    public HospitalLocation cafe;
    public HospitalLocation patientRoom;
    
    [Header("Chances (must add up to 100)")]
    [Range(0, 100)] public int emergencyRoomChance = 40;
    [Range(0, 100)] public int cafeChance = 30;
    [Range(0, 100)] public int patientRoomChance = 30;
    
    [Header("UI Elements")]
    public Button simulateButton;
    public Button endSimulationButton;
    public GameObject situationPanel;
    public TextMeshProUGUI situationDescriptionText;
    public TextMeshProUGUI locationText;
    public TextMeshProUGUI simulationCounterText;
    
    private FlowFieldAgent agent;
    private bool isSimulating = false;
    private bool forceStop = false;
    private HospitalLocation lastLocation = null;
    private Coroutine simulationCoroutine;
    private int simulationCount = 0;
    
    void Start()
    {
        agent = FindFirstObjectByType<FlowFieldAgent>();
        
        if (situationPanel != null)
            situationPanel.SetActive(false);
        
        if (simulateButton != null)
            simulateButton.onClick.AddListener(OnSimulateClicked);
        
        if (endSimulationButton != null)
        {
            endSimulationButton.onClick.AddListener(OnEndSimulationClicked);
            endSimulationButton.gameObject.SetActive(false);
        }
        
        if (endSimulationButton != null)
            endSimulationButton.onClick.AddListener(OnDismissClicked);
        
        int total = emergencyRoomChance + cafeChance + patientRoomChance;
        if (total != 100)
        {
            Debug.LogWarning("Chances don't add up to 100! Total: " + total);
        }
        
        if (simulationCounterText != null)
            simulationCounterText.text = "";
    }
    
    void OnSimulateClicked()
    {
        if (isSimulating) return;
        
        forceStop = false;
        simulationCount = 0;
        lastLocation = null;
        simulationCoroutine = StartCoroutine(RunSimulations());
    }
    
    void OnEndSimulationClicked()
    {
        forceStop = true;
        
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
            simulationCoroutine = null;
        }
        
        ResetUI();
        
        if (locationText != null)
            locationText.text = "Simulation ended after " + simulationCount + " runs.";
        
        if (simulationCounterText != null)
            simulationCounterText.text = "";
    }
    
    IEnumerator RunSimulations()
    {
        isSimulating = true;
        
        if (simulateButton != null)
            simulateButton.interactable = false;
        
        if (endSimulationButton != null)
            endSimulationButton.gameObject.SetActive(true);
        
        // Infinite loop until forceStop
        while (!forceStop)
        {
            simulationCount++;
            
            if (simulationCounterText != null)
                simulationCounterText.text = "Simulation #" + simulationCount;
            
            HospitalLocation selectedLocation = PickLocation();
            lastLocation = selectedLocation;
            
            yield return StartCoroutine(SimulateSequence(selectedLocation));
            
            // Small pause between simulations
            if (!forceStop)
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }
    
    HospitalLocation PickLocation()
    {
        HospitalLocation[] allLocations = { emergencyRoom, cafe, patientRoom };
        int[] chances = { emergencyRoomChance, cafeChance, patientRoomChance };
        
        // Calculate total weight excluding last location
        int totalWeight = 0;
        for (int i = 0; i < allLocations.Length; i++)
        {
            if (allLocations[i] != lastLocation)
                totalWeight += chances[i];
        }
        
        // Roll within reduced pool
        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;
        
        for (int i = 0; i < allLocations.Length; i++)
        {
            if (allLocations[i] == lastLocation) continue;
            
            cumulative += chances[i];
            
            if (roll < cumulative)
                return allLocations[i];
        }
        
        // Fallback
        return emergencyRoom;
    }
    
    IEnumerator SimulateSequence(HospitalLocation location)
    {
        // Show situation panel
        ShowSituationPanel(location);
        
        // Wait before moving
        yield return new WaitForSeconds(2f);
        
        if (forceStop) yield break;
        
        // Move agent to location
        if (agent != null && location.locationTransform != null)
        {
            agent.SetDestination(location.locationTransform.position);
            
            if (locationText != null)
                locationText.text = "Moving to: " + location.locationName;
            
            yield return StartCoroutine(WaitForArrival(location));
        }
        
        if (forceStop) yield break;
        
        if (locationText != null)
            locationText.text = "Arrived at: " + location.locationName;
        
        yield return new WaitForSeconds(1f);
    }
    
    IEnumerator WaitForArrival(HospitalLocation location)
    {
        while (agent != null && location.locationTransform != null && !forceStop)
        {
            float distance = Vector3.Distance(
                agent.transform.position,
                location.locationTransform.position
            );
            
            if (distance < 1.0f) break;
            
            yield return null;
        }
    }
    
    void ShowSituationPanel(HospitalLocation location)
    {
        if (situationPanel == null) return;
        
        situationPanel.SetActive(true);
        
        if (situationDescriptionText != null)
            situationDescriptionText.text = location.situationDescription;
    }
    
    void ResetUI()
    {
        isSimulating = false;
        
        if (simulateButton != null)
            simulateButton.interactable = true;
        
        if (endSimulationButton != null)
            endSimulationButton.gameObject.SetActive(false);
    }
    
    void OnDismissClicked()
    {
        if (situationPanel != null)
            situationPanel.SetActive(false);
    }
}