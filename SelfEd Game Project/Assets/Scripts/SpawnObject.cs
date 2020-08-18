using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    [SerializeField]
    float SpawnTimer;

    [SerializeField]
    GameObject SpawnObjectPrefab;

    float timePassed = 0;
    void Update()
    {
        if (timePassed >= SpawnTimer)
        {
            GameObject spawnedObject = Instantiate(SpawnObjectPrefab, transform.position, transform.rotation) as GameObject;
            timePassed = 0;
        }
        timePassed += Time.deltaTime;
    }
}
