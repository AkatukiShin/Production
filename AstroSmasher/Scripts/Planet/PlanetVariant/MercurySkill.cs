using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mercury", menuName = "ScriptableObjects/MercrySkill")]
public class MercurySkill : PlanetSkills
{
    public float multiplyAttack; 

    public override void Activate(GameObject user, GameObject target)
    {
        Parameter parameter = user.GetComponent<Parameter>();
        parameter.SetAttack(8);
    }

    public override void InActivate(GameObject user, GameObject target)
    {
        Parameter parameter = user.GetComponent<Parameter>();
        parameter.SetAttack(0.125f);
    }
}
