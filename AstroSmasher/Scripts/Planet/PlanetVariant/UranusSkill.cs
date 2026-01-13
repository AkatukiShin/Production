using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UranusSkill", menuName = "ScriptableObjects/UranusSkill")]
public class UranusSkill : PlanetSkills
{
    public override void Activate(GameObject user, GameObject target)
    {
        Parameter parameter = user.GetComponent<Parameter>();
        parameter.SetSpeed(24);
    }

    public override void InActivate(GameObject user, GameObject target)
    {
        Parameter parameter = user.GetComponent<Parameter>();
        parameter.SetSpeed(8);
    }
}
