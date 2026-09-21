using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ParticleLauncher : MonoBehaviour
{
    private bool isHolding = false;
    private bool hasLaunched = false;
    private Rigidbody2D particleRB;
    private Vector3 delta;

    [SerializeField] private float maxRadius;
    [SerializeField] private float radiusToForce;

    [SerializeField] private LineRenderer aimLine;
    [SerializeField] private LineRenderer trajectoryLine;

    [SerializeField] private GameObject launchedParticlePrefab;

    public bool HasLaunched => hasLaunched;
    public Rigidbody2D ParticleRB => particleRB;

    private void OnEnable()
    {
        trajectoryLine.SetPosition(0, transform.position);
        trajectoryLine.SetPosition(1, transform.position);

        aimLine.SetPosition(0, transform.position);
        aimLine.SetPosition(1, transform.position);
        
        //Instantiate particle
        particleRB = Instantiate(launchedParticlePrefab, transform.position, Quaternion.identity)
            .GetComponent<Rigidbody2D>();
        particleRB.transform.localScale = Vector3.one * Upgrades.ActiveUpgrades.ParticleSize.EvaluatedValue;
        particleRB.bodyType = RigidbodyType2D.Kinematic;
        particleRB.mass = Upgrades.ActiveUpgrades.ParticleMass.EvaluatedValue;
    }

    private void Update()
    {
        if (HasLaunched)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            StartHolding();
        else if (Mouse.current.leftButton.wasReleasedThisFrame && isHolding)
            ReleaseParticle();

        if (!isHolding)
            return;

        delta = GetClampedMouseToWorldDelta();

        trajectoryLine.SetPosition(0,transform.position - delta.normalized * 0.1f);
        trajectoryLine.SetPosition(1, transform.position - delta);
        aimLine.SetPosition(1, transform.position + delta);
    }
    private Vector3 GetClampedMouseToWorldDelta()
    {
        var closestWorldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        closestWorldPoint.z = 0;
        return Vector3.ClampMagnitude(closestWorldPoint - transform.position, maxRadius);
    }

    private void StartHolding()
    {
        isHolding = true;
    }
    private void ReleaseParticle()
    {
        isHolding = false;
        particleRB.bodyType = RigidbodyType2D.Dynamic;
        particleRB.linearVelocity = -delta * Upgrades.ActiveUpgrades.LauncherSpeed.EvaluatedValue;

        hasLaunched = true;
        gameObject.SetActive(false);
    }
}
