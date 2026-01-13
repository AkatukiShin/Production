using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameter : MonoBehaviour
{
    [Tooltip("惑星のHP")]
    [SerializeField] private float hp;
    [Tooltip("惑星の攻撃力")]
    [SerializeField] private float attack;
    [Tooltip("惑星のスピード")]
    [SerializeField] private float speed;
    [Tooltip("惑星の大きさ")]
    [SerializeField] private float scale;
    [Tooltip("惑星のスキル１回のコスト")]
    [SerializeField] private float skillCost;

    public float GetHp()
    {
        return hp;
    }

    public float GetAttack()
    {
        return attack;
    }

    public void SetAttack(float value)
    {
        attack *= value;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetSpeed(float value)
    {
        speed *= value;
    }

    public float GetScale()
    {
        return scale;
    }

    public float SetScale(float value)
    {
        scale *= value;
        return scale;
    }

    public float GetSkillCost()
    {
        return skillCost;
    }

}
