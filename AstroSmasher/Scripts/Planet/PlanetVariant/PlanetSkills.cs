using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/Skill")]
public abstract class PlanetSkills : ScriptableObject
{
    public string skillName; // スキル名
    public string description; // スキルの説明
    public int cost; // スキルの消費リソース（例: マナやスタミナ）

    // スキルの処理を実行する抽象メソッド
    public abstract void Activate(GameObject user, GameObject target);
    public abstract void InActivate(GameObject user, GameObject target);
}
