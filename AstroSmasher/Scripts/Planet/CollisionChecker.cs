using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    private GameObject hpSlider;
    private HPManager hpManager;

    private void Start()
    {
        hpSlider = GameObject.FindWithTag("HP");
        hpManager = hpSlider.GetComponent<HPManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 自分のRigidbody
        Rigidbody myRigidbody = GetComponent<Rigidbody>();
        // 衝突相手のRigidbody
        Rigidbody otherRigidbody = collision.rigidbody;

        if (myRigidbody != null && otherRigidbody != null)
        {
            // 自分と相手の速度を比較
            float mySpeed = myRigidbody.velocity.magnitude;
            float otherSpeed = otherRigidbody.velocity.magnitude;

            // 結果をログに表示
            if (mySpeed > otherSpeed)
            {
                Debug.Log($"{gameObject.name} の速度が速かった！");
            }
            else if (mySpeed < otherSpeed)
            {
                Debug.Log($"{collision.gameObject.name} の速度が速かった！");
            }
            else
            {
                Debug.Log("両方の速度は同じでした！");
            }
        }
    }
}
