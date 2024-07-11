
using UnityEngine;

public class CaameraFollowPlayer : MonoBehaviour
{
    public Transform player;
    GameObject player1;
    public bool followPlayer = true;
    Camera cam;

    public float followSpeed = 5f; // Speed at which the camera follows the player
    public float maxLookAheadFactor = 5f; // Maximum lookahead factor

    void Start()
    {
        player1 = GameObject.FindGameObjectWithTag("Player");

        cam = Camera.main;
    }

    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
            {
            player = GameObject.FindWithTag("Player").transform;
            CamFollowPlayerWithLookAhead();
            }
        
    }

    void CamFollowPlayerWithLookAhead()
    {
        // Calculate the new position to follow the player
        Vector3 playerPos = new Vector3(player.position.x, player.position.y, -10f);

        // Calculate the cursor position in world coordinates
        Vector3 cursorScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane);
        Vector3 cursorWorldPos = cam.ScreenToWorldPoint(cursorScreenPos);
        cursorWorldPos.z = -10f;

        // Calculate the direction from the player to the cursor
        Vector3 directionToCursor = cursorWorldPos - playerPos;

        // Calculate the distance between the player and the cursor
        float distanceToCursor = directionToCursor.magnitude;

        // Calculate the lookAheadFactor based on the distance to the cursor, clamped to the maximum lookahead factor
        float lookAheadFactor = Mathf.Clamp(distanceToCursor, 0, maxLookAheadFactor);

        // Normalize the direction to avoid infinity values
        directionToCursor.Normalize();

        // Adjust the camera position based on the player's position and the cursor's distance
        Vector3 targetPos = playerPos + directionToCursor * lookAheadFactor;

        // Smoothly move the camera towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}

