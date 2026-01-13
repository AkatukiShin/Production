using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WallBreakManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] explosion;
    
    private CameraShake cameraShake;
    private SE se;

    private void Start()
    {
        cameraShake = GetComponent<CameraShake>();
        
        GameObject obj = GameObject.Find("SE");
        se = obj.GetComponent<SE>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("1PPlayer"))
        {
            SceneManager.LoadScene("LoseScene");
        }
        else
        {
            PlayExplosionEffects(other.gameObject);
            cameraShake.CameraShaker();
            Destroy(other.gameObject);
            Judge.enemyKillCount++;
        }
    }
    
    private void PlayExplosionEffects(GameObject targetObject)
    {
        if (explosion == null || explosion.Length == 0) return;

        foreach (var particle in explosion)
        {
            if (particle != null)
            {
                se.test();
                // パーティクルを targetObject の位置に生成して再生
                ParticleSystem instance = Instantiate(particle, targetObject.transform.position, Quaternion.identity);
                instance.Play();
                cameraShake.CameraShaker();
                Destroy(instance.gameObject, instance.main.duration); // パーティクルの再生が終わったら破棄
            }
        }
    }
}
