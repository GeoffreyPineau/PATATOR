using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour {

    public static TutorialManager Instance;

    public bool tutorialEnabled;
    public bool tutorialOnGoing;
    public Animator fence1Anim;
    public Animator fence2Anim;

    public GameObject tequilaPress;
    public GameObject grenadaFabric;
    public Animator potatoLifeAnim;
    public Animator fuelBarAnim;
    public Animator tutorialPanelAnim;
    public Animator skullAnim;
    public Animator potatoHeartAnim;

    public PlayerController player;

    public TextMeshProUGUI tutorialText;
    public float letterDelay;
    public GameObject pressEnterToContinue;

    bool t1;
    public string text1;
    bool t2;
    public string text2;
    bool t3;
    public string text3;
    bool t4;
    public string text4;
    public GameObject target1;

    private void Awake()
    {
        Instance = this;
        player = FindObjectOfType<PlayerController>();
        if(tutorialEnabled)
        {
            player.canMove = false;
            tutorialOnGoing = true;
        }
        else
        {
            player.canMove = true;
        }
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Return))
        {
            if(t1)
            {
                t1 = false;
                StartCoroutine(TypeSentence(text2));
                t2 = true;
            }
            else if(t2)
            {
                target1.SetActive(true);
                t2 = false;
                pressEnterToContinue.SetActive(false);
                StartCoroutine(TypeSentence(text3));
                t3 = true;
            }
        }

        if(t3)
        {
            float dist = Vector3.Distance(player.transform.position, target1.transform.position);
            if(dist < 0.8f)
            {
                t3 = false;
                StartCoroutine(TypeSentence(text4));
                t4 = true;
                pressEnterToContinue.SetActive(true);
                target1.SetActive(false);
            }
        }
    }

    private void Start()
    {
        tutorialText.text = "";
        target1.SetActive(false);
        if(tutorialEnabled)
        {
            tequilaPress.SetActive(false);
            grenadaFabric.SetActive(false);
            fence1Anim.SetBool("isDeployed", true);
            fence2Anim.SetBool("isDeployed", true);
            TimeManager.Instance.infinite = true;
            potatoLifeAnim.SetBool("isHidden", true);
            fuelBarAnim.SetBool("isHidden", true);
            StartCoroutine(ShowPanel(3, text1));
            StartCoroutine(Enable1(3));
        }
    }

    IEnumerator ShowPanel(float timeWaited, string sentence)
    {
        
        yield return new WaitForSeconds(timeWaited);
        tutorialPanelAnim.SetBool("isHidden", false);
        yield return new WaitForSeconds(1);
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator Enable1(float timeWaited)
    {
        yield return new WaitForSeconds(timeWaited);
        t1 = true;
    }

    IEnumerator TypeSentence(string sentence)
    {
        skullAnim.SetBool("isTalking", true);
        tutorialText.text = sentence;
        tutorialText.maxVisibleCharacters = 0;
        foreach(char letter in sentence.ToCharArray())
        {
            tutorialText.maxVisibleCharacters += 1;
            yield return new WaitForSeconds(letterDelay);
        }
        skullAnim.SetBool("isTalking", false);
    }
}