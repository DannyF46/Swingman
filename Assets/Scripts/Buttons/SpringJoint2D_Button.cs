using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class SpringJoint2D_Button : MonoBehaviour    //Component that goes on the button used to control a spring joint
{

    [SerializeField] private SpringJoint2D TargetSpring;
    [SerializeField] private bool DeactivateDelay;
    [SerializeField] private bool StartTimerOnPress; //start the deactivation delay timer upon pushing button. If disabled, timer will start upon TriggerExit
    [SerializeField] private float DeactivateTime;

    [SerializeField] private bool ControlButtonLight;
    private Light2D buttonlight;

    [SerializeField] private bool ButtonDown;

    private bool UpdateTimer = false;
    private float deactivetimer = 0;
    public enum modes
    {
        SpringOn_Hold, SpringOff_Hold, SpringOn_Toggle, SpringOff_Toggle, Spring_Toggle
    };

    public modes Button_Mode = new modes();

    


    // Start is called before the first frame update
    void Start()
    {

        if (ControlButtonLight) 
        {
            buttonlight = this.transform.GetChild(0).GetComponent<Light2D>();
        }
        
    }

    void Update()
    {
        if(DeactivateDelay && UpdateTimer)
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

        if(ControlButtonLight)
        {
            if(buttonlight.enabled != TargetSpring.enabled)
            {
                buttonlight.enabled = TargetSpring.enabled;
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

        if(DeactivateDelay && !StartTimerOnPress)
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
        if (Button_Mode == modes.SpringOn_Hold || Button_Mode == modes.SpringOn_Toggle)
        {
            TargetSpring.enabled = true;
        }
        else if (Button_Mode == modes.SpringOff_Hold || Button_Mode == modes.SpringOff_Toggle)
        {
            TargetSpring.enabled = false;
        }
        else if (Button_Mode == modes.Spring_Toggle)
        {
            TargetSpring.enabled = !TargetSpring.enabled;

        }
    }
    void OnButtonUp()
    {
        if (Button_Mode == modes.SpringOn_Hold)
        {
            TargetSpring.enabled = false;
        }
        else if (Button_Mode == modes.SpringOff_Hold)
        {
            TargetSpring.enabled = true;
        }
        else if(DeactivateDelay && deactivetimer >= DeactivateTime)
        {
            TargetSpring.enabled = false;
        }
    }
}
