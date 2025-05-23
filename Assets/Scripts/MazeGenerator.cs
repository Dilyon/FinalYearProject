using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Settings")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public float wallHeight = 1f;
    public float floorThickness = 0.25f;

    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject goalPrefab;

    [Header("Goal Settings")]
    public float goalHeight = 0.1f;
    public Color goalColor = Color.green;

    [Header("Ceiling Settings")]
    public bool addCeiling = true;
    public bool ceilingVisible = false; // Set default to false for WebGL
    public float ceilingTransparency = 0.0f; // 0 = fully transparent, 1 = fully opaque

    private bool[,] visited;
    private Cell[,] maze;
    private Stack<Vector2Int> stack = new Stack<Vector2Int>();
    private Vector2Int startPosition;
    private GameObject goalMarker;

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
        PlaceGoal();

        // Always ensure ceiling is invisible in WebGL builds
#if UNITY_WEBGL
        DisableCeilings();
#endif
    }

    // New method to force disable all ceilings
    public void DisableCeilings()
    {
        Transform mazeTransform = transform.Find("Maze");
        if (mazeTransform != null)
        {
            foreach (Transform child in mazeTransform)
            {
                // Check if this is a ceiling by its Y position
                if (child.position.y > wallHeight / 2)
                {
                    MeshRenderer renderer = child.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.enabled = false;
                    }
                }
            }
        }
    }

    void PlaceGoal()
    {
        // Calculate the center position of the maze
        int centerX = width / 2;
        int centerZ = height / 2;

        // Calculate the world position for the goal, accounting for floor thickness
        Vector3 goalPosition = new Vector3(
            centerX * cellSize,
            floorThickness / 2 + goalHeight, // Place just above the floor surface
            centerZ * cellSize
        );

        // Instantiate the goal marker
        if (goalPrefab != null)
        {
            goalMarker = Instantiate(goalPrefab, goalPosition, Quaternion.identity);

            // Scale the goal marker to fit within the cell
            float goalScale = cellSize * 0.8f; // Make it slightly smaller than the cell
            goalMarker.transform.localScale = new Vector3(goalScale, goalHeight, goalScale);

            // Set the color
            Renderer renderer = goalMarker.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = goalColor;
            }

            // Make the goal marker a child of the maze
            goalMarker.transform.parent = transform.Find("Maze");
        }
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

        // Create floor and ceiling
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Create floor
                Vector3 floorPosition = new Vector3(x * cellSize, 0, z * cellSize);
                GameObject floor = Instantiate(floorPrefab, floorPosition, Quaternion.identity, mazeParent.transform);
                floor.transform.localScale = new Vector3(cellSize, floorThickness, cellSize);

                // Create ceiling (skipped in WebGL builds)
#if !UNITY_WEBGL
                if (addCeiling)
#endif
                {
                    Vector3 ceilingPosition = new Vector3(x * cellSize, wallHeight, z * cellSize);
                    GameObject ceiling = Instantiate(floorPrefab, ceilingPosition, Quaternion.identity, mazeParent.transform);
                    ceiling.transform.localScale = new Vector3(cellSize, floorThickness, cellSize);

                    // Get the MeshRenderer component and set its enabled state
                    MeshRenderer ceilingRenderer = ceiling.GetComponent<MeshRenderer>();
                    if (ceilingRenderer != null)
                    {
#if UNITY_WEBGL
                        ceilingRenderer.enabled = false; // Always false in WebGL
#else
                        ceilingRenderer.enabled = ceilingVisible;
#endif

                        // Create a new transparent material
                        Material ceilingMaterial = new Material(Shader.Find("Standard"));
                        ceilingMaterial.SetFloat("_Mode", 3); // Set to transparent mode
                        ceilingMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        ceilingMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        ceilingMaterial.SetInt("_ZWrite", 0);
                        ceilingMaterial.DisableKeyword("_ALPHATEST_ON");
                        ceilingMaterial.EnableKeyword("_ALPHABLEND_ON");
                        ceilingMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        ceilingMaterial.renderQueue = 3000;

                        // Set transparency
#if UNITY_WEBGL
                        ceilingMaterial.color = new Color(1, 1, 1, 0); // Fully transparent in WebGL
#else
                        ceilingMaterial.color = new Color(1, 1, 1, ceilingTransparency);
#endif

                        ceilingRenderer.material = ceilingMaterial;
                    }
                }

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

    // Toggle ceiling visibility at runtime
    public void ToggleCeilingVisibility()
    {
#if !UNITY_WEBGL
        ceilingVisible = !ceilingVisible;

        // Find all ceiling objects and update their visibility
        Transform mazeTransform = transform.Find("Maze");
        if (mazeTransform != null)
        {
            MeshRenderer[] ceilingRenderers = mazeTransform.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in ceilingRenderers)
            {
                // Check if this is a ceiling object
                if (renderer.transform.position.y > wallHeight / 2)
                {
                    renderer.enabled = ceilingVisible;
                }
            }
        }
#endif
    }
}