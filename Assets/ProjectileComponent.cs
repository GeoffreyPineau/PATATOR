using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileComponent : MonoBehaviour
{
    public Rigidbody rb;
    public float damage;
    public float lifetime;
    
    private float lifeStartTimestamp;

    private void Start()
    {
        lifeStartTimestamp = Time.time;
    }

    private void Update()
    {
        if (Time.time > lifeStartTimestamp + lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        FlyController flyController = other.GetComponent<FlyController>();

        if (flyController)
        {
            if (flyController.state == FlyState.ash) return;

            flyController.Damage(damage);
            
            lifetime = lifetime * GameManager.Instance.flameAbsorbtion;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, .2f);
    }
}
