using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnScipt : MonoBehaviour
{
    private string playerType;
    public GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {

        if (!GameObject.FindGameObjectWithTag("Player"))
        {
                Instantiate(playerPrefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
