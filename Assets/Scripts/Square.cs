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

public enum Interaction
{
    explode,
    uproot,
    root
}

public class Square : MonoBehaviour {

    public SquareState state;
    public Animator numberPanelAnim;
    public TextMeshPro numberPanelText;
    public int potatoAmount;

    public Transform leavesParent;
    public List<GameObject> leaves;

    void Awake()
    {
        state = SquareState.empty;
        numberPanelAnim.SetBool("isBurrowed", false);

        //disable leaves
        leaves[0].SetActive(false);
        leaves[1].SetActive(false);
        leaves[2].SetActive(false);
        leaves[3].SetActive(false);
        leaves[4].SetActive(false);
        leavesParent.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
    }

    void Update()
    {
        //show and hide panel
        if(potatoAmount > 0)
        {
            if(numberPanelAnim.GetBool("isBurrowed") == true)
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
        if(potatoAmount > 0)
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

    public void Interact(Interaction interaction)
    {
        if(interaction == Interaction.explode)
        {
            state = SquareState.empty;
        }
        else if(interaction == Interaction.uproot)
        {
            state = SquareState.empty;
        }
        else
        {
            state = SquareState.potato;
        }
    }

    public void GrowPotato()
    {
        state = SquareState.potato;
    }

    
    public void AddPotato()
    {
        potatoAmount += Mathf.RoundToInt(GameManager.Instance.potatoAddingCurve.Evaluate(potatoAmount));
    }
}
