using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyLineOfSightChecker : MonoBehaviour
{   
    public SphereCollider Collider;

    public float FieldView = 90f;
    public LayerMask LineOfSightLayers; // Fixed variable name
    public delegate void GainSightEvent(Player player);
    public GainSightEvent OnGainSight;
    public delegate void LoseSightEvent(Player player);
    public LoseSightEvent OnLoseSight;

    private Coroutine checkForLineOfSight; // Fixed variable naming convention

    private void Awake()
    {
        Collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player;
        if (other.TryGetComponent<Player>(out player))
        {
            if (!CheckLineOfSight(player))
            {
                checkForLineOfSight = StartCoroutine(CheckLineOfSightCoroutine(player)); // Fixed coroutine call
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player;
        if (other.TryGetComponent<Player>(out player))
        {   
            OnLoseSight?.Invoke(player);
            if (checkForLineOfSight != null)
            {   
                StopCoroutine(checkForLineOfSight); // Fixed coroutine stop
                checkForLineOfSight = null;
            }
        }
    }

    private bool CheckLineOfSight(Player player)
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        if (Vector3.Dot(transform.forward, direction) >= Mathf.Cos(FieldView * Mathf.Deg2Rad)) // Fixed condition
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, Collider.radius, LineOfSightLayers))
            {
                if (hit.transform.GetComponent<Player>() != null)
                {
                    OnGainSight?.Invoke(player);
                    return true;
                }
            }
        }
        return false; // Fixed missing return statement
    }

    private IEnumerator CheckLineOfSightCoroutine(Player player) // Renamed method to avoid naming conflict
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f); // Fixed incorrect declaration
        while (!CheckLineOfSight(player))
        {
            yield return wait;
        }
    }
}
