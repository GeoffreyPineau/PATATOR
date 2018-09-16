using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonHeatController : MonoBehaviour {

    [ColorUsage(false,true)]public Color heatColor;
    public MeshRenderer meshRenderer;
    

    private void Start()
    {
        meshRenderer.material.EnableKeyword("_EMISSION");
    }

    private void Update()
    {
        heatColor = Color.Lerp(GameManager.Instance.sombreroastMinGlow, GameManager.Instance.sombreroastMaxGlow,
            Mathf.InverseLerp(0, GameManager.Instance.sombreroastMaxHeat, GameManager.Instance.sombreroastCurrentHeat));
        meshRenderer.material.SetColor("_EmissionColor", heatColor);
    }
}
