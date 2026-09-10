using Unity.Scripting.LifecycleManagement;
using UnityEngine;

public partial class Particle : MonoBehaviour
{
    [AutoStaticsCleanup] public static int collisionScore = 0;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        //Since both will receive this collision event, we discard the point counting logic
        //for the inferior entity id.
        if (other.transform.GetEntityId() > transform.GetEntityId())
            ++collisionScore;
        
        //Maybe add health decrease logic and stuff.
    }
}