using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    Transform Attacker;
    Transform Defender;
    UnitInfo atkInfo;
    UnitInfo defInfo;

    public void DoBattle(Transform atk, Transform def)
    {
        Attacker = atk;
        Defender = def;
        atkInfo = Attacker.GetComponent<UnitInfo>();
        defInfo = Defender.GetComponent<UnitInfo>();

        defInfo.health -= (atkInfo.attack * (1 + ((atkInfo.health / atkInfo.maxHealth))) - (defInfo.armor + (int)(defInfo.health * 0.1f))) > 0 ? (atkInfo.attack - (defInfo.armor + (int)(defInfo.health * 0.1f))) : 0;

        if (atkInfo.CheckStatus() && defInfo.CheckStatus())
        {
            atkInfo.health -= (defInfo.attack * (1 + ((defInfo.health / defInfo.maxHealth))) - (atkInfo.armor + (int)(atkInfo.health * 0.1f))) > 0 ? (defInfo.attack - (atkInfo.armor + (int)(atkInfo.health * 0.1f))) : 0;
        }

        this.GetComponent<PlayerController>().canMakeMove(true);
        this.GetComponent<PlayerController>().turn.FinishAction();

    }

}
