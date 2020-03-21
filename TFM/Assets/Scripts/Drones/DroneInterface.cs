using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface DroneInterface
{
    void Attack(GameObject enemy);
    void SetCaptured(bool isCaptured);
    bool GetCaptured();
    float GetFiringRange();
    void OnTriggerEnter(Collider other);
    void OnTriggerStay(Collider other);
    void OnTriggerExit(Collider other);
}
