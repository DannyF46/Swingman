using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Polybrush;

public class TriggerLift : MonoBehaviour
{
    private SliderJoint2D lift;
    private float deltat = 0;
    public bool isTrigged = false;
    private JointLimitState2D liftonLims;
    // Start is called before the first frame update
    void Start()
    {
        lift = this.GetComponent<SliderJoint2D>();
    }
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        lift.useMotor = false;
        isTrigged = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isTrigged = false;
    }

}
