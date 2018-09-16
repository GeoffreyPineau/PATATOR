using System.Collections;
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
    public List<Square> dirtSquareList = new List<Square>();
    List<Square> emptySquares;

    [Header("Player Values")]
    public int potatoesHeld;
    public TextMeshPro potatoesText;
    public bool hasGrenada;
    public float heldTequila;
    public float maxTequila;
    public float flameAbsorbtion;
    public float sombreroastSpeed;
    public float sombreroastLifetime;
    public float sombreroastRateOverTime;
    public float sombreroastMinDamage;
    public float sombreroastMaxDamage;
    public float sombreroastMinConsumption;
    public float sombreroastMaxConsumption;
    public float sombreroastCooldown = .5f;
    public float sombreroastHeatLossMultiplier = 2f;
    public float sombreroastMaxHeat = 20;
    public float sombreroastCurrentHeat = 0;
    [ColorUsage(false, true)]public  Color sombreroastMinGlow;
    [ColorUsage(false, true)]public  Color sombreroastMaxGlow;
    [ColorUsage(true, true)] public Color sombreroastMinColor;
    [ColorUsage(true, true)] public Color sombreroastMaxColor;

    [Header("Heart Values")]
    public int heartMaxLife;
    public int heartCurrentLife;
    public float heartRadius;
    public Vector3 heartPosition;

    [Header("Monster Values")]
    public int flyDamage;
    public float flyLife;

    [Header("Containers Values")]
    public int pressTequila;
    public int maxPressTequila;
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
    public List<Vector2> noHolesPositions;
    public List<Vector2> heartSquaresPositions;
    public Vector2 tequilaPressPosition;
    public Vector2 tequilaPumpPosition;
    public Vector2 grenadaCompressorPosition;
    public Vector2 grenadaPosition;
    public int tequilaMultiplier;

    [Header("Animations")]
    public Animator tequilaPressAnim;
    public Transform tequilaTransform;
    Vector3 targetScale;
    float scaleMultiplier;

    public Animator grenadaFabricAnim;
    public Animator playerHandsAnim;
    public Animator animatedGrenadaAnim;

    void Awake()
    {
        Instance = this;

        heartCurrentLife = heartMaxLife;

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

                    newSquareComponent.canBeHoled = true;
                    foreach (Vector2 position in noHolesPositions)
                    {
                        if(position == new Vector2(x, y))
                        {
                            newSquareComponent.canBeHoled = false;
                        }
                    }

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
                if(new Vector2(x, y) == grenadaCompressorPosition)
                {
                    GameObject newSquare = Instantiate(squarePrefab, squareParentTransform);
                    Square newSquareComponent = newSquare.GetComponent<Square>();

                    squaresArray[x, y] = newSquareComponent;
                    newSquareComponent.type = SquareType.compressor;
                    newSquare.name = "GrenadaCompressor";

                    newSquare.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
                }
                if(new Vector2(x, y) == grenadaPosition)
                {
                    GameObject newSquare = Instantiate(squarePrefab, squareParentTransform);
                    Square newSquareComponent = newSquare.GetComponent<Square>();

                    squaresArray[x, y] = newSquareComponent;
                    newSquareComponent.type = SquareType.grenada;
                    newSquare.name = "GrenadaPosition";

                    newSquare.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
                }
                foreach(Vector2 position in heartSquaresPositions)
                {
                    if(new Vector2(x, y) == position)
                    {
                        GameObject newSquare = Instantiate(squarePrefab, squareParentTransform);
                        Square newSquareComponent = newSquare.GetComponent<Square>();

                        squaresArray[x, y] = newSquareComponent;
                        newSquareComponent.type = SquareType.heart;
                        newSquare.name = "HeartPosition";

                        newSquare.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
                    }
                }
            }
        }
        FindObjectOfType<HoleCreator>().dirtSquares = dirtSquareList;

        scaleMultiplier = 1 / maxPressTequila;
    }

    void Update()
    {
        playerHandsAnim.SetBool("holdsPotato", false);
        playerHandsAnim.SetBool("holdsGrenada", false);
        if (potatoesHeld > 0)
        {
            potatoesText.text = potatoesHeld.ToString();
            playerHandsAnim.SetBool("holdsPotato", true);
            playerHandsAnim.SetBool("holdsGrenada", false);
        }
        else
        {
            potatoesText.text = "0";
        }

        //grenada compression
        if(grenadaPotatoes >= potatoesForGrenada)
        {
            grenadas++;
            grenadaPotatoes -= potatoesForGrenada;

        }

        if(hasGrenada)
        {
            playerHandsAnim.SetBool("holdsPotato", false);
            playerHandsAnim.SetBool("holdsGrenada", true);
        }

        //tequila level
        if(pressTequila > 0)
        {
            targetScale = new Vector3(1, scaleMultiplier * pressTequila, 1);
        }
        else
        {
            targetScale = Vector3.zero;
        }

        tequilaTransform.localScale = Vector3.Lerp(tequilaTransform.localScale, targetScale, 0.2f);

        if(grenadas > 0)
        {
            animatedGrenadaAnim.SetBool("isVisible", true);
        }
        else
        {
            animatedGrenadaAnim.SetBool("isVisible", false);
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

    public void DamageHeart(int damage)
    {
        if (heartCurrentLife <= 0) return;
        heartCurrentLife -= damage;

        if (heartCurrentLife <= 0)
        {
            LoseGame();
        }
    }

    public void LoseGame()
    {

    }

    public void WinGame()
    {

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
