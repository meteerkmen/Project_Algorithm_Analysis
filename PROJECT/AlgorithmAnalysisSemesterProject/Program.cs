using AlgorithmAnalysisSemesterProject;
using System;
using System.Collections;
using System.IO;

static class Program
{
    public static readonly float BANDWITH_DEMAND = 5;

    static void PrintPath(int[] path)
    {
        for (int i = 0; i < path.Length; i++)
        {
            Console.Write(path[i] + (i < path.Length - 1 ? " -> " : "\n"));
        }
    }

    static float CalculatePathDistance(int[] path, float[][] adjMatrix, Request request)
    {
        // Here we calculate distance of the path from sourceNode to targetNode
        float distance = 0;
        int currentNodeId = request.sourceNodeId;
        for (int i = 1; i < path.Length; i++)
        {
            distance += adjMatrix[currentNodeId][path[i]];
            currentNodeId = path[i];
        }

        return distance;
    }

    static int[] ConstructPath(int[] previousRecords, Request request)
    {
        List<int> path = new List<int>();

        // Here we backtrack from targetNode to sourceNode while checking if the path is valid
        // to obtain shortest path
        int currentNode = request.targetNodeId;
        path = path.Prepend(currentNode).ToList();
        while (currentNode != -1 && currentNode != request.sourceNodeId)
        {
            int previousNode = previousRecords[currentNode];
            currentNode = previousNode;
            path = path.Prepend(currentNode).ToList();
        }

        // If last node we have visited isn't valid that means path is not valid
        if (currentNode == -1)
        {
            return null;
        }

        return path.ToArray();
    }

    static int[] Dijkstra(float[][] adjMatrix, float[][] bandwithMatrix, Request request)
    {
        int NODE_COUNT = adjMatrix.Length;
        // To keep nodes to visit
        List<int> list = new List<int>();

        // This keeps distances from sourceNode for each node
        float[] distanceRecords = new float[NODE_COUNT];

        // This keeps from which node we came to that node in the shortest path
        int[] previousRecords = new int[NODE_COUNT];

        // For each node we initialize distance record with maximum distance value possible
        // and initialize preivous record with non valid value to indicate it isn't visited yet
        for (int i = 0; i < NODE_COUNT; i++)
        {
            if (i != request.sourceNodeId)
            {
                distanceRecords[i] = int.MaxValue;
                previousRecords[i] = -1;
            }

            // add each node to visit list
            list.Add(i);
        }

        // Source npde's distance from start(from sourceNode) is 0
        distanceRecords[request.sourceNodeId] = 0;

        // While there is some node in the list we visit them
        // (Since every node's distance from start except sourceNode's distance is max value possible
        //  we start with source node)
        while (list.Count > 0)
        {
            // Here we find node with minimum distance from start on the visit list
            float closestDistance = int.MaxValue;
            int closestNodeId = -1;
            int closestNodeIndex = -1;
            for (int i = 0; i < list.Count; i++)
            {
                int currentNodeId = list[i];
                if (distanceRecords[currentNodeId] < closestDistance)
                {
                    closestDistance = distanceRecords[currentNodeId];
                    closestNodeId = currentNodeId;
                    closestNodeIndex = i;
                }
            }

            // If closest node to sourceNode in the visit list is target node
            // we completed the job
            if (closestNodeId == request.targetNodeId)
            {
                break;
            }

            // Remove closest node from visit list
            list.RemoveAt(closestNodeIndex);

            // for each neighbour node of the closest node
            for (int i = 0; i < NODE_COUNT; i++)
            {
                // If node is itself or bandwith demand of the neighbour is not satisfied we pass
                if (i == closestNodeId || bandwithMatrix[closestNodeId][i] < BANDWITH_DEMAND) continue;

                float distanceToCurrentNode = adjMatrix[closestNodeId][i];

                // That means this node isn't neighbour of the closest node
                if (distanceToCurrentNode == 0) continue;

                if (distanceToCurrentNode == int.MaxValue) return null;

                // Accumulated distance of the neighbour node from sourceNode
                float newDistance = closestDistance + distanceToCurrentNode;

                // If path from closest node's to neighbour node is closer to sourceNode than previous path
                // we make it new path
                if (newDistance < distanceRecords[i])
                {
                    distanceRecords[i] = newDistance;
                    previousRecords[i] = closestNodeId;
                }
            }
        }

        // We construct path from calculated results
        int[] path = ConstructPath(previousRecords, request);

        return path;
    }
    // 0 -> 1 -> 2 -> 4 -> 7 -> 6 -> 5
    static float[][][] parseMatrixes(string fileContent)
    {
        // Split matrix strings by newline
        string[] matrixStrings = fileContent.Split("\n\n").Where(str => str != "").ToArray();
        // Split each matrix's string by newline to obtain rows
        string[][] matrixRows = matrixStrings.Select(str => str.Split("\n").Where(str => str != "").ToArray()).ToArray();
        // Split each matrixs rows by : to obtain values for each row
        float[][][] parsedMatrixes = matrixRows.Select(matrix => matrix.Select(matrixRow => matrixRow.Split(":").Select(strValue => float.Parse(strValue)).ToArray()).ToArray()).ToArray();

        // The result is matrixes[] -> rows[] -> values[]
        return parsedMatrixes;
    }

    static void Solution(string txtFilePath, Request request)
    {
        string fileContent = File.ReadAllText(txtFilePath);
        float[][][] parsedMatrixes = parseMatrixes(fileContent);
        
        // Since we only care about adjacency and bandwith for this solution
        // we only use them from parsed matrixes
        float[][] adjMatrix = parsedMatrixes[0];
        float[][] bandwithMatrix = parsedMatrixes[1];

        int[] shortestPath = Dijkstra(adjMatrix, bandwithMatrix, request);

        if (shortestPath == null)
        {
            Console.WriteLine("There is no path");
        }
        else
        {
            PrintPath(shortestPath);
            float pathCost = CalculatePathDistance(shortestPath, adjMatrix, request);
            Console.WriteLine($"Path cost = {pathCost} * {BANDWITH_DEMAND} = {pathCost * BANDWITH_DEMAND}");
        }
    }
    static void Main(string[] args)
    {
        Solution("C:\\Users\\user\\Desktop\\PROJECT\\8300689.txt", new Request(0, 5));
    }
}