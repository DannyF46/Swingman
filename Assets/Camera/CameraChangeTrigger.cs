using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;

public class CameraChangeTrigger : MonoBehaviour
{

    [SerializeField] private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var LevelCamParam = animator.GetBool("LevelCam");
        if(collision.gameObject.CompareTag("Player"))
        {
            if(!LevelCamParam)
            {
                animator.SetBool("LevelCam", true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var LevelCamParam = animator.GetBool("LevelCam");
        if (collision.gameObject.CompareTag("Player"))
        {
            if (LevelCamParam)
            {
                animator.SetBool("LevelCam", false);
            }
        }
    }
}
