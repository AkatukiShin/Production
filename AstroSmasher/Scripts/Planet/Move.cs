using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    [SerializeField] private float maxChargePower;
    [SerializeField] private float decelerationRate = 0.95f;
    [SerializeField] private float minSpeedThreshold = 0.1f;

    private bool isMove = false;
    private bool isChargeDash = false;
    private bool isDecelerating = false;

    private Rigidbody plRigidbody;

    private float acceleration;
    private float chargePower = 0.0f;
    private float inputValue_x;
    private float inputValue_y;
    private float scale;
    
    private void Start()
    {
        plRigidbody = GetComponent<Rigidbody>();
        Parameter parameter = GetComponent<Parameter>();
        acceleration = parameter.GetSpeed();    
        scale = parameter.GetScale();
        this.gameObject.transform.localScale = new Vector3(scale, scale, scale);
    }

    private void FixedUpdate()
    {
        PlanetMove();
        ChargeDash();
        ApplyDeceleration();
    }

    private void PlanetMove()
    {
        if (isMove) plRigidbody.AddForce(new Vector3(acceleration * inputValue_x, 0, acceleration * inputValue_y));
    }

    private void ChargeDash()
    {
        if (isChargeDash && chargePower <= maxChargePower)
        {
            chargePower += 0.1f;
        }
        if (!isChargeDash && chargePower > 0)
        {
            plRigidbody.AddForce(new Vector3(acceleration * inputValue_x * (chargePower / 2), 0, acceleration * inputValue_y * (chargePower / 2)), ForceMode.Impulse);
            chargePower = 0;
            isDecelerating = true;
        }
    }

    private void ApplyDeceleration()
    {
        if (isDecelerating)
        {
            Vector3 velocity = plRigidbody.velocity;
            velocity *= decelerationRate;

            //最小速度以下ならば減速を止める
            if (velocity.magnitude < minSpeedThreshold)
            {
                velocity = Vector3.zero;
                isDecelerating = false;
            }

            plRigidbody.velocity = velocity;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputValue_x = context.ReadValue<Vector2>().x;
        inputValue_y = context.ReadValue<Vector2>().y;

        if (context.performed) isMove = true;
        if (context.canceled)  isMove = false;
    }

    public void OnChargeDash(InputAction.CallbackContext context)
    {
        if (context.performed) isChargeDash = true;
        if (context.canceled)  isChargeDash = false;
    }
}
