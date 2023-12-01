using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConstantRotation_Button : MonoBehaviour
{

    [SerializeField] private ConstantRotationalVelocity Target; //Replace "Joint2D" with the type of joint to be controlled
    [SerializeField] private bool DeactivateDelay = false;
    [SerializeField] private float DeactivateTime = 1f;
    [SerializeField] private bool StartTimerOnPress; //start the deactivation delay timer upon pushing button. If disabled, timer will start upon TriggerExit

    //remove ControlMotor if the target joint does not have a motor
    [SerializeField] private bool ControlMotor = true; //ONLY control the motor of the SliderJoint -- if enabled, wont control the component itself 

    [SerializeField] private bool ControlButtonLight = false;
    private GameObject buttonlight;

    [SerializeField] private bool ButtonDown;
    private bool UpdateTimer = false;
    private float deactivetimer = 0;
    public enum modes
    {
        On_Hold, Off_Hold, On_Toggle, Off_Toggle, Toggle
    };

    public modes Button_Mode = new();




    // Start is called before the first frame update
    void Start()
    {
        if (ControlButtonLight)
        {
            buttonlight = this.transform.GetChild(0).GameObject();
        }
    }

    void Update()
    {
        if (DeactivateDelay && UpdateTimer)
        {
            if (deactivetimer >= DeactivateTime)
            {
                OnButtonUp();
                UpdateTimer = false;
                deactivetimer = 0;
            }
            else
            {
                deactivetimer += Time.deltaTime;
            }
        }
        
        if (ControlButtonLight && !ControlMotor)
        {
            if (buttonlight.activeInHierarchy != Target.enabled)
            {
                buttonlight.SetActive(Target.enabled);
            }
        }
        else if (ControlButtonLight && ControlMotor)
        {
            if (buttonlight.activeInHierarchy != Target.useMotor)
            {
                buttonlight.SetActive(Target.useMotor);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ButtonDown = true;

        OnButtonDown();

        if (StartTimerOnPress)
        {
            UpdateTimer = true;
        }

    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        ButtonDown = false;
        if (DeactivateDelay && !StartTimerOnPress)
        {
            UpdateTimer = true;
        }
        else
        {
            OnButtonUp();
        }
    }

    void OnButtonDown()
    {
        if (ControlMotor)
        {
            if (Button_Mode == modes.On_Hold || Button_Mode == modes.On_Toggle)
            {
                Target.useMotor = true;
            }
            else if (Button_Mode == modes.Off_Hold || Button_Mode == modes.Off_Toggle)
            {
                Target.useMotor = false;
            }
            else if (Button_Mode == modes.Toggle)
            {
                Target.useMotor = !Target.useMotor;

            }
        }
        else
        {
            if (Button_Mode == modes.On_Hold || Button_Mode == modes.On_Toggle)
            {
                Target.enabled = true;
            }
            else if (Button_Mode == modes.Off_Hold || Button_Mode == modes.Off_Toggle)
            {
                Target.enabled = false;
            }
            else if (Button_Mode == modes.Toggle)
            {
                Target.enabled = !Target.enabled;
            }
        }

    }

    void OnButtonUp()
    {
        if (ControlMotor)
        {
            if (Button_Mode == modes.On_Hold)
            {
                Target.useMotor = false;
            }
            else if (Button_Mode == modes.Off_Hold)
            {
                Target.useMotor = true;
            }
            else if (DeactivateDelay && deactivetimer >= DeactivateTime)
            {
                Target.useMotor = false;
            }
        }
        else
        {
            if (Button_Mode == modes.On_Hold)
            {
                Target.enabled = false;
            }
            else if (Button_Mode == modes.Off_Hold)
            {
                Target.enabled = true;
            }
            else if (DeactivateDelay && deactivetimer >= DeactivateTime)
            {
                Target.enabled = false;
            }
        }

    }

}
