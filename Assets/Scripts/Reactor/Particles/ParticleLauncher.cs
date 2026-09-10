using UnityEngine;
using UnityEngine.InputSystem;

public class ParticleLauncher : MonoBehaviour
{
    private bool isHolding = false;
    private bool hasLaunched = false;
    private Rigidbody2D particleRB;

    [SerializeField] private float maxRadius;
    [SerializeField] private float radiusToForce;

    [SerializeField] private GameObject launchedParticlePrefab;
    
    public bool HasLaunched => hasLaunched;
    public Rigidbody2D ParticleRB => particleRB;

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

        var closestWorldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        closestWorldPoint.z = 0;
        var delta = Vector3.ClampMagnitude(closestWorldPoint - transform.position, maxRadius);
        particleRB.position = transform.position + delta;
    }
    private void StartHolding()
    {
        particleRB = Instantiate(launchedParticlePrefab, transform.position, Quaternion.identity)
            .GetComponent<Rigidbody2D>();
        particleRB.bodyType = RigidbodyType2D.Kinematic;
        isHolding = true;
    }
    private void ReleaseParticle()
    {
        isHolding = false;
        var delta = transform.position - (Vector3)particleRB.position;
        particleRB.bodyType = RigidbodyType2D.Dynamic;
        particleRB.linearVelocity = delta * radiusToForce;
        
        hasLaunched = true;
    }
}
