using UnityEngine;
using Utils;

public class LaunchedParticle : MonoBehaviour
{
    private float collisionBoostLeft;
    private Rigidbody2D rb2D;
    private Vector3 lookaheadPosition;

    [SerializeField] private LayerMask collisionMask;

    [SerializeField] private float lookaheadTime;
    [SerializeField] private float lookaheadSmoothing;

    public Transform LookaheadTransform { get; private set; }

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        LookaheadTransform = transform.Find("Lookahead");
    }

    private void OnEnable()
    {
        collisionBoostLeft = Upgrades.ActiveUpgrades.CollisionBoost.EvaluatedValue;
        lookaheadPosition = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (LookaheadTransform != null)
        {
            lookaheadPosition = LerpUtility.ExpDecayLerp(lookaheadPosition,
                (Vector3)rb2D.linearVelocity * lookaheadTime,
                lookaheadSmoothing, Time.fixedDeltaTime);
            LookaheadTransform.localPosition = lookaheadPosition;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (collisionBoostLeft > 0f && !collisionMask.HasLayer(other.gameObject.layer))
            return;

        rb2D.linearVelocity += rb2D.linearVelocity.normalized * collisionBoostLeft;
        collisionBoostLeft *= 0.5f;
    }
}
