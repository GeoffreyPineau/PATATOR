using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleCreator : MonoBehaviour {

    public static HoleCreator Instance;

    public List<Square> dirtSquares;
    List<Square> availableSquares;

    public Color normalColor;
    public Color dangerColor;

    [Header("Tweaking")]
    public int initialNumber;
    public int numberBonus;
    public int maxHoles;
    int currentWave;

    private void Awake()
    {
        Instance = this;
    }

    void RefreshSquares()
    {
        availableSquares = new List<Square>();
        foreach(Square square in dirtSquares)
        {
            if(square.state != SquareState.hole && square.canBeHoled)
            {
                availableSquares.Add(square);
            }
        }
    }

    void WarnHole()
    {
        RefreshSquares();
        Square chosenSquare = availableSquares[Random.Range(0, availableSquares.Count)];
        chosenSquare.Redden();
        warnedHoles.Add(chosenSquare);
    }

    public void CreateHoles()
    {
        foreach(Square square in warnedHoles)
        {
            square.CreateHole();
            square.Normalize();
        }
    }

    List<Square> warnedHoles;
    public void WarnHoles()
    {
        warnedHoles = new List<Square>();
        int holes = initialNumber + (numberBonus * currentWave);
        if (holes > maxHoles) holes = maxHoles;
        for(int i = 0; i < holes; i++)
        {
            WarnHole();
        }
        currentWave++;
    }
}
