using UnityEngine;
using System.Collections;
using Pathfinding;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using System.Linq;

public class EnemyScript : MonoBehaviour
{
    public int health = 100;
    public float detectionRadius = 10f;
    public Transform player;
    public Transform firePoint;
    public GameObject bulletPrefab; // Assign the bullet prefab in the Inspector
    public GameObject blastPrefab;
    public GameObject bulletView;
    public float bulletSpeed = 60f;
    public float shootDelay = 1f; // Delay between shots
    public float fieldOfViewAngle = 90f; // Field of view in degrees

    private SpriteRenderer spriteRenderer;
    private Collider2D enemyCollider;
    public Collider2D enemyViewCollider;
    private bool isShooting = false;
    private bool playerInView = false;
    private Coroutine shootingCoroutine;
    private Coroutine waitCoroutine;

    public float scatterAngle = 10f;

    public List<Transform> waypoints; // List of waypoints
    public float speed = 5f; // Movement speed
    public float nextWaypointDistance = 3f; // Distance to the next waypoint to be considered as reached

    private Seeker seeker;
    private Rigidbody2D rb;
    private Path path;
    private int currentWaypoint = 0;
    private int currentTargetIndex = 0;
    private bool reachedEndOfPath = false;
    private bool reachedEndOfPathCheck = false;
    private Vector3 lastSeenPosition;
    private bool lastSeenPositionChecked = true;
    private bool playerOutOfViewSinceShooting = false;
    private bool pathStartedFollow = false;
    private bool playerNoticed = false;
    private bool goingBack = false;
    private bool lastPath;

    public LayerMask layerMaskBullet;


    public string type;

    public float rotationSpeed = 5f;

    public string states;

    public Vector2 spawnPosition;
    public float spawnAngle;

    public List<Transform> targets = new List<Transform>();
    public List<GameObject> bullets = new List<GameObject>();

    void Start()
    {
        layerMaskBullet = ~layerMaskBullet;
        //Physics2D.IgnoreCollision(enemyCollider.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>(); // Get the enemy's collider
        enemyViewCollider = GetComponentInChildren<PolygonCollider2D>(); // Get the enemy's collider

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        spawnPosition = transform.position;
        spawnAngle = transform.eulerAngles.z;

        //rb.gravityScale = 0; // Ensure no gravity
        //rb.drag = 0; // Reduce friction

        states = "idle";
        // Start pathfinding to the first target in the list
        if (type == "moving")
        {
            if (targets.Count > 0)
            {
                SetTarget(targets[currentTargetIndex]);
            }
        }

        

    }

    void FixedUpdate()
    {

        switch (states)
        {
            case "idle":


                FollowPath();



                break;

            case "shoot":

                CheckLineOfSight();

                break;

            case "follow":

                FollowLastSeenPosition();

                break;

        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        float angleToPlayer = Vector3.Angle(transform.right, directionToPlayer);

        enemyCollider.enabled = false;
        enemyViewCollider.enabled = false;

        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, layerMaskBullet);


        enemyCollider.enabled = true;
        enemyViewCollider.enabled = true;
        Debug.DrawRay(transform.position, directionToPlayer * distanceToPlayer, Color.red); // Debug ray

        //print(hit.collider);

        if (distanceToPlayer <= detectionRadius && angleToPlayer <= fieldOfViewAngle / 2 && (hit.collider != null && hit.collider.CompareTag("Player")))
        {
            // Temporarily disable the enemy's collider


            // Perform the raycast



            // Re-enable the enemy's collider



            states = "shoot";
            playerInView = true;
            playerOutOfViewSinceShooting = true;
            lastSeenPosition = player.position;
            pathStartedFollow = true;




        }
        else playerInView = false;

        //print(playerInView);
        if (playerInView) states = "shoot";
        if (!playerInView && !playerOutOfViewSinceShooting) states = "idle";
        if (!playerInView && playerOutOfViewSinceShooting) states = "follow";

        //print("current waypoint: " + currentWaypoint);
        //print("path vectorpath count: " + path.vectorPath.Count);
    }
    void FollowPath()
    {

        //print("Targets Count: " + targets.Count);
        //print("Current Target Index: " + currentTargetIndex);
        //print("reached end of path: " + reachedEndOfPath);
        if (path == null)
        {
            return;
        }
        if (type == "moving")
        {
            if (currentWaypoint >= path.vectorPath.Count)
            {

                currentTargetIndex = (currentTargetIndex + 1) % targets.Count;
                SetTarget(targets[currentTargetIndex]);

                return;
            }
        }

        else if (type == "idle")
        {


            if (currentWaypoint >= path.vectorPath.Count)
            {
                lastPath = true;
                currentTargetIndex = (currentTargetIndex + 1) % targets.Count;
                SetTarget(targets[currentTargetIndex]);

                return;
            }





            //rb.position = spawnPosition;
            //float angle = Mathf.Atan2(spawnAngle.y, spawnAngle.x) * Mathf.Rad2Deg;
            //float smoothAngle = Mathf.LerpAngle(rb.rotation, angle, rotationSpeed * Time.deltaTime);
            //rb.rotation = smoothAngle;



        }

        if (type == "idle")
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 targetPosition = (Vector2)rb.position + direction * speed * Time.deltaTime;


            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.LerpAngle(rb.rotation, angle, rotationSpeed * Time.deltaTime);
            rb.rotation = smoothAngle;


            // Move the character instantly
            rb.MovePosition(targetPosition);

            // Distance to the next waypoint
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            // Check if we reached the waypoint
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
                //print(nextWaypointDistance);
                //print (distance);
            }
            if (currentWaypoint == path.vectorPath.Count)
            {
                print("tesst");
            }
        }

        if (type == "moving")
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 targetPosition = (Vector2)rb.position + direction * speed * Time.deltaTime;


            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.LerpAngle(rb.rotation, angle, rotationSpeed * Time.deltaTime);
            rb.rotation = smoothAngle;


            // Move the character instantly
            rb.MovePosition(targetPosition);

            // Distance to the next waypoint
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            // Check if we reached the waypoint
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
                //print(nextWaypointDistance);
                //print (distance);
            }
            if (currentWaypoint == path.vectorPath.Count)
            {
                //print("end reached");
            }

        }
        //print("currentWaypoint: " + currentWaypoint);
        //print("path vector count: " + path.vectorPath.Count);
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            if (type == "moving" || !lastPath) currentWaypoint = 0;
            if (type == "idle" || lastPath)
            {

                float smoothAngle = Mathf.LerpAngle(rb.rotation, spawnAngle, 20f * Time.deltaTime);
                rb.rotation = smoothAngle;

            }

        }

    }

    void FollowLastSeenPosition()
    {
        if (path != null)
        {
            // Release the current path
            //path = null; // Clear the path
        }

        if (pathStartedFollow)
        {
            seeker.StartPath(rb.position, lastSeenPosition, OnPathComplete);
            pathStartedFollow = false;
            currentWaypoint = 0;
        }



        if (currentWaypoint >= path.vectorPath.Count)
        {
            lastPath = false;
            reachedEndOfPathCheck = true;
            // Check if the player is still there
            float distanceToLastSeen = Vector3.Distance(transform.position, lastSeenPosition);
            if (reachedEndOfPathCheck)
            {
                if (playerInView)
                {

                    states = "shoot";
                }
                else
                {
                    states = "idle";

                    playerOutOfViewSinceShooting = false;
                    pathStartedFollow = false; // Reset for next follow
                    reachedEndOfPath = true;
                    if (type == "idle")
                    {
                        SetTarget(targets[0]);
                        lastSeenPositionChecked = true;

                    }
                    if (type == "moving")
                    {
                        SetTarget(targets[currentTargetIndex + 1]);
                        lastSeenPositionChecked = true;
                    }

                }
            }
            return;
        }


        // Direction to the next waypoint
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 targetPosition = (Vector2)rb.position + direction * speed * Time.deltaTime;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.LerpAngle(rb.rotation, angle, rotationSpeed * Time.deltaTime);
        rb.rotation = smoothAngle;

        // Move the character instantly
        rb.MovePosition(targetPosition);

        // Distance to the next waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        // Check if we reached the waypoint
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }


    }

    public void SetTarget(Transform newTarget)
    {
        seeker.StartPath(rb.position, newTarget.position, OnPathComplete);
    }

    public void TakeDamage(int damage)
    {

        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);

        }
        FollowPath();
    }

    void CheckLineOfSight()
    {

        // Clear line of sight to the player
        Vector3 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.LerpAngle(rb.rotation, angle, rotationSpeed * 5 * Time.deltaTime);
        rb.rotation = smoothAngle;
        print("Angle: " + angle);
        print("Smooth Angle: " + smoothAngle);
        rb.MovePosition(transform.position);
        // Start shooting if not already shooting
        if (!isShooting)
        {
            isShooting = true;
            shootingCoroutine = StartCoroutine(ShootAtPlayer());
        }
        return; // Early return if the player is in view




        // If the player is not in view, stop shooting

    }
    public void GetBullet(GameObject bullet)
    {

    }
    public void HitByBullet(GameObject bullet)
    {
        Vector3 direction = player.position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.LerpAngle(rb.rotation, angle, rotationSpeed * Time.deltaTime);
        rb.rotation = smoothAngle;

        rb.MovePosition(transform.position);
    }
    public void BulletFlyBy(GameObject bullet)
    {
        if (lastSeenPositionChecked)
        {
            playerInView = false;
            playerOutOfViewSinceShooting = true;
            lastSeenPosition = player.position;
            pathStartedFollow = true;
            lastSeenPositionChecked = false;
        }

    }

    IEnumerator ShootAtPlayer()
    {

        if (playerInView)
        {
            // Create the bullet
            GameObject blast = Instantiate(blastPrefab, firePoint.position, firePoint.rotation);
            Destroy(blast, 0.05f); // Destroy the blast effect after 0.05 seconds

            float scatter = Random.Range(-scatterAngle, scatterAngle);

            // Calculate the rotation with scatter
            Quaternion scatterRotation = firePoint.rotation * Quaternion.Euler(0, 0, scatter);

            // Instantiate the bullet
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, scatterRotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetParent("Enemy");

            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = scatterRotation * Vector2.up * bulletSpeed; // Set the bullet speed, adjust as necessary

            yield return new WaitForSeconds(shootDelay);
            isShooting = false;
        }
    }

    void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfViewAngle / 2, Vector3.forward) * transform.right * detectionRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfViewAngle / 2, Vector3.forward) * transform.right * detectionRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        if (Vector3.Distance(transform.position, player.position) < detectionRadius)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.right, directionToPlayer);

            if (angleToPlayer < fieldOfViewAngle / 2)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, directionToPlayer * detectionRadius);
            }
        }

        if (!bullets.Any()) 
        {

        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Enemy")
        {
            Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }

    }

}