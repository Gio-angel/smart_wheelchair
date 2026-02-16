using System.Collections.Generic;
using UnityEngine;

public class FlowField : MonoBehaviour
{
    private GridController gridController;
    private GridCell[,] grid;
    private Vector2Int gridSize;
    private GridCell destinationCell;
    
    void Awake()
    {
        gridController = GetComponent<GridController>();
    }
    
    public void CreateFlowField(Vector3 destinationPosition)
    {
        grid = gridController.GetGrid();
        gridSize = gridController.gridSize;
        
        // Reset the grid
        ResetGrid();
        
        // Set destination
        destinationCell = gridController.GetCellFromWorldPosition(destinationPosition);
        
        if (destinationCell.cost == 255)
        {
            Debug.LogWarning("Destination is unwalkable!");
            return;
        }
        
        
        CreateCostField();
        
        // Create flow field from cost field
        CreateFlowFieldDirections();
    }
    
    void ResetGrid()
    {
        foreach (GridCell cell in grid)
        {
            cell.bestCost = ushort.MaxValue;
            cell.bestDirection = Vector2Int.zero;
        }
    }
    
    void CreateCostField()
    {
        Queue<GridCell> cellsToCheck = new Queue<GridCell>();
        
        destinationCell.bestCost = 0;
        cellsToCheck.Enqueue(destinationCell);
        
        while (cellsToCheck.Count > 0)
        {
            GridCell currentCell = cellsToCheck.Dequeue();
            List<GridCell> neighbors = GetNeighborCells(currentCell);
            
            foreach (GridCell neighbor in neighbors)
            {
                if (neighbor.cost == 255) continue; // unwalkable
                
                ushort newCost = (ushort)(currentCell.bestCost + neighbor.cost);
                
                if (newCost < neighbor.bestCost)
                {
                    neighbor.bestCost = newCost;
                    cellsToCheck.Enqueue(neighbor);
                }
            }
        }
    }
    
    void CreateFlowFieldDirections()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                GridCell cell = grid[x, y];
                
                if (cell.cost == 255) continue; // unwalkable
                
                List<GridCell> neighbors = GetNeighborCells(cell);
                
                int bestCost = cell.bestCost;
                GridCell bestNeighbor = null;
                
                foreach (GridCell neighbor in neighbors)
                {
                    if (neighbor.bestCost < bestCost)
                    {
                        bestCost = neighbor.bestCost;
                        bestNeighbor = neighbor;
                    }
                }
                
                if (bestNeighbor != null)
                {
                    cell.bestDirection = new Vector2Int(
                        bestNeighbor.gridX - cell.gridX,
                        bestNeighbor.gridY - cell.gridY
                    );
                }
            }
        }
    }
    
    List<GridCell> GetNeighborCells(GridCell cell)
    {
        List<GridCell> neighbors = new List<GridCell>();
        
        // 8-directional neighbors
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                
                int checkX = cell.gridX + x;
                int checkY = cell.gridY + y;
                
                if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        
        return neighbors;
    }
    
    public Vector3 GetFlowDirection(Vector3 worldPosition)
    {
        GridCell cell = gridController.GetCellFromWorldPosition(worldPosition);
        
        if (cell.bestDirection == Vector2Int.zero)
            return Vector3.zero;
        
        return new Vector3(cell.bestDirection.x, 0, cell.bestDirection.y).normalized;
    }

    void OnDrawGizmos()
    {
        if (grid == null) return;
        
        foreach (GridCell cell in grid)
        {
            Gizmos.color = cell.cost == 255 ? Color.red : Color.white;
            Gizmos.DrawWireCube(cell.worldPosition, Vector3.one * gridController.cellDiameter * 0.9f);
            
            if (cell.bestDirection != Vector2Int.zero)
            {
                Vector3 direction = new Vector3(cell.bestDirection.x, 0, cell.bestDirection.y);
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(cell.worldPosition, direction.normalized * gridController.cellRadius);
            }
        }
    }
}