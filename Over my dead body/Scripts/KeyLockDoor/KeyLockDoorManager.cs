using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class KeyLockDoorManager : SingletonMonoBehaviour<KeyLockDoorManager>
{
    [SerializeField, Header("ドアのカギをリセットする時間：2.0秒以上推奨")]
    private float ResetTime = 3.0f;
    [SerializeField]
    private List<GameObject> keyLockDoors = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DoorResetStart();   
    }

    public void DoorResetStart()
    {
        StartCoroutine(DoorReset());
    }

    IEnumerator DoorReset()
    {
        yield return new WaitForSeconds(ResetTime);
        foreach (var doorObj in keyLockDoors)
        {
            if (doorObj == null)
            {
                keyLockDoors.Remove(doorObj);
                continue;
            }
            var door = doorObj.GetComponent<KeyLockDoor>();

            if (door.isUnLock && door != null)
            {
                door.isUnLock = false;
                door.keyObj.SetActive(true);
            }
        }
    }
}
