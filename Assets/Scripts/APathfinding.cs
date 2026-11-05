//using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class APathfinding : MonoBehaviour
{
    private List<Vector2Int> path = new List<Vector2Int>();

    private Vector2Int start = new Vector2Int(0, 1);

    private Vector2Int goal = new Vector2Int(4, 4);

    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    [SerializeField] private int width = 4;

    [SerializeField] private int height = 4;

    [Tooltip("Number out of 100 for probability.")]
    [SerializeField] private float obstacleProbability = 25f;

    public Vector2Int newObstacle = new Vector2Int(0, 0);

    private int[,] grid = new int[,]
    {
        { 0, 1, 0, 0, 0 },

        { 0, 1, 0, 1, 0 },

        { 0, 0, 0, 1, 0 },

        { 0, 1, 1, 1, 0 },

        { 0, 0, 0, 0, 0 }
    };
    private void Start()
    {
        GenerateRandomGrid(width, height, obstacleProbability);
        FindPath(start, goal);

    }

    private void OnDrawGizmos()
    {
        float cellSize = 1f;

        for (int y = 0; y < grid.GetLength(0); y++)
        {

            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, 0, y * cellSize);

                Gizmos.color = grid[y, x] == 1 ? Color.black : Color.white;

                Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
            }

        }

        foreach (var step in path)
        {
            Vector3 cellPosition = new Vector3(step.x * cellSize, 0, step.y * cellSize);

            Gizmos.color = Color.blue;

            Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
        }

        Gizmos.color = Color.green;

        Gizmos.DrawCube(new Vector3(start.x * cellSize, 0, start.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));

        Gizmos.color = Color.red;

        Gizmos.DrawCube(new Vector3(goal.x * cellSize, 0, goal.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));
    }
    private bool IsInBounds(Vector2Int point)
    {
        return point.x >= 0 && point.x < grid.GetLength(1) && point.y >= 0 && point.y < grid.GetLength(0);
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {

        // Manhattan distance

        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

    }

    private void FindPath(Vector2Int start, Vector2Int goal)
    {
        var frontier = new PriorityQueue<Vector2Int, int>();

        frontier.Enqueue(start, 0);

        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        var costSoFar = new Dictionary<Vector2Int, int>();

        cameFrom[start] = start;

        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == goal)
            {
                break;
            }

            foreach (Vector2Int direction in directions)
            {

                Vector2Int next = current + direction;

                if (!IsInBounds(next) || grid[next.y, next.x] == 1)
                {
                    continue;
                }

                int newCost = costSoFar[current] + 1; // all moves cost 1

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;

                    int priority = newCost + Heuristic(next, goal);

                    frontier.Enqueue(next, priority);

                    cameFrom[next] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal))
        {

            Debug.Log("Path not found.");

            return;

        }

        // Reconstruct path
        path.Clear();

        Vector2Int step = goal;

        while (step != start)

        {

            path.Add(step);

            step = cameFrom[step];

        }

        path.Add(start);

        path.Reverse();
    }

    private void GenerateRandomGrid(int width, int height, float obstacleProbability)
    {
        int[,] newGrid = new int[width,height];
        GridCheck(newGrid, 0, 0);
        for (int i = 0; i < width - 1; i++)
        {
            for(int j = 0;  j < height - 1; j++)
            {
                newGrid[i, j] = 0;
                
                if(start == new Vector2Int(i, j))
                {
                    continue;
                }

                if(goal == new Vector2Int(i, j))
                {
                    continue;
                }

                if (GridCheck(newGrid, i, j -1) == true )
                {
                    if (newGrid[i, j - 1] == 1)
                    {
                        continue;
                    }
                }

                if(GridCheck(newGrid, i, j + 1) == true)
                {
                    if(newGrid[i, j + 1] == 1)
                    {
                        continue;
                    }
                }

                if(GridCheck(newGrid, i - 1, j) == true)
                {
                    if (newGrid[i - 1, j] == 1)
                    {
                        continue;
                    }
                }

                if(GridCheck(newGrid, i + 1, j) == true)
                {
                    if (newGrid[i + 1, j] == 1)
                    {
                        continue;
                    }
                }

                if (GridCheck(newGrid, i + 1, j + 1) == true)
                {
                    if (newGrid[i + 1, j] == 1)
                    {
                        continue;
                    }
                }

                if (GridCheck(newGrid, i + 1, j - 1) == true)
                {
                    if (newGrid[i + 1, j] == 1)
                    {
                        continue;
                    }
                }

                if (GridCheck(newGrid, i - 1, j - 1) == true)
                {
                    if (newGrid[i - 1, j] == 1)
                    {
                        continue;
                    }
                }

                if (GridCheck(newGrid, i - 1, j + 1) == true)
                {
                    if (newGrid[i - 1, j] == 1)
                    {
                        continue;
                    }
                }

                int selectedNum = Random.Range(0, 101);

                if(selectedNum <= obstacleProbability)
                {
                    newGrid[i, j] = 1;   
                }
            }
        }

        grid = newGrid;
    }

    private bool GridCheck(int[,] gridToCheck, int horiSlot, int vertiSlot)
    {
        //Debug.Log(gridToCheck.GetLength(0));
        //Debug.Log(gridToCheck.GetLength(1));
        if (horiSlot >= 0 && horiSlot < gridToCheck.GetLength(0))
        {
            if(vertiSlot >= 0 && vertiSlot < gridToCheck.GetLength(1))
            {
                return true;
            }
        }

        //Debug.Log("Out of range!");
        return false;
    }

    public void UIAddObstacle()
    {
        AddObstacle(newObstacle);
    }



    public void AddObstacle(Vector2Int position)
    {
        if(GridCheck(grid, position.x, position.y))
        {
            if (start == new Vector2Int(position.x, position.y))
            {
                Debug.LogError("Attempting to place an obstacle over the start position!");
            }
            else if (goal == new Vector2Int(position.x, position.y))
            {
                Debug.LogError("Attempting to place an obstacle over the goal position!");
            }
            else
            {
                grid[position.x, position.y] = 1;
                FindPath(start, goal);
            }
        }
        else
        {
            Debug.LogError("Invalid grid position entered!");
        }
    }
}
