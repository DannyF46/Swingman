using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class GrappleHook : MonoBehaviour
{
    private Rigidbody2D playerRB;
    private DistanceJoint2D Grapple;
    private LineRenderer lr;
    private GameLogic GameLogic;

    public float Range = 12f; //grapple range
    public int Shots = 2; //max number of shots
    public int shotsRemaining = 2; //current number of shots
    public bool isGrappling; 
    public float RappelSpeed = 1f; //falling/extending grapple rope speed
    public float ClimbSpeed = 0.5f; //climbing grapple rope speed
    [SerializeField] private float MinTimeBetweenWrapandUnwrap = 0.01f;
    private float wrapunwraptimer = 0f;
    private bool readyToWrap = true;

    public Vector2 CurrentPoint;
    public Rigidbody2D CurrentBody;

    private readonly int RaycastMask = ~(1 << 6 | 1 << 2);
    private readonly int groundLayer = 1 << 7;
    
    private Vector2 Target;
    private RaycastHit2D Grapplehit;
    private RaycastHit2D Wraphit;
    private RaycastHit2D GroundCheck;
    private Vector2 CharPos;
    private Vector2 AimDirection;

    private Dictionary<int, Vector2> GrapplePoints = new(); //grapple points in local (rigidbody) coords
    private Dictionary<int, Rigidbody2D> GrappleBodies = new();
    private Dictionary<int, int> WrapAngleLookup = new();

    [SerializeField] private bool LClickDown = false;
    private bool LClickUp = false;
    [SerializeField] private bool LClickHold = false;
    private float Vert = 0;
    private float rappeltimer = 0;
    private float ropelength = 0;
    
    //UI
    [SerializeField] private TMP_Text ShotsCounter;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = this.GetComponent<Rigidbody2D>();
        Grapple = this.GetComponent<DistanceJoint2D>();
        lr = transform.Find("Grapple").GetComponent<LineRenderer>();
        Grapple.enabled = false;
        GameLogic = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogic>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GroundCheck.collider != null)
        {
            shotsRemaining = Shots;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
        if (GroundCheck.collider != null)
        {
            shotsRemaining = Shots;
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        if (!GameLogic.Paused)
        {
            
            ShotsCounter.text = "Shots remaining: " + shotsRemaining.ToString();
            CharPos = playerRB.position;
            Target = Aim(Input.mousePosition);
            AimDirection = (Target - CharPos).normalized;
            
            if (LClickDown)
            {
                if(!LClickHold)
                {
                    LClickDown = Input.GetMouseButtonDown(0);
                }
            }
            else
            {
                LClickDown = Input.GetMouseButtonDown(0);
            }

            LClickHold = Input.GetMouseButton(0);
            Vert = Input.GetAxisRaw("Vertical");


            if (isGrappling && lr.positionCount > 0)
            {
                ropelength = 0;

                for (int i = 0; i < lr.positionCount - 1; i++)
                {
                    lr.SetPosition(i, GrappleBodies[i].transform.TransformPoint(GrapplePoints[i]));
                    ropelength += (lr.GetPosition(i) - lr.GetPosition(i + 1)).magnitude;


                }
                lr.SetPosition(lr.positionCount - 1, CharPos);

                if (ropelength > Range)
                {
                    Grapple.distance -= (ropelength - Range);

                }
                
                if(!readyToWrap && wrapunwraptimer < MinTimeBetweenWrapandUnwrap)
                {
                    wrapunwraptimer += Time.deltaTime;
                }
                else if(wrapunwraptimer >= MinTimeBetweenWrapandUnwrap)
                {
                    readyToWrap = true;
                    wrapunwraptimer = 0;
                }

            }
        }

    }

    void FixedUpdate()
    {
        if(!GameLogic.Paused)
        {
            GroundCheck = Physics2D.Raycast(CharPos, Vector2.down, 3f, groundLayer);

            if (LClickDown && shotsRemaining > 0)
            {

                Grapplehit = Physics2D.CircleCast(CharPos, 0.1f, AimDirection, Range, RaycastMask);

                if (Grapplehit.collider != null)
                {
                    GrappleBodies.Add(0, Grapplehit.rigidbody);
                    GrapplePoints.Add(0, GrappleBodies[0].transform.InverseTransformPoint(Grapplehit.point));
                    WrapAngleLookup.Add(0, 0);

                    Grapple.connectedBody = GrappleBodies[0];
                    Grapple.connectedAnchor = GrapplePoints[0];
                    Grapple.distance = Grapplehit.distance;

                    Grapple.enabled = true;
                    isGrappling = true;

                    lr.positionCount = 2;
                    lr.SetPosition(1, CharPos);
                    lr.SetPosition(0, Grapplehit.point);
                    shotsRemaining--;
                }
                else
                {
                    isGrappling = false;
                }
                LClickDown = false;
            }
            else if (LClickHold && isGrappling)
            {
                CurrentBody = GrappleBodies[GrappleBodies.Count - 1];  
                CurrentPoint = GrapplePoints[GrapplePoints.Count - 1];

                if(readyToWrap)
                {
                    Wrap();
                }
                

                Vector2 PointinWorld = CurrentBody.transform.TransformPoint(CurrentPoint);
                Vector2 ChartoPoint = (PointinWorld - CharPos).normalized;
                if (ChartoPoint.y < 0) //if player is above grapple point, flip vertical controls
                {
                    if(Vert > 0)
                    {
                        Vert = 0;
                    }
                    Vert *= -1;
                }

                if (GrappleBodies.Count > 1)
                {
                    WrapAngleSet(GrappleBodies.Count - 1);
                }

                if (Vert > 0)
                {
                    //Grapple.distance -= Vert * ClimbSpeed; //Letting players shorten rope gives too much freedom -- makes axels pointless
                }
                else if(Vert < 0)
                {
                    var currentfallspeed = RappelSpeed * rappeltimer;
                    Grapple.distance -= Vert * currentfallspeed;
                }
                else if(Vert == 0)
                {
                    rappeltimer = 0;
                }
            
                if (Vert != 0)
                {
                    rappeltimer += Time.fixedDeltaTime;
                }

            }
            if (!LClickHold)
            {
                lr.positionCount = 0;

                GrappleBodies.Clear();
                GrapplePoints.Clear();  
                WrapAngleLookup.Clear();

                isGrappling = false;
                Grapple.enabled = false;
                Grapple.connectedBody = null;

            }

        }

    }

    Vector2 Aim(Vector2 MousePosition)
    {
        
        var CursorPos = Camera.main.ScreenToWorldPoint(new(MousePosition.x, MousePosition.y, -Camera.main.transform.position.z)); //perpective camera needs us to pass the camera's z position
        return CursorPos;
    }

    void Wrap()
    {
        var newWrapindex = GrappleBodies.Count;

        Vector2 PrevWrap = GrappleBodies[newWrapindex - 1].transform.TransformPoint(GrapplePoints[newWrapindex - 1]);

        Vector2 dir = (PrevWrap - CharPos).normalized;
        float distance = (PrevWrap - CharPos).magnitude;

        Wraphit = Physics2D.Raycast(CharPos, dir, 0.9f*distance, RaycastMask);

        if(Wraphit.collider!= null)
        {
            
            GrappleBodies.Add(newWrapindex, Wraphit.rigidbody);
            GrapplePoints.Add(newWrapindex, GrappleBodies[newWrapindex].transform.InverseTransformPoint(Wraphit.point));
            WrapAngleLookup.Add(newWrapindex, 0);

            Grapple.connectedBody = GrappleBodies[newWrapindex];
            Grapple.connectedAnchor = GrapplePoints[newWrapindex];
            Grapple.distance = Wraphit.distance;

            

            lr.positionCount++;
            var newLRindex = lr.positionCount;

            lr.SetPosition(newLRindex - 2, Wraphit.point);
            lr.SetPosition(newLRindex - 1, CharPos);

        }
    }

    void WrapAngleSet(int wrapIndex)
    {

        Vector2 NewWrappos = lr.GetPosition(lr.positionCount - 2);
        Vector2 PrevWrappos = lr.GetPosition(lr.positionCount - 3);

        Vector2 LineBetweenWraps = (NewWrappos - PrevWrappos).normalized;
        Vector2 LineBetweenNewWrapAndPlayer = (CharPos - NewWrappos).normalized;

        float WrapAngle = Vector2.SignedAngle(LineBetweenWraps, LineBetweenNewWrapAndPlayer);

        if (WrapAngle > 0f)
        {
            if (WrapAngleLookup[wrapIndex] == -1)
            {
                Unwrap(wrapIndex);
                return;
            }
            else
            {
                WrapAngleLookup[wrapIndex] = 1;
            }
        }
        else if(WrapAngle < 0f)
        {
            if (WrapAngleLookup[wrapIndex] == 1)
            {
                Unwrap(wrapIndex);
                return;
            }
            else
            {
                WrapAngleLookup[wrapIndex] = -1;
            }
        }

    }
    void Unwrap(int WrapIndex)
    {
        GrappleBodies.Remove(WrapIndex);
        GrapplePoints.Remove(WrapIndex);
        WrapAngleLookup.Remove(WrapIndex);

        Grapple.connectedBody = GrappleBodies[WrapIndex-1];
        Grapple.connectedAnchor = GrapplePoints[WrapIndex-1];

        Vector2 WorldGrapplePoint = GrappleBodies[WrapIndex - 1].transform.TransformPoint(GrapplePoints[WrapIndex - 1]);
        Grapple.distance = (WorldGrapplePoint - CharPos).magnitude;

        lr.SetPosition(lr.positionCount - 2, CharPos);
        lr.positionCount--;
        readyToWrap = false;

    }
}
