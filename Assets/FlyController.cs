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

public enum FlyType
{
    attack,
    explode
}

public class FlyController : MonoBehaviour {

    public FlyType type = FlyType.attack;
    public FlyState state = FlyState.alive;

    public ParticleSystem combustion;
    public GameObject ashPrefab;
    public GameObject explosionPrefab;

    public AudioSource exploSource;
    public AudioSource burnSource;
    public AudioSource wingSource;

    public float attackCooldown;
    public Vector3 attackSquash;
    public float attackDuration;

    public NavMeshAgent agent;
    public Transform modelPivot;
    public Transform bodyPivot;
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

    private bool reachedHeart;
    private Tween squashTween;

    private List<SkinnedMeshRenderer> skinnedRendererList = new List<SkinnedMeshRenderer>();
    private List<Color> skinnedStartColorList = new List<Color>();

    private float lastAttackTimestamp;

    private Collider myCollider;

    [SerializeField] private List<MeshRenderer> rendererList = new List<MeshRenderer>();
    [SerializeField] private List<Color> startColorList = new List<Color>();

    private void Start()
    {
        myCollider = GetComponent<Collider>();

        foreach(MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.material.EnableKeyword("_EMISSION");
            rendererList.Add(meshRenderer);
            startColorList.Add(meshRenderer.material.GetColor("_EmissionColor"));
        }
        foreach (SkinnedMeshRenderer meshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            meshRenderer.material.EnableKeyword("_EMISSION");
            skinnedRendererList.Add(meshRenderer);
            skinnedStartColorList.Add(meshRenderer.material.GetColor("_EmissionColor"));
        }


        health = GameManager.Instance.flyLife;
        agent.SetDestination(GameManager.Instance.heartPosition);
    }

    // Update is called once per frame
    void Update ()
    {
        if (state != FlyState.alive) return;


        RaycastHit hit;
        Physics.Raycast(new Vector3(transform.position.x,1, transform.position.z),Vector3.down,out hit,10f);
        Debug.DrawLine(new Vector3(transform.position.x, 1, transform.position.z), hit.point);
        if (hit.collider != null && hit.collider != myCollider)
        {
            Debug.Log(hit.collider.name);
            bodyPivot.position = new Vector3(bodyPivot.position.x, hit.point.y, bodyPivot.position.z);
        }

        if (Vector3.Distance(transform.position, GameManager.Instance.heartPosition) > GameManager.Instance.heartRadius && reachedHeart)
        {
            reachedHeart = false;
            agent.isStopped = false;
        }

        switch (type)
        {
            case FlyType.explode:
                if (!reachedHeart && Vector3.Distance(transform.position, GameManager.Instance.heartPosition) <= GameManager.Instance.heartRadius)
                {
                    agent.isStopped = true;

                    reachedHeart = true;

                    if (squashTween != null) squashTween.Kill(true);
                    squashTween = modelPivot.DOShakeScale(explodeDuration, explodeShakeStrength, explodeVibrato, 90, false).OnComplete(delegate
                    {
                        AudioSource.PlayClipAtPoint(exploSource.clip, transform.position, 4f);
                        GameManager.Instance.DamageHeart(GameManager.Instance.flyDamage);
                        Instantiate(explosionPrefab, transform.position + Vector3.up * 0.5f, explosionPrefab.transform.rotation);
                        Destroy(gameObject);
                    });
                }
                break;
            case FlyType.attack:

                if (!reachedHeart && Vector3.Distance(transform.position, GameManager.Instance.heartPosition) <= GameManager.Instance.heartRadius)
                {
                    agent.isStopped = true;
                    reachedHeart = true;
                }

                if (Vector3.Distance(transform.position, GameManager.Instance.heartPosition) <= GameManager.Instance.heartRadius)
                {
                    if (Time.time > lastAttackTimestamp + attackCooldown)
                    {
                        if (squashTween != null) squashTween.Kill(true);
                        squashTween = modelPivot.DOPunchScale(attackSquash, attackDuration);
                        GameManager.Instance.DamageHeart(GameManager.Instance.flyDamage);
                        lastAttackTimestamp = Time.time;
                    }
                }

                break;
        }
       
	}
    
    public void Damage(float _damage)
    {
        if (state == FlyState.ash) return;

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
                    meshRenderer.material.DOVector(burnColor, "_EmissionColor", fallSpeed);
                }
                foreach (SkinnedMeshRenderer meshRenderer in skinnedRendererList)
                {
                    meshRenderer.material.DOColor(burnColor, fallSpeed);
                    meshRenderer.material.DOVector(burnColor, "_EmissionColor", fallSpeed);
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

    public void Evaporate()
    {

        if (state == FlyState.ash) return;
        
        wingSource.Stop();
        burnSource.Play();
        combustion.Play();

        agent.enabled = false;
        
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


    IEnumerator BlinkRed()
    {
        foreach (MeshRenderer meshRenderer in rendererList)
        {
            meshRenderer.material.SetColor("_EmissionColor", blinkColor);
        }
        foreach (SkinnedMeshRenderer meshRenderer in skinnedRendererList)
        {
            meshRenderer.material.SetColor("_EmissionColor", blinkColor);
        }

        yield return new WaitForSeconds(blinkTime);

        int i = 0;
        foreach (MeshRenderer meshRenderer in rendererList)
        {
            meshRenderer.material.SetColor("_EmissionColor", startColorList[i]);
            i++;
        }
        i = 0;
        foreach (SkinnedMeshRenderer meshRenderer in skinnedRendererList)
        {
            meshRenderer.material.SetColor("_EmissionColor", startColorList[i]);
            i++;
        }
    }
}
