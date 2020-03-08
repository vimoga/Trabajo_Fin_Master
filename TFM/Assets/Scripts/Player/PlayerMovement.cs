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
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(MousePosition(), out hit))
        {

            //indicamos al agente que su destino es el punto marcado
            agente.destination = hit.point;
            
        }

    }

    void RightClicked() {
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(MousePosition(), out hit))
        {

           GameObject auxiliar = hit.transform.gameObject;
           if (auxiliar.tag.Equals("Player_Drone"))
           {
                GameObject.FindGameObjectWithTag("Player").tag = "Player_Drone";
                auxiliar.tag = "Player";

                jugador = auxiliar;
                agente = auxiliar.GetComponent<NavMeshAgent>();
            }

        }
    }

    /// <summary>
    /// Obtencion del punto donde a clickado el jugador
    /// </summary>
    /// <returns>punto donde a clickado el jugador</returns>
    Ray MousePosition() {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    } 


    // Update is called once per frame
    void Update()
    {
        //detección del input del jugador
        if (Input.GetMouseButtonDown(0))
        {
            Clicked();
        }

        //detección del input del jugador
        if (Input.GetMouseButtonDown(1))
        {
            RightClicked();
        }

        //jugador.transform.LookAt(agente.nextPosition);
    }
}
