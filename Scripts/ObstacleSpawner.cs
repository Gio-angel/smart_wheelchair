using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public int numberOfObstacles = 5;
    
    // Drag empty GameObjects here to mark spawn positions
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;
    public Transform spawnPoint4;
    public Transform spawnPoint5;
    
    void Start()
    {
        SpawnObstacles();
        
        // Regenerate grid after spawning
        GridController grid = FindFirstObjectByType<GridController>();
        if (grid != null)
        {
            grid.RegenerateGrid();
        }
    }
    
    void SpawnObstacles()
    {
        if (obstaclePrefab == null)
        {
            Debug.LogError("Obstacle Prefab is not assigned!");
            return;
        }
        
        // Collect all spawn positions from the transforms
        Vector3[] possiblePositions = new Vector3[5];
        int validPositions = 0;
        
        if (spawnPoint1 != null) possiblePositions[validPositions++] = spawnPoint1.position;
        if (spawnPoint2 != null) possiblePositions[validPositions++] = spawnPoint2.position;
        if (spawnPoint3 != null) possiblePositions[validPositions++] = spawnPoint3.position;
        if (spawnPoint4 != null) possiblePositions[validPositions++] = spawnPoint4.position;
        if (spawnPoint5 != null) possiblePositions[validPositions++] = spawnPoint5.position;
        
        if (validPositions == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }
        
        // Make sure we don't try to spawn more than we have positions
        int obstaclesToSpawn = Mathf.Min(numberOfObstacles, validPositions);
        
        // Shuffle positions
        for (int i = 0; i < validPositions; i++)
        {
            int randomIndex = Random.Range(0, validPositions);
            Vector3 temp = possiblePositions[i];
            possiblePositions[i] = possiblePositions[randomIndex];
            possiblePositions[randomIndex] = temp;
        }
        
        // Spawn obstacles at shuffled positions
        for (int i = 0; i < obstaclesToSpawn; i++)
        {
            Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            Instantiate(obstaclePrefab, possiblePositions[i], randomRotation);
            Debug.Log("Spawned obstacle at: " + possiblePositions[i]);
        }
    }
}