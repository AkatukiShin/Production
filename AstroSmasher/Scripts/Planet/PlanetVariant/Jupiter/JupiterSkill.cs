using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JupiterSkill", menuName = "ScriptableObjects/JupiterSkill")]
public class JupiterSkill : PlanetSkills
{
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
