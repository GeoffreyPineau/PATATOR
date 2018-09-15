﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public TextMeshPro potatoesText;
    public bool hasGrenada;
    public int heldTequila;
    public int maxTequila;

    [Header("Containers Values")]
    public int pressTequila;
    public int grenadaPotatoes;
    public int potatoesForGrenada;
    public int grenadas;

    [Header("Potato Spawning")]
    public int initialPotatoNumber;
    public int maxPotatoes;
    public AnimationCurve potatoAddingCurve;
    public int twoLeavesMin;
    public int threeLeavesMin;
    public int fourLeavesMin;
    public int fiveLeavesMin;

    public Square[,] squaresArray;

    public List<Vector2> excludedPositions;
    public Vector2 tequilaPressPosition;
    public Vector2 tequilaPumpPosition;
    public int tequilaMultiplier;


    void Awake()
    {
        Instance = this;

        squaresArray = new Square[columns, rows];
        //generate squares
        for (int x = 0; x < squaresArray.GetLength(0); x++)
        {
            for (int y = 0; y < squaresArray.GetLength(1); y++)
            {
                bool createsSquare = true;
                foreach(Vector2 position in excludedPositions)
                {
                    if(position == new Vector2(x, y))
                    {
                        createsSquare = false;
                        break;
                    }
                }
                if(createsSquare)
                {
                    GameObject newSquare = Instantiate(squarePrefab, squareParentTransform);
                    Square newSquareComponent = newSquare.GetComponent<Square>();

                    squaresArray[x, y] = newSquareComponent;
                    dirtSquareList.Add(newSquareComponent);
                    newSquareComponent.type = SquareType.dirt;
                    newSquare.name = "DirtSquare " + x + "," + y;

                    newSquare.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
                }
                if(new Vector2(x, y) == tequilaPressPosition)
                {
                    GameObject newSquare = Instantiate(squarePrefab, squareParentTransform);
                    Square newSquareComponent = newSquare.GetComponent<Square>();

                    squaresArray[x, y] = newSquareComponent;
                    newSquareComponent.type = SquareType.press;
                    newSquare.name = "TequilaPress";
                    newSquareComponent.tequilaMultiplier = tequilaMultiplier;

                    newSquare.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
                }
                if (new Vector2(x, y) == tequilaPumpPosition)
                {
                    GameObject newSquare = Instantiate(squarePrefab, squareParentTransform);
                    Square newSquareComponent = newSquare.GetComponent<Square>();

                    squaresArray[x, y] = newSquareComponent;
                    newSquareComponent.type = SquareType.pump;
                    newSquare.name = "TequilaPump";

                    newSquare.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
                }
            }
        }
    }

    void Update()
    {
        if(potatoesHeld > 0)
        {
            potatoesText.text = potatoesHeld.ToString();
        }
        else
        {
            potatoesText.text = "0";
        }

        //grenada compression

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
                bool createsSquare = true;
                foreach (Vector2 position in excludedPositions)
                {
                    if (position == new Vector2(x, y))
                    {
                        createsSquare = false;
                        break;
                    }
                }
                if (createsSquare)
                {
                    Gizmos.DrawSphere(new Vector3(x + .5f, 0f, y + .5f), .1f);
                }
            }
        }
    }
}