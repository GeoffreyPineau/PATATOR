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

    [Header("Dirt")]
    public Animator numberPanelAnim;
    public TextMeshPro numberPanelText;
    public int potatoAmount;

    public Transform leavesParent;
    public List<GameObject> leaves;

    public List<GameObject> stateGraphics;

    [Header("Tequila")]
    public int tequilaMultiplier;


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
                    stateGraphics[2].SetActive(false);
                    stateGraphics[3].SetActive(true);
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
    }

    public void Interact()
    {
        if (type == SquareType.dirt)
        {
            if(state == SquareState.hole)
            {
                if (!TimeManager.Instance.isDay)
                {
                    if(GameManager.Instance.hasGrenada)
                    {
                        Explode();
                        GameManager.Instance.hasGrenada = false;
                    }
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
                    int addedTequila = GameManager.Instance.maxTequila - GameManager.Instance.heldTequila;
                    if(addedTequila > GameManager.Instance.pressTequila)
                    {
                        addedTequila = GameManager.Instance.pressTequila;
                    }
                    GameManager.Instance.pressTequila -= addedTequila;
                    GameManager.Instance.heldTequila += addedTequila;
                    if(GameManager.Instance.heldTequila > GameManager.Instance.maxTequila)
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
    }

    void Explode()
    {
        state = SquareState.empty;
    }

    public void CreateHole()
    {
        state = SquareState.hole;
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

    public void PressTequila(int potatoAmount)
    {
        GameManager.Instance.pressTequila += potatoAmount * tequilaMultiplier;
    }

    public void CompressPotato(int potatoAmount)
    {
        GameManager.Instance.grenadaPotatoes += potatoAmount;
    }
}
