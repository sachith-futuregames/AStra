using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathNode : IEquatable<PathNode>, IComparable<PathNode>
{
    public Vector3Int Location;
    public PathNode Previous;
    public Tile currentTile;
    public int GCost;
    public int HCost;
    public int FCost()
    {
        return GCost + HCost;
    }

    public PathNode(Vector3Int InLocation)
    {
        Location = InLocation;
    }

    public PathNode(Vector3Int InLocation, Tile InTile)
    {
        Location = InLocation;
        currentTile = InTile;
    }

    public PathNode(Tile InTile)
    {
        currentTile = InTile;
    }
    public bool Equals(PathNode otherNode)
    {
        return (Location == otherNode.Location);
    }
    public static bool operator ==(PathNode firNode, PathNode secNode)
    {
        if (ReferenceEquals(firNode, secNode))
            return true;
        if (ReferenceEquals(firNode, null))
            return false;
        if (ReferenceEquals(secNode, null))
            return false;
        return firNode.Equals(secNode);
    }

    public static bool operator !=(PathNode firNode, PathNode secNode)
    {
        return !(firNode == secNode);
    }

    public override bool Equals(object obj) => Equals(obj as PathNode);
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = Location.GetHashCode();
            hashCode = (hashCode * 397) ^ Location.GetHashCode();
            return hashCode;
        }
    }

    public int CompareTo(PathNode other)
    {
        return FCost().CompareTo(other.FCost());
    }
}
