using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public class PlanetUI
    {
        public GameObject planet;
        public Slider hpSlider;
        public Slider skillSlider;
    }
    
    [SerializeField] private List<PlanetUI> playerUIList = new List<PlanetUI>();

    
    //Planetを取得する処理を記述する
    
    public Slider GetPlanetSlider(GameObject planet, string sliderType)
    {
        foreach (var playerUI in playerUIList)
        {
            if (playerUI.planet == planet)
            {
                switch (sliderType)
                {
                    case "hp"   : 
                        return playerUI.hpSlider;
                    case "skill": 
                        return playerUI.skillSlider;
                    default     : 
                        Debug.LogError("It's a non-existent gauge");
                        break;
                }
            }
        }

        return null;
    }
}
