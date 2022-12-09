using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[RequireComponent(typeof(Tilemap))]
public class PlayerMap : MonoBehaviour
{
    public List<Tile> ValidTiles;
    public List<Tile> BlockingTiles;
    public Tile PathTile;
    public Tile UnreachableTile;
    public Tilemap TileMap;
    public int Width;
    public int Height;
    [SerializeField] private AStar PathFinding;
    //Flag Variables
    bool AllowsDiagonal;

    private void Awake()
    {
        TileMap = GetComponent<Tilemap>();
        
        
        PathFinding.Width = Width;
        PathFinding.Height = Height;
        PathFinding.playerMap = this;
    }

    public void ResetPath()
    {
        for(int i = 0; i < Width; ++i)
        {
            for(int j = 0; j < Height; ++j)
            {
                Tile Test = (Tile)TileMap.GetTile(new Vector3Int(i, j, 0));
                if(Test == PathTile)
                {
                    TileMap.SetTile(new Vector3Int(i, j, 0), ValidTiles[0]);
                }
            }
        }

    }

}
