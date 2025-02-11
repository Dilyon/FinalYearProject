using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Settings")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 2f;
    public float wallHeight = 2f;

    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject ballPrefab;

    [Header("Ball Settings")]
    public float ballSpawnHeight = 1f;

    private bool[,] visited;
    private Cell[,] maze;
    private Stack<Vector2Int> stack = new Stack<Vector2Int>();
    private Vector2Int startPosition;

    private class Cell
    {
        public bool northWall = true;
        public bool southWall = true;
        public bool eastWall = true;
        public bool westWall = true;
    }

    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        // Ensure width and height are odd numbers to have a proper center cell
        width = width % 2 == 0 ? width + 1 : width;
        height = height % 2 == 0 ? height + 1 : height;

        // Initialize arrays
        visited = new bool[width, height];
        maze = new Cell[width, height];

        // Initialize cells
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                maze[x, z] = new Cell();
            }
        }

        // Create a clear starting area in the exact center
        int centerX = width / 2;
        int centerZ = height / 2;
        CreateClearArea(centerX, centerZ);

        // Start maze generation from just outside the clear area
        startPosition = new Vector2Int(centerX + 1, centerZ);
        Vector2Int currentPos = startPosition;
        visited[currentPos.x, currentPos.y] = true;
        stack.Push(currentPos);

        // Mark the clear area as visited
        MarkClearAreaAsVisited(centerX, centerZ);

        while (stack.Count > 0)
        {
            currentPos = stack.Peek();
            List<Vector2Int> unvisitedNeighbors = GetUnvisitedNeighbors(currentPos);

            if (unvisitedNeighbors.Count > 0)
            {
                Vector2Int nextPos = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];
                RemoveWallsBetween(currentPos, nextPos);
                visited[nextPos.x, nextPos.y] = true;
                stack.Push(nextPos);
            }
            else
            {
                stack.Pop();
            }
        }

        BuildMaze();
        SpawnBall();
    }

    void CreateClearArea(int centerX, int centerZ)
    {
        // Clear the center cell completely
        Cell centerCell = maze[centerX, centerZ];
        centerCell.northWall = false;
        centerCell.southWall = false;
        centerCell.eastWall = false;
        centerCell.westWall = false;

        // Clear adjacent cells' walls that face the center
        if (IsInBounds(new Vector2Int(centerX - 1, centerZ)))
            maze[centerX - 1, centerZ].eastWall = false;
        if (IsInBounds(new Vector2Int(centerX + 1, centerZ)))
            maze[centerX + 1, centerZ].westWall = false;
        if (IsInBounds(new Vector2Int(centerX, centerZ - 1)))
            maze[centerX, centerZ - 1].northWall = false;
        if (IsInBounds(new Vector2Int(centerX, centerZ + 1)))
            maze[centerX, centerZ + 1].southWall = false;
    }

    void MarkClearAreaAsVisited(int centerX, int centerZ)
    {
        // Mark the center and adjacent cells as visited
        visited[centerX, centerZ] = true;
        if (IsInBounds(new Vector2Int(centerX - 1, centerZ)))
            visited[centerX - 1, centerZ] = true;
        if (IsInBounds(new Vector2Int(centerX + 1, centerZ)))
            visited[centerX + 1, centerZ] = true;
        if (IsInBounds(new Vector2Int(centerX, centerZ - 1)))
            visited[centerX, centerZ - 1] = true;
        if (IsInBounds(new Vector2Int(centerX, centerZ + 1)))
            visited[centerX, centerZ + 1] = true;
    }

    void SpawnBall()
    {
        if (ballPrefab != null)
        {
            // Calculate exact center of the maze
            float centerX = (width / 2) * cellSize;
            float centerZ = (height / 2) * cellSize;

            // Create ball and position it immediately at the correct height
            Vector3 spawnPosition = new Vector3(centerX, ballSpawnHeight, centerZ);
            GameObject ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);

            // Add BallController if it doesn't exist
            if (!ball.GetComponent<BallController>())
            {
                ball.AddComponent<BallController>();
            }

            // Configure Rigidbody
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = ball.AddComponent<Rigidbody>();
            }
            // Improved physics settings
            rb.mass = 1f;                    // Lighter mass for better control
            rb.drag = 0.5f;                  // Add some air resistance
            rb.angularDrag = 0.5f;           // Smooth out rotation
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;  // Smoother movement
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;  // Better collision detection
            rb.maxAngularVelocity = 7f;      // Limit maximum rotation speed

            // Configure SphereCollider
            SphereCollider sphereCollider = ball.GetComponent<SphereCollider>();
            if (sphereCollider == null)
            {
                sphereCollider = ball.AddComponent<SphereCollider>();
            }
            sphereCollider.radius = 0.5f;
            sphereCollider.isTrigger = false;

            ball.transform.parent = transform;

            // Reset physics state
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.WakeUp();
        }
        else
        {
            Debug.LogWarning("Ball prefab not assigned in MazeGenerator!");
        }

    }
    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),  // North
            new Vector2Int(0, -1), // South
            new Vector2Int(1, 0),  // East
            new Vector2Int(-1, 0)  // West
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int newPos = pos + dir;
            if (IsInBounds(newPos) && !visited[newPos.x, newPos.y])
            {
                neighbors.Add(newPos);
            }
        }

        return neighbors;
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    void RemoveWallsBetween(Vector2Int current, Vector2Int next)
    {
        int dx = next.x - current.x;
        int dy = next.y - current.y;

        if (dx == 1) // Moving east
        {
            maze[current.x, current.y].eastWall = false;
            maze[next.x, next.y].westWall = false;
        }
        else if (dx == -1) // Moving west
        {
            maze[current.x, current.y].westWall = false;
            maze[next.x, next.y].eastWall = false;
        }
        else if (dy == 1) // Moving north
        {
            maze[current.x, current.y].northWall = false;
            maze[next.x, next.y].southWall = false;
        }
        else if (dy == -1) // Moving south
        {
            maze[current.x, current.y].southWall = false;
            maze[next.x, next.y].northWall = false;
        }
    }

    void BuildMaze()
    {
        GameObject mazeParent = new GameObject("Maze");
        mazeParent.transform.parent = transform;


        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * cellSize, 0, z * cellSize);
                GameObject floor = Instantiate(floorPrefab, position, Quaternion.identity, mazeParent.transform);
                floor.transform.localScale = new Vector3(cellSize, 0.1f, cellSize);

                Cell cell = maze[x, z];

                if (cell.northWall)
                    CreateWall(new Vector3(x * cellSize, 0, (z + 0.5f) * cellSize), Quaternion.identity, mazeParent.transform);

                if (cell.southWall)
                    CreateWall(new Vector3(x * cellSize, 0, (z - 0.5f) * cellSize), Quaternion.identity, mazeParent.transform);

                if (cell.eastWall)
                    CreateWall(new Vector3((x + 0.5f) * cellSize, 0, z * cellSize), Quaternion.Euler(0, 90, 0), mazeParent.transform);

                if (cell.westWall)
                    CreateWall(new Vector3((x - 0.5f) * cellSize, 0, z * cellSize), Quaternion.Euler(0, 90, 0), mazeParent.transform);
            }
        }

    }

    void CreateWall(Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject wall = Instantiate(wallPrefab, position, rotation, parent);
        wall.transform.localScale = new Vector3(cellSize, wallHeight, 0.1f);
    }
}