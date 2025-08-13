using System;
using System.Collections;
using DG.Tweening;
using Save;
using UnityEngine;

public class Moon : MonoBehaviour
{
    private static readonly int flashPropertyID = Shader.PropertyToID("_Flash");

    private Material material;

    [SerializeField] private uint[] moonTiers;

    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;

    [SerializeField] private Vector2 scaleRange;

    [Range(0, 1)]
    [SerializeField] private float normalizedPos;

    [SerializeField] private float crashTimeInSeconds = 25 * 60;
    [SerializeField] private float updateInterval;
    [SerializeField] private Vector3 rotationPerSecond;
    
    private uint CurrentTierDamage => moonTiers[SaveFile.Current.moonDamageTier];
    
    public float NormalizedPos
    {
        get => normalizedPos;
        set
        {
            normalizedPos = value;
            SaveFile.Current.normalizedMoonTime = value;
        }
    }

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    private void OnEnable()
    {
        normalizedPos = SaveFile.Current.normalizedMoonTime;
        StartCoroutine(UpdateTimeCoroutine());
    }

    private void OnValidate()
    {
        UpdatePosition();
    }

    public void DealDamage(uint amount)
    {
        SaveFile.Current.moonDamage += amount;
        SaveFile.Current.lunarite += amount;

        if (SaveFile.Current.moonDamage >= CurrentTierDamage && moonTiers.Length > SaveFile.Current.moonDamageTier + 1)
            IncreaseMoonTier();

        this.DOKill();
        material.DOFloat(0, flashPropertyID, .2f).From(1).SetEase(Ease.InOutFlash).SetTarget(this);
    }

    private static void IncreaseMoonTier()
    {
        SaveFile.Current.moonDamageTier++;
        SaveFile.Current.moonDamage = 0;

        // Unlock stuff depending on the tier.
    }

    private IEnumerator UpdateTimeCoroutine()
    {
        var wait = new WaitForSeconds(updateInterval);

        while (isActiveAndEnabled)
        {
            yield return wait;
            NormalizedPos += updateInterval / crashTimeInSeconds;
            transform.localEulerAngles += rotationPerSecond * updateInterval;
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        transform.localScale = Vector3.one * Mathf.Lerp(scaleRange.x, scaleRange.y, NormalizedPos);
        transform.localPosition = Vector3.Lerp(startPos, endPos, NormalizedPos);
    }
}
