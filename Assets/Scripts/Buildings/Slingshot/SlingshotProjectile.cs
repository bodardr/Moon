using System;
using UnityEngine;
public class SlingshotProjectile : MonoBehaviour
{
    private float baseScale;
    private float baseZ;

    [SerializeField] private float scaleDecayRate = 0.05f;
    
    public uint Damage { get; set; }
    public float Speed { get; set; }

    private void Awake()
    {
        baseScale = transform.localScale.x;
        baseZ = transform.position.z;
    }

    private void Update()
    {
        transform.position += transform.forward * (Speed * Time.deltaTime);
        transform.localScale = Vector3.one * (baseScale / (1 + (transform.position.z - baseZ) * scaleDecayRate));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Moon"))
            return;

        other.GetComponent<Moon>().DealDamage(Damage);
        Destroy(gameObject);
    }
}
