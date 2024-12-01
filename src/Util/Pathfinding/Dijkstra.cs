using System.Numerics;

namespace aoc2024.Util.Pathfinding;

public class Dijkstra
{
    /*
     * 1  function Dijkstra(Graph, source):
2      dist[source] ← 0                           // Initialization
3
4      create vertex priority queue Q
5
6      for each vertex v in Graph.Vertices:
7          if v ≠ source
8              dist[v] ← INFINITY                 // Unknown distance from source to v
9              prev[v] ← UNDEFINED                // Predecessor of v
10
11         Q.add_with_priority(v, dist[v])
12
13
14     while Q is not empty:                      // The main loop
15         u ← Q.extract_min()                    // Remove and return best vertex
16         for each neighbor v of u:              // Go through all v neighbors of u
17             alt ← dist[u] + Graph.Edges(u, v)
18             if alt < dist[v]:
19                 dist[v] ← alt
20                 prev[v] ← u
21                 Q.decrease_priority(v, alt)
22
23     return dist, prev
     */
    public static (long[,], Dictionary<ivec2, ivec2>) Solve(long[,] graph, ivec2 start)
    {
        long width = graph.GetLength(0);
        long height = graph.GetLength(1);
        long[,] dist = new long[width, height];
        Dictionary<ivec2, ivec2> prev = new();
        PriorityQueue<ivec2, long> pq = new();
        for (long x = 0; x < width; x++)
        {
            for (long y = 0; y < height; y++)
            {
                ivec2 pt = new(x, y);
                if (start.x == x && start.y == y)
                {
                    dist[x, y] = 0;
                }
                else
                {
                    dist[x, y] = long.MaxValue;
                }
            }
        }

        while (pq.Count > 0)
        {
            var u = pq.Dequeue();
            foreach (var neighbor in u.GetBoundedOrthogonalNeighbors(0, 0, (int)width - 1, (int)height - 1))
            {
                if (!prev.ContainsKey(neighbor))
                {
                    pq.Enqueue(neighbor, dist[neighbor.x, neighbor.y]);
                }

                long alt = dist[u.x, u.y] + graph[neighbor.x, neighbor.y];
                if (alt < dist[neighbor.x, neighbor.y])
                {
                    dist[neighbor.x, neighbor.y] = alt;
                    prev.TryAdd(neighbor, u);
                }
            }
        }

        return (dist, prev);
    }
}