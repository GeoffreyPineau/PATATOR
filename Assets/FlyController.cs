using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class FlyController : MonoBehaviour {

    public NavMeshAgent agent;
    public Transform modelPivot;
    public float explodeShakeStrength;
    public float explodeDuration;
    public int explodeVibrato;

    private bool isExploding;

    private void Start()
    {
        agent.SetDestination(GameManager.Instance.heartPosition);
    }

    // Update is called once per frame
    void Update ()
    {
		if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono), out hit, Mathf.Infinity, 1 << 11);

            agent.SetDestination(hit.point);
        }
        
        if (!isExploding && Vector3.Distance(transform.position, GameManager.Instance.heartPosition) <= GameManager.Instance.heartRadius)
        {
            agent.isStopped = true;

            isExploding = true;

            modelPivot.DOShakeScale(explodeDuration, explodeShakeStrength, explodeVibrato,90,false).OnComplete(delegate
            {
                GameManager.Instance.DamageHeart(GameManager.Instance.flyDamage);
                Destroy(gameObject);
            });
        }
	}
}
