using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillBase : MonoBehaviour
{
    private bool onSkill = false;
    private Parameter parameter;
    [SerializeField] private PlanetSkills planetSkills;
    public SkillType skillType;

    public enum SkillType
    {
        Keep, Burst
    }
    
    // Start is called before the first frame update
    void Start()
    {
        parameter = GetComponent<Parameter>();
    }

    private void Skill()
    {
        onSkill = true;
        planetSkills.Activate(gameObject, null);
    }

    public void OffSkill()
    {
        //Debug.Log("ho");
        onSkill = false;
        planetSkills.InActivate(gameObject, null);
    }

    public void OnSkill(InputAction.CallbackContext context)
    {
        //わかりずらいから直そう
        if (context.performed)
        {
            if (onSkill) OffSkill();
            else         Skill(); 
        }
    }

    public bool GetSkill()
    {
        return onSkill;
    }
    public void SetSkill(bool value)
    {
        onSkill = value;
    }
}
