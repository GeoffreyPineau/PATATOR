using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int columns;
    public int rows;

    public GameObject squarePrefab;
    public Transform squareParentTransform;
    List<Square> dirtSquareList = new List<Square>();
    List<Square> emptySquares;

    [Header("Player Values")]
    public int potatoesHeld;

    [Header("Potato Spawning")]
    public int initialPotatoNumber;
    public AnimationCurve potatoAddingCurve;
    public int twoLeavesMin;
    public int threeLeavesMin;
    public int fourLeavesMin;
    public int fiveLeavesMin;

    public Square[,] squaresArray;

    void Awake()
    {
        Instance = this;

        squaresArray = new Square[columns, rows];
        //generate squares
        for (int x = 0; x < squaresArray.GetLength(0); x++)
        {
            for (int y = 0; y < squaresArray.GetLength(1); y++)
            {
                if ((x == 5 || x == 6) && (y == 3 || y == 4))
                {
                }
                else
                {
                    GameObject newSquare = Instantiate(squarePrefab, squareParentTransform);
                    Square newSquareComponent = newSquare.GetComponent<Square>();

                    squaresArray[x, y] = newSquareComponent;
                    dirtSquareList.Add(newSquareComponent);

                    newSquare.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
                }
            }
        }
    }

    public void SpawnPotatos()
    {
        for(int i = 0; i < initialPotatoNumber; i++)
        {
            RefreshEmptySquares();
            Square chosenSquare = emptySquares[Random.Range(0, emptySquares.Count)];
            chosenSquare.GrowPotato();
        }
        foreach (Square square in dirtSquareList)
        {
            if (square.state == SquareState.potato)
            {
                square.AddPotato();
            }
        }
    }

    void RefreshEmptySquares()
    {
        emptySquares = new List<Square>();
        foreach(Square square in dirtSquareList)
        {
            if(square.state == SquareState.empty)
            {
                emptySquares.Add(square);
            }
        }
    }

    private void OnDrawGizmos()
    {
        int [,] fakeArray = new int[columns, rows];

        for (int x = 0; x < fakeArray.GetLength(0); x ++)
        {
            for (int y = 0; y < fakeArray.GetLength(1); y++)
            {
                if ((x == 5 || x == 6) && (y == 3 || y == 4))
                {
                }
                else
                {
                    Gizmos.DrawSphere(new Vector3(x + .5f, 0f, y + .5f), .1f);
                }
            }
        }
    }
}
