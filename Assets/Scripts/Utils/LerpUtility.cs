using UnityEngine;

public static class LerpUtility
{
    /// <summary>
    ///     Formula from Freya Holmer's "Lerp smoothing is broken" talk.
    ///     A range of 0-25 is recommended for the decay
    /// </summary>
    /// <param name="a">Interpolation start point.</param>
    /// <param name="b">Interpolation end point.</param>
    /// <param name="decay">The half-life decay rate. How fast it reaches the 'b' point. Recommended between 0 and 25</param>
    /// <param name="deltaTime">The time elapsed between the last call. Generally Time.deltaTime or Time.fixedDeltaTime</param>
    /// <returns>A smooth linear interpolation that is framerate independent.</returns>
    public static float ExpDecayLerp(float a, float b, float decay, float deltaTime)
    {
        return b + (a - b) * Mathf.Exp(-decay * deltaTime);
    }

    public static Vector3 ExpDecayLerp(Vector3 a, Vector3 b, float decay, float deltaTime)
    {
        return Vector3.Lerp(a, b, ExpDecayLerp(0, 1, decay, deltaTime));
    }
}
