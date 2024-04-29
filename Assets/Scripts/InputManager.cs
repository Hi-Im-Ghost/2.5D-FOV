using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static event Action<Vector3> PlayerMovementEvent;
    public static event Action<float> PlayerRotationEvent;

    private void FixedUpdate()
    {
        HandlePlayerMovement();
        HandlePlayerRotation();
    }


    private void HandlePlayerMovement()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            PlayerMovementEvent?.Invoke(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        }
    }

    private void HandlePlayerRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            PlayerRotationEvent?.Invoke(-1f);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            PlayerRotationEvent?.Invoke(1f);
        }
    }
}
