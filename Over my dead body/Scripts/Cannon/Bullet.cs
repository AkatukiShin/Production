using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 3.0f;
    [HideInInspector]
    public float bulletSpeed {  get; set; }
    [HideInInspector]
    public float shotDirection { get; set; }

    private void Start()
    {
        StartCoroutine(LifeTime());
    }

    private void FixedUpdate()
    {
        BulletMovement(bulletSpeed);
    }

    /// <summary>
    /// íeÇìÆÇ©Ç∑ä÷êî
    /// </summary>
    /// <param name="speed">íeë¨</param>
    private void BulletMovement(float speed)
    {
        Vector2 vector = transform.position;
        vector.x += bulletSpeed * shotDirection;
        vector.y = transform.position.y;
        transform.position = vector;
    }

    /// <summary>
    /// íeÇÃÇ†ÇΩÇËîªíË
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.I.Dead();
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// ÉâÉCÉtÉ^ÉCÉÄ
    /// </summary>
    /// <returns></returns>
    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
