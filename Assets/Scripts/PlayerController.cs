using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    public Rigidbody rb;
    public Transform modelPivot;

    [Header("Variables")]
    [Space]
    [Header("Movement")]
    public float speed = 300;
    public float dashSpeed = 100;
    [Range(0f,1f)]public float turnDamp = 0.1f;
    [Header("Movement")]
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
        
        //Turn player model
        if (directionInput != Vector3.zero)
        {
            targetAngle = Utilities.Angle(directionInput.x, directionInput.z);
            targetAngleFull = Utilities.AngleFull(directionInput.x, directionInput.z);
        }
        modelPivot.eulerAngles = new Vector3(0, Mathf.SmoothDampAngle(modelPivot.eulerAngles.y, targetAngle, ref currentRotationVelocity, turnDamp, Mathf.Infinity, Time.deltaTime));
        //
        
        //Interaction
        Vector3 direction = new Vector3(Mathf.Cos(targetAngleFull * Mathf.Deg2Rad), 0, Mathf.Sin(targetAngleFull * Mathf.Deg2Rad)).normalized;
        Vector3 interactionPosition = transform.position + (direction * interactionPositionOffset);
        interactionPosition = Utilities.GetFlooredPosition(interactionPosition);

        interactionInput = Input.GetMouseButtonDown(1);
        if (interactionInput)
        {
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(directionInput * speed);
    }

    private void OnDrawGizmos()
    {
        Vector3 direction = new Vector3(Mathf.Cos(targetAngleFull * Mathf.Deg2Rad), 0, Mathf.Sin(targetAngleFull * Mathf.Deg2Rad)).normalized;
        Vector3 interactionPosition = transform.position + (direction * interactionPositionOffset);
        Gizmos.DrawSphere(interactionPosition, .1f);

        interactionPosition = Utilities.GetFlooredPosition(interactionPosition);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(interactionPosition,new Vector3(1,.1f,1));
        Gizmos.color = Color.gray;
    }
}
