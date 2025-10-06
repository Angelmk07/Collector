using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    //[Header("Prefabs")]
    //[SerializeField] private GameObject[] prefabs;   // массив спавнимых объектов

    //[Header("Spawn Points")]
    //[SerializeField] private Transform[] spawnPoints; // точки появления

    //[Header("Spawn Settings")]
    //[SerializeField] private bool spawnOnStart = true;
    //[SerializeField] private bool randomOrder = false;
    //[SerializeField] private float spawnDelay = 0f; // если > 0 — спавнит с задержкой

    //private void Start()
    //{
    //    if (spawnOnStart)
    //        SpawnAll();
    //}
    ///// <summary>
    ///// Спавнит все объекты из массива в соответствующих точках.
    ///// Если объектов меньше, чем точек, то использует циклически.
    ///// </summary>
    //public void SpawnAll()
    //{
    //    if (spawnPoints.Length == 0 || prefabs.Length == 0)
    //    {
    //        Debug.LogWarning("❗ ObjectSpawner: не заданы точки или префабы!");
    //        return;
    //    }

    //    for (int i = 0; i < spawnPoints.Length; i++)
    //    {
    //        int prefabIndex = randomOrder ? Random.Range(0, prefabs.Length) : i % prefabs.Length;

    //        if (spawnDelay > 0)
    //            StartCoroutine(SpawnWithDelay(prefabs[prefabIndex], spawnPoints[i].position, spawnDelay * i));
    //        else
    //            Instantiate(prefabs[prefabIndex], spawnPoints[i].position, Quaternion.identity);
    //    }
    //}

    ///// <summary>
    ///// Спавнит один объект в случайной точке.
    ///// </summary>
    //public void SpawnRandom()
    //{
    //    if (spawnPoints.Length == 0 || prefabs.Length == 0)
    //        return;

    //    Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
    //    GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];

    //    Instantiate(prefab, point.position, Quaternion.identity);
    //}

    //private System.Collections.IEnumerator SpawnWithDelay(GameObject prefab, Vector3 position, float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    Instantiate(prefab, position, Quaternion.identity);
    //}
}
