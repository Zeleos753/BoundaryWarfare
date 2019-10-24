using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] tileActionPrefab;
    public static GameObject[] tileActions;
    /* 
     * [0] Move
     * [1] Attack
     * 
     */

    public static List<GameObject> spawnedTileAction;

    public GameObject[] tilePrefab;

    private void Awake()
    {
        tileActions = new GameObject[tileActionPrefab.Length];
        for (int i = 0; i < tileActionPrefab.Length; i++)
        {
            tileActions[i] = tileActionPrefab[i];
        }
        spawnedTileAction = new List<GameObject>();

        List<Transform> allTile = this.GetComponent<PlayerController>().allTile();
        foreach (Transform tile in allTile)
        {
            TileType type = tile.GetComponent<TileData>().type;
            GameObject tileToReplace = tilePrefab[0];
            if (type == TileType.GRASS)
            {
                tileToReplace = tilePrefab[0];
            }
            else if (type == TileType.FORREST)
            {
                tileToReplace = tilePrefab[1];
            }
            else if (type == TileType.ROCK_1)
            {
                tileToReplace = tilePrefab[2];
            }
            GameObject newTile = Instantiate(tileToReplace, tile.position, Quaternion.identity, this.GetComponent<PlayerController>().tileParent);
            Destroy(tile.gameObject);
        }

    }

    public static void showTileAction(Transform targetTiles, int tileTypes)
    {
        Vector3 tileActionPos = new Vector3(targetTiles.position.x, targetTiles.position.y + 0.35f, targetTiles.position.z);
        GameObject tileToSpawn = Instantiate(tileActions[tileTypes], tileActionPos, Quaternion.identity, targetTiles);
        spawnedTileAction.Add(tileToSpawn);
    }


    public static Vector2[] getMoveUnitMoveStyle(Transform unit)
    {
        UnitInfo info = unit.GetComponent<UnitInfo>();
        return info.movePath;

    }

}
