using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int columns;
    public int rows;

    public GameObject squarePrefab;
    public Transform squareParentTransform;
    List<Square> squareList = new List<Square>();
    List<Square> emptySquares;

    [Header("Potato Spawning")]
    public int initialPotatoNumber;
    public AnimationCurve potatoAddingCurve;

    private int[,] squaresArray;

    void Awake()
    {
        squaresArray = new int[columns, rows];
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
                    squareList.Add(newSquare.GetComponent<Square>());
                    newSquare.transform.position = new Vector3(x + 0.5f, 0, y);
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
    }

    void RefreshEmptySquares()
    {
        emptySquares = new List<Square>();
        foreach(Square square in squareList)
        {
            if(square.state == SquareState.empty)
            {
                emptySquares.Add(square);
            }
        }
    }

    private void OnDrawGizmos()
    {
        squaresArray = new int[columns, rows];

        for (int x = 0; x < squaresArray.GetLength(0); x ++)
        {
            for (int y = 0; y < squaresArray.GetLength(1); y++)
            {
                if ((x == 5 || x == 6) && (y == 3 || y == 4))
                {
                }
                else
                {
                    Gizmos.DrawSphere(new Vector3(x + .5f, 0f, y + .5f), .3f);
                }
            }
        }
    }
}
