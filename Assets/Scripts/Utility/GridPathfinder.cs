using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridPathfinder
{
    private Grid attachedGrid;
    private Grid.CollisionFlags[,] GridCollisions { get { return attachedGrid.gridCollisions; } }
    private bool[,] visited;
    private Vector2Int[,] track;

    private Queue<Vector2Int> pathQueue;

    public GridPathfinder(Grid grid)
    {
        attachedGrid = grid;
        visited = new bool[GridCollisions.GetLength(0), GridCollisions.GetLength(1)];
        track = new Vector2Int[GridCollisions.GetLength(0), GridCollisions.GetLength(1)];
        pathQueue = new Queue<Vector2Int>();
    }

    public List<Vector2Int> GetPath(Vector2Int from, Vector2Int to)
    {
        Reset();

        Vector2Int start = GridToIndex(from);
        Vector2Int end = GridToIndex(to);

        // Check if the end is at all reachable before beginning
        if (IsOutOfBounds(end) || !IsReachable(end)) return null;

        // Get started with source node
        pathQueue.Enqueue(start);
        visited[start.x, start.y] = true;

        // Run BFS
        if (BreadthFirstSearch(end))
        {
            // Path was found, retrace steps
            List<Vector2Int> path = new List<Vector2Int>();
            Vector2Int current = end;
            while(current != start)
            {
                path.Add(IndexToGrid(current));
                current = track[current.x, current.y];
            }

            // Add the start to close the path off
            path.Add(from);
            // Reverse so the path goes start to finish
            path.Reverse();
            return path;
        }

        // Path was not found
        return null;
    }

    private bool BreadthFirstSearch(Vector2Int target)
    {
        Vector2Int current;
        while (pathQueue.Count > 0)
        {
            current = pathQueue.Dequeue();

            // Check if the target was reached
            if (current == target) return true;

            // Advance on all four directions
            BFSStep(current, Grid.CollisionFlags.North);
            BFSStep(current, Grid.CollisionFlags.South);
            BFSStep(current, Grid.CollisionFlags.East);
            BFSStep(current, Grid.CollisionFlags.West);
        }

        // Path not found
        return false;
    }

    private void BFSStep(Vector2Int current, Grid.CollisionFlags direction)
    {
        // Check if the current node has a wall in the outgoing direction
        if (!Grid.CheckCollisionFlags(GridCollisions[current.x, current.y], direction))
        {
            // Obtain next node
            Vector2Int next = NextFrom(current, direction);

            // Abort if the node is out of bounds
            if (IsOutOfBounds(next)) return;

            // Abort if the node has already been visited
            if (visited[next.x, next.y]) return;

            // Check if the next node has a wall in the incoming direction or central collision
            if (!Grid.CheckCollisionFlags(GridCollisions[next.x, next.y], Inverse(direction) | Grid.CollisionFlags.Center))
            {
                // Keep track of the path
                track[next.x, next.y] = current;
                visited[next.x, next.y] = true;

                pathQueue.Enqueue(next);
            }
        }
    }

    private Grid.CollisionFlags Inverse(Grid.CollisionFlags direction)
    {
        switch(direction)
        {
            case Grid.CollisionFlags.North:
                return Grid.CollisionFlags.South;
            case Grid.CollisionFlags.South:
                return Grid.CollisionFlags.North;
            case Grid.CollisionFlags.East:
                return Grid.CollisionFlags.West;
            case Grid.CollisionFlags.West:
                return Grid.CollisionFlags.East;
        }

        return Grid.CollisionFlags.None;
    }

    private Vector2Int NextFrom(Vector2Int current, Grid.CollisionFlags direction)
    {
        switch (direction)
        {
            case Grid.CollisionFlags.North:
                return new Vector2Int(current.x, current.y + 1);
            case Grid.CollisionFlags.South:
                return new Vector2Int(current.x, current.y - 1);
            case Grid.CollisionFlags.East:
                return new Vector2Int(current.x + 1, current.y);
            case Grid.CollisionFlags.West:
                return new Vector2Int(current.x - 1, current.y);
        }
        return current;
    }

    private bool IsReachable(Vector2Int node)
    {
        Grid.CollisionFlags flags = GridCollisions[node.x, node.y];
        if ((flags & Grid.CollisionFlags.Center) != 0) return false;
        if ((flags & Grid.CollisionFlags.AllDirections) == Grid.CollisionFlags.AllDirections) return false;
        return true;
    }

    private bool IsOutOfBounds(Vector2Int node)
    {
        return (node.x < 0 || node.x >= GridCollisions.GetLength(0) || node.y < 0 || node.y >= GridCollisions.GetLength(1));
    }

    private void Reset()
    {
        pathQueue.Clear();

        for (int x = 0; x < GridCollisions.GetLength(0); x++)
        {
            for (int y = 0; y < GridCollisions.GetLength(1); y++)
            {
                visited[x, y] = false;
            }
        }
    }

    private Vector2Int GridToIndex(Vector2Int gridPosition)
    {
        return new Vector2Int(gridPosition.x + attachedGrid.gridSize.x, gridPosition.y + attachedGrid.gridSize.y);
    }

    private Vector2Int IndexToGrid(Vector2Int gridPosition)
    {
        return new Vector2Int(gridPosition.x - attachedGrid.gridSize.x, gridPosition.y - attachedGrid.gridSize.y);
    }
}
