using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Button : MonoBehaviour
{
    
    public enum buttons
    {
        PressurePlate, Switch
    };
    public enum targets
    {
        HingeJoint2D, SliderJoint2D
    };
    public bool isButtonPressed;

    public buttons buttonType = new();
    public targets targetType = new();

    public Button()
    {
        
    }
    /*
    public bool isButtonPressed()
    {
        bool isPressed;
        isPressed = false;
        return isPressed;
    }*/
    public void getTargetType()
    {
        
    }
       
}
