using System;
using Bodardr.Databinding.Runtime;
using Save;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[Serializable]
public class Currencies
{
    public static Currencies Active => SaveFile.Current.Currencies;
    
    [FormerlySerializedAsBinding("Money")]
    public int Credits;
    public int Research;
}

public class ReactorGameFlow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ReactorParticleSpawner spawner;
    [SerializeField] private ParticleLauncher launcher;
    [SerializeField] private ReactorEndDetector endDetector;

    [SerializeField] private CinemachineVirtualCameraBase reactorCam;
    [SerializeField] private CinemachineVirtualCameraBase launcherCam;
    [SerializeField] private CinemachineVirtualCameraBase particleCam;

    [SerializeField] private Transform lookaheadTransform;
    [SerializeField] private CinemachineTargetGroup targetGroup;

    private void Start()
    {
        GameFlowCoroutine();
    }
    private async Awaitable GameFlowCoroutine()
    {
        reactorCam.enabled = true;

        spawner.CreateWalls();
        await spawner.SpawnParticles();

        //Wait for click
        while (!Mouse.current.leftButton.wasReleasedThisFrame)
            await Awaitable.NextFrameAsync();

        var pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        pos.z = 0;
        launcher.transform.position = pos;

        await Awaitable.NextFrameAsync();
        launcher.gameObject.SetActive(true);

        launcherCam.enabled = true;

        while (!launcher.HasLaunched)
            await Awaitable.NextFrameAsync();

        targetGroup.Targets[0].Object = launcher.ParticleRB.transform;
        var launchedParticle = launcher.ParticleRB.GetComponent<LaunchedParticle>();
        targetGroup.Targets[1].Object = launchedParticle.LookaheadTransform;

        particleCam.enabled = true;
        endDetector.ParticleRB = launcher.ParticleRB;

        while (!endDetector.IsRunEnded)
            await Awaitable.NextFrameAsync();

        OnRunFinished();
    }
    private static void OnRunFinished()
    {
        Currencies.Active.Credits += ReactorParticle.collisionScore;
        Currencies.Active.Research += ReactorParticle.collisionScore;
        ReactorParticle.collisionScore = 0;

        SceneManager.LoadScene("Upgrades Scene");
    }
}
