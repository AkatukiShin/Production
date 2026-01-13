using UnityEngine;

public class KeyLockDoor : MonoBehaviour
{
    [HideInInspector]
    public bool isUnLock = false;
    [SerializeField]
    public GameObject keyObj;

    [HideInInspector]
    public Vector3 keyPosition;

    private Key key;

    private void Start()
    {
        if(key == null)
        {
            GameObject keyObj = gameObject.transform.GetChild(1).gameObject;
            key = keyObj.GetComponent<Key>();
            keyPosition = keyObj.transform.position;
        }
        key = keyObj.GetComponent<Key>();
        keyPosition = keyObj.transform.position;
    }

    public void OpenDoor()
    {
        SoundManager.I.CallSE(SE.Interact, 1);
        Destroy(gameObject);
    }
}
