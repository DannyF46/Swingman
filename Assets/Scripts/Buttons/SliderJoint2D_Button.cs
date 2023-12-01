using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SliderJoint2D_Button : MonoBehaviour    //Component that goes on the button used to control a spring joint
{

    [SerializeField] private SliderJoint2D TargetSlider;
    [SerializeField] private bool DeactivateDelay = false;
    [SerializeField] private float DeactivateTime = 1f;
    [SerializeField] private bool StartTimerOnPress; //start the deactivation delay timer upon pushing button. If disabled, timer will start upon TriggerExit
    [SerializeField] private bool ControlMotor = true; //ONLY control the motor of the SliderJoint -- if enabled, wont control the component itself 

    [SerializeField] private bool ControlButtonLight = false;
    private Light2D buttonlight;


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
        if(ControlButtonLight)
        {
            buttonlight = this.transform.GetChild(0).GetComponent<Light2D>();
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
            if (buttonlight.enabled != TargetSlider.enabled)
            {
                buttonlight.enabled = TargetSlider.enabled;
            }
        }
        else if(ControlButtonLight && ControlMotor)
        {
            if (buttonlight.enabled != TargetSlider.useMotor)
            {
                buttonlight.enabled = TargetSlider.useMotor;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
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
                TargetSlider.useMotor = true;
            }
            else if (Button_Mode == modes.Off_Hold || Button_Mode == modes.Off_Toggle)
            {
                TargetSlider.useMotor = false;
            }
            else if (Button_Mode == modes.Toggle)
            {
                TargetSlider.useMotor = !TargetSlider.useMotor;

            }
        }
        else
        {
            if (Button_Mode == modes.On_Hold || Button_Mode == modes.On_Toggle)
            {
                TargetSlider.enabled = true;
            }
            else if (Button_Mode == modes.Off_Hold || Button_Mode == modes.Off_Toggle)
            {
                TargetSlider.enabled = false;
            }
            else if (Button_Mode == modes.Toggle)
            {
                TargetSlider.enabled = !TargetSlider.enabled;
            }
        }
     
    }
   
    void OnButtonUp()
    {
        if(ControlMotor)
        {
            if (Button_Mode == modes.On_Hold)
            {
                TargetSlider.useMotor = false;
            }
            else if (Button_Mode == modes.Off_Hold)
            {
                TargetSlider.useMotor = true;
            }
            else if (DeactivateDelay && deactivetimer >= DeactivateTime)
            {
                TargetSlider.useMotor = false;
            }
        }
        else
        {
            if (Button_Mode == modes.On_Hold)
            {
                TargetSlider.enabled = false;
            }
            else if (Button_Mode == modes.Off_Hold)
            {
                TargetSlider.enabled = true;
            }
            else if (DeactivateDelay && deactivetimer >= DeactivateTime)
            {
                TargetSlider.enabled = false;
            }
        }
        
    }
  
}
