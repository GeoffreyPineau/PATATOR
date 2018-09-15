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
    public GameObject flamePrefab;

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
    private float targetAngleFull;
    private float targetAngle;
    private float currentRotationVelocity;

    private Tween squashTween;
    private Vector3 aimPosition;
    private Vector3 aimDirection;
    private Vector3 interactionPosition;

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
            rb.AddForce(directionInput * dashSpeed, ForceMode.Impulse);
            //Squash
            if (squashTween != null) squashTween.Kill(true);
            squashTween = modelPivot.DOPunchScale(squashDashValue, squashDashDuration);
        }
        //
        
        //Turn player model body
        if (directionInput != Vector3.zero)
        {
            targetAngle = Utilities.Angle(directionInput.x, directionInput.z);
            
            targetAngleFull = Utilities.AngleFull(directionInput.x, directionInput.z);
        }
        bodyPivot.eulerAngles = new Vector3(0, targetAngle);
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

        interactionInput = Input.GetMouseButtonDown(1);
        if (interactionInput)
        {
            GameManager.Instance.squaresArray[(int)interactionPosition.x, (int)interactionPosition.z].Interact();
            print(GameManager.Instance.squaresArray[(int)interactionPosition.x, (int)interactionPosition.z].state);
        }

        //Shoot
        Shoot();
    }

    private void FixedUpdate()
    {
        rb.AddForce(directionInput * speed);
    }

    void Shoot()
    {
        if (!Input.GetMouseButton(0))
        {
            Cooldown();
            return;
        }

        if (Time.time < lastFlameTimestamp + GameManager.Instance.sombreroastCooldown)
        {
            Cooldown();
            return;
        }

        int tequilaCost = (int)Mathf.Lerp(GameManager.Instance.sombreroastMinConsumption, GameManager.Instance.sombreroastMaxConsumption,
            Mathf.InverseLerp(0, GameManager.Instance.sombreroastMaxHeat, GameManager.Instance.sombreroastCurrentHeat));
        if (GameManager.Instance.heldTequila < tequilaCost)
        {
            Cooldown();
            return;
        }
        else
        {
            GameManager.Instance.heldTequila -= tequilaCost;

            ProjectileComponent projectile = Instantiate(flamePrefab, transform.position + aimDirection * interactionPositionOffset*4, Quaternion.identity).GetComponent<ProjectileComponent>();
            projectile.rb.velocity = aimDirection * GameManager.Instance.sombreroastProjectileSpeed;
            projectile.damage = (int)Mathf.Lerp(GameManager.Instance.sombreroastMinDamage, GameManager.Instance.sombreroastMaxDamage,
            Mathf.InverseLerp(0, GameManager.Instance.sombreroastMaxHeat, GameManager.Instance.sombreroastCurrentHeat));

            GameManager.Instance.sombreroastCurrentHeat += GameManager.Instance.sombreroastHeatGain;
            GameManager.Instance.sombreroastCurrentHeat = Mathf.Clamp(GameManager.Instance.sombreroastCurrentHeat, 0, GameManager.Instance.sombreroastMaxHeat);
        }

        lastFlameTimestamp = Time.time;
    }

    void Cooldown()
    {
        if (Time.time > lastFlameTimestamp + GameManager.Instance.sombreroastCooldown)
        {
            GameManager.Instance.sombreroastCurrentHeat -= Time.deltaTime;
            GameManager.Instance.sombreroastCurrentHeat = Mathf.Clamp(GameManager.Instance.sombreroastCurrentHeat, 0, GameManager.Instance.sombreroastMaxHeat);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(interactionPosition, .1f);

        interactionPosition = Utilities.GetFlooredPosition(interactionPosition);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(interactionPosition,new Vector3(1,.1f,1));
        Gizmos.color = Color.gray;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(aimPosition, .2f);
        Gizmos.color = Color.gray;

        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, aimDirection + Vector3.up * 0.5f);
    }
}
