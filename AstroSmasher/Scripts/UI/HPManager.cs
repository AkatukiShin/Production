using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HPManager : MonoBehaviour, IDamage
{
    [SerializeField] private string targetObjectTag;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private ParticleSystem[] explosion;
    

    private Parameter parameter;
    private CameraShake cameraShake;
    private GameObject targetObject; // 管理対象のオブジェクト
    private SE se;
    private float maxHp;
    private float currentHp;

    private void Start()
    {
        targetObject = GameObject.FindWithTag(targetObjectTag);

        parameter = targetObject.GetComponent<Parameter>();
        maxHp = parameter.GetHp();
        currentHp = maxHp;
        UpdateSlider();

        cameraShake = GetComponent<CameraShake>();
        //これも修正する
        //SEを鳴らすようのスクリプトを作ろう
        GameObject obj = GameObject.Find("SE");
        se = obj.GetComponent<SE>();
    }

    public GameObject GetTargetObject()
    {
        return targetObject;
    }

    public void ApplyDamage(float damage)
    {
        currentHp -= damage;
        if (currentHp <= 0) currentHp = 0;
        UpdateSlider();
        if (currentHp <= 0)
        {
            Judge.enemyKillCount++;
            PlayExplosionEffects();
            se.test();
            if (targetObject.gameObject.CompareTag("1PPlayer")) SceneManager.LoadScene("LoseScene");
            Destroy(targetObject);
            this.gameObject.SetActive(false);
            Debug.Log("hhee");
        }
    }

    public void Damaged(GameObject planet, float damageValue)
    {
        
    }
    
    private void PlayExplosionEffects()
    {
        if (explosion == null || explosion.Length == 0) return;

        foreach (var particle in explosion)
        {
            if (particle != null)
            {
                // パーティクルを targetObject の位置に生成して再生
                ParticleSystem instance = Instantiate(particle, targetObject.transform.position, Quaternion.identity);
                instance.Play();
                cameraShake.CameraShaker();
                Destroy(instance.gameObject, instance.main.duration); // パーティクルの再生が終わったら破棄
            }
        }
    }

    private void UpdateSlider()
    {
        if (hpSlider != null)
        {
            hpSlider.value = currentHp / maxHp;
        }
    }
}