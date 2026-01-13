using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public static CollisionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void HandleCollision(GameObject obj1, GameObject obj2, float speed1, float speed2)
    {
        Debug.Log($"HandleCollision called with {obj1.name} and {obj2.name}");
        Debug.Log($"Speeds: obj1 = {speed1}, obj2 = {speed2}");
        
        float speedDifference = Mathf.Abs(speed1 - speed2);

        // どちらが速いかを判定
        if (speed1 > speed2)
        {
            Parameter parameter = obj1.GetComponent<Parameter>();
            Rigidbody rigidbody = obj2.GetComponent<Rigidbody>();

            if (rigidbody != null)
            {
                // 吹き飛ばす方向を計算（相手が遅い場合）
                Vector3 forceDirection = (obj2.transform.position - obj1.transform.position).normalized;
                rigidbody.AddForce(forceDirection * speedDifference * 3, ForceMode.Impulse);
            }
            ApplyDamageToTarget(obj2, parameter.GetAttack());
        }
        else if (speed1 < speed2)
        {
            Parameter parameter = obj2.GetComponent<Parameter>();
            Rigidbody rigidbody = obj1.GetComponent<Rigidbody>();

            if (rigidbody != null)
            {
                // 吹き飛ばす方向を計算（自分が遅い場合）
                Vector3 forceDirection = (obj1.transform.position - obj2.transform.position).normalized;
                rigidbody.AddForce(forceDirection * speedDifference * 3, ForceMode.Impulse);
            }

            ApplyDamageToTarget(obj1, parameter.GetAttack());
        }
        else
        {
            Debug.Log($"{obj1.name} and {obj2.name} have the same speed. No damage applied.");
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