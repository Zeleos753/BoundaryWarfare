using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo : MonoBehaviour
{
    //Unit Info
    public string unitName;
    public int health;
    public int maxHealth;
    public int attack;
    public int armor;
    public string team;
    public Vector2[] movePath;
    public Vector2[] attackPath;
    //Unit Info

    //In Game Info
    public string tileID;
    public Transform tileOn;
    public bool isMoving;
    public bool canTarget;
    public bool moved;

    private void OnEnable()
    {
        health = maxHealth;
    }

    //Alive ?
    public bool CheckStatus()
    {
        if(health <= 0)
        {
            TileData tileInfo = tileOn.GetComponent<TileData>();
            tileInfo.canMoveTo = true;
            tileInfo.haveUnit = false;
            tileInfo.UnitOn = null;
            Destroy(this.gameObject);
            return false;
        }
        else
        {
            return true;
        }
    }

}
