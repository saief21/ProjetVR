using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float headshotMultiplier = 2f;
    [SerializeField] private int maxAmmo = 30;
    private int currentAmmo;
    private float nextFireTime = 0f;
    private bool isReloading = false;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject bulletHolePrefab;
    [SerializeField] private GameObject bulletTrailPrefab;
    [SerializeField] private Transform bulletSpawnPoint;

    private Camera mainCamera;
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;

    private void Start()
    {
        mainCamera = Camera.main;
        currentAmmo = maxAmmo;

        // Créer le point de spawn des balles s'il n'existe pas
        if (bulletSpawnPoint == null)
        {
            bulletSpawnPoint = transform.Find("BulletSpawnPoint");
            if (bulletSpawnPoint == null)
            {
                GameObject spawnPoint = new GameObject("BulletSpawnPoint");
                bulletSpawnPoint = spawnPoint.transform;
                bulletSpawnPoint.SetParent(transform);
                bulletSpawnPoint.localPosition = new Vector3(0, 0, 0.5f); // Ajustez selon votre arme
            }
        }
    }

    private void Update()
    {
        if (isReloading) return;

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
            currentAmmo--;
        }

        if ((Input.GetKeyDown(KeyCode.R) || currentAmmo == 0) && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }

    private void Shoot()
    {
        // Effet de flash
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Créer l'effet de traînée de balle
        if (bulletTrailPrefab != null)
        {
            GameObject bulletTrail = Instantiate(bulletTrailPrefab, bulletSpawnPoint.position, Quaternion.identity);
            LineRenderer line = bulletTrail.GetComponent<LineRenderer>();
            if (line != null)
            {
                line.SetPosition(0, bulletSpawnPoint.position);
                line.SetPosition(1, ray.origin + ray.direction * range);
                Destroy(bulletTrail, 0.1f);
            }
        }

        if (Physics.Raycast(ray, out hit, range))
        {
            // Créer l'impact de balle
            if (bulletHolePrefab != null)
            {
                GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bulletHole, 5f);
            }

            // Chercher le composant EnemyHealth
            Transform targetTransform = hit.transform;
            EnemyHealth enemy = null;

            while (targetTransform != null && enemy == null)
            {
                enemy = targetTransform.GetComponent<EnemyHealth>();
                if (enemy == null)
                {
                    targetTransform = targetTransform.parent;
                }
            }

            if (enemy != null)
            {
                // Vérifier si c'est un headshot
                float finalDamage = damage;
                if (IsHeadshot(hit, enemy.transform))
                {
                    finalDamage *= headshotMultiplier;
                    Debug.Log("HEADSHOT!");
                }

                enemy.TakeDamage(finalDamage);
                Debug.Log($"Hit enemy! Damage dealt: {finalDamage}");
            }
        }
    }

    private bool IsHeadshot(RaycastHit hit, Transform enemyTransform)
    {
        // La tête est généralement dans la partie supérieure du modèle
        float headThreshold = 1.5f; // Ajustez selon la taille de vos zombies
        float hitHeight = hit.point.y - enemyTransform.position.y;
        return hitHeight > headThreshold;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(2f);
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Reload complete!");
    }
}
