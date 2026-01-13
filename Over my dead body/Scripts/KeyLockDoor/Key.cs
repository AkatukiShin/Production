using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField]
    private KeyLockDoor keyLockDoor;

    private void Start()
    {
        if(keyLockDoor == null)
        {
            Debug.LogError("KeyLockDoorがNullです。この鍵で開くKeyLockDoorオブジェクトを設定してください");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
          if(collision.gameObject.CompareTag("Player") && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            keyLockDoor.isUnLock = true;
            gameObject.SetActive(false);
        }
    }
}
