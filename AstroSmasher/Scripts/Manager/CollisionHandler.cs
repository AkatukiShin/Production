using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] private float hp = 100; // HP
    [SerializeField] private Rigidbody rb; // Rigidbody コンポーネント

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 衝突相手のオブジェクトとその Rigidbody を取得
        GameObject otherObject = collision.gameObject;
        Rigidbody otherRb = otherObject.GetComponent<Rigidbody>();

        if (otherRb != null)
        {
            // 各オブジェクトの速度を計算
            float mySpeed = rb.velocity.magnitude;
            float otherSpeed = otherRb.velocity.magnitude;

            // CollisionManager にデータを送信
            CollisionManager.Instance.HandleCollision(this.gameObject, otherObject, mySpeed, otherSpeed);
        }
    }
}