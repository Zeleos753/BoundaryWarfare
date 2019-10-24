using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class TurnBaseManager : MonoBehaviour
{

    public List<string> teamName;
    public string turnTag;

    PlayerController playerCtrl;
    CameraManager camManager;

    public Transform aUnitParent;
    public Transform bUnitParent;
    List<Transform> aUnit = new List<Transform>();
    List<Transform> bUnit = new List<Transform>();

    CompareClass compareClass;

    public GameObject teamSwitchPanel;
    //UI

    public Vector3[] toSpawnA;
    public Vector3[] toSpawnB;
    public GameObject[] unitPrefab;
    //Spawn

    void Start()
    {
        camManager = this.GetComponent<CameraManager>();
        playerCtrl = this.gameObject.GetComponent<PlayerController>();
        compareClass = new CompareClass();

        spawnUnit();

        if (teamName.Count == 1)
        {
            teamName.Add("bot");
            turnTag = teamName[0];
            playerCtrl.state = PlayerController.ControlState.Play;

            InsertUnitToQueue(0);
            
        }
        else
        {
            InsertUnitToQueue(0);
            InsertUnitToQueue(1);
            playerCtrl.state = PlayerController.ControlState.Play;
            turnTag = teamName[Random.Range(0, 1)];
        }

        MakeAction();

    }

    public void spawnUnit()
    {
        foreach (Vector3 point in toSpawnA)
        {
            GameObject unit = Instantiate(unitPrefab[(int)point.z], new Vector3(point.x, 0.5f, point.y), Quaternion.identity, aUnitParent);
            string tileName = ("[" + point.x + "," + point.y + "]");
            unit.GetComponent<UnitInfo>().tileOn = playerCtrl.allTile().Find(t => t.name.Contains(tileName));
            unit.GetComponent<UnitInfo>().tileOn.GetComponent<TileData>().haveUnit = true;
            unit.GetComponent<UnitInfo>().tileOn.GetComponent<TileData>().UnitOn = unit.transform;
        }
        foreach (Vector3 point in toSpawnB)
        {
            GameObject unit = Instantiate(unitPrefab[(int)point.z], new Vector3(point.x, 0.5f, point.y), Quaternion.Euler(0, 180, 0), bUnitParent);
            string tileName = ("[" + point.x + "," + point.y + "]");
            unit.GetComponent<UnitInfo>().tileOn = playerCtrl.allTile().Find(t => t.name.Contains(tileName));
            unit.GetComponent<UnitInfo>().tileOn.GetComponent<TileData>().haveUnit = true;
            unit.GetComponent<UnitInfo>().tileOn.GetComponent<TileData>().UnitOn = unit.transform;
        }
    }

    public void InsertUnitToQueue(int i)
    {
        if (i == 0)
        {

            aUnit.Clear();
            foreach (Transform unit in aUnitParent)
            {
                aUnit.Add(unit);
                unit.GetComponent<UnitInfo>().moved = false;
            }
            aUnit.Sort(compareClass);
        }
        else if (i == 1)
        {

            bUnit.Clear();
            foreach (Transform unit in bUnitParent)
            {
                bUnit.Add(unit);
                unit.GetComponent<UnitInfo>().moved = false;
            }
            bUnit.Sort(compareClass);
        }
    }

    public void FinishAction()
    {
        if (unitActionQueue().Count > 0)
        {
            unitActionQueue().Remove(unitActionQueue().First());
            MakeAction();
        }
    }

    public void MakeAction()
    {
        if (unitActionQueue().Count > 0)
        {
            StartCoroutine(delayCam());
            playerCtrl.unitOnControl = unitActionQueue().First();
            playerCtrl.unitCtrl.unit = unitActionQueue().First();
            playerCtrl.playerUIManage();
        }
        else
        {
            StartCoroutine(switchTeamPanel());
        }
    }

    IEnumerator delayCam()
    {
        yield return new WaitForSeconds(0.5f);
        camManager.camMoving = false;
        StartCoroutine(camManager.LerpCam(camManager.cam.transform.position, camManager.camPosStatic(unitActionQueue().First().position)));
    }

    public string TurnSwitch()
    {
        if (turnTag == teamName[0])
        {
            //print("Bot");
            turnTag = teamName[1];
            InsertUnitToQueue(1);
        }
        else
        {
            //print("Player");
            turnTag = teamName[0];
            playerCtrl.state = PlayerController.ControlState.Play;
            playerCtrl.controlPanel.SetActive(true);
            InsertUnitToQueue(0);
        }
        return turnTag;
    }

    IEnumerator switchTeamPanel()
    {
        playerCtrl.canMakeMove(false);
        teamSwitchPanel.SetActive(true);
        TurnSwitch();
        teamSwitchPanel.GetComponentsInChildren<Text>()[0].text = turnTag;
        yield return new WaitForSeconds(1);
        teamSwitchPanel.SetActive(false);
        playerCtrl.canMakeMove(true);
        MakeAction();
        if (teamName[1] == "bot")
        {
            playerCtrl.state = PlayerController.ControlState.Wait;
            playerCtrl.controlPanel.SetActive(false);
        }
        else
        {
            playerCtrl.state = PlayerController.ControlState.Play;
            playerCtrl.controlPanel.SetActive(true);
        }

    }

    public List<Transform> unitActionQueue()
    {
        List<Transform> unitOnAction = new List<Transform>();
        if (turnTag == teamName[0])
            unitOnAction = aUnit;
        else if (turnTag == teamName[1])
            unitOnAction = bUnit;

        return unitOnAction;

    }

    //

    class CompareClass : IComparer<Transform>
    {
        public int Compare(Transform a, Transform b)
        {
            int x = a.GetComponent<UnitInfo>().health;
            int y = b.GetComponent<UnitInfo>().health;

            if (x == 0 || y == 0)
            {
                return 0;
            }

            return x.CompareTo(y);
        }
    }


}
