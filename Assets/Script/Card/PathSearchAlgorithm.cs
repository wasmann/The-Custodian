using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PathSearchAlgorithm : MonoBehaviour
{
    private struct NodeInfo
    {
        public Vector2 parent;
        // f = g + h
        public double f, g, h;
    };
    private bool IsValid(Vector2 pos)
    {
        return BattleData.enviromentData[pos] == 0;
    }

    private bool IsDestination(Vector2 src, Vector2 dst)
    {
        if (src == dst)
            return (true);
        else
            return (false);
    }

    private float CalculateHValue(Vector2 src, Vector2 dst)
    {
        // Return using the distance formula
        return ((float)Mathf.Sqrt(
            (src.x - dst.x) * (src.x - dst.x)
            + (src.y - dst.y) * (src.y - dst.y)));
    }

    private List<Vector2> TracePath(Dictionary<Vector2, NodeInfo> nodeInfos, Vector2Int dest)
    {
        Debug.Log("\nThe Path is ");
        int row = dest.x;
        int col = dest.y;

        List<Vector2> path = new List<Vector2>();

        while (!(nodeInfos[new Vector2(row, col)].parent.x == row
                 && nodeInfos[new Vector2(row, col)].parent.y == col))
        {
            path.Add(new Vector2(row, col));
            row = (int)nodeInfos[new Vector2(row, col)].parent.x;
            col = (int)nodeInfos[new Vector2(row, col)].parent.y;
        }

        path.Add(new Vector2(row, col));
        path.Reverse();

        for (int i = 0; i < path.Count; i++)
        {
            Debug.Log(" -> " + path[i].x + "," + path[i].y);
        }

        return path;
    }

    public List<Vector2> AStarSearch( Vector2Int src, Vector2Int dest)
    {

        Dictionary<Vector2, bool> closedList = new Dictionary<Vector2, bool>();

        for (int i = 0; i < BattleData.enviromentData.Count; i++)
        {
            closedList.Add(BattleData.enviromentData.ElementAt(i).Key, false);
        }

        // Declare a 2D array of structure to hold the details
        // of that cell
        Dictionary<Vector2, NodeInfo> cellDetails = new Dictionary<Vector2, NodeInfo>();


        for (int i = 0; i < BattleData.enviromentData.Count; i++)
        {
            NodeInfo info = new NodeInfo();
            info.f = float.MaxValue;
            info.g = float.MaxValue;
            info.h = float.MaxValue;
            info.parent = new Vector2Int(-1, -1);
            cellDetails.Add(BattleData.enviromentData.ElementAt(i).Key, info);
        }

        // Initialising the parameters of the starting node
        NodeInfo startNodeInfo = new NodeInfo();
        startNodeInfo.f = 0.0;
        startNodeInfo.g = 0.0;
        startNodeInfo.h = 0.0;
        startNodeInfo.parent = src;

        List<Vector2> openList=new List<Vector2>();

        // Put the starting cell on the open list
        openList.Add(src);

        // We set this boolean value as false as initially
        // the destination is not reached.
        bool foundDest = false;

        while (openList.Count != 0)
        {
            Vector2 now = openList[0];

            // Remove this vertex from the open list
            openList.Remove(now);

            // Add this vertex to the closed list
            closedList.Add(now, true);

            /*
             Generating all the 4 successor of this cell

             Cell-->Popped Cell (i, j)
             N -->  North       (i-1, j)
             S -->  South       (i+1, j)
             E -->  East        (i, j+1)
             W -->  West           (i, j-1)
            */

            // To store the 'g', 'h' and 'f' of the 8 successors
            double gNew, hNew, fNew;
            Vector2 next=new Vector2();
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        next = new Vector2(now.x, now.y + 1);
                        break;
                    case 1:
                        next = new Vector2(now.x, now.y -1);
                        break;
                    case 2:
                        next = new Vector2(now.x+1, now.y);
                        break;
                    case 3:
                        next = new Vector2(now.x-1, now.y + 1);
                        break;
                }

                if (IsValid(next) == true)
                {

                    if (IsDestination(next, dest) == true)
                    {
                        NodeInfo info = new NodeInfo();
                        info.parent = now;
                        cellDetails[next] = info;
                        foundDest = true;
                        return TracePath(cellDetails, dest);
                    }

                    else if (closedList[next] == false && IsValid(next) == true)
                    {
                        gNew = cellDetails[now].g + 1.0;
                        hNew = CalculateHValue(next, dest);
                        fNew = gNew + hNew;

                        if (cellDetails[next].f == float.MaxValue
                            || cellDetails[next].f > fNew)
                        {
                            openList.Add(next);
                            NodeInfo info = new NodeInfo();
                            info.f = fNew;
                            info.g = gNew;
                            info.h = hNew;
                            info.parent = now;
                            cellDetails[next] = info;
                        }
                    }
                }
            }
            if (foundDest == false)
                Debug.Log("Failed to find the Destination Cell\n");
            
        }
        return new List<Vector2>();

    }
}
