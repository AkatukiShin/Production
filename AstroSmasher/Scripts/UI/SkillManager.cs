using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    private GameObject player;
    private SkillBase skillBase;
    private bool doOnce = false;
    private float decreaseRate = 0.1f; // ゲージ減少速度（毎秒）

    [SerializeField] private Slider gaugeSlider; // ゲージ用のスライダー
    [SerializeField] private float increaseRate = 0.05f; // ゲージ増加速度（毎秒）

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("1PPlayer");
        skillBase = player.GetComponent<SkillBase>();
        decreaseRate = player.GetComponent<Parameter>().GetSkillCost();

        if (gaugeSlider == null)
        {
            Debug.LogError("Gauge Slider is not assigned in the Inspector!");
            return;
        }

        // スライダーの初期値を設定
        gaugeSlider.value = gaugeSlider.maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (gaugeSlider == null) return; // スライダーが設定されていない場合は処理しない
        
        if (skillBase.GetSkill())
        {
            if (skillBase.skillType == SkillBase.SkillType.Keep)    KeepSkill();
            else if (skillBase.skillType == SkillBase.SkillType.Burst)   BurstSkill();
        }
        else
        {
            // ゲージを増加させる
            gaugeSlider.value += increaseRate * Time.deltaTime;
        }

        // ゲージの範囲を制限（スライダーの minValue と maxValue に従う）
        gaugeSlider.value = Mathf.Clamp(gaugeSlider.value, gaugeSlider.minValue, gaugeSlider.maxValue);
    }

    void KeepSkill()
    {
        // ゲージを減少させる
        gaugeSlider.value -= decreaseRate * Time.deltaTime;
        if (gaugeSlider.value == 0)
        {
            skillBase.OffSkill();
        }
    }

    void BurstSkill()
    {
        if (gaugeSlider.value < decreaseRate || gaugeSlider.value == 0)
        {
            skillBase.OffSkill();
            return;
        }

        if (!doOnce)
        {
            doOnce = true;
            gaugeSlider.value -= decreaseRate;
            Debug.Log("skillOn");
            skillBase.OffSkill();
            doOnce = false;
        }
    }
}