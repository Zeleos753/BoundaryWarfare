using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathManager : MonoBehaviour
{

    public static List<TileData> ActionTile = new List<TileData>();

    public static List<Transform> DoMovePath(Transform startPosTrans, Transform endPosTrans, Transform unit, PlayerController control)
    {
        //Add path
        List<Transform> allPath = new List<Transform>();
        Transform nextPos = startPosTrans;
        TileData data = startPosTrans.GetComponent<TileData>();
        TileData targetData = endPosTrans.GetComponent<TileData>();
        int x = (int)data.GridPos.x;
        int y = (int)data.GridPos.y;

        allPath.Add(nextPos);

        while (allPath.Last() != endPosTrans)
        {

            if (x < targetData.GridPos.x)
                x++;
            else if (x > targetData.GridPos.x)
                x--;
            else if (y < targetData.GridPos.y)
                y++;
            else if (y > targetData.GridPos.y)
                y--;
            else if (x == targetData.GridPos.x && y == targetData.GridPos.y)
            {
                allPath.Add(endPosTrans);
                print(endPosTrans);
                break;
            }

            string tileName = "["+ x + "," + y + "]";
            nextPos = control.allTile().Find(t => t.name.Contains(tileName));
            
            allPath.Add(nextPos);
        }
        return allPath;
    }


    public static void DoShowActionTile(string actionType)
    {
        int type = actionType == "move" ? 0 : actionType == "attack" ? 1 : 0;
        foreach (TileData tile in ActionTile)
        {
            TileManager.showTileAction(tile.transform, type);
        }

    }

    public static List<TileData> MakeTileList(List<Transform> tiles, Transform startPosTrans, Transform unit, string actionType)
    {
        TileData current = startPosTrans.GetComponent<TileData>();
        int x = (int)current.GridPos.x;
        int y = (int)current.GridPos.y;
        string tileName = null;
        int type = actionType == "move" ? 0 : actionType == "attack" ? 1 : 0;

        ActionTile = new List<TileData>();
        
        UnitInfo uInfo = unit.GetComponent<UnitInfo>();

        for (int i = 0; i < (type == 0 ? uInfo.movePath.Length : type == 1 ? uInfo.attackPath.Length : uInfo.movePath.Length); i++)
        {

            if (type == 0)
            {
                tileName = ("[" + (x + uInfo.movePath[i].x) + "," + (y + uInfo.movePath[i].y) + "]");
            }
            else if (type == 1)
            {
                tileName = ("[" + (x + uInfo.attackPath[i].x) + "," + (y + uInfo.attackPath[i].y) + "]");
            }

            var tile = tiles.Find(t => t.name.Contains(tileName));
            if (tile != null)
            {
                Transform tileObj = tile;
                if (type == 0 && !tile.GetComponent<TileData>().haveUnit)
                {
                    ActionTile.Add(tileObj.GetComponent<TileData>());
                    tileObj.GetComponent<TileData>().canMoveTo = true;
                }
                else if (type == 1 && tile.GetComponent<TileData>().haveUnit)
                {
                    if (uInfo.team != tile.GetComponent<TileData>().UnitOn.GetComponent<UnitInfo>().team)
                    {
                        ActionTile.Add(tileObj.GetComponent<TileData>());
                    }

                }

            }
        }
        return ActionTile;
    }

    public static void DoHideMovableTile()
    {
        for(int i = 0; i < ActionTile.Count; i++)
        {
            TileManager.spawnedTileAction[i].GetComponentInParent<TileData>().canMoveTo = false;
            Destroy(TileManager.spawnedTileAction[i].gameObject);
        }
        TileManager.spawnedTileAction = new List<GameObject>();
              
    }
   


}
