using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Added for NavMeshAgent

public class PlayerMovement : MonoBehaviour
{   
    [SerializeField]
    private Camera camera;
    private NavMeshAgent agent;
    private RaycastHit[] hits = new RaycastHit[1];

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (camera == null)
            {
                Debug.LogWarning("Camera not assigned to PlayerMovement script.");
                return;
            }

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.RaycastNonAlloc(ray, hits) > 0)
            {
                agent.SetDestination(hits[0].point);
            }
        }
    }
}
