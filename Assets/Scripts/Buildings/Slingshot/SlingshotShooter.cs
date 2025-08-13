using System;
using System.ComponentModel;
using Save;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public enum Resource
{
    Wood,
    Stone,
    Iron,
    Lunarite,
    Gold
}

public class SlingshotShooter : MonoBehaviour, INotifyPropertyChanged
{
    private uint capacityLoaded;
    private bool isLaunching = false;
    private Vector3 targetPos;
    private Vector3 springVelocity;

    [SerializeField] private float capacityLoadRatioPerClick = 0.15f;
    [SerializeField] private uint loadCapacityRequired;
    [SerializeField] private Resource ammoType;

    [FormerlySerializedAs("slingshotProjectile")]
    [Header("Launch")]
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private Moon moon;
    [SerializeField] private GameObject aimCursor;

    [SerializeField] private Transform payload;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float launchDistance;
    [SerializeField] private float strength;
    [SerializeField] private float damping;
    [SerializeField] private float projectileSpeed = 10f;

    public float CapacityLoadRatio => capacityLoaded / (float)loadCapacityRequired;
    public bool LaunchReady => CapacityLoadRatio >= 1f;

    public event PropertyChangedEventHandler PropertyChanged;

    public void LoadMaterials()
    {
        var save = SaveFile.Current;

        uint amount = 0;
        switch (ammoType)
        {
            case Resource.Wood:
                amount = save.wood;
                break;
            case Resource.Stone:
                amount = save.stone;
                break;
            case Resource.Iron:
                amount = save.iron;
                break;
            case Resource.Lunarite:
                amount = save.lunarite;
                break;
            case Resource.Gold:
                amount = save.gold;
                break;
        }


        var amountToLoad = (uint)(loadCapacityRequired * capacityLoadRatioPerClick);
        if (amount >= amountToLoad)
        {
            capacityLoaded += amountToLoad;

            switch (ammoType)
            {
                case Resource.Wood:
                    save.wood -= amountToLoad;
                    break;
                case Resource.Stone:
                    save.stone -= amountToLoad;
                    break;
                case Resource.Iron:
                    save.iron -= amountToLoad;
                    break;
                case Resource.Lunarite:
                    save.lunarite -= amountToLoad;
                    break;
                case Resource.Gold:
                    save.gold -= amountToLoad;
                    break;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CapacityLoadRatio)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LaunchReady)));
        }
    }

    public void OnLaunchHold()
    {
        isLaunching = true;
    }

    public void OnLauncherRelease()
    {
        var launchRay = new Ray(targetPos, launchPoint.position - targetPos);

        var projectile =
            Instantiate(projectilePrefab, launchRay.origin, Quaternion.identity)
                .GetComponent<SlingshotProjectile>();
        projectile.transform.forward = launchRay.direction;
        projectile.Damage = CalculateDamage();
        projectile.Speed = projectileSpeed;


        springVelocity = Vector3.zero;
        isLaunching = false;
        capacityLoaded = 0;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CapacityLoadRatio)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LaunchReady)));
    }
    private uint CalculateDamage() => loadCapacityRequired * 1;

    private void Update()
    {
        aimCursor.gameObject.SetActive(isLaunching);
        payload.position += springVelocity * Time.fixedDeltaTime;

        if (!isLaunching)
            return;

        payload.forward = (launchPoint.position - targetPos).normalized;
        var plane = new Plane(Vector3.back, moon.transform.position);
        var launchRay = new Ray(targetPos, targetPos - launchPoint.position);
        plane.Raycast(launchRay, out var dist);

        var aimPoint = launchRay.GetPoint(dist);
        aimCursor.transform.position = aimPoint;
    }

    private void FixedUpdate()
    {
        if (isLaunching)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var ray = new Ray(launchPoint.position, mousePos - launchPoint.position);
            targetPos = ray.GetPoint(launchDistance);
        }
        else
        {

            payload.rotation = Quaternion.identity;
            targetPos = launchPoint.position - payload.TransformVector(payload.GetChild(0).localPosition);
        }

        var force = (targetPos - payload.position) * strength - springVelocity * damping;
        springVelocity += force;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(payload.position, launchPoint.position);
    }
}
