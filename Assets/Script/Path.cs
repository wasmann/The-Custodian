using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleData;
using static Unity.VisualScripting.Member;
using Vector2 = UnityEngine.Vector2;
//using Vector2 = System.Numerics.Vector2;



public class PathSearchAlgorithm
{

    public class PriorityQueue<Vector2>
    {
        public List<Tuple<Vector2, float>> elements = new List<Tuple<Vector2, float>>();
        public int Count()
        {
            return elements.Count;
        }

        public void Enqueue(Vector2 item, float priority)
        {
            elements.Add(Tuple.Create(item, priority));
        }

        public Vector2 Dequeue()
        {
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Item2 < elements[bestIndex].Item2)
                {
                    bestIndex = i;
                }
            }

            Vector2 bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
    public bool IsValid(Vector2 pos)
    {
        if (BattleData.enviromentData.ContainsKey(pos))
        {
            var a = BattleData.enviromentData[pos];
            if (a == BattleData.EnvironmentType.Obstacle)
            {
                var length = Vector2.Distance(Destination, Source);
                var side1 = Vector2.Distance(pos, Source);
                var side2 = Vector2.Distance(Destination, pos);
                if (side1 * side1 + side2 * side2 < length * length)
                    ObstacleInBetween = true;
                //if(side1 + side2 == length)
                //   ObstacleInBetween = false;
                return false;
            }
            return true;
        }
        return false;
    }
    public bool ObstacleInBetween;
    private Vector2 Destination;
    private Vector2 Source;

    private float Heuristic(Vector2 current, Vector2 destination)
    {
        // Manhattan distance
        return Math.Abs(current.x - destination.x) + Math.Abs(current.y - destination.y);

        // Octile distance
        float dx = Math.Abs(current.x - destination.x);
        float dy = Math.Abs(current.y - destination.y);
        float D = 1;
        float D2 = (float)Math.Sqrt(2);
        return D * (dx + dy) + (D2 - 2 * D) * Math.Min(dx, dy);
    }
    //...
    public List<Vector2> AStarSearch(Vector2 source, Vector2 destination)
    {
        Vector2[] directions = { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };

        Source = source;
        Destination = destination;
        int count = BattleData.enviromentData.Count;
        PriorityQueue<Vector2> queue = new PriorityQueue<Vector2>();
        List<Vector2> path = new List<Vector2>();
        bool[,] visited = new bool[count, count];
        Vector2[,] cameFrom = new Vector2[count, count];
        float[,] priority = new float[count, count];

        queue.Enqueue(source, 0);
        visited[(int)source.x, (int)source.y] = true;

        while (queue.elements.Count != 0)
        {
            Vector2 current = queue.Dequeue();

            if (current == destination)
            {
                Vector2 temp = destination;
                path.Add(temp);
                while (temp != source)
                {
                    temp = cameFrom[(int)temp.x, (int)temp.y];
                    path.Add(temp);
                }
                path.Reverse();
                return path;
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 next = new Vector2(current.x + directions[i].x, current.y + directions[i].y);

                if (IsValid(next))
                {
                    float newPriority = priority[(int)current.x, (int)current.y] + Vector2.Distance(current, next);

                    if (!visited[(int)next.x, (int)next.y] || newPriority < priority[(int)next.x, (int)next.y])
                    {
                        cameFrom[(int)next.x, (int)next.y] = current;
                        priority[(int)next.x, (int)next.y] = newPriority;
                        float priority1 = newPriority + Heuristic(next, destination);
                        queue.Enqueue(next, priority1);
                        visited[(int)next.x, (int)next.y] = true;
                    }
                }
            }
        }
        return path;
    }
}
