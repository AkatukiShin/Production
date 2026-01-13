using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mars : MonoBehaviour
{
    private Coroutine damageCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        // ダメージを与える処理を開始
        if (damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(ApplyDamageOverTime(other.gameObject));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // トリガーから出たとき、ダメージ処理を停止
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private IEnumerator ApplyDamageOverTime(GameObject target)
    {
        while (true)
        {
            ApplyDamageToTarget(target, 1); // ダメージ値を適宜変更
            yield return new WaitForSeconds(1f); // 1秒待つ
        }
    }
    
    private void ApplyDamageToTarget(GameObject target, float damage)
    {
        // すべての HPManager を取得
        HPManager[] hpManagers = FindObjectsOfType<HPManager>();

        foreach (var hpManager in hpManagers)
        {
            if (hpManager.GetTargetObject() == target)
            {
                Debug.Log($"Applying {damage} damage to {target.name}");
                hpManager.ApplyDamage(damage);
                return;
            }
        }

        Debug.LogWarning($"No HPManager found for {target.name}");
    }
}
