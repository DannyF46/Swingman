using Mono.Cecil.Rocks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class portalbutton : MonoBehaviour
{


    public GameObject target;
    public GameObject buttonlight;

    private bool initialstate; //0 - off  , 1 - on
    public enum modes 
    {
        Motor_on, Motor_off, Motor_perma_on, Motor_perma_off
    };
    public modes Button_Mode = new modes();
    //private int mode = 0;
    private Collider2D buttoncolider;
    private Collider2D targetcolider;
    private SpringJoint2D spring;
    
    // Start is called before the first frame update
    void Start()
    {
        buttoncolider = this.GetComponent<Collider2D>();
        targetcolider = target.GetComponent<Collider2D>(); 
        initialstate = target.GetComponent<SliderJoint2D>().useMotor;
        spring = target.GetComponent<SpringJoint2D>();

        buttonlight = this.transform.GetChild(0).GameObject();
        buttonlight.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Button_Mode == modes.Motor_on || Button_Mode == modes.Motor_perma_on)
        {
            target.GetComponent<SliderJoint2D>().useMotor = true;
            spring.enabled = true;
            buttonlight.SetActive(true);
        };
        if (Button_Mode == modes.Motor_off)
        {
            target.GetComponent<SliderJoint2D>().useMotor = false;
        };

        
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (Button_Mode == modes.Motor_on)
        {
            target.GetComponent<SliderJoint2D>().useMotor = false;
            spring.enabled = false;
            buttonlight.SetActive(false);
        };
        if (Button_Mode == modes.Motor_off || Button_Mode == modes.Motor_perma_off)
        {
            target.GetComponent<SliderJoint2D>().useMotor = true;
        };
        
    }






}
