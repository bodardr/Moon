using Unity.Scripting.LifecycleManagement;
using UnityEngine;

public partial class ReactorParticle : MonoBehaviour
{
    [AutoStaticsCleanup] public static int collisionScore = 0;
    
    [SerializeField] private int particleHealth;
    
    private int currentHealth;

    private void OnEnable()
    {
        currentHealth = particleHealth;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //Since both will receive this collision event, we discard the point counting logic
        //for the inferior entity id.
        if (other.transform.GetEntityId() > transform.GetEntityId())
            ++collisionScore;

        if (--currentHealth > 0)
            return;
        
        Destroy(gameObject);
        collisionScore += (int)Upgrades.ActiveUpgrades.DestroyBonus.EvaluatedValue;
    }
}
