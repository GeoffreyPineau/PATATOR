using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TimeManager : MonoBehaviour {

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

    float currentDayTime;
    float currentTime;
    bool isDay;

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
        isDay = true;
        sunLight.DOColor(dayStartingColor, transitionTime);
        GameManager.Instance.SpawnPotatos();
    }

    void NightFall()
    {
        isDay = false;
        currentDayTime -= dayTimeReduction;
        if(currentDayTime < minDayTime)
        {
            currentDayTime = minDayTime;
        }
        sunLight.DOColor(nightStartingColor, transitionTime);
    }
}
