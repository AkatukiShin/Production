using UnityEngine;

public class DoorSensor : MonoBehaviour
{
    private KeyLockDoor keyLockDoor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject parent = gameObject.transform.parent.gameObject;
        keyLockDoor = parent.GetComponent<KeyLockDoor>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && keyLockDoor.isUnLock)
        {
            keyLockDoor.OpenDoor();
        }
    }
}
