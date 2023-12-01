using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

public class Respawn : MonoBehaviour
{
    private GameObject SpawnPoint;
    private Vector2 SpawnPointPosition;
    private Rigidbody2D Player;
    private List<GameObject> prevPoints = new();

    [SerializeField] private Vector2 CurrentCP;
    [SerializeField] private float RespawnTime = 2f;
    private float respawntime = 0;
    private bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPoint = GameObject.FindWithTag("Respawn");
        SpawnPointPosition = SpawnPoint.transform.position;
        Player = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CurrentCP = GameObject.FindWithTag("Respawn").transform.position;
        if (SpawnPointPosition != CurrentCP)
        {
            SpawnPointPosition = CurrentCP;
        }

        if (dead)
        {
            respawntime += Time.deltaTime;
            if(respawntime >= RespawnTime) 
            {
                Player.position = SpawnPointPosition;
                Player.velocity = Vector2.zero;
                dead = false;
                CharacterStatus(dead);
                respawntime = 0;
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("DeathZone"))
        {
            dead = true;
            CharacterStatus(dead);
        }

        else if (collision.CompareTag("Checkpoints"))
        {
            if (prevPoints.Count > 0 && collision.gameObject != prevPoints[0])
            {
                var prevpoint = prevPoints[0];
                prevpoint.GetComponent<ParticleSystem>().Stop();
                prevPoints.Clear();
            }

            var point = collision.gameObject;
            Vector2 NewSpawnPoint = point.transform.GetChild(0).GetComponent<Transform>().position;
            SpawnPoint.transform.position = NewSpawnPoint;
            collision.GetComponent<ParticleSystem>().Play();

            prevPoints.Add(point);
        }
    }

    void CharacterStatus(bool status)
    {
        this.GetComponent<SpriteRenderer>().enabled = !status;
        this.GetComponent<PlayerController>().enabled = !status;
        this.GetComponent<GrappleHook>().enabled = !status;
        this.GetComponent<Light2D>().enabled = !status;
    }

}
