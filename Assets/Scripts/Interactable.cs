using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private float radius = 4f;
    public bool isCurrentlyUnuseable = false;
    [SerializeField] public string interactType;
    public bool isBeingUsed = false;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
