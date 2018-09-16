using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TimeManager : MonoBehaviour {

    public static TimeManager Instance;

    [Header("Tweaking")]
    public float cycleTime;
    public float initialDayTime;
    public float dayTimeReduction;
    public float minDayTime;
    public float transitionTime;

    [Header("Light")]
    public Light sunLight;
    public Color nightStartingColor;
    public Color nightMiddleColor;
    public Color nightEndColor;
    public Color dayStartingColor;
    public Color dayMiddleColor;
    public Color dayEndColor;

    public float currentDayTime;
    float currentTime;
    public bool isDay;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
        currentDayTime = initialDayTime;
        sunLight.color = nightEndColor;
        DayRise();
    }


    private void Update()
    {
        currentTime += Time.deltaTime;
        if(isDay)
        {
            if (currentTime > currentDayTime)
            {
                currentTime = 0;
                NightFall();
            }
        }
        else
        {
            if(currentTime > cycleTime - currentDayTime)
            {
                currentTime = 0;
                DayRise();

            }
        }

        //color evolution
        if(sunLight.color == dayStartingColor)
        {
            sunLight.DOColor(dayMiddleColor, (currentDayTime - transitionTime) / 2);
        }
        if(sunLight.color == dayMiddleColor)
        {

            sunLight.DOColor(dayEndColor, (currentDayTime - transitionTime) / 2);
        }
        if(sunLight.color == nightStartingColor)
        {
            sunLight.DOColor(nightMiddleColor, ((cycleTime - currentDayTime) - transitionTime) / 2);
        }
        if (sunLight.color == nightMiddleColor)
        {
            sunLight.DOColor(nightEndColor, ((cycleTime - currentDayTime) - transitionTime) / 2);
        }

    }

    void DayRise()
    {
        StartCoroutine("WaitThenWarn");
        isDay = true;
        sunLight.DOColor(dayStartingColor, transitionTime);
        GameManager.Instance.SpawnPotatos();
        MusicManager.Instance.Day();
    }

    void NightFall()
    {
        HoleCreator.Instance.CreateHoles();
        isDay = false;
        currentDayTime -= dayTimeReduction;
        if(currentDayTime < minDayTime)
        {
            currentDayTime = minDayTime;
        }
        sunLight.DOColor(nightStartingColor, transitionTime);
        MusicManager.Instance.Night();
    }

    IEnumerator WaitThenWarn()
    {
        yield return new WaitForSeconds(currentDayTime / 2);
        HoleCreator.Instance.WarnHoles();
    }
}
