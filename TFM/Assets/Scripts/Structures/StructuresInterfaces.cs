using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the basic funtions of the structures
/// </summary>
public interface StructuresInterfaces
{
    void Attack();
    void SetCaptured(bool isCaptured);
    bool GetCaptured();
    void OnTriggerEnter(Collider other);
    void OnTriggerStay(Collider other);
    void OnTriggerExit(Collider other);
}
