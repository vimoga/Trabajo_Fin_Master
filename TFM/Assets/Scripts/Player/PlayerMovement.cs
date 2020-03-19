using RTS_Cam;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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

    private bool isAttacking = false;

    private GameObject currentObjective;

    private GameObject currentSetection;

    private RTS_Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player");
        currentSetection = AuxiliarOperations.GetChildObject(jugador.transform, "Selection");
        agente = jugador.GetComponent<NavMeshAgent>();
        camera = GameObject.FindObjectOfType<RTS_Camera>();
        camera.SetTarget(jugador.transform);
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
            isAttacking = false;
        }

        
            GameObject auxiliar = hit.transform.gameObject;
            if (AuxiliarOperations.IsPlayableObject(hit.transform.gameObject.tag))
            {
                if (currentSetection.transform.parent != null && currentSetection.transform.parent.tag != "Player")
                {
                    Unselect();
                }
                Select(auxiliar);
            }
            else {
               if (currentSetection.transform.parent != null && currentSetection.transform.parent.tag != "Player")
                {
                    Unselect();
                }
            }


    }

    void RightClicked() {
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(MousePosition(), out hit))
        {

            GameObject auxiliar = hit.transform.gameObject;

            if (AuxiliarOperations.IsPlayableObject(hit.transform.gameObject.tag))
            {
                //change drone
                if (auxiliar.tag.Equals("Player_Drone"))
                {
                    GameObject.FindGameObjectWithTag("Player").tag = "Player_Drone";
                    auxiliar.tag = "Player";

                    jugador = auxiliar;
                    agente = auxiliar.GetComponent<NavMeshAgent>();
                    camera.SetTarget(jugador.transform);
                }
                else if (auxiliar.tag.Equals("Enemy") || auxiliar.tag.Equals("Enemy_Structure"))
                {
                    currentObjective = auxiliar;
                    isAttacking = true;
                }

                if (currentSetection.transform.parent != null && currentSetection.transform.parent.tag != "Player")
                {
                    Unselect();
                }
                Select(auxiliar);
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

    private void Select(GameObject toSelect)
    {
        GameObject selection=AuxiliarOperations.GetChildObject(toSelect.transform, "Selection");
        if (selection)
        {
            selection.SetActive(true);
            RawImage rawImage = selection.GetComponent<RawImage>();
            switch (toSelect.gameObject.tag)
            {
                case "Enemy":
                    rawImage.texture = (Texture)Resources.Load("Textures/selection_enemy");
                    break;
                case "Enemy_Structure":
                    rawImage.texture = (Texture)Resources.Load("Textures/selection_enemy");
                    break;
                case "Player_Drone":
                    rawImage.texture = (Texture)Resources.Load("Textures/selection_friend");
                    break;
                case "Player_Structure":
                    rawImage.texture = (Texture)Resources.Load("Textures/selection_friend");
                    break;
                case "Player":
                    rawImage.texture = (Texture)Resources.Load("Textures/selection");
                    break;
            }

            currentSetection = selection;
        }
            
    }

    private void Unselect()
    {
        currentSetection.SetActive(false);
    }




    // Update is called once per frame
    void Update()
    {
        if (!AuxiliarOperations.IsDestroyed(jugador))
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

            if (isAttacking)
            {
                if (!AuxiliarOperations.IsDestroyed(currentObjective) && !AuxiliarOperations.IsCaptured(currentObjective))
                {
                    if (Vector3.Distance(jugador.transform.position, currentObjective.transform.position) > jugador.GetComponent<DroneInterface>().GetFiringRange())
                    {
                        agente.destination = currentObjective.transform.position;
                    }
                    else
                    {
                        if (!currentObjective.GetComponent<CommonInterface>().isDestroyed())
                        {
                            jugador.GetComponent<DroneInterface>().Attack(currentObjective);
                        }

                    }
                }
                else
                {
                    currentObjective = null;
                    isAttacking = false;
                }
            }
        }

    }
}
