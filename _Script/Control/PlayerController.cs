using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public enum ControlState { Wait, Play, Move, Attack, Map }

    public static ControlState waitState = ControlState.Wait;

    [HideInInspector]public CameraManager camManager;
    Camera cam;
    //Cam

    public Transform tileParent;
    private SpriteRenderer oldRender;
    public Transform targetTile;
    //Tile

    public Transform unitOnControl;
    float speed = 5;
    Vector3 targetDir = Vector3.zero;
    Vector3 lookDir = Vector3.zero;
    public UnitMoveControl unitCtrl;
    //Unit

    public GameObject tileInfoPanel;
    public GameObject unitInfoPanel1;
    public GameObject unitInfoPanel2;
    public Text tileInfoText;
    public Text[] unitInfoText1;
    public Slider unitHealth1;
    public Text[] unitInfoText2;
    public Slider unitHealth2;
    public GameObject controlPanel;
    public GameObject[] actionButton;
    public GameObject backPanel;
    //UI

    BattleManager battle;
    //Battle

    [HideInInspector]public TurnBaseManager turn;
    //Turn

    public ControlState state;

    void Start()
    {
        state = ControlState.Wait;
        unitCtrl = this.GetComponent<UnitMoveControl>();
        camManager = this.GetComponent<CameraManager>();
        battle = this.GetComponent<BattleManager>();
        turn = this.GetComponent<TurnBaseManager>();
        controlPanel.SetActive(true);

        cam = camManager.cam;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {

                unitCtrl.targetTile = hit.transform;
                targetTile = hit.transform;

                //State Map
                if (state == ControlState.Map && !camManager.camMoving)
                {

                    if (oldRender != null)
                        oldRender.color = new Color(255, 255, 255);

                    //Get Tile Selected
                    SpriteRenderer sprLine = hit.transform.GetChild(0).GetComponent<SpriteRenderer>();
                    oldRender = sprLine;
                    sprLine.color = new Color(0, 255, 0);

                    //Move Camera
                    StartCoroutine(camManager.LerpCam(cam.transform.position, camManager.camPosStatic(targetTile.position)));

                    //Get Tile Info
                    TileData tileInfo = hit.transform.GetComponent<TileData>();

                    //Get Unit Info
                    if (tileInfo.haveUnit)
                    {
                        unitUIInfo(tileInfo.UnitOn.GetComponent<UnitInfo>(), null, true);
                    }
                    else
                    {
                        unitUIInfo(null, null, false);
                    }

                    //UI Manage
                    tileInfoPanel.SetActive(true);
                    tileInfoText.text = GetThing.getTileType(targetTile);
                    
                }
                else if (state == ControlState.Move)
                {
                    if (unitCtrl.targetTile.GetComponentInParent<TileData>().canMoveTo)
                    {
                        //print(targetTile.GetComponentInParent<TileData>().canMoveTo);
                        TileData data = unitOnControl.GetComponent<UnitInfo>().tileOn.GetComponent<TileData>();
                        data.haveUnit = false;
                        data.canMoveTo = true;
                        unitCtrl.allPath = PathManager.DoMovePath(unitOnControl.GetComponent<UnitInfo>().tileOn, targetTile, unitOnControl, this.GetComponent<PlayerController>());
                        unitCtrl.unit.GetComponent<UnitInfo>().isMoving = true;
                        PathManager.DoHideMovableTile();
                        controlPanel.SetActive(false);
                    }

                }
                else if (state == ControlState.Attack)
                {
                    if (targetTile.GetComponent<TileData>().UnitOn != null)
                    {
                        PathManager.DoHideMovableTile();
                        Transform unitTarget = targetTile.GetComponent<TileData>().UnitOn;
                        battle.DoBattle(unitOnControl, unitTarget);
                        turn.FinishAction();
                        controlPanel.SetActive(true);
                    }
                }
            }

        }
    }

    public void canMakeMove(bool can)
    {
        if (can)
        {
            state = ControlState.Play;
            controlPanel.SetActive(true);
        }
        else
        {
            state = ControlState.Wait;
            controlPanel.SetActive(false);
        }
    }

    //Button
    public void MoveButton()
    {
        if (state == ControlState.Play)
        {
            state = ControlState.Move;
            if (TileManager.spawnedTileAction.Count == 0)
            {
                PathManager.MakeTileList(allTile(), unitOnControl.GetComponent<UnitInfo>().tileOn, unitOnControl, "move");
                PathManager.DoShowActionTile("move");
            }
        }
    }

    public void AttackButton()
    {
        if (state == ControlState.Play)
        {
            state = ControlState.Attack;
            if (TileManager.spawnedTileAction.Count == 0 && PathManager.MakeTileList(allTile(), unitOnControl.GetComponent<UnitInfo>().tileOn, unitOnControl, "attack").Count > 0)
            {
                PathManager.MakeTileList(allTile(), unitOnControl.GetComponent<UnitInfo>().tileOn, unitOnControl, "attack");
                PathManager.DoShowActionTile("attack");
            }
        }
    }

    public void MapButton()
    {
        if (state == ControlState.Play)
        {
            state = ControlState.Map;
            controlPanel.SetActive(false);
            backPanel.SetActive(true);
        }
    }

    public void SkipButton()
    {
        if (state == ControlState.Play)
        {
            turn.FinishAction();
            print("Skip");
        }
    }

    public void BackButton()
    {
        state = ControlState.Play;
        backPanel.SetActive(false);
        controlPanel.SetActive(true);
        StartCoroutine(camManager.LerpCam(cam.transform.position, camManager.camPosStatic(unitOnControl.position)));
    }

    public void unitUIInfo(UnitInfo uInfo1, UnitInfo uInfo2, bool show)
    {
        UnitInfo[] uInfo = { uInfo1, uInfo2 };
        GameObject[] panel = { unitInfoPanel1, unitInfoPanel2 };
        Text[][] text = { unitInfoText1, unitInfoText2 };
        Slider[] slider = { unitHealth1, unitHealth2 };
        int count = (uInfo[0] != null) && (uInfo[1] != null) ? 2 : (uInfo[0] != null) || (uInfo[1] != null) ? 1 : 0;
        if (show)
        {

            if (count == 2)
            {
                unitInfoPanel1.GetComponent<RectTransform>().anchoredPosition = new Vector3(-537, 0);
                unitInfoPanel2.GetComponent<RectTransform>().anchoredPosition = new Vector3(-262, 0);
            }
            else
            {
                print("Print Unit");
                unitInfoPanel1.GetComponent<RectTransform>().anchoredPosition = new Vector3(-95, -50, 0);
            }
            //Change Pos
            for (int i = 0; i < count; i++)
            {
                text[i][0].text = "Unit : " + uInfo[i].unitName;
                text[i][1].text = "ATK(" + uInfo[i].attack + ")";
                text[i][2].text = "DEF(" + uInfo[i].armor + ")";
                text[i][3].text = uInfo[i].health + " / " + uInfo[i].maxHealth;
                slider[i].value = uInfo[i].health / uInfo[i].maxHealth;
                panel[i].SetActive(true);
            }
        }
        else if (!show)
        {
            unitInfoPanel1.SetActive(false);
            unitInfoPanel2.SetActive(false);
        }

    }

    public void playerUIManage()
    {
        if(PathManager.MakeTileList(allTile(), unitOnControl.GetComponent<UnitInfo>().tileOn, unitOnControl, "attack").Count > 0)
        {
            actionButton[0].SetActive(true);
        }
        else
        {
            actionButton[0].SetActive(false);
        }
        if (!unitOnControl.GetComponent<UnitInfo>().moved)
        {
            actionButton[1].SetActive(true);
        }
        else
        {
            actionButton[1].SetActive(false);
        }
    }

    public void showUnitControlData()
    {

    }

    public List<Transform> allTile()
    {
        List<Transform> allTiles = new List<Transform>();
        foreach (Transform tile in tileParent)
        {
            if (tile.tag == "Tile")
            {
                allTiles.Add(tile);
            }
        }
        return allTiles;
    }
   

}
