using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class BulletCannon : MonoBehaviour
{
    [SerializeField, Header("弾のPrefab")]
    private GameObject bulletPrefab;
    [SerializeField, Header("弾速(m/s)")]
    private float bulletSpeed = 2.0f;
    [SerializeField, Header("発射間隔")]
    private float bulletInterval = 3.0f;
    [SerializeField, Header("右発射地点")]
    private GameObject bulletRightLaunchObj;
    [SerializeField, Header("左発射地点")]
    private GameObject bulletLeftLaunchObj;
    [SerializeField, Header("出る方向が右か？")]
    private bool isRight = false;
    [SerializeField, Header("右砲身")]
    private GameObject rightBarrel;
    [SerializeField, Header("左砲身")]
    private GameObject leftBarrel;
    
    [SerializeField, Header("SE を再生する距離")]
    private float callSEDistance;

    private GameObject bulletObj;
    private int shotDirection;
    private Vector3 bulletLaunchPos;

    public void OnActive()
    {
        Initialize();
    }

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 初期化関数
    /// </summary>
    private void Initialize()
    {
        if(bulletPrefab == null)
        {
            Debug.LogError("bulletPrefabが設定されていません");
            return;
        }
        if(bulletRightLaunchObj == null)
        {
            Debug.LogError("bulletLaunchObjが設定されていません");
            return;
        }

        if(isRight) // 右から発射される場合
        {
            bulletLaunchPos = bulletRightLaunchObj.transform.position;
            leftBarrel.SetActive(false);
            shotDirection = 1;
        }
        else        // 左から発射される場合
        {
            bulletLaunchPos = bulletLeftLaunchObj.transform.position;
            rightBarrel.SetActive(false);
            shotDirection = -1;
        }
        StartCoroutine(BulletGenerator());
    }

    /// <summary>
    /// 弾をbulletIntervalの間隔で生成するコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator BulletGenerator()
    {
        yield return new WaitForSeconds(bulletInterval);
        bulletObj = Instantiate(bulletPrefab, bulletLaunchPos, Quaternion.identity);
        Bullet cpBullet = bulletObj.GetComponent<Bullet>();
        cpBullet.bulletSpeed = bulletSpeed;
        cpBullet.shotDirection = shotDirection;

        // プレイヤーが一定距離以内にいたら SE を鳴らす
        if (Player.I.gameObject != null)
        {
            var playerPos = Player.I.gameObject.transform.position;
            if (Vector3.Distance(playerPos, this.gameObject.transform.position) <= callSEDistance)
            {
                SoundManager.I.CallSE(SE.Boom, 2);
            }
        }
        

        StartCoroutine(BulletGenerator());
    }


}
