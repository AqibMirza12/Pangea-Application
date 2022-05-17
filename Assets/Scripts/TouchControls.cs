using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TouchControls : MonoBehaviour
{
    public UnityEvent buttonPress;

    private bool pressed = false;

    public void SetPressed(bool value)
    {
        pressed = value;
    }

    void Update()
    {
        if (pressed)
        {
            buttonPress.Invoke();
        }
    }
}
