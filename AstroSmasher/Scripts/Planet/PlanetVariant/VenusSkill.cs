using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Venus", menuName = "ScriptableObjects/VenusSkill")]
public class VenusSkill : PlanetSkills
{
    public float multiplyAttack; 

    public override void Activate(GameObject user, GameObject target)
    {
        Parameter parameter = user.GetComponent<Parameter>();
        user.gameObject.transform.localScale = new Vector3(parameter.SetScale(0.5f), parameter.SetScale(1), parameter.SetScale(1));
    }

    public override void InActivate(GameObject user, GameObject target)
    {
        Parameter parameter = user.GetComponent<Parameter>();
        user.gameObject.transform.localScale = new Vector3(parameter.SetScale(2), parameter.SetScale(1), parameter.SetScale(1));
    }
}
