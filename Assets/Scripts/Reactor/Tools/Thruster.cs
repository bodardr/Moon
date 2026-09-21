using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class Thruster : MonoBehaviour
{
    private Rigidbody2D rb2D;

    private float fuelLeft;
    private float force;
    private bool thrusterActive = false;
    private bool thrusterUsed = false;

    [SerializeField] private GameObject thrusterVisuals;
    [SerializeField] private ParticleSystem thrusterParticles;
    
    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        thrusterUsed = false;
    }

    private void OnDisable()
    {
        thrusterVisuals.gameObject.SetActive(false);
    }

    public void UseThruster()
    {
        if (!Upgrades.ActiveUpgrades.ThrusterUnlockable.Unlocked || thrusterUsed)
            return;

        fuelLeft = Upgrades.ActiveUpgrades.ThrusterFuel.EvaluatedValue;
        force = Upgrades.ActiveUpgrades.ThrusterForce.EvaluatedValue;
        thrusterActive = true;
        thrusterUsed = true;
        thrusterVisuals.gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (!thrusterActive)
            return;

        fuelLeft -= Time.fixedDeltaTime;
        rb2D.linearVelocity += rb2D.linearVelocity.normalized * (Time.fixedDeltaTime * force);

        if (fuelLeft > 0f)
            return;

        StopThruster();
    }

    private void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame && !thrusterUsed && Upgrades.ActiveUpgrades.ThrusterUnlockable.Unlocked)
            UseThruster();
        
        var vel = rb2D.linearVelocity;
        if (vel.sqrMagnitude < 0.1f)
            return;
        
        thrusterVisuals.transform.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg);
    }

    private void StopThruster()
    {
        fuelLeft = 0f;
        thrusterActive = false;
        thrusterParticles.Stop(false,  ParticleSystemStopBehavior.StopEmitting);
    }
}
