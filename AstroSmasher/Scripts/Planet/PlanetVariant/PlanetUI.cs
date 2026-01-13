using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlanetParamAsset")]

public class PlanetUI : ScriptableObject
{
    public List<PlanetParameters> PlanetParamList = new List<PlanetParameters>();
}

[System.Serializable]
public class PlanetParameters
{
    public string planetName;

    [SerializeField] private int hp;
    [SerializeField] private int attack;
    [SerializeField] private int speed;
}
