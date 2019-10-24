using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour
{

    public TileType type;

    public Vector2 GridPos;

    public bool haveUnit;
    public Transform UnitOn;
    public bool canMoveTo = false;

    private void Start()
    {
        GridPos.x = (int)transform.position.x;
        GridPos.y = (int)transform.position.z;
        this.name = "[" + GridPos.x + "," + GridPos.y + "]";
    }


}
