using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum SquareState
{
    empty,
    potato,
    hole
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

    public Transform graphicsParent;

    public List<GameObject> stateGraphics;

    public bool canBeHoled;

    [Header("Tequila")]
    public int tequilaMultiplier;

    [Header("Flies")]
    float flyTimer;


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
                if(!stateGraphics[3].activeInHierarchy)
                {
                    stateGraphics[0].SetActive(false);
                    stateGraphics[1].SetActive(false);
                    stateGraphics[2].SetActive(true);
                    stateGraphics[3].SetActive(false);
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
                }

                flyTimer += Time.deltaTime;
                if(flyTimer > GameManager.Instance.flyDelay)
                {
                    flyTimer = 0;
                    Transform newFly = Instantiate(GameManager.Instance.flyPrefab, GameManager.Instance.flyParent).transform;
                    newFly.position = transform.position;
                    float rand = Random.Range(-0.1f, 0.1f);
                    newFly.localScale += new Vector3(rand, rand, rand);
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
            }
        }
        else
        {
            if (!stateGraphics[0].activeInHierarchy)
            {
                stateGraphics[0].SetActive(true);
                stateGraphics[1].SetActive(false);
                stateGraphics[2].SetActive(false);
                stateGraphics[3].SetActive(false);
            }
        }

        //deselect
        if(!selected)
        {
            if(type != SquareType.heart)
            selectionLid.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if(selected)
        {
            if (type != SquareType.heart)
            selected = false;

        }
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
                    if(GameManager.Instance.potatoesHeld > 0)
                    {
                        GrowPotato(GameManager.Instance.potatoesHeld);
                        GameManager.Instance.potatoesHeld = 0;
                    }
            } 
            else if(state == SquareState.potato)
            {
                if((GameManager.Instance.potatoesHeld + potatoAmount) <= GameManager.Instance.maxPotatoes)
                {
                    GameManager.Instance.potatoesHeld += potatoAmount;
                    if(GameManager.Instance.hasGrenada)
                    {
                        GameManager.Instance.hasGrenada = false;
                        GameManager.Instance.grenadas++;
                    }
                    state = SquareState.empty;
                    potatoAmount = 0;
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
            }
        }
    }

    void Explode()
    {
        state = SquareState.empty;
        GameManager.Instance.hasGrenada = false;
    }

    public void CreateHole()
    {
        state = SquareState.hole;
        potatoAmount = 0;
    }

    public void GrowPotato()
    {
        state = SquareState.potato;
    } 

    public void GrowPotato(int amount)
    {
        state = SquareState.potato;
        potatoAmount += amount;
    }

    public void AddPotato()
    {
        potatoAmount += Mathf.RoundToInt(GameManager.Instance.potatoAddingCurve.Evaluate(potatoAmount));
        if(potatoAmount > GameManager.Instance.maxPotatoes)
        {
            potatoAmount = GameManager.Instance.maxPotatoes;
        }
    }

    public bool selected;
    public void Select()
    {
        if(type != SquareType.heart)
        {
            selectionLid.SetActive(true);
        }


        selected = true;
    }

    public void PressTequila(int potatoAmount)
    {
        GameManager.Instance.pressTequila += potatoAmount * tequilaMultiplier;
        GameManager.Instance.tequilaPressAnim.SetTrigger("press");
    }

    public void CompressPotato(int potatoAmount)
    {
        GameManager.Instance.grenadaPotatoes += potatoAmount;
        GameManager.Instance.grenadaFabricAnim.SetTrigger("compress");
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
