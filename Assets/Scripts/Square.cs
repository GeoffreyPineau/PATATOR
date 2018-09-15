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

    AnimationCurve addingCurve;
    GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        addingCurve = gameManager.potatoAddingCurve;
        state = SquareState.empty;
        numberPanelAnim.SetBool("isBurrowed", false);
    }

    void Update()
    {
        //show and hide panel
        if(potatoAmount > 0)
        {
            numberPanelAnim.SetBool("isBurrowed", false);
            numberPanelText.text = potatoAmount.ToString();
        }
        else
        {
            numberPanelAnim.SetBool("isBurrowed", true);
        }
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
        potatoAmount = 1;
    }

    
    void AddPotato()
    {
        potatoAmount += Mathf.RoundToInt(addingCurve.Evaluate(potatoAmount));
    }
}
