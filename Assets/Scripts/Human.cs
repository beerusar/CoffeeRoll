using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour
{
    #region Variables
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Renderer ren;
    [HideInInspector] public Table table;

    private bool arrived, done;
    private Vector3 startPos; 
    #endregion

    #region Default Functions
    private void Awake()
    {
        startPos = transform.position;
        ren = GetComponent<Renderer>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (arrived)
        {
            var diff = table.transform.position - transform.position;
            diff.y = 0;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(diff), Time.deltaTime * 8f);
            if (table.moneyObject.activeSelf && !done)
            {
                table.humanCount--;
                table.arrived--;
                arrived = false;
                agent.SetDestination(startPos);
                done = true;
            }
        }
        else
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        if (done)
                        {
                            //table.done = false;
                            Destroy(gameObject);
                        }
                        else
                        {
                            table.arrived++;
                            arrived = true;
                        }
                    }
                }
            }
        }
    } 
    #endregion
}
