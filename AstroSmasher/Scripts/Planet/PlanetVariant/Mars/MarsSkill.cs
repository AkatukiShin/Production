using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MarsSkill", menuName = "ScriptableObjects/MarsSkill")]
public class MarsSkill : PlanetSkills
{
    private GameObject mars;
    private GameObject skillRange;

    public override void Activate(GameObject user, GameObject target)
    {
        skillRange = user.gameObject.transform.GetChild(0).gameObject;
        skillRange.gameObject.SetActive(true);
    }

    public override void InActivate(GameObject user, GameObject target)
    {
        skillRange.gameObject.SetActive(false);
    }
}
