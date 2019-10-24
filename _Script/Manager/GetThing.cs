using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetThing : MonoBehaviour
{
    // Start is called before the first frame update
    public static string getTileType(Transform tile)
    {
        if (tile.tag == "Tile")
        {
            switch (tile.GetComponent<TileData>().type)
            {
                case TileType.DEEP_WATER:
                    return "Deep Water";
                case TileType.FORREST:
                    return "Forrest";
                case TileType.GRASS:
                    return "Grassland";
                case TileType.ROCK_1:
                    return "Mountain";
                case TileType.ROCK_2:
                    return "Mountain";
                case TileType.SAND:
                    return "Desert";
                case TileType.WATER:
                    return "River";
                default:
                    return null;
            }
        }
        else
        {
            return null;
        }
    }

    public static Dictionary<string,string> getUnitInfo(Transform unit)
    {
        Dictionary<string,string> unitDataList = new Dictionary<string, string>();
        UnitInfo info = unit.GetComponent<UnitInfo>();

        unitDataList.Add("name", info.unitName.ToString());
        unitDataList.Add("health", info.health.ToString());
        unitDataList.Add("attack", info.attack.ToString());
        unitDataList.Add("armor", info.armor.ToString());
        unitDataList.Add("maxhealth", info.maxHealth.ToString());

        return unitDataList;
        
    }

    public static int getPathScore(int startX, int startY, int targetX, int targetY)
    {
        return Mathf.Abs(targetX - startX) + Mathf.Abs(targetY - startY);
    }



}
