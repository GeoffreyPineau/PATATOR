using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicManager : MonoBehaviour {

    public AudioSource dayMusic;
    public AudioSource nightMusic;

    public static MusicManager Instance;

    float dayTarget;
    float nightTarget;

    float reference;

    private void Awake()
    {
        Instance = this;
    }

    public void Night()
    {
        StartCoroutine("StartNight");
    }

    public void Day()
    {
        StartCoroutine("StartDay");
    }

    IEnumerator StartNight()
    {
        dayMusic.DOFade(0, 3);
        yield return new WaitForSeconds(2);
        dayMusic.Stop();
        nightMusic.Play();
        nightMusic.DOFade(0.2f, 3);
    }

    IEnumerator StartDay()
    {
        nightMusic.DOFade(0, 3);
        yield return new WaitForSeconds(2);
        dayMusic.Play();
        nightMusic.Stop();
        dayMusic.DOFade(0.2f, 3);
    }
}
