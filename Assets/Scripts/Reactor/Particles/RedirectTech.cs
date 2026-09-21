using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RedirectTech : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private float savedMagnitude;
    private bool aimActive;

    [SerializeField] private GameObject regions;
    [SerializeField] private float regionsAimSmoothing;

    [SerializeField] private float timeScaleSlow = 0.2f;
    [SerializeField] private float timeScaleTweenDuration;

    [SerializeField] private float[] angleRegions;
    [SerializeField] private Color[] regionColors;
    [SerializeField] private float[] regionMultipliers;
    [SerializeField] private float[] regionBonusVelocities;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        InitializeRegions(regions.transform.GetChild(0), true);
        InitializeRegions(regions.transform.GetChild(1), false);
    }
    private void InitializeRegions(Transform region, bool addHalfTurn)
    {
        var images = region.GetComponentsInChildren<Image>(true);
        for (int i = 0; i < angleRegions.Length; i++)
        {
            var image = images[i];
            var angle = angleRegions[i];

            image.fillAmount = angle / 360f;
            image.transform.localRotation = Quaternion.Euler(0, 0, angle * 0.5f + (addHalfTurn ? 180 : 0));
            image.color = regionColors[i];
        }
    }

    private void Update()
    {
        //If it is sleeping.
        if (rb2D.bodyType != RigidbodyType2D.Dynamic || rb2D.IsSleeping())
            return;

        if (!aimActive)
            UpdateRegionsRotation();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (aimActive)
                ExecuteRedirect();
            else
                StartAim();
        }
    }
    private void UpdateRegionsRotation()
    {
        var linearVelocity = rb2D.linearVelocity;
        regions.transform.rotation = Quaternion.Slerp(regions.transform.rotation,
            Quaternion.LookRotation(Vector3.forward, linearVelocity.normalized),
            LerpUtility.ExpDecayLerp(0, 1, regionsAimSmoothing, Time.deltaTime));
    }

    private void StartAim()
    {
        regions.SetActive(true);
        aimActive = true;

        //Tween timescale.
        DOTween.To(() => Time.timeScale, val => Time.timeScale = val, timeScaleSlow, timeScaleTweenDuration)
            .SetUpdate(true);

        savedMagnitude = rb2D.linearVelocity.magnitude;
    }

    private void ExecuteRedirect()
    {
        aimActive = false;
        regions.SetActive(false);

        //Tween timescale.
        DOTween.To(() => Time.timeScale, val => Time.timeScale = val, 1f, timeScaleTweenDuration)
            .SetUpdate(true);

        var mouseWS = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        mouseWS.z = transform.position.z;
        var aimDirection = (transform.position - mouseWS).normalized;

        var dot = Vector2.Dot(regions.transform.right, aimDirection);
        var absDotAngle = Mathf.Acos(Mathf.Abs(dot)) * Mathf.Rad2Deg;
        var multiplier = 0f;
        var bonusVelocity = 0f;

        for (int i = angleRegions.Length - 1; i >= 0; i--)
        {
            var angle = angleRegions[i] * 0.5f;

            if (angle <= absDotAngle)
                continue;

            multiplier = regionMultipliers[i];
            bonusVelocity = regionBonusVelocities[i];
            break;
        }

        if (multiplier <= 0f)
            return;

        Debug.Log(multiplier);

        rb2D.linearVelocity = regions.transform.right.normalized *
            ((savedMagnitude * multiplier + bonusVelocity) * -Mathf.Sign(dot));
    }

    private void OnDrawGizmos()
    {
        if (!aimActive)
            return;

        var mouseWS = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        mouseWS.z = transform.position.z;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, mouseWS);
    }
}
