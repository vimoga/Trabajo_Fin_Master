using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface DroneInterface
{
    void Attack(GameObject enemy);
    void SetCaptured(bool isCaptured);
    bool GetCaptured();
    float GetFiringRange();
}
