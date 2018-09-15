using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour {

    public GameObject pauseGraphics;
    bool selected;

    private void Awake()
    {
        Unpause();
    }

    private void Update()
    {
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

    void Unpause()
    {
        pauseGraphics.SetActive(false);
        Time.timeScale = 1;
    }
}
