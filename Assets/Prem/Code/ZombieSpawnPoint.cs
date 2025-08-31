using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    public float spawnRadius = 10f;
    public float activationDistance = 15f;
    public int maxZombiesFromThisPoint = 5;

    [Header("Gizmo Settings")]
    public Color activationColor = Color.green;
    public Color spawnColor = Color.yellow;

    [HideInInspector] public bool isActive = false;
    [HideInInspector] public int currentZombiesFromThisPoint = 0;

    private void OnDrawGizmosSelected()
    {
        // «“¥√—»¡’ activation
        Gizmos.color = isActive ? Color.red : activationColor;
        Gizmos.DrawWireSphere(transform.position, activationDistance);

        // «“¥√—»¡’ spawn
        Gizmos.color = spawnColor;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}