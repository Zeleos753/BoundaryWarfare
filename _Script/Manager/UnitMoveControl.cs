using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UnitMoveControl : MonoBehaviour
{
    public float rotateSpeed = 5;

    public PlayerController playerCtrl;

    public Transform unit;
    public Transform nextTile;
    public Transform targetTile;
    public List<Transform> allPath;

    Vector3 targetDir = Vector3.zero;
    Vector3 lookDir = Vector3.zero;

    public bool arriveDest = true;

    public CameraManager camManager;

    private void Awake()
    {
        playerCtrl = this.GetComponent<PlayerController>();
        camManager = this.GetComponent<CameraManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (unit.GetComponent<UnitInfo>().isMoving)
        {
            if (!arriveDest)
            {
                //Cam
                if (!camManager.camMoving)
                {
                    StartCoroutine(camManager.LerpCam(camManager.cam.transform.position, camManager.camPosStatic(nextTile.position)));
                    camManager.camMoving = true;
                }
                unit.position = Vector3.MoveTowards(unit.position, new Vector3(nextTile.position.x, unit.position.y, nextTile.position.z), 2 * Time.deltaTime);

                float step = rotateSpeed * Time.deltaTime;
                targetDir = new Vector3(nextTile.position.x, unit.position.y, nextTile.position.z) - unit.position;
                lookDir = Vector3.RotateTowards(unit.forward, targetDir, step, 0.0f);

                unit.rotation = Quaternion.LookRotation(lookDir);

                if (Vector3.Distance(unit.position, targetTile.position) <= 0.5)
                {
                    UnitInfo unitInfo = unit.GetComponent<UnitInfo>();
                    unitInfo.isMoving = false;
                    unitInfo.tileOn = targetTile;
                    unitInfo.tileID = targetTile.name;
                    TileData tileData = targetTile.GetComponent<TileData>();
                    tileData.UnitOn = unit;
                    tileData.canMoveTo = false;
                    tileData.haveUnit = true;
                    //Arrive Destination
                    PathManager.MakeTileList(playerCtrl.allTile(), targetTile, unit, "attack");
                    unitInfo.moved = true;

                    allPath = new List<Transform>();

                    if(PathManager.ActionTile.Count > 0)
                    {
                        playerCtrl.state = PlayerController.ControlState.Attack;
                        playerCtrl.playerUIManage();
                        playerCtrl.canMakeMove(true);
                    }
                    else
                    {
                        arriveDest = true;
                        playerCtrl.canMakeMove(true);
                        playerCtrl.turn.FinishAction();
                    }

                }
                else if(Vector3.Distance(unit.position, nextTile.position) <= 0.5)
                {
                    nextTile.GetComponent<TileData>().canMoveTo = false;
                    arriveDest = true;
                }
            }
            else if(arriveDest)
            {
                arriveDest = false;
                nextTile = allPath.First();
                allPath.Remove(allPath.First());
            }
        }
    }
}
