using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanSpawner : MonoBehaviour
{
    public Vector3 min, max;
    public GameObject prefab;
    public Material[] materials;

    public void SpawnAndWalk(Table t)
    {
        var spawnPos = new Vector3(Random.Range(min.x, max.x), 1, Random.Range(min.z, max.z));

        t.humanCount = 2;

        var human = Instantiate(prefab, spawnPos, Quaternion.identity).GetComponent<Human>();
        human.table = t;
        human.agent.SetDestination(t.chair1);
        human.ren.sharedMaterial = materials[Random.Range(0, materials.Length)];

        human = Instantiate(prefab, spawnPos, Quaternion.identity).GetComponent<Human>();
        human.table = t;
        human.agent.SetDestination(t.chair2);
        human.ren.sharedMaterial = materials[Random.Range(0, materials.Length)];
    }
}
