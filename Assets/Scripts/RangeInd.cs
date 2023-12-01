using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class RangeInd : MonoBehaviour
{
    public int points = 1000;
    private GrappleHook Grapple;
    private Rigidbody2D rb;
    private LineRenderer Indicator;

    private float range;
    // Start is called before the first frame update
    void Start()
    {
        Indicator = GetComponent<LineRenderer>();
        rb = GetComponentInParent<Rigidbody2D>();   
        Indicator.loop = true;
        
        Grapple = GetComponentInParent<GrappleHook>();

    }

    // Update is called once per frame
    void Update()
    {
        Indicator.positionCount = points;
        range = Grapple.Range;

        float ang = 2 * Mathf.PI / points;
        Vector3 center = rb.position;
        
        float theta = 0;
        for (int i = 0; i < points; i++)
        {
            theta = i * ang;
            float y = range*Mathf.Sin(theta) ;
            float x = range*Mathf.Cos(theta) ;
            Vector3 drawpoint = new Vector3(x, y, center.z);
            Indicator.SetPosition(i, center + drawpoint);
        }

    }
}
