using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileComponent : MonoBehaviour
{
    public Rigidbody rb;
    public int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        FlyController flyController = other.GetComponent<FlyController>();

        if (flyController)
        {
            flyController.Damage(damage);
        }

        Destroy(gameObject);
    }
}
