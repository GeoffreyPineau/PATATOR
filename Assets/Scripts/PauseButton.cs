using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour {

    public GameObject pauseGraphics;
    public static PauseButton Instance;
    bool selected;

    private void Awake()
    {
        Instance = this;
        Unpause();
        gameOver.SetActive(false);
    }

    private void Update()
    {/*
        if(Input.GetKeyUp("M"))
        {
            SceneManager.LoadScene("MainMenu");
        }
        if(Input.GetKeyUp("Q"))
        {
            Application.Quit();
        }*/
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            selected = true;
        }
        if(selected)
        {
            if (Time.timeScale == 0)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    Unpause();
                }
            }
            else
            {
                if(Input.GetMouseButtonUp(0))
                {
                    Pause();
                }
            }

            selected = false;
        }

    }

    public void Select()
    {
        selected = true;
    }

    void Pause()
    {
        pauseGraphics.SetActive(true);
        Time.timeScale = 0;
    }

    public GameObject gameOver;
    public TextMeshPro timeCount;
    public void GameOver()
    {
        gameOver.SetActive(true);
        timeCount.text = "You lasted " + Mathf.RoundToInt(Time.timeSinceLevelLoad) + " seconds !";
        Pause();
    }

    void Unpause()
    {
        pauseGraphics.SetActive(false);
        Time.timeScale = 1;
    }
}
