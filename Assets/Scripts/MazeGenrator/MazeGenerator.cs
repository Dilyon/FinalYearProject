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

    private GameObject mazeParent;
    private bool[,] visited;
    private Cell[,] maze;
    private Stack<Vector2Int> stack = new Stack<Vector2Int>();

    private class Cell
    {
        public bool northWall = true;
        public bool southWall = true;
        public bool eastWall = true;
        public bool westWall = true;
    }

    void Start()
    {
        mazeParent = new GameObject("Maze");
        GenerateMaze();
        CenterMaze();
    }

    void GenerateMaze()
    {
        visited = new bool[width, height];
        maze = new Cell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                maze[x, z] = new Cell();
            }
        }

        Vector2Int startPos = new Vector2Int(width / 2, height / 2);
        visited[startPos.x, startPos.y] = true;
        stack.Push(startPos);

        while (stack.Count > 0)
        {
            Vector2Int currentPos = stack.Peek();
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

    void BuildMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * cellSize, 0, z * cellSize);

                GameObject floor = Instantiate(floorPrefab, position, Quaternion.identity, mazeParent.transform);
                floor.transform.localScale = new Vector3(cellSize, 0.1f, cellSize);

                if (!floor.GetComponent<Collider>())
                {
                    floor.AddComponent<BoxCollider>();
                }

                Cell cell = maze[x, z];

                if (cell.northWall)
                    CreateWall(new Vector3(x * cellSize, 0, (z + 0.5f) * cellSize), Quaternion.identity);

                if (cell.southWall)
                    CreateWall(new Vector3(x * cellSize, 0, (z - 0.5f) * cellSize), Quaternion.identity);

                if (cell.eastWall)
                    CreateWall(new Vector3((x + 0.5f) * cellSize, 0, z * cellSize), Quaternion.Euler(0, 90, 0));

                if (cell.westWall)
                    CreateWall(new Vector3((x - 0.5f) * cellSize, 0, z * cellSize), Quaternion.Euler(0, 90, 0));
            }
        }
    }

    void CreateWall(Vector3 position, Quaternion rotation)
    {
        GameObject wall = Instantiate(wallPrefab, position, rotation, mazeParent.transform);
        wall.transform.localScale = new Vector3(cellSize, wallHeight, 0.1f);

        if (!wall.GetComponent<Collider>())
        {
            wall.AddComponent<BoxCollider>();
        }
    }

    void CenterMaze()
    {
        Vector3 centerOffset = new Vector3(
            -width * cellSize / 2f,
            0,
            -height * cellSize / 2f
        );
        mazeParent.transform.localPosition = centerOffset;
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

        if (dx == 1)
        {
            maze[current.x, current.y].eastWall = false;
            maze[next.x, next.y].westWall = false;
        }
        else if (dx == -1)
        {
            maze[current.x, current.y].westWall = false;
            maze[next.x, next.y].eastWall = false;
        }
        else if (dy == 1)
        {
            maze[current.x, current.y].northWall = false;
            maze[next.x, next.y].southWall = false;
        }
        else if (dy == -1)
        {
            maze[current.x, current.y].southWall = false;
            maze[next.x, next.y].northWall = false;
        }
    }
}
