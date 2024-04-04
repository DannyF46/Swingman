using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMusic : MonoBehaviour
{
    public bool MusicPaused = false;
    AudioSource audioSource;
    GameLogic gameLogic;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        gameLogic = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameLogic.Paused && MusicPaused)
        {
            audioSource.Play();
            MusicPaused = false;
        }
        else if(gameLogic.Paused && !MusicPaused)
        {
            audioSource.Pause();
            MusicPaused=true;
        }
        
    }
}
