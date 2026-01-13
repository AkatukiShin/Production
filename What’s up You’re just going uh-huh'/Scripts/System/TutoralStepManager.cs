using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutoralStepManager : MonoBehaviour
{
    public static int tutorialStep = 0;
    
    [SerializeField] private List<TextMeshProUGUI> tutorialTexts = new List<TextMeshProUGUI>();

    void Start()
    {
        tutorialStep = 0;
    }

    void Update()
    {
        switch (tutorialStep)
        {
            case 0:
                break;
            
            case 1: 
                tutorialTexts[0].text = "四拍目に";
                tutorialTexts[1].text = "カットインが";
                tutorialTexts[2].text = "入ったら";
                tutorialTexts[3].text = "(カットイン)";
                break;
            case 2:
                tutorialTexts[0].color = new Color32(241,90,36,255);
                tutorialTexts[1].color = new Color32(241, 90, 36, 255);
                tutorialTexts[2].color = new Color32(241, 90, 36, 255);
                tutorialTexts[3].color = new Color32(66, 33, 11, 255);
                tutorialTexts[0].text = "←";
                tutorialTexts[1].text = "↑or↓";
                tutorialTexts[2].text = "→";
                tutorialTexts[3].text = "を連続入力！";
                break;
            case 3:
                tutorialTexts[0].text = "操作OK！";
                tutorialTexts[1].text = "楽しんで";
                tutorialTexts[2].text = "来てね！";
                tutorialTexts[3].text = "(方向キー入力)";
                break;

        }
    }
}
