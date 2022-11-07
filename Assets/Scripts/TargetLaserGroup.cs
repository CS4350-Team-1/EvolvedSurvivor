using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLaserGroup : MonoBehaviour
{
    [SerializeField] private List<TargetingLaser> lasers;
    [SerializeField] private float fireAfterSeconds;
    [SerializeField] private float fireDuration = 4f;
    [SerializeField] private float missileSpawnHeight = 10f;

    [Header ("Particles")]
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private ParticleSystem missileParticles;

    [SerializeField] private SpawnManagerScriptableObject spawnManager;
    [SerializeField] private float damageValue = 1000f;

    [Header("Sound")]
    [SerializeField] private LoopingSfxHandler warmupSfxHandler;
    [SerializeField] private SfxHandler fireSfxHandler;
    [SerializeField] private SfxHandler explosionSfxHandler;

    private bool isMissileFired = false;

    private Health playerHealth;

    private void Start()
    {
        InvokeRepeating("WarmUpLasers", 0f, 8f);
    }

    private void Update()
    {
        if (isMissileFired)
        {
            missileParticles.gameObject.transform.position -= new Vector3(0f, 1.9f * missileSpawnHeight, 0f) * Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        spawnManager.PlayerSpawnEvent.AddListener(AttachToPlayer);
    }

    private void OnDisable()
    {
        spawnManager.PlayerSpawnEvent.RemoveListener(AttachToPlayer);
        StopAllCoroutines();
    }

    private void AttachToPlayer(PlayerController player)
    {
        gameObject.transform.position = player.transform.position;
        gameObject.transform.parent = player.transform;

        playerHealth = player.GetComponent<Health>();
    }

    private void WarmUpLasers()
    {
        StartCoroutine(WarmUpLaserCoroutine());
    }

    private IEnumerator WarmUpLaserCoroutine()
    {
        yield return new WaitForSeconds(fireAfterSeconds);

        warmupSfxHandler.PlaySfx();
        SetLasers(true);
        yield return new WaitForSeconds(0.1f);
        SetLasers(false);
        yield return new WaitForSeconds(0.1f);
        SetLasers(true);
        yield return new WaitForSeconds(0.2f);
        SetLasers(false);
        yield return new WaitForSeconds(0.2f);
        SetLasers(true);

        yield return new WaitForSeconds(0.3f);

        ArmLasers();

        yield return new WaitForSeconds(fireDuration);
        warmupSfxHandler.StopSfx();
        StartCoroutine(FireMissile());
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Explode());
    }

    private IEnumerator FireMissile()
    {
        isMissileFired = true;
        missileParticles.gameObject.SetActive(true);
        missileParticles.Play();
        missileParticles.transform.localPosition = new Vector3(0f, missileSpawnHeight, 0f);
        fireSfxHandler.PlaySfx();

        yield return new WaitForSeconds(1f);

        isMissileFired = false;
        missileParticles.gameObject.SetActive(false);
    }

    private void SetLasers(bool isActive)
    {
        foreach (TargetingLaser laser in lasers)
        {
            laser.gameObject.SetActive(isActive);
        }
    }

    private void ArmLasers()
    {
        foreach (TargetingLaser laser in lasers)
        {
            // Give at least 0.5f for the lasers to converge and stay on target
            laser.ArmLaser(fireDuration);
        }
    }

    private IEnumerator Explode()
    {
        MMFlashEvent.Trigger(Color.red, 0.5f, 1f, 0, 0, TimescaleModes.Unscaled);
        explosionParticles.gameObject.SetActive(true);
        explosionParticles.Play();
        explosionSfxHandler.PlaySfx();

        playerHealth.Damage(damageValue, gameObject, 0f, 0f, Vector3.zero);

        yield return new WaitForSeconds(1f);
        explosionParticles.gameObject.SetActive(false);
    }
}
