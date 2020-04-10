using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public GameObject fogOfWarPlane;
    public Transform player;
    public LayerMask fogLayer;
    public float radius = 5f;

    private float radiusSqr { get { return radius * radius; } }
    private Mesh m_Mesh;
    private Vector3[] vertices;
    private Color[] colors;

    // Start is called before the first frame update
    void Start()
    {
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

    void UpdateColor()
    {
        m_Mesh.colors = colors;
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = fogOfWarPlane.transform.TransformPoint(vertices[i]);
            float dist = Vector3.SqrMagnitude(v - player.transform.position);
            dist /= 200;
            /*if (dist < radiusSqr)
            {*/
            float alpha = Mathf.Min(colors[i].a, dist / radiusSqr);
            colors[i].a = alpha;
            //}
        }
        
        /*//Ray r = new Ray(transform.position, player.position - transform.position);
        //Debug.DrawRay(transform.position, player.position - transform.position, Color.green, 2, false);

        Vector3 up = player.transform.TransformDirection(Vector3.up) * 10;
        Debug.DrawRay(player.transform.position, up, Color.green);


        RaycastHit hit;
        //if (Physics.Raycast(r, out hit, 1000, fogLayer)) {
        if (Physics.Raycast(player.transform.position,Vector3.up, out hit, 1000, fogLayer))
        {
            for (int i=0; i<vertices.Length; i++)
            {
                Vector3 v = fogOfWarPlane.transform.TransformPoint(vertices[i]);
                float dist = Vector3.SqrMagnitude(v - hit.point);
                if (dist < radiusSqr)
                {
                    float alpha = Mathf.Min(colors[i].a, dist / radiusSqr);
                    colors[i].a = alpha;
                }
            }
        }*/
        UpdateColor();
    }
}
