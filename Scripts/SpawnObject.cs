using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Вешается на пустые объекты-спавнеры на противниках
public class SpawnObject : MonoBehaviour
{
    //Промежуток времени между спавном 
    [SerializeField]
    float SpawnTimer;

    //Префаб, который нужно заспавнить
    [SerializeField]
    GameObject SpawnObjectPrefab;

    //Таймер
    float timePassed = 0;
    void Update()
    {
        //По прошествию определенного времени спавнит объект и перезапускает таймер
        timePassed += Time.deltaTime;
        if (timePassed >= SpawnTimer)
        {
            GameObject spawnedObject = Instantiate(SpawnObjectPrefab, transform.position, transform.rotation) as GameObject;
            timePassed = 0;
        }
    }
}
