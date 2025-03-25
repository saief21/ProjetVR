using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private float range = 100f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private float reloadTime = 1.5f;

    private Camera mainCamera;
    private float nextFireTime = 0f;
    private int currentAmmo;
    private bool isReloading = false;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;

    private void Start()
    {
        mainCamera = Camera.main;
        currentAmmo = maxAmmo;
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
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 1f); // Ligne de debug pour voir le rayon

        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log($"Hit something: {hit.transform.name} at distance {hit.distance}");
            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                Debug.Log($"Hit enemy! Current health before damage: {enemy.GetHealthPercentage() * 100}%");
                enemy.TakeDamage(damage);
            }
            else
            {
                Debug.Log("Hit object has no EnemyHealth component");
            }
        }
        else
        {
            Debug.Log("Did not hit anything");
        }
    }

    private System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Reload complete!");
    }
}
