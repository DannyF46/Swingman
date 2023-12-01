using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ConstantRotationalVelocity : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] public bool useMotor = true;
    [SerializeField] private float RotationalVelocity = 10f;
    [SerializeField] private float accelerationTime = 1f;
    [SerializeField] private float decelertationTime = 1f;

    private float t_acc = 0;
    private float t_dec = 0;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(useMotor)
        {
            t_dec = 0;
            if(rb.angularVelocity == RotationalVelocity)
            {
                return;
            }
            else if (rb.angularVelocity >= 0.99*RotationalVelocity || t_acc > accelerationTime)
            {
                rb.angularVelocity = RotationalVelocity;
            }
            else if (rb.angularVelocity < RotationalVelocity || t_acc < accelerationTime)
            {
                rb.angularVelocity = Mathf.Lerp(rb.angularVelocity, RotationalVelocity, t_acc / accelerationTime);
                t_acc += Time.fixedDeltaTime;
            }
        }
        else
        {
            t_acc = 0f;
            if(rb.angularVelocity == 0f)
            {
                return;
            }
            else if (rb.angularVelocity < 0.01f || t_dec > decelertationTime)
            {
                rb.angularVelocity = 0;
            }
            else if (rb.angularVelocity > 0f || t_dec < decelertationTime)
            {
                rb.angularVelocity = Mathf.Lerp(rb.angularVelocity, 0, t_dec / decelertationTime);
                t_dec += Time.fixedDeltaTime;
            }



        }


    }

}
