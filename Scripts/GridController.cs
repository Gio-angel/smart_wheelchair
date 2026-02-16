using UnityEngine;

public class GridCell
{
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public byte cost = 1; // terrain cost (1 = walkable, 255 = unwalkable)
    public ushort bestCost = ushort.MaxValue; // cost from destination
    public Vector2Int bestDirection; // direction to move
}

public class GridController : MonoBehaviour
{
    public Vector2Int gridSize = new Vector2Int(50, 50);
    public float cellRadius = 0.5f;
    public LayerMask unwalkableMask;
    
    private GridCell[,] grid;
    public float cellDiameter;
    
    void Awake()
    {
        cellDiameter = cellRadius * 2;
        CreateGrid();
    }

    public void RegenerateGrid()
    {
        CreateGrid();
        Debug.Log("Grid regenerated!");
    }
    
    void CreateGrid()
    {
        grid = new GridCell[gridSize.x, gridSize.y];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridSize.x / 2 * cellDiameter 
                                                      - Vector3.forward * gridSize.y / 2 * cellDiameter;
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * cellDiameter + cellRadius)
                                                      + Vector3.forward * (y * cellDiameter + cellRadius);
                
                bool walkable = !Physics.CheckSphere(worldPoint, cellRadius, unwalkableMask);
                
                grid[x, y] = new GridCell
                {
                    worldPosition = worldPoint,
                    gridX = x,
                    gridY = y,
                    cost = walkable ? (byte)1 : (byte)255
                };
            }
        }
    }
    
    public GridCell GetCellFromWorldPosition(Vector3 worldPosition)
    {
        Vector3 localPos = worldPosition - (transform.position - Vector3.right * gridSize.x / 2 * cellDiameter 
                                                                - Vector3.forward * gridSize.y / 2 * cellDiameter);
        
        int x = Mathf.Clamp(Mathf.FloorToInt(localPos.x / cellDiameter), 0, gridSize.x - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(localPos.z / cellDiameter), 0, gridSize.y - 1);
        
        return grid[x, y];
    }

    void OnDrawGizmos(){

        // Draw grid even before play mode
        if (grid != null)
        {
            foreach (GridCell cell in grid)
            {
                // Color code: Green = walkable, Red = unwalkable
                Gizmos.color = cell.cost == 255 ? new Color(1, 0, 0, 0.3f) : new Color(0, 1, 0, 0.3f);
                Gizmos.DrawCube(cell.worldPosition, Vector3.one * cellDiameter * 0.9f);
            }
        }
        // Draw preview grid in edit mode
        else
        {
            float cellDiameter = cellRadius * 2;
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridSize.x / 2 * cellDiameter 
                                                        - Vector3.forward * gridSize.y / 2 * cellDiameter;
            
            Gizmos.color = new Color(1, 1, 1, 0.2f);
            
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * cellDiameter + cellRadius) 
                                                        + Vector3.forward * (y * cellDiameter + cellRadius);
                    
                    Gizmos.DrawWireCube(worldPoint, Vector3.one * cellDiameter * 0.9f);
                }
            }
            
            // Draw grid bounds
            Gizmos.color = Color.yellow;
            Vector3 center = transform.position;
            Vector3 size = new Vector3(gridSize.x * cellDiameter, 0.1f, gridSize.y * cellDiameter);
            Gizmos.DrawWireCube(center, size);
        }
    }
    
    public GridCell[,] GetGrid() => grid;
}