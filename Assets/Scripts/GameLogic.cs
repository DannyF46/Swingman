using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{

    
    public bool Paused;
    private GameObject Player;
    private GameObject SpawnPoint;

    [SerializeField] private TMP_Text PauseScreen;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        SpawnPoint = GameObject.FindGameObjectWithTag("Respawn");

        Unpause();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Paused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Unpause();
            }
            if(Input.GetKeyDown(KeyCode.R)) 
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Unpause();
            }
            if(Input.GetKeyDown(KeyCode.Q))
            {
                Application.Quit();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            Player.transform.position = SpawnPoint.transform.position;
            Player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    void Pause()
    {
        Time.timeScale = 0f;
        Paused = true;
        PauseScreen.enabled = true;
    }
    void Unpause()
    {
        Time.timeScale = 1f;
        Paused = false;
        PauseScreen.enabled = false;
    }


}
