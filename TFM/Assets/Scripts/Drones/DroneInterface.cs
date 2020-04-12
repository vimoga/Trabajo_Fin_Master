using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the basic funtions of the drones
/// </summary>
public interface DroneInterface
{
    void Attack(GameObject enemy);
    void SetCaptured(bool isCaptured);
    bool GetCaptured();
    float GetFiringRange();
    void OnTriggerEnter(Collider other);
    void OnTriggerStay(Collider other);
    void OnTriggerExit(Collider other);
    void GoToAttackState();
    void GoToAlertState();
    void GoToPatrolState();
    void GoToCapturedState();
}
