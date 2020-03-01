using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Modificacion de rotor.cs para laa estructura del arma antiaerea 
/// </summary>
public class AA_Rotation : MonoBehaviour
{
    Rigidbody rBody;
    public float power;

    /// <summary>
    /// Specify the verse of the rotation
    /// <para> Set this in the editor
    /// </summary>
    public bool counterclockwise = false;

    /// <summary>
    /// Specify is the rotors are animated or static. Just a visual effect
    /// <para> Set this in the editor
    /// </summary>
    public bool animationActivated = false;

    /// <summary>
    /// Function called before of the first update
    /// </summary>
    void Start()
    {
        Transform t = this.transform;
        while (t.parent != null && t.tag != "Player") t = t.parent;
        rBody = t.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Sets the rotating power of the rotor
    /// </summary>
    /// <param name="intensity"> The rotating power of the rotor </param>
    public void setPower(float intensity) { power = intensity; }

    /// <summary>
    /// Gets the rotating power of the rotor
    /// </summary>
    /// <returns>the actual rotating power of the rotor</returns>
    public float getPower() { return power; }

    /// <summary>
    /// Function called once per frame
    /// </summary>
    void Update() { if (animationActivated) transform.Rotate(0, power * 70 * Time.deltaTime * (counterclockwise ? -1 : 1), 0); }
}
