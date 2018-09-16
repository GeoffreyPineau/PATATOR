using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameCollisionController : MonoBehaviour {

    public GameObject flameCollider;
    
    private float lastColliderTimestamp;

    private void OnEnable()
    {
        lastColliderTimestamp = 0;
    }

    private void Update()
    {
        if (Time.time > lastColliderTimestamp + (1/GameManager.Instance.sombreroastRateOverTime))
        {
            ProjectileComponent projComponent = Instantiate(flameCollider).GetComponent<ProjectileComponent>();
            projComponent.rb.transform.position = transform.position;
            projComponent.rb.velocity = transform.forward * GameManager.Instance.sombreroastSpeed;
            projComponent.lifetime = GameManager.Instance.sombreroastLifetime;
            
            projComponent.damage = Mathf.Lerp(GameManager.Instance.sombreroastMinDamage, GameManager.Instance.sombreroastMaxDamage,
            Mathf.InverseLerp(0, GameManager.Instance.sombreroastMaxHeat, GameManager.Instance.sombreroastCurrentHeat));

            lastColliderTimestamp = Time.time;
        }
    }
}
