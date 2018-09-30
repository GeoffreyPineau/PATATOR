using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int columns;
    public int rows;

    public GameObject squarePrefab;
    public Transform squareParentTransform;
    public List<Square> dirtSquareList = new List<Square>();
    public List<Square> holableSquares;
    List<Square> emptySquares;

    [Header("Leveling")]
    public List<Level> levels;
    public int currentLevel;
    public TextMeshProUGUI levelText;

    [Header("UI")]
    public Texture2D cursor;
    public GameObject heartExploPrefab;
    public Image healthBar;
    public TextMeshProUGUI tequilaText;
    public TextMeshPro pressTequilaText;
    public Image tequilaBar;
    public Image tequilaBarBack;
    float targetFill = 1;

    [Header("Player Values")]
    public Vector3 squashStepValue = new Vector3(0.2f, -0.2f, 0.2f);
    public float squashStepDuration = .8f;
    public int potatoesHeld;
    public TextMeshPro potatoesText;
    public bool hasGrenada;
    public float stepCooldown;
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
    public float flyBurnedLife;

    [Header("Containers Values")]
    public int pressTequila;
    public int maxPressTequila;
    public int grenadaPotatoes;
    public int potatoesForGrenada;
    public int grenadas;

    public AudioClip grenadaDrop;
    public float dropVolume;

    public AudioClip tequilaLiquidSound;
    public float tequilaLiquidVolume;

    [Header("Potato Spawning")]
    public int initialPotatoNumber;
    public int potatoIncrement;
    int wave;
    public float potatoSpawningDelay;
    public int maxPotatoes;
    public int potatoGrowth;
    public int twoLeavesMin;
    public int threeLeavesMin;
    public int fourLeavesMin;
    public int fiveLeavesMin;
    
    public AudioClip potatoUproot;
    public float uprootVolume;
    public AudioClip potatoPlanting;
    public float plantingVolume;

    [Header("Monster Spawning")]
    float flyTimer;
    public float outsideFlyDelay;
    public AnimationCurve yCurve;
    public GameObject flyPrefab;
    public Transform flyParent;
    public float flyDelay;

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
    Vector3 tequilaTargetScale;
    float scaleMultiplier;

    public Transform heartPotatoTransform;
    float heartScaleMultiplier;

    public Animator grenadaFabricAnim;
    public Animator grenadaPanelAnim;
    public TextMeshPro grenadaText;
    public Animator playerHandsAnim;
    public Animator animatedGrenadaAnim;

    public GameObject heartLid;


    void Awake()
    {
        Instance = this;
        heartScaleMultiplier = (0.938f - 0.623f) / heartMaxLife;
        heartPotatoTransform.localScale = new Vector3(0.938f, 0.938f, 0.938f);
        newScale = 0.938f;

        Cursor.SetCursor(cursor, new Vector2(cursor.width, cursor.height), CursorMode.Auto);

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
                        if (position == new Vector2(x, y))
                        {
                            newSquareComponent.canBeHoled = false;
                        }
                    }
                    if(newSquareComponent.canBeHoled)
                    {
                        holableSquares.Add(newSquareComponent);
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
                heartSquares = new List<Square>();
                foreach(Vector2 position in heartSquaresPositions)
                {
                    if(new Vector2(x, y) == position)
                    {
                        GameObject newSquare = Instantiate(squarePrefab, squareParentTransform);
                        Square newSquareComponent = newSquare.GetComponent<Square>();

                        squaresArray[x, y] = newSquareComponent;
                        newSquareComponent.type = SquareType.heart;
                        newSquare.name = "HeartPosition";
                        Destroy(newSquareComponent.selectionLid);
                        newSquareComponent.selectionLid = Instantiate(heartLid);
                        newSquareComponent.selectionLid.transform.position = heartLid.transform.position;
                        heartSquares.Add(newSquareComponent);

                        newSquare.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
                    }
                }
            }
        }
        FindObjectOfType<HoleCreator>().dirtSquares = dirtSquareList;


    }

    private void Start()
    {
        scaleMultiplier = 1f / maxPressTequila;
        print("Scale multiplier = " + scaleMultiplier);
        levelText.text = "1";
        heartMaxLife = levels[0].heartMaxLife;
        heartCurrentLife = heartMaxLife / 2;
        potatoGrowth = levels[0].potatoGrowth;
        maxPotatoes = levels[0].potatoMax;
        for(int i = 0; i< initialPotatoNumber; i++)
        {
            SpawnPotatos();
        }

    }

    float newScale;
    float newLifeScale;
    float currentTequilaNumber;
    List<Square> heartSquares;
    void Update()
    {
        tequilaText.text = (Mathf.Round(heldTequila)).ToString();
        currentTequilaNumber = Mathf.Lerp(currentTequilaNumber, pressTequila, 0.1f);
        pressTequilaText.text = Mathf.RoundToInt(currentTequilaNumber).ToString();
    
        //heart scale
        newScale = Mathf.Lerp(newScale, 0.623f + (heartScaleMultiplier * heartCurrentLife), 0.1f);
        heartPotatoTransform.localScale = new Vector3(newScale, newScale, newScale);

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

        newLifeScale = Mathf.Lerp(newLifeScale, (1f/heartMaxLife) * heartCurrentLife, 0.4f);
        healthBar.fillAmount = newLifeScale;

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



        if(grenadas > 0)
        {
            animatedGrenadaAnim.SetBool("isVisible", true);
            grenadaPanelAnim.SetBool("isBurrowed", false);
            grenadaText.text = grenadas.ToString();
        }
        else
        {
            animatedGrenadaAnim.SetBool("isVisible", false);
            grenadaPanelAnim.SetBool("isBurrowed", true);
        }

        //level up
        if(heartCurrentLife >= heartMaxLife)
        {
            LevelUp();
        }
       
        //outside fly spawning
        if(!TimeManager.Instance.isDay)
        {
            flyTimer += Time.deltaTime;
            if(flyTimer >= outsideFlyDelay)
            {
                flyTimer = 0;
                SpawnOutsideFly();
            }
        }

    }

    float ref1;
    float ref2;
    private void FixedUpdate()
    {
        //tequila level
        if (pressTequila > 0)
        {
            tequilaTargetScale = new Vector3(1, scaleMultiplier * pressTequila, 1);
        }
        else
        {
            tequilaTargetScale = Vector3.zero;
        }

        tequilaTransform.localScale = Vector3.Lerp(tequilaTransform.localScale, tequilaTargetScale, 0.1f);

        targetFill = (1 / maxTequila) * heldTequila;
        tequilaBar.fillAmount = Mathf.SmoothDamp(tequilaBar.fillAmount, targetFill, ref ref1, 0.02f);
        tequilaBarBack.fillAmount = Mathf.SmoothDamp(tequilaBarBack.fillAmount, targetFill, ref ref2, 3f);
    }

    public void SpawnPotatos()
    {
        foreach (Square square in dirtSquareList)
        {
            if (square.state == SquareState.potato)
            {
                square.PotatoGrowth();
            }
        }
        float ia = 0;
        //for (int i = 0; i < (initialPotatoNumber + (potatoIncrement * wave)); i++)
        //{
            RefreshEmptySquares();
            Square chosenSquare = emptySquares[Random.Range(0, emptySquares.Count)];
            StartCoroutine(WaitThenSpawnPotato(ia, chosenSquare));
            //RefreshEmptySquares();
            //chosenSquare = emptySquares[Random.Range(0, emptySquares.Count)];
            //StartCoroutine(WaitThenSpawnPotato(potatoSpawningDelay, chosenSquare));
            //ia += potatoSpawningDelay;
        //}

        //wave++;


    }

    IEnumerator WaitThenSpawnPotato(float delay, Square square)
    {
        yield return new WaitForSeconds(delay);
        square.GrowPotato();
        square.potatoAmount = 1;
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
            Instantiate(heartExploPrefab, heartPosition, heartExploPrefab.transform.rotation);
        }
    }

    public void LoseGame()
    {
        PauseButton.Instance.GameOver();
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

    void LevelUp()
    {
        currentLevel++;
        levelText.text = (currentLevel + 1).ToString();
        Level newLevel = levels[currentLevel];
        heartMaxLife = newLevel.heartMaxLife;
        maxPotatoes = newLevel.potatoMax;
        potatoGrowth = newLevel.potatoGrowth;

        int d = Mathf.RoundToInt(newLevel.potatoMax / 5f);
        twoLeavesMin = d;
        threeLeavesMin = d * 2;
        fourLeavesMin = d * 3;
        fiveLeavesMin = d * 4;

    }

    Vector3 flyPos;
    public void SpawnOutsideFly()
    {
        float x = Random.Range(-10f, 10f);
        float y = yCurve.Evaluate(x);
        flyPos = new Vector3(x, 0, y);

        flyPos += heartPosition;
        FlyController newFly = Instantiate(flyPrefab, flyPos, Quaternion.identity, flyParent).GetComponent<FlyController>();
    }
}
