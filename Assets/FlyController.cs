using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public enum FlyState
{
    alive,
    dead,
    ash
}

public class FlyController : MonoBehaviour {

    private FlyState state = FlyState.alive;

    public NavMeshAgent agent;
    public Transform modelPivot;
    public float explodeShakeStrength;
    public float explodeDuration;
    public int explodeVibrato;

    public float hitShakeStrength;
    public float hitDuration;
    public int hitVibrato;

    public float blinkTime;
    [ColorUsage(false,true)] public Color blinkColor;

    public float fallSpeed;
    [ColorUsage(false, true)] public Color burnColor;

    private float health;

    private bool isExploding;
    private Tween squashTween;
    [SerializeField] private List<MeshRenderer> rendererList = new List<MeshRenderer>();
    [SerializeField] private List<Color> startColorList = new List<Color>();

    private void Start()
    {
        foreach(MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            rendererList.Add(meshRenderer);
            startColorList.Add(meshRenderer.material.color);
        }

        health = GameManager.Instance.flyLife;
        agent.SetDestination(GameManager.Instance.heartPosition);
    }

    // Update is called once per frame
    void Update ()
    {/*
		if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono), out hit, Mathf.Infinity, 1 << 11);

            agent.SetDestination(hit.point);
        }*/
        
        if (!isExploding && Vector3.Distance(transform.position, GameManager.Instance.heartPosition) <= GameManager.Instance.heartRadius)
        {
            agent.isStopped = true;

            isExploding = true;

            if (squashTween != null) squashTween.Kill(true);
            squashTween = modelPivot.DOShakeScale(explodeDuration, explodeShakeStrength, explodeVibrato,90,false).OnComplete(delegate
            {
                GameManager.Instance.DamageHeart(GameManager.Instance.flyDamage);
                Destroy(gameObject);
            });
        }
	}
    
    public void Damage(float _damage)
    {
        if (isExploding) return;

        health -= _damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(BlinkRed());
            if (squashTween != null) squashTween.Kill(true);
            squashTween = modelPivot.DOShakeScale(hitDuration, hitShakeStrength, hitVibrato, 90, false);
        }
    }

    IEnumerator BlinkRed()
    {
        foreach (MeshRenderer meshRenderer in rendererList)
        {
            meshRenderer.material.color = blinkColor;
        }

        yield return new WaitForSeconds(blinkTime);

        int i = 0;
        foreach (MeshRenderer meshRenderer in rendererList)
        {
            meshRenderer.material.color = startColorList[i];
            i++;
        }
    }
}
