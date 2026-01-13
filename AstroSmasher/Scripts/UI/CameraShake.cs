using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private Vector3 positionStrength;
    [SerializeField] private Vector3 rotationStrength;

    private float shakeDuration = 0.3f;

    public void CameraShaker()
    {
        cam.DOComplete();
        cam.DOShakePosition(shakeDuration, positionStrength);
        cam.DOShakeRotation(shakeDuration, rotationStrength);
    }
}