using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Clase encargada de implementar el movimento del jugador 
/// mediante los elementos A* inplementados en Unity (NavMesh y NavMeshAgent).
/// El personaje del jugador se movera donde este haya clicado del escenario.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// GameObject del avatar del jugador
    /// </summary>
    private GameObject jugador;

    /// <summary>
    /// Agente del NavMesh del jugador
    /// </summary>
    private NavMeshAgent agente;

    // Start is called before the first frame update
    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player");
        agente = jugador.GetComponent<NavMeshAgent>();
    }


    void Clicked()
    {

        //Obtencion del punto donde a clickado el jugador
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {

            //indicamos al agente que su destino es el punto marcado
            agente.destination = hit.point;
            
        }

    }


    // Update is called once per frame
    void Update()
    {
        //detección del input del jugador
        if (Input.GetMouseButtonDown(0))
        {
            Clicked();
        }

        jugador.transform.LookAt(agente.nextPosition);
    }
}
