using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Spark : MonoBehaviour
{
    private float amplitude;
    private int bounceCount;
    private float speed;
    private float length;
    private ObjectPool<Spark> pool;

    private Vector3 currentStart, currentEnd;
    private Vector3 bounceTangent;
    private float normalizedBounceTime;
    private uint gearAmplitude;
    private int bounceIndex = 0;
    private List<Gear> gearColumn;
    private GearTarget target;

    [Header("Settings Per Bounce")]
    [SerializeField] private float baseLength;
    [SerializeField] private float lengthPerBounce;

    [SerializeField] private float baseSpeed;
    [SerializeField] private float speedPerBounce;

    [SerializeField] private float baseAmplitude;
    [SerializeField] private float amplitudePerBounce;


    public void Initialize(int bounceCount, Vector3 initialPosition, ObjectPool<Spark> pool, GearTarget target,
        List<Gear> gearColumn, uint amplitude)
    {
        transform.SetParent(target.transform);
        transform.localScale = Vector3.one;

        this.gearColumn = gearColumn;
        this.bounceCount = bounceCount;
        this.pool = pool;
        this.target = target;
        gearAmplitude = amplitude;

        bounceIndex = 0;
        normalizedBounceTime = 0;

        transform.localPosition = currentStart = initialPosition;

        CalculateNextBounce();
    }
    private void CalculateNextBounce()
    {
        if (bounceIndex <= 0)
        {
            amplitude = baseAmplitude + amplitudePerBounce * bounceCount;
            speed = baseSpeed + speedPerBounce * bounceCount;
            length = baseLength + lengthPerBounce * bounceCount;
        }
        else if (bounceIndex < bounceCount)
        {
            var multiplier = (bounceCount - bounceIndex) / (bounceIndex - bounceIndex + 1);
            amplitude *= multiplier;
            speed *= multiplier;
            length *= multiplier;
        }

        normalizedBounceTime = 0f;

        // Pick a random direction tangent to the current position
        var randomDir = Random.onUnitSphere;

        if (bounceIndex == 0)
            bounceTangent = Vector3.Cross(currentStart, randomDir).normalized;
        else
            bounceTangent = Quaternion.AngleAxis(Random.Range(70, 110) * (Random.value > 0.5f ? 1 : -1),
                    transform.localPosition.normalized) *
                bounceTangent;

        var angle = length / currentStart.magnitude * Mathf.Rad2Deg;

        currentEnd = Quaternion.AngleAxis(angle, bounceTangent) * currentStart;
    }

    void Update()
    {
        if (bounceIndex >= bounceCount) return;

        normalizedBounceTime += Time.deltaTime * speed;

        while (normalizedBounceTime >= 1f)
        {
            bounceIndex++;
            normalizedBounceTime -= 1;

            GearUtility.PlayGearColumn(gearColumn, gearAmplitude, null, target);
            if (bounceIndex < bounceCount)
            {
                currentStart = currentEnd;
                CalculateNextBounce();
            }
            else
            {
                pool.Release(this);
                return;
            }
        }

        // Spherical Lerp for the base path
        var surfacePos = Vector3.Slerp(currentStart, currentEnd, normalizedBounceTime);

        // Standard Parabola: 4 * h * t * (1 - t)
        var height = 4 * amplitude * normalizedBounceTime * (1f - normalizedBounceTime);

        // Apply height along the normal (outward from center)
        transform.localPosition = surfacePos + surfacePos.normalized * height;
    }
}
