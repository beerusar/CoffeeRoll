using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupSpawner : MonoBehaviour
{
    public GameObject prefab;
    public Transform target;
    public int spawned;

    private IEnumerator Spawner()
    {
        while (spawned < Global.data.cups)
        {
            yield return new WaitForSeconds(1.5f);
            Instantiate(prefab, transform.position, Quaternion.identity).GetComponent<TableCup>().SetTarget(target.position);
            spawned++;
        }
    }

    private void Start()
    {
        StartCoroutine(Spawner());
    }

}
