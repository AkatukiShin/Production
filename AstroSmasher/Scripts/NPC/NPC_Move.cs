using UnityEngine;

public class NPC_Move : MonoBehaviour
{
    [SerializeField] private float acceleration = 1.3f;
    [SerializeField] private float randomAngleRange = 90f; // 衝突後のランダム角度範囲

    private Vector3 direction;
    private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        // 初期のランダム方向を設定
        float randomAngle = Random.Range(0, 360);
        direction = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), 0, Mathf.Sin(randomAngle * Mathf.Deg2Rad));
    }

    void Update()
    {
        // 現在の方向に力を加える
        rigidbody.AddForce(direction * acceleration, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // 衝突後にランダムな角度で方向を変更
            float randomAngle = Random.Range(-randomAngleRange, randomAngleRange);
            Quaternion rotation = Quaternion.Euler(0, randomAngle, 0);
            direction = rotation * -direction; // 壁から反射するように方向を変える
            direction.Normalize();
        }
    }
}