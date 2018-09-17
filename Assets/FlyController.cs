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

    public FlyState state = FlyState.alive;

    public ParticleSystem combustion;
    public GameObject ashPrefab;
    public GameObject explosionPrefab;

    public AudioSource exploSource;
    public AudioSource burnSource;
    public AudioSource wingSource;

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
    public AnimationCurve deathSquashCurve;
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
    {
        if (!isExploding && Vector3.Distance(transform.position, GameManager.Instance.heartPosition) <= GameManager.Instance.heartRadius)
        {
            agent.isStopped = true;

            isExploding = true;

            if (squashTween != null) squashTween.Kill(true);
            squashTween = modelPivot.DOShakeScale(explodeDuration, explodeShakeStrength, explodeVibrato,90,false).OnComplete(delegate
            {
                AudioSource.PlayClipAtPoint(exploSource.clip, transform.position,4f);
                GameManager.Instance.DamageHeart(GameManager.Instance.flyDamage);
                Instantiate(explosionPrefab, transform.position + Vector3.up *0.5f, explosionPrefab.transform.rotation);
                Destroy(gameObject);
            });
        }
	}
    
    public void Damage(float _damage)
    {
        if (state == FlyState.ash) return;
        if (isExploding) return;

        health -= _damage;

        if (health <= 0)
        {
            if (state == FlyState.alive)
            {
                wingSource.Stop();
                burnSource.Play();
                combustion.Play();

                agent.enabled = false;
                foreach (MeshRenderer meshRenderer in rendererList)
                {
                    meshRenderer.material.DOColor(burnColor,fallSpeed);
                }
                modelPivot.DOLocalMoveY(0, fallSpeed);
                modelPivot.DOPunchScale(new Vector3(0.3f, -0.5f, 0.3f)*2,fallSpeed).SetDelay(fallSpeed);
                //Die
                health = GameManager.Instance.flyBurnedLife;
                state = FlyState.dead;
                GetComponent<BoxCollider>().bounds.Expand(-0.15f);
                GetComponentInChildren<Animator>().enabled = false;
            }
            else if (state == FlyState.dead)
            {
                burnSource.Play();
                combustion.Play();
                gameObject.layer = 13;
                modelPivot.gameObject.SetActive(false);
                ashPrefab.SetActive(true);
                ashPrefab.transform.eulerAngles = new Vector3(-90, Random.Range(-90, 360), 0);
                state = FlyState.ash;
                GetComponent<BoxCollider>().enabled = false;

                ashPrefab.transform.DOLocalMoveY(-1, 3).SetDelay(5).OnComplete(delegate
                 {
                     Destroy(gameObject);
                 });
            }
        }
        else
        {
            if (state == FlyState.alive)
            {
                StartCoroutine(BlinkRed());
                if (squashTween != null) squashTween.Kill(true);
                squashTween = modelPivot.DOShakeScale(hitDuration, hitShakeStrength, hitVibrato, 90, false);
            }
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
