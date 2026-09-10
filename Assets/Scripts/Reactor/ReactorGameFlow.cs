using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReactorGameFlow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ReactorParticleSpawner spawner;
    [SerializeField] private ParticleLauncher launcher;
    [SerializeField] private ReactorEndDetector endDetector;
    

    [SerializeField] private CinemachineVirtualCameraBase reactorCam;
    [SerializeField] private CinemachineVirtualCameraBase launcherCam;
    [SerializeField] private CinemachineVirtualCameraBase particleCam;

    private void Start()
    {
        GameFlowCoroutine();
    }
    private async Awaitable GameFlowCoroutine()
    {
        reactorCam.enabled = true;

        await spawner.SpawnParticles();
        spawner.CreateWalls();

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

        particleCam.enabled = true;
        particleCam.Follow = particleCam.LookAt = launcher.ParticleRB.transform;
        
        endDetector.ParticleRB = launcher.ParticleRB;

        while (!endDetector.IsInactive)
            await Awaitable.NextFrameAsync();
    }
}