using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour {


    [Header("References")]
    public Rigidbody rb;
    public Transform modelPivot;
    public Transform bodyPivot;
    public Transform headPivot;
    public ParticleSystem psFlames;
    public GameObject psDamage;
    public GameObject stepPrefab;
    public ParticleSystem psFart;
    public Transform canonPivot;
    public GameObject armeVidePrefab;
    public AudioSource flameSource;
    public AudioSource puffSource;
    public AudioSource stepSource;
    public AudioSource fartSource;
    public AudioSource flameFartSource;
    public List<AudioClip> footsteps = new List<AudioClip>();
    public List<AudioClip> farts = new List<AudioClip>();
    public float footstepVolume = 0.2f;
    public float flameVolume = 0.3f;

    [Header("Variables")]
    [Space]
    [Header("Movement")]
    public float speed = 300;
    public float dashSpeed = 100;
    [Range(0f,1f)]public float turnDamp = 0.1f;
    [Header("Interaction")]
    public float interactionPositionOffset = .3f;
    [Header("Juice")]
    public Vector3 squashDashValue = new Vector3(0.2f, -0.2f, 0.2f);
    public float squashDashDuration = .8f;

    private bool interactionInput;
    private bool dashInput;
    private Vector3 directionInput;
    //private float targetAngleFull;
    private float targetAngle;
    private float currentRotationVelocity;

    private Tween squashTween;
    private Vector3 aimPosition;
    private Vector3 aimDirection;
    private Vector3 interactionPosition;
    private float lastStepTimestamp;

    private float lastFlameTimestamp;

    private void Update()
    {
        //Get player input
        //Movement
        directionInput.x = Input.GetAxisRaw("Horizontal");
        directionInput.z = Input.GetAxisRaw("Vertical");
        directionInput.Normalize();

        //Dash
        dashInput = Input.GetKeyDown(KeyCode.Space) && directionInput != Vector3.zero;
        if (dashInput)
        {
            /*GameObject smokePuff = Instantiate(stepPrefab, transform.position, stepPrefab.transform.rotation);
            ParticleSystem parts = smokePuff.GetComponent<ParticleSystem>();
            float totalDuration = parts.main.startLifetime.constantMax;
            Destroy(smokePuff, totalDuration);*/
            psFart.Play();
            lastStepTimestamp = Time.time;

            rb.AddForce(directionInput * dashSpeed, ForceMode.Impulse);
            //Squash
            if (squashTween != null) squashTween.Kill(true);
            squashTween = modelPivot.DOPunchScale(squashDashValue, squashDashDuration);

            fartSource.clip = farts[Random.Range(0, farts.Count)];
            fartSource.pitch = Random.Range(0.8f, 1.2f);
            fartSource.Play();

            flameFartSource.pitch = Random.Range(0.7f, 1.3f);
            flameFartSource.Play();
        }
        //
        
        if (directionInput != Vector3.zero)
        {
            // STEP
            if (Time.time > lastStepTimestamp + GameManager.Instance.stepCooldown)
            {
                GameObject smokePuff = Instantiate(stepPrefab, transform.position, stepPrefab.transform.rotation);
                ParticleSystem parts = smokePuff.GetComponent<ParticleSystem>();
                float totalDuration = parts.main.startLifetime.constantMax;
                Destroy(smokePuff, totalDuration);
                lastStepTimestamp = Time.time;

                if (squashTween != null) squashTween.Kill(true);
                squashTween = modelPivot.DOPunchScale(GameManager.Instance.squashStepValue, GameManager.Instance.squashStepDuration);

                stepSource.clip = footsteps[Random.Range(0, footsteps.Count)];
                stepSource.Play();
            }

            targetAngle = Utilities.Angle(directionInput.x, directionInput.z);
            //targetAngleFull = Utilities.AngleFull(directionInput.x, directionInput.z);
        }

        //Turn player model body
        bodyPivot.eulerAngles = new Vector3(0, Mathf.SmoothDampAngle(modelPivot.eulerAngles.y, targetAngle, ref currentRotationVelocity, turnDamp),0);
        // Mathf.SmoothDampAngle(modelPivot.eulerAngles.y, targetAngle, ref currentRotationVelocity, turnDamp, Mathf.Infinity, Time.deltaTime)
        //

        //Aim
        RaycastHit hit;
        // A OPTIMISER 
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono),out hit,Mathf.Infinity,1 << 11);
        aimPosition = hit.point + Vector3.up * 0.5f;

        aimDirection = aimPosition - (transform.position + Vector3.up * 0.5f);
        aimDirection.Normalize();
        //Turn Head
        headPivot.eulerAngles = new Vector3(0, Utilities.Angle(aimDirection.x, aimDirection.z));
        //

        //Interaction
        aimDirection.y = 0;
        interactionPosition = transform.position + (aimDirection * interactionPositionOffset);
        interactionPosition = Utilities.GetFlooredPosition(interactionPosition);

        if (interactionPosition.x > GameManager.Instance.squaresArray.GetLength(0) ||
            interactionPosition.x < 0 ||
            interactionPosition.y > GameManager.Instance.squaresArray.GetLength(1) ||
            interactionPosition.y < 0)
            return;

        interactionInput = Input.GetMouseButtonDown(1);
        if (interactionInput)
        {
            GameManager.Instance.squaresArray[(int)interactionPosition.x, (int)interactionPosition.z].Interact();
            print(GameManager.Instance.squaresArray[(int)interactionPosition.x, (int)interactionPosition.z].state);
        }

        GameManager.Instance.squaresArray[(int)interactionPosition.x, (int)interactionPosition.z].Select();
        
    }

    private void FixedUpdate()
    {
        rb.AddForce(directionInput * speed);

        //Shoot
        Shoot();
    }
    
    void Shoot()
    {

        bool hasShot = false;
        /*
        var main = psFlames.main;
        main.startColor = Color.Lerp(GameManager.Instance.sombreroastMinColor, GameManager.Instance.sombreroastMaxColor,
            Mathf.InverseLerp(0, GameManager.Instance.sombreroastMaxHeat, GameManager.Instance.sombreroastCurrentHeat));
            */
        if (!Input.GetMouseButton(0))
        {
            Cooldown();
            return;
        }
        float tequilaCost = Mathf.Lerp(GameManager.Instance.sombreroastMinConsumption, GameManager.Instance.sombreroastMaxConsumption,
            Mathf.InverseLerp(0, GameManager.Instance.sombreroastMaxHeat, GameManager.Instance.sombreroastCurrentHeat));
        if (GameManager.Instance.heldTequila < tequilaCost)
        {
            Cooldown();
        }
        else
        {
            GameManager.Instance.heldTequila -= tequilaCost;

            if (!psFlames.isEmitting) psFlames.Play();
            if (!psDamage.activeInHierarchy) psDamage.SetActive(true);

            hasShot = true;
            if (!flameSource.isPlaying) flameSource.Play();

            GameManager.Instance.sombreroastCurrentHeat += Time.deltaTime;
            GameManager.Instance.sombreroastCurrentHeat = Mathf.Clamp(GameManager.Instance.sombreroastCurrentHeat, 0, GameManager.Instance.sombreroastMaxHeat);
        }

        if (!hasShot && Time.time > lastFlameTimestamp + GameManager.Instance.sombreroastCooldown)
        {
            GameObject smokePuff = Instantiate(armeVidePrefab, canonPivot.position, canonPivot.rotation);
            ParticleSystem parts = smokePuff.GetComponent<ParticleSystem>();
            float totalDuration = parts.main.startLifetime.constantMax;
            Destroy(smokePuff, totalDuration);

            puffSource.Play();

            lastFlameTimestamp = Time.time;
        }
    }

    void Cooldown()
    {
        if (flameSource.isPlaying) flameSource.Stop();

        if (psFlames.isEmitting) psFlames.Stop();
        if (psDamage.activeInHierarchy) psDamage.SetActive(false);

        if (Time.time > lastFlameTimestamp + GameManager.Instance.sombreroastCooldown)
        {
            GameManager.Instance.sombreroastCurrentHeat -= Time.deltaTime * GameManager.Instance.sombreroastHeatLossMultiplier;
            GameManager.Instance.sombreroastCurrentHeat = Mathf.Clamp(GameManager.Instance.sombreroastCurrentHeat, 0, GameManager.Instance.sombreroastMaxHeat);
        }
    }


    private void OnDrawGizmos()
    {/*
        Gizmos.DrawSphere(interactionPosition, .1f);

        interactionPosition = Utilities.GetFlooredPosition(interactionPosition);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(interactionPosition,new Vector3(1,.1f,1));
        Gizmos.color = Color.gray;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(aimPosition, .2f);
        Gizmos.color = Color.gray;

        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, aimDirection + Vector3.up * 0.5f);*/
    }
}
