using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class AStar : MonoBehaviour
{
    Dictionary<int, Vector3Int> DirectionsAllowed = new Dictionary<int, Vector3Int>();
    public PathNode StartNode;
    public PathNode EndNode;
    public int Height;
    public int Width;
    public Stack<PathNode> path;
    public PlayerMap playerMap;
    private List<PathNode> OpenNodes;
    private List<PathNode> CloseNodes;

    private void Awake()
    {
        
        DirectionsAllowed.Add(1, new Vector3Int(0, 1, 0));
        DirectionsAllowed.Add(2, new Vector3Int(1, 0, 0));
        DirectionsAllowed.Add(4, new Vector3Int(0, -1, 0));
        DirectionsAllowed.Add(8, new Vector3Int(-1, 0, 0));
        
    }
    public Stack<PathNode> GetPath(Vector3Int Start, Vector3Int End, bool DiagonalAllowed = false)
    {
        playerMap.ResetPath();
        path = new Stack<PathNode>();
        StartNode = new PathNode(Start);
        EndNode = new PathNode(End);
        OpenNodes = new List<PathNode>();
        CloseNodes = new List<PathNode>();
        PathNode currentNode = StartNode;
        currentNode.GCost = 0;
        currentNode.HCost = GetHCost(Start, End);
        currentNode.currentTile = playerMap.TileMap.GetTile<Tile>(currentNode.Location);
        OpenNodes.Add(currentNode);
        while (currentNode != EndNode && OpenNodes.Count > 0)
        {
            GetNeighBours(currentNode, DiagonalAllowed);
            OpenNodes.Remove(currentNode);
            CloseNodes.Add(currentNode);
            OpenNodes.Sort();
            if(OpenNodes.Count > 0)
            {
                currentNode = OpenNodes[0];
                currentNode.currentTile = playerMap.TileMap.GetTile<Tile>(currentNode.Location);
            }
        }
        if (currentNode == EndNode)
        {
            while (currentNode != StartNode)
            {
                path.Push(currentNode);
                playerMap.TileMap.SetTile(currentNode.Location, playerMap.PathTile);
                currentNode = currentNode.Previous;
            }
            return path;
        }
        else
        {
            playerMap.TileMap.SetTile(EndNode.Location, playerMap.UnreachableTile);
            return null;
        }
    }

    private void GetNeighBours(PathNode CurrentTile, bool DiagonalAllowed = false)
    {
        for(int i = 0; i < 4; ++i)
        {
            Vector3Int Target = CurrentTile.Location + DirectionsAllowed[((int)Mathf.Pow(2, i))];
            CheckAddNeighbour(CurrentTile, Target, false);

            if(DiagonalAllowed)
            {
                int diagIndex = (i + 1) % 4;
                
                Target = CurrentTile.Location + DirectionsAllowed[((int)Mathf.Pow(2, i))] + DirectionsAllowed[((int)Mathf.Pow(2, diagIndex))];
                
                CheckAddNeighbour(CurrentTile, Target, true);
            }
        }
    }

    private int GetHCost(Vector3Int Start, Vector3Int End)
    {
        return ((int)Mathf.Abs((Start.x - End.x) + (int)Mathf.Abs(Start.y - End.y)));
    }

    private void SortedAdd(ref List<PathNode> OutList, PathNode InNode)
    { 
        for(int i = 0; i < OutList.Count; ++i)
        {
            if(OutList[i].CompareTo(InNode) > 0)
            {
                OutList.Insert(i, InNode);
                return;
            }
        }
        OutList.Add(InNode);
    }

    private void OnDrawGizmos()
    {
        if(path!= null && path.Count > 0)
        {
            foreach (var node in path)
            {
                Vector3 LineStart = playerMap.TileMap.GetCellCenterWorld(node.Location); 
                LineStart.z = 2;
                if(node.Previous!= null)
                {
                    Vector3 LineEnd = playerMap.TileMap.GetCellCenterWorld(node.Previous.Location);
                    LineEnd.z = 2;
                    Debug.DrawLine(LineStart, LineEnd, Color.red, 0.0001f);
                }
            }
        }
    }

    private void CheckAddNeighbour(PathNode CurrentTile, Vector3Int Target, bool Diagonal)
    {
        if (Target.x >= 0 && Target.y >= 0 && Target.x < Width && Target.y < Height) // Check if within Bounds
        {
            PathNode currentinQ = new PathNode(Target);
            currentinQ.Previous = CurrentTile;
            if (!playerMap.BlockingTiles.Contains((Tile)playerMap.TileMap.GetTile(Target))! && !CloseNodes.Contains(currentinQ) && !OpenNodes.Contains(currentinQ))
            {
                if (playerMap.ValidTiles.Contains((Tile)playerMap.TileMap.GetTile(Target)))
                {
                    currentinQ.GCost = CurrentTile.GCost + (Diagonal ? 14 : 10);
                    currentinQ.HCost = GetHCost(currentinQ.Location, EndNode.Location);
                    SortedAdd(ref OpenNodes, currentinQ);
                }

            }
        }
    }
}
