using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{   
    public int NumberOfEnemiesToSpawn = 5;
    public float SpawnDelay = 3.0f;
    public Transform Player;  // Added missing Player reference
    private NavMeshTriangulation Triangulation;

    public SpawnMethod EnemySpawnMethod = SpawnMethod.Random;

    public List<Enemy> EnemyPrefabs = new List<Enemy>(); 
    public Dictionary<int, ObjectPool> EnemyObjectPools = new Dictionary<int, ObjectPool>();
    
    void Awake()
    {   
        for (int i = 0; i < EnemyPrefabs.Count; i++)
        {
            EnemyObjectPools.Add(i, ObjectPool.CreateInstance(EnemyPrefabs[i], NumberOfEnemiesToSpawn));
        }
    }
    
    private void Start()
    {   
        Triangulation = NavMesh.CalculateTriangulation();  // Fixed method name
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds Wait = new WaitForSeconds(SpawnDelay);  // Fixed 'New' to 'new'
        int EnemiesSpawned = 0;

        while (EnemiesSpawned < NumberOfEnemiesToSpawn)
        {   
            if (EnemySpawnMethod == SpawnMethod.Random)
            {
                DoSpawnEnemy(EnemiesSpawned);
            }
            
            EnemiesSpawned++;  // Moved inside the while loop
            yield return Wait;
        }
    }

    private void SpawnRandomEnemy()
    {
        DoSpawnEnemy(Random.Range(0, EnemyPrefabs.Count - 1));
    }

    private void DoSpawnEnemy(int SpawnIndex)
    {
        PoolableObject poolableObject = EnemyObjectPools[SpawnIndex].GetObject();

        if (poolableObject != null)
        {
            Enemy enemy = poolableObject.GetComponent<Enemy>();
            
            int VertexIndex = Random.Range(0, Triangulation.vertices.Length);
            NavMeshHit Hit;  // Fixed type name
            if(NavMesh.SamplePosition(Triangulation.vertices[VertexIndex], out Hit, 2f, -1))  // Fixed method name
            {
                enemy.Agent.Warp(Hit.position);
                enemy.Agent.enabled = true;
                enemy.Movement.Triangulation = Triangulation;
                enemy.Movement.Spawn();
            }  
        }
    }

    public enum SpawnMethod
    {
        Random,
        RoundRobin
    }
}