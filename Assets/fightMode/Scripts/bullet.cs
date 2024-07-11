using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    public string parent;
    public GameObject parentObject;
    public GameObject player;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (parent == "Player")
        {

        }

     }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //print(collision.GetComponent<Collider2D>());
        // Check if the bullet collided with a TilemapCollider2D
        if (collision.GetComponent<Collider2D>() is CompositeCollider2D)
        {
            Destroy(gameObject); // Destroy the bullet
        }
        else if (collision.GetComponent<Collider2D>().CompareTag("Bullet"))
        {
            //Destroy(gameObject); // Destroy the bullet
        }

        if (parent == "Player")
        {
            if (collision.GetComponent<Collider2D>().CompareTag("Enemy"))
            {
                
                Destroy(gameObject);
                EnemyScript enemyCollided = collision.GetComponent<EnemyScript>();
                enemyCollided.TakeDamage(20);
                enemyCollided.HitByBullet(gameObject);
            }
            if (collision.GetComponent<Collider2D>().CompareTag("EnemyBulletView"))
            {
                EnemyScript enemyCollided = collision.transform.parent.GetComponent<EnemyScript>();
                //enemyCollided.HitByBullet(gameObject);
                enemyCollided.BulletFlyBy(gameObject);
            }
        }
        if (parent == "Enemy")
        {
            if (collision.GetComponent<Collider2D>().CompareTag("Player"))
            {
                Destroy(gameObject);
            }
        }



    }
    //public void IgnoreCollisionWith(Collider2D collider)
    //{
    //    Collider2D bulletCollider = GetComponent<Collider2D>();
    //    if (bulletCollider != null && collider != null)
    //    {
    //        Physics2D.IgnoreCollision(bulletCollider, collider);
    //    }
    //}

    public void SetParent(string Parent)
    {
        this.parent = Parent;
        this.parentObject = gameObject;
    }
    
}