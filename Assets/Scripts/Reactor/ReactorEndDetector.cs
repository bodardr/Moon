using System;
using UnityEngine;
using UnityEngine.UI;

public class ReactorEndDetector : MonoBehaviour
{
    [SerializeField] private Slider inactiveImageFill;
    [SerializeField] private float inactiveVelocityThreshold;
    [SerializeField] private float fillTime;

    public Rigidbody2D ParticleRB { get; set; }

    public bool IsInactive => inactiveImageFill.value >= 1f;

    private void Update()
    {
        if (ParticleRB == null)
            return;
        
        var inactive = ParticleRB.linearVelocity.magnitude <= inactiveVelocityThreshold;
        
        inactiveImageFill.value += 1f / fillTime * Time.deltaTime * (inactive ? 1f : -1f);
        inactiveImageFill.gameObject.SetActive(inactiveImageFill.value > 0f);
    }
}
