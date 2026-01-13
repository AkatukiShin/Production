using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : MonoBehaviour
{
    [SerializeField] private Vector3 forceVector;
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = this.gameObject.GetComponent<Rigidbody>();
        rigidbody.AddForce(forceVector, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
