using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum SquareState
{
    empty,
    potato,
    hole,
    grenada
}

public enum SquareType
{
    dirt,
    heart,
    press,
    pump,
    compressor,
    grenada
}

public class Square : MonoBehaviour {

    public SquareState state;
    public SquareType type;

    public GameObject selectionLid;
    
    [Header("Dirt")]
    public Animator numberPanelAnim;
    public TextMeshPro numberPanelText;
    public int potatoAmount;

    public Transform leavesParent;
    public List<GameObject> leaves;
    public Animator leavesAnim;

    public Transform graphicsParent;

    public List<GameObject> stateGraphics;

    public bool canBeHoled;

    [Header("Tequila")]
    public int tequilaMultiplier;

    [Header("Flies")]
    float flyTimer;

    AudioSource mySource;

    void Awake()
    {
        state = SquareState.empty;
        if (type == SquareType.dirt)
        {
            numberPanelAnim.SetBool("isBurrowed", false);
            //disable leaves
            leaves[0].SetActive(false);
            leaves[1].SetActive(false);
            leaves[2].SetActive(false);
            leaves[3].SetActive(false);
            leaves[4].SetActive(false);
            leavesParent.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);

            int rot = Random.Range(1, 5);
            float rot2;
            if (rot == 1) rot2 = 90;
            else if (rot == 2) rot2 = 180;
            else if (rot == 3) rot2 = 270;
            else rot2 = 0;

            graphicsParent.transform.eulerAngles = new Vector3(0, rot2, 0);

            mySource = gameObject.AddComponent<AudioSource>();

        }
    }

    void Update()
    {
        //show and hide panel
        if(type == SquareType.dirt)
        {
            if (potatoAmount > 0)
            {
                if (numberPanelAnim.GetBool("isBurrowed") == true)
                {
                    leavesParent.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                    numberPanelAnim.SetBool("isBurrowed", false);

                }
                numberPanelText.text = potatoAmount.ToString();
            }
            else
            {
                numberPanelAnim.SetBool("isBurrowed", true);
            }

            //leaves
            #region
            if (potatoAmount > 0)
            {
                if (potatoAmount > GameManager.Instance.twoLeavesMin)
                {
                    if (potatoAmount > GameManager.Instance.threeLeavesMin)
                    {
                        if (potatoAmount > GameManager.Instance.fourLeavesMin)
                        {
                            if (potatoAmount > GameManager.Instance.fiveLeavesMin)
                            {
                                if (!leaves[4].activeInHierarchy)
                                {
                                    leaves[0].SetActive(false);
                                    leaves[1].SetActive(false);
                                    leaves[2].SetActive(false);
                                    leaves[3].SetActive(false);
                                    leaves[4].SetActive(true);
                                }
                            }
                            else
                            {
                                if (!leaves[3].activeInHierarchy)
                                {
                                    leaves[0].SetActive(false);
                                    leaves[1].SetActive(false);
                                    leaves[2].SetActive(false);
                                    leaves[3].SetActive(true);
                                    leaves[4].SetActive(false);
                                }
                            }
                        }
                        else
                        {
                            if (!leaves[2].activeInHierarchy)
                            {
                                leaves[0].SetActive(false);
                                leaves[1].SetActive(false);
                                leaves[2].SetActive(true);
                                leaves[3].SetActive(false);
                                leaves[4].SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        if (!leaves[1].activeInHierarchy)
                        {
                            leaves[0].SetActive(false);
                            leaves[1].SetActive(true);
                            leaves[2].SetActive(false);
                            leaves[3].SetActive(false);
                            leaves[4].SetActive(false);
                        }
                    }
                }
                else
                {
                    if (!leaves[0].activeInHierarchy)
                    {
                        leaves[0].SetActive(true);
                        leaves[1].SetActive(false);
                        leaves[2].SetActive(false);
                        leaves[3].SetActive(false);
                        leaves[4].SetActive(false);
                    }
                }
            }
            else
            {
                leaves[0].SetActive(false);
                leaves[1].SetActive(false);
                leaves[2].SetActive(false);
                leaves[3].SetActive(false);
                leaves[4].SetActive(false);
            }

            #endregion
        }
        else
        {
            numberPanelAnim.SetBool("isBurrowed", true);
        }

        //graphics
        if(state == SquareState.hole)
        {
            if(TimeManager.Instance.isDay)
            {
                if(!stateGraphics[2].activeInHierarchy)
                {
                    stateGraphics[0].SetActive(false);
                    stateGraphics[1].SetActive(false);
                    stateGraphics[2].SetActive(true);
                    stateGraphics[3].SetActive(false);
                    stateGraphics[4].SetActive(false);
                }
            }
            else
            {
                if(!stateGraphics[2].activeInHierarchy)
                {
                    stateGraphics[0].SetActive(false);
                    stateGraphics[1].SetActive(false);
                    stateGraphics[2].SetActive(true);
                    stateGraphics[3].SetActive(false);
                    stateGraphics[4].SetActive(false);
                }

                flyTimer += Time.deltaTime;
                if(flyTimer > GameManager.Instance.flyDelay)
                {
                    flyTimer = 0;
                    SpawnFly();
                }
            }
        }
        else if(state == SquareState.potato)
        {
            if (!stateGraphics[1].activeInHierarchy)
            {
                stateGraphics[0].SetActive(false);
                stateGraphics[1].SetActive(true);
                stateGraphics[2].SetActive(false);
                stateGraphics[3].SetActive(false);
                stateGraphics[4].SetActive(false);
            }
        }
        else if(state == SquareState.empty)
        {
            if (!stateGraphics[0].activeInHierarchy)
            {
                stateGraphics[0].SetActive(true);
                stateGraphics[1].SetActive(false);
                stateGraphics[2].SetActive(false);
                stateGraphics[3].SetActive(false);
                stateGraphics[4].SetActive(false);
            }
        }
        else
        {
            if (!stateGraphics[4].activeInHierarchy)
            {
                stateGraphics[0].SetActive(false);
                stateGraphics[1].SetActive(false);
                stateGraphics[2].SetActive(false);
                stateGraphics[3].SetActive(false);
                stateGraphics[4].SetActive(true);
            }
        }

        //deselect
        if(!selected)
        {
            //if(type != SquareType.heart)
            selectionLid.SetActive(false);
        }
    }

    public void SpawnFly()
    {
        Transform newFly = Instantiate(GameManager.Instance.flyPrefab, GameManager.Instance.flyParent).transform;
        newFly.position = transform.position;
        float rand = Random.Range(-0.1f, 0.1f);
        newFly.localScale += new Vector3(rand, rand, rand);
    }

    private void LateUpdate()
    {
        if(selected)
        {
            //if (type != SquareType.heart)
            selected = false;

        }
    }

    void PlayRandomizedSource(AudioClip clip, float volume)
    {
        mySource.Stop();
        mySource.clip = clip;
        mySource.volume = volume + Random.Range(-0.02f, 0.02f);
        mySource.pitch = Random.Range(0.92f, 1.08f);
        mySource.Play();
    }

    public void Interact()
    {
        if (type == SquareType.dirt)
        {
            if(state == SquareState.hole)
            {
                    if(GameManager.Instance.hasGrenada)
                    {
                        Explode();
                        GameManager.Instance.hasGrenada = false;
                    }
            }
            else if(state == SquareState.empty)
            {
                if(GameManager.Instance.hasGrenada)
                {
                    state = SquareState.grenada;
                    potatoAmount = 0;
                    GameManager.Instance.hasGrenada = false;
                }
                else
                {
                    if (GameManager.Instance.potatoesHeld > 0)
                    {
                        PlantPotato(1);
                        GameManager.Instance.potatoesHeld -= 1;
                    }
                }
            } 
            else if(state == SquareState.potato)
            {
                if(GameManager.Instance.hasGrenada)
                {
                    state = SquareState.grenada;
                    potatoAmount = 0;

                    GameManager.Instance.hasGrenada = false;
                }
                else
                {
                        PlayRandomizedSource(GameManager.Instance.potatoUproot, GameManager.Instance.uprootVolume);
                        //AudioSource.PlayClipAtPoint(GameManager.Instance.potatoUproot, transform.position, GameManager.Instance.uprootVolume);

                        GameManager.Instance.potatoesHeld += potatoAmount;
                        if (GameManager.Instance.hasGrenada)
                        {
                            GameManager.Instance.hasGrenada = false;
                            GameManager.Instance.grenadas++;
                        }
                        state = SquareState.empty;
                        potatoAmount = 0;
                        leavesAnim.SetTrigger("uproot");
                }
            }
            else if(state == SquareState.grenada)
            {
                if(!GameManager.Instance.hasGrenada)
                {
                    GameManager.Instance.hasGrenada = true;
                    state = SquareState.empty;
                }
            }

        }
        else if(type == SquareType.press)
        {
            if(GameManager.Instance.potatoesHeld > 0)
            {
                PressTequila(GameManager.Instance.potatoesHeld);
                GameManager.Instance.potatoesHeld = 0;
            }
        }
        else if(type == SquareType.pump)
        {
            if(GameManager.Instance.pressTequila > 0)
            {
                if (GameManager.Instance.heldTequila < GameManager.Instance.maxTequila)
                {
                    int addedTequila = (int)(GameManager.Instance.maxTequila - GameManager.Instance.heldTequila);
                    if(addedTequila > GameManager.Instance.pressTequila)
                    {
                        addedTequila = GameManager.Instance.pressTequila;
                    }
                    GameManager.Instance.pressTequila -= addedTequila;
                    GameManager.Instance.heldTequila += addedTequila;

                    PlayRandomizedSource(GameManager.Instance.tequilaLiquidSound, GameManager.Instance.tequilaLiquidVolume);
                    GameManager.Instance.tequilaPressAnim.SetTrigger("pour");


                    if (GameManager.Instance.heldTequila > GameManager.Instance.maxTequila)
                    {
                        GameManager.Instance.heldTequila = GameManager.Instance.maxTequila;
                    }
                }
            }
        }
        else if(type == SquareType.compressor)
        {
            if (GameManager.Instance.potatoesHeld > 0)
            {
                CompressPotato(GameManager.Instance.potatoesHeld);
                GameManager.Instance.potatoesHeld = 0;
            }
        }
        else if(type == SquareType.grenada)
        {
            if(GameManager.Instance.grenadas > 0 && GameManager.Instance.potatoesHeld <= 0 && GameManager.Instance.hasGrenada == false)
            {
                
                GameManager.Instance.hasGrenada = true;
                GameManager.Instance.grenadas--;
                if(GameManager.Instance.grenadas > 0)
                {
                    GameManager.Instance.animatedGrenadaAnim.SetTrigger("recharge");
                }
            }
        }
        else
        {
            if (GameManager.Instance.heartCurrentLife < GameManager.Instance.heartMaxLife)
            {
                AbsorbPotato(GameManager.Instance.potatoesHeld);
                GameManager.Instance.potatoesHeld = 0;
            }
        }
    }

    public Collider[] holes;
    public Collider[] flies;
    void Explode()
    {
        state = SquareState.empty;
        GameManager.Instance.hasGrenada = false;
        holes = Physics.OverlapSphere(transform.position, 1, LayerMask.GetMask("Hole"));
        foreach (Collider hole in holes)
        {
            hole.GetComponentInParent<Square>().state = SquareState.empty;
        }

        flies = Physics.OverlapSphere(transform.position, 1, LayerMask.GetMask("Enemy"));
        foreach (Collider fly in flies)
        {
            fly.GetComponentInParent<FlyController>().Damage(999);
        }

        StartCoroutine("Explosion");
    }

    public GameObject explosion;
    IEnumerator Explosion()
    {
        explosion.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        explosion.SetActive(false);
    }

    public void CreateHole()
    {
        state = SquareState.hole;
        potatoAmount = 0;
    }

    public void GrowPotato()
    {
        state = SquareState.potato;
        leavesAnim.SetTrigger("grow");
    } 

    public void PlantPotato(int amount)
    {
        state = SquareState.potato;
        potatoAmount += amount;
        leavesAnim.SetTrigger("plant");

        PlayRandomizedSource(GameManager.Instance.potatoPlanting, GameManager.Instance.plantingVolume);
        //AudioSource.PlayClipAtPoint(GameManager.Instance.potatoPlanting, transform.position, GameManager.Instance.plantingVolume);
    }

    public void PotatoGrowth()
    {
        potatoAmount += Mathf.RoundToInt(GameManager.Instance.potatoAddingCurve.Evaluate(potatoAmount));
        if(potatoAmount > GameManager.Instance.maxPotatoes)
        {
            potatoAmount = GameManager.Instance.maxPotatoes;
        }
        leavesAnim.SetTrigger("growth");
    }

    public bool selected;
    public void Select()
    {
        //if(type != SquareType.heart)
        //{
            selectionLid.SetActive(true);
//        }


        selected = true;
    }

    public void PressTequila(int potatoAmount)
    {
        PlayRandomizedSource(GameManager.Instance.grenadaDrop, GameManager.Instance.dropVolume);

        GameManager.Instance.pressTequila += potatoAmount * tequilaMultiplier;
        GameManager.Instance.tequilaPressAnim.SetTrigger("press");
    }

    public void CompressPotato(int potatoAmount)
    {
        GameManager.Instance.grenadaPotatoes += potatoAmount;
        GameManager.Instance.grenadaFabricAnim.SetTrigger("compress");

        PlayRandomizedSource(GameManager.Instance.grenadaDrop, GameManager.Instance.dropVolume);
        //AudioSource.PlayClipAtPoint(GameManager.Instance.grenadaDrop, transform.position, GameManager.Instance.dropVolume);

    }

    public void AbsorbPotato(int potatoAmount)
    {
        GameManager.Instance.heartCurrentLife += potatoAmount;
        if(GameManager.Instance.heartCurrentLife > GameManager.Instance.heartMaxLife)
        {
            GameManager.Instance.heartCurrentLife = GameManager.Instance.heartMaxLife;
        }
    }

    public void Redden()
    {
        foreach(GameObject graphic in stateGraphics)
        {
            graphic.GetComponent<MeshRenderer>().material.color = HoleCreator.Instance.dangerColor;
        }
    }

    public void Normalize()
    {
        foreach (GameObject graphic in stateGraphics)
        {
            graphic.GetComponent<MeshRenderer>().material.color = HoleCreator.Instance.normalColor;
        }
    }
}
