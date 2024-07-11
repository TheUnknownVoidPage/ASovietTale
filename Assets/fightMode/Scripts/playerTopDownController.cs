using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public bool moving = true;
    float speed = 8.0f;
    Vector3 mousePos;
    Camera cam;
    Rigidbody2D rb;

    public Transform firePoint; // Point where the bullet will be instantiated
    public Transform firePoint2; // Point where the bullet will be instantiated
    public GameObject bulletPrefab; // Prefab for the bullet
    public GameObject blastPrefab; // Prefab for the blast effect

    public float bulletSpeed = 60f;
    public float scatterAngle = 10f;

    public Transform leftArm;
    public Transform rightArm;

    private bool isRotatingArms = false;
    private bool isReturningArms = false;
    private float rotationSpeed = 180f; // Degrees per second
    private float rotationSpeedBack = 360f; // Degrees per second

    public string mode;
    public bool isAutomatic;
    public float fireRate = 0.1f; // Time between shots in seconds

    private bool isShooting = false;

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void Update()
    {
        movement();
        rotateToCamera();

        if (mode == "single")
        {
            if (Input.GetMouseButtonDown(0) || (isAutomatic && Input.GetMouseButton(0))) // Left mouse button
            {
                StartShooting();
            }
            if (Input.GetMouseButtonUp(0)) // Left mouse button release
            {
                StopShooting();
            }
        }
        else if (mode == "two")
        {
            if (Input.GetMouseButtonDown(1))
            {
                isRotatingArms = true;
                isReturningArms = false;
            }

            if (Input.GetMouseButtonUp(1))
            {
                isRotatingArms = false;
                isReturningArms = true;
            }

            if (isRotatingArms)
            {
                RotateArms(90f);
            }

            if (isReturningArms)
            {
                RotateArms(0f);
            }

            if (Input.GetMouseButtonDown(0) || (isAutomatic && Input.GetMouseButton(0))) // Left mouse button
            {
                StartShooting();
            }
            if (Input.GetMouseButtonUp(0)) // Left mouse button release
            {
                StopShooting();
            }
        }
    }

    void movement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(moveX * speed, moveY * speed);
    }

    void rotateToCamera()
    {
        mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));
        rb.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2((mousePos.y - transform.position.y), (mousePos.x - transform.position.x)) * Mathf.Rad2Deg);
    }

    void StartShooting()
    {
        if (!isShooting)
        {
            isShooting = true;
            StartCoroutine(ShootRoutine());
            if (!isAutomatic)
            {
                StopShooting();
            }
        }
    }

    void StopShooting()
    {
        isShooting = false;
        StopCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        while (isShooting)
        {
            if (mode == "single")
            {
                ShootLeft();
            }
            else if (mode == "two")
            {
                ShootRight();
            }
            yield return new WaitForSeconds(fireRate);
        }
    }

    void ShootLeft()
    {
        // Instantiate the blast effect
        GameObject blast = Instantiate(blastPrefab, firePoint.position, firePoint.rotation);
        Destroy(blast, 0.02f); // Destroy the blast effect after 0.1 seconds

        // Calculate a random scatter angle
        float scatter = Random.Range(-scatterAngle, scatterAngle);

        // Calculate the rotation with scatter
        Quaternion scatterRotation = firePoint.rotation * Quaternion.Euler(0, 0, scatter);

        // Instantiate the bullet with the scatter rotation
        GameObject bullet = Instantiate(bulletPrefab, new Vector3(firePoint.position.x, firePoint.position.y, firePoint.position.z - 2), scatterRotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetParent("Player");

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = scatterRotation * Vector2.up * bulletSpeed; // Set the bullet speed with the scatter

        // Add shooting sound effect here if you have one
        // AudioSource.PlayClipAtPoint(shootingSound, transform.position);
    }

    void ShootRight()
    {
        // Instantiate the blast effect
        GameObject blast2 = Instantiate(blastPrefab, firePoint2.position, firePoint2.rotation);
        Destroy(blast2, 0.02f); // Destroy the blast effect after 0.1 seconds

        // Calculate a random scatter angle
        float scatter2 = Random.Range(-scatterAngle, scatterAngle);

        // Calculate the rotation with scatter
        Quaternion scatterRotation2 = firePoint2.rotation * Quaternion.Euler(0, 0, scatter2);

        // Instantiate the bullet with the scatter rotation
        GameObject bullet2 = Instantiate(bulletPrefab, new Vector3(firePoint2.position.x, firePoint2.position.y, firePoint2.position.z - 2), scatterRotation2);
        Bullet bulletScript2 = bullet2.GetComponent<Bullet>();
        bulletScript2.SetParent("Player");

        Rigidbody2D bulletRb2 = bullet2.GetComponent<Rigidbody2D>(); // Set the bullet speed with the scatter
        bulletRb2.velocity = scatterRotation2 * Vector2.up * bulletSpeed; // Set the bullet speed with the scatter
        Invoke("ShootLeft", 0.1f);
        // Add shooting sound effect here if you have one
        // AudioSource.PlayClipAtPoint(shootingSound, transform.position);
    }

    void RotateArms(float targetAngle)
    {
        float step = rotationSpeed * Time.deltaTime; // Default assignment

        if (targetAngle == 0)
        {
            step = rotationSpeedBack * Time.deltaTime;
        }

        // Rotate leftArm
        leftArm.localEulerAngles = new Vector3(
            leftArm.localEulerAngles.x,
            leftArm.localEulerAngles.y,
            Mathf.MoveTowardsAngle(leftArm.localEulerAngles.z, targetAngle, step)
        );

        // Rotate rightArm
        rightArm.localEulerAngles = new Vector3(
            rightArm.localEulerAngles.x,
            rightArm.localEulerAngles.y,
            Mathf.MoveTowardsAngle(rightArm.localEulerAngles.z, -targetAngle, step)
        );

        // Check if rotation is complete
        if (Mathf.Approximately(leftArm.localEulerAngles.z, targetAngle) &&
            Mathf.Approximately(rightArm.localEulerAngles.z, -targetAngle))
        {
            isRotatingArms = false;
        }

        if (Mathf.Approximately(leftArm.localEulerAngles.z, 0f) &&
            Mathf.Approximately(rightArm.localEulerAngles.z, 0f))
        {
            isReturningArms = false;
        }
    }
}