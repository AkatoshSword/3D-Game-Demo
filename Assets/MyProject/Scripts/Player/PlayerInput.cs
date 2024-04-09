using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 Move
    {
        get
        {
            if (!isCanControl)
            {
                return Vector2.zero;
            }
            return _move;
        }
    }
    public bool Jump
    {
        get
        {
            return _jump && isCanControl;
        }
    }
    public bool LightAttack
    {
        get
        {
            return _lightAttack && isCanControl;
        }
    }
    public bool HeavyAttack
    {
        get
        {
            return _heavyAttack && isCanControl;
        }
    }
    public bool Roll
    {
        get
        {
            return _roll && isCanControl;
        }
    }

    private bool isCanControl = true;
    private Vector2 _move;
    private bool _jump;
    private bool _lightAttack;
    private bool _heavyAttack;
    private bool _roll;

    private void Update()
    {
        _move.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _jump = Input.GetButtonDown("Jump");
        _lightAttack = Input.GetMouseButtonDown(0);
        _heavyAttack = Input.GetMouseButtonDown(1);
        _roll = Input.GetKeyDown(KeyCode.LeftShift);
}

    public void GainControl()
    {
        isCanControl = true;
    }

    public void ReleaseControl()
    {
        isCanControl = false;
    }
}
