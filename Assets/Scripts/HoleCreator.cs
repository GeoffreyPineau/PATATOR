using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleCreator : MonoBehaviour {

    public static HoleCreator Instance;

    public List<Square> dirtSquares;
    List<Square> availableSquares;

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
            if(square.state != SquareState.hole)
            {
                availableSquares.Add(square);
            }
        }
    }

    IEnumerator SpawnHole()
    {
        RefreshSquares();
        Square chosenSquare = availableSquares[Random.Range(0, availableSquares.Count)];
        yield return new WaitForSeconds(TimeManager.Instance.currentDayTime / 2);
        chosenSquare.CreateHole();
    }

    public void SpawnHoles()
    {

        int holes = initialNumber + (numberBonus * currentWave);
        if (holes > maxHoles) holes = maxHoles;
        for(int i = 0; i < holes; i++)
        {
            StartCoroutine("SpawnHole");
        }
        currentWave++;
    }
}
