using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for the Radar Tower Structure
/// </summary>
public class Radar : MonoBehaviour, StructuresInterfaces
{

    /// <summary>
    /// Plane of where the fog of war is implemented
    /// </summary>
    public GameObject fogOfWarPlane;

    /// <summary>
    /// Distance the structure can provide cover
    /// </summary>
    public float coverRange = 7f;

    /// <summary>
    /// Distance where the fog of war is revelated
    /// </summary>
    public float fogOfWarCover = 0.75f;

    private float coverRangeSqr { get { return coverRange * coverRange; } }
    private Mesh m_Mesh;
    private Vector3[] vertices;
    private Color[] colors;

    private bool isCaptured = false;

    private bool isDestroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponentInChildren<SphereCollider>().radius = coverRange;

        isCaptured = GetComponent<BasicStructure>().isCaptured;

        m_Mesh = fogOfWarPlane.GetComponent<MeshFilter>().mesh;
        vertices = m_Mesh.vertices;
        colors = new Color[vertices.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.black;
            colors[i].a = 255;
        }
        UpdateColor();
    }

    public void Attack()
    {
        Debug.Log("Radars are unable to attack");
    }

    public void SetCaptured(bool isCaptured)
    {
        GetComponent<BasicStructure>().isCaptured = isCaptured;
    }

    public bool GetCaptured()
    {
        return GetComponent<BasicStructure>().isCaptured;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                if (other.transform.gameObject.GetComponent<BasicDrone>())
                {
                    if (!other.transform.gameObject.GetComponent<BasicDrone>().isOnCover)
                    {
                        other.transform.gameObject.SendMessage("InCover", SendMessageOptions.RequireReceiver);
                    }
                }
            }
        }       
    }

    public void OnTriggerStay(Collider other)
    {
        if (isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                if (other.transform.gameObject.GetComponent<BasicDrone>())
                {
                    if (!other.transform.gameObject.GetComponent<BasicDrone>().isOnCover)
                    {
                        other.transform.gameObject.SendMessage("InCover", SendMessageOptions.RequireReceiver);
                    }
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                if (other.transform.gameObject.GetComponent<BasicDrone>())
                {
                    if (other.transform.gameObject.GetComponent<BasicDrone>().isOnCover)
                    {
                        other.transform.gameObject.SendMessage("OutCover", SendMessageOptions.RequireReceiver);
                    }
                }
            }
        }
    }

    void UpdateColor()
    {
        m_Mesh.colors = colors;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCaptured)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = fogOfWarPlane.transform.TransformPoint(vertices[i]);
                float dist = Vector3.SqrMagnitude(v - transform.position);
                dist /= coverRangeSqr;
                if (dist < fogOfWarCover)
                {
                    float alpha = Mathf.Min(colors[i].a, dist / fogOfWarCover);
                    colors[i].a = alpha;
                }
            }
            UpdateColor();
        }
        

        isCaptured = GetComponent<BasicStructure>().isCaptured;
        isDestroyed = GetComponent<CommonInterface>().isDestroyed();
    }
    
}
