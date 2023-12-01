using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{

    [SerializeField] private GameObject box;
    [SerializeField] private Vector2 spawnpoint;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        spawnpoint = this.transform.position;
        if(collision.gameObject.CompareTag("Player"))
        {
            Instantiate(box,spawnpoint,Quaternion.identity);
            this.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
