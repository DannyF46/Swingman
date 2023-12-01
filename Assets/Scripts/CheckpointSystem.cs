using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    private GameObject SpawnPoint;
    private List<GameObject> prevPoints = new();
    // Start is called before the first frame update
    void Start()
    {
        SpawnPoint = GameObject.FindWithTag("Respawn");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Checkpoints"))
        {
            if(prevPoints.Count > 0 && collision.gameObject != prevPoints[0])
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


}
