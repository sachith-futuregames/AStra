using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    private AStar Pathfinder;
    Stack<PathNode> currentPath;
    public Transform SpriteTransform;
    public bool AllowDiagonal;

    private void Awake()
    {
        Pathfinder = GetComponent<AStar>();
        currentPath = new Stack<PathNode>();
        AllowDiagonal = true;
    }

    private void Update()
    {
        if(currentPath != null && currentPath.Count > 0)
        {
            Vector3 ToDest = (Pathfinder.playerMap.TileMap.GetCellCenterWorld(currentPath.Peek().Location) - transform.position);
            if (ToDest.magnitude <= 0.2f)
            {
                currentPath.Pop();
            }
            else
            {
                //transform.position = Vector3.Lerp(transform.position, Pathfinder.playerMap.TileMap.GetCellCenterWorld(currentPath.Peek().Location), Time.deltaTime);
                transform.Translate(ToDest.normalized * 2f * Time.deltaTime);
                Quaternion ToRot = Quaternion.LookRotation(Vector3.forward, ToDest);
                SpriteTransform.rotation = Quaternion.RotateTowards(SpriteTransform.rotation, ToRot, 700 * Time.deltaTime);


            }
        }
        
    }

    public void TileSelection(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 Worldpos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int TilePos = Pathfinder.playerMap.TileMap.WorldToCell(Worldpos);
        if(TilePos.x >= 0 && TilePos.y >= 0 && TilePos.x < Pathfinder.Width && TilePos.y < Pathfinder.Height)
        {
            currentPath = new Stack<PathNode>();
            Stopwatch W = new Stopwatch();
            W.Start();
            currentPath = Pathfinder.GetPath(Pathfinder.playerMap.TileMap.WorldToCell(transform.position), TilePos, AllowDiagonal);
            W.Stop();
            UnityEngine.Debug.Log(W.ElapsedMilliseconds);
        }
        //else
        //{
        //    currentPath = new Stack<PathNode>();
        //}
        
        
    }

    public void SetDiagonal(bool Diagonal)
    {
        AllowDiagonal = Diagonal;
    }

}
