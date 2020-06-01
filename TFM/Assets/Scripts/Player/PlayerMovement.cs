using RTS_Cam;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Implements the player movement via  the A* implmented in Unity (NavMesh y NavMeshAgent).
/// Also manage the selection of playable elements.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// GameObject of the player
    /// </summary>
    private GameObject jugador;


    /// <summary>
    /// Players NavMesh Agent
    /// </summary>
    private NavMeshAgent agente;

    private bool isAttacking = false;

    private GameObject currentObjective;

    private GameObject currentSetection;

    private GameObject currentPlayerSetection;

    private RTS_Camera camera;

    private bool buildFix = false;

    // Start is called before the first frame update
    void Start()
    {
        //Capture the elements needed
        if (GameConstants.playerTemp != null)
        {
            jugador = GameConstants.playerTemp;
        }
        else {
            jugador = GameObject.FindGameObjectWithTag("Player");
        }
        
        currentSetection = AuxiliarOperations.GetChildObject(jugador.transform, "Selection");
        currentPlayerSetection = currentSetection;
        agente = jugador.GetComponent<NavMeshAgent>();
        camera = GameObject.FindObjectOfType<RTS_Camera>();
        camera.SetTarget(jugador.transform);

        if (!currentSetection.activeSelf)
        {
            currentSetection.SetActive(true);
        }     
    }


    void Clicked()
    {
        //Get clicked point
        RaycastHit hit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit))
        {          
            GameObject auxiliar = hit.transform.gameObject;
            if (AuxiliarOperations.IsPlayableObject(hit.transform.gameObject.tag))
            {
                //select element clicked
                if (!AuxiliarOperations.IsDestroyed(currentSetection))
                {
                    if (currentSetection.transform.parent != null && currentSetection.transform.parent.tag != "Player")
                    {
                        Unselect();
                    }
                    Select(auxiliar);
                }                              
            }
            else
            {
                if (!AuxiliarOperations.IsDestroyed(jugador))
                {
                    //Move the player to the selected point
                    agente.isStopped = false;
                    agente.destination = hit.point;
                    currentObjective = null;
                    isAttacking = false;
                }

                if (!AuxiliarOperations.IsDestroyed(currentSetection))
                {
                    if (currentSetection.transform.parent != null && currentSetection.transform.parent.tag != "Player")
                    {
                        Unselect();
                    }                    
                }
                else {
                    currentSetection = null;
                }                
            }
        }

        
    }

    void RightClicked() {
        //Get clicked point
        RaycastHit hit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            GameObject auxiliar = hit.transform.gameObject;

            if (AuxiliarOperations.IsPlayableObject(hit.transform.gameObject.tag))
            {
                //change drone
                if (auxiliar.tag.Equals("Player_Drone"))
                {
                    if (!AuxiliarOperations.IsDestroyed(jugador))
                    {
                        GameObject.FindGameObjectWithTag("Player").tag = "Player_Drone";
                    }

                    UnselectPlayer();
                    auxiliar.tag = "Player";                   
                    jugador = auxiliar;
                    agente = auxiliar.GetComponent<NavMeshAgent>();
                    camera.SetTarget(jugador.transform);
                    SelectPlayerSelection(jugador);
                }
                else if (auxiliar.tag.Equals("Enemy") || auxiliar.tag.Equals("Enemy_Structure"))
                {
                    //atack clicked element
                    if (!AuxiliarOperations.IsDestroyed(jugador))
                    {
                        currentObjective = auxiliar;
                        isAttacking = true;
                    }
                }
                               
                if (!AuxiliarOperations.IsDestroyed(currentSetection))
                {                   
                    if (currentSetection.transform.parent != null)
                    {
                        Unselect();
                    }
                    Select(auxiliar);                
                }
                else
                {
                    Select(auxiliar);                  
                }                
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

    /// <summary>
    /// selects the element on the gameplay screen
    /// </summary>
    /// <param name="toSelect">ellemt to select</param>
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

    /// <summary>
    /// Defines the current player drone selection UIs
    /// </summary>
    /// <param name="toSelect">player drone</param>
    private void SelectPlayerSelection(GameObject toSelect)
    {
        currentPlayerSetection = AuxiliarOperations.GetChildObject(toSelect.transform, "Selection");
    }

    /// <summary>
    /// Unselect the current selection
    /// </summary>
    private void Unselect()
    {
        currentSetection.SetActive(false);
    }

    /// <summary>
    /// Unselect the current player selection
    /// </summary>
    private void UnselectPlayer()
    {
        if (currentPlayerSetection)
        {
            if (currentPlayerSetection.activeSelf)
            {
                currentPlayerSetection.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Allows to select a player from other script
    /// </summary>
    /// <param name="toSelect">drone to select</param>
    public void ExternalSelect(GameObject toSelect) {
        if (toSelect.tag != "Player")
        {
            if (!AuxiliarOperations.IsDestroyed(jugador))
            {
                GameObject.FindGameObjectWithTag("Player").tag = "Player_Drone";
            }

            UnselectPlayer();
            toSelect.tag = "Player";
            jugador = toSelect;
            agente = toSelect.GetComponent<NavMeshAgent>();
            camera.SetTarget(jugador.transform);
            SelectPlayerSelection(jugador);

            if (!AuxiliarOperations.IsDestroyed(currentSetection))
            {
                if (currentSetection.transform.parent != null)
                {
                    Unselect();
                }
                Select(toSelect);
            }
            else
            {
                Select(toSelect);
            }
        }
        else {
            jugador = toSelect;
            agente = toSelect.GetComponent<NavMeshAgent>();           
            camera.SetTarget(jugador.transform);
            SelectPlayerSelection(jugador);         
        }       
    }

    // Update is called once per frame
    void Update()
    {
        //Detects player left click
        if (Input.GetMouseButtonDown(0)&& !EventSystem.current.IsPointerOverGameObject())
        {
            Clicked();
        }

        //Detects player right click
        if (Input.GetMouseButtonDown(1))
        {
            RightClicked();
        }

        //Move to the enemy if plater is out attack reach or attack if is in the range
        if (!AuxiliarOperations.IsDestroyed(jugador))
        {         
            if (isAttacking)
            {
                if (!AuxiliarOperations.IsDestroyed(currentObjective) && !AuxiliarOperations.IsCaptured(currentObjective))
                {
                    float testDistance = Vector3.Distance(jugador.transform.position, currentObjective.transform.position);
                    if (Vector3.Distance(jugador.transform.position, currentObjective.transform.position) > (jugador.GetComponent<DroneInterface>().GetFiringRange()))
                    {
                        agente.isStopped = false;
                        agente.destination = currentObjective.transform.position;
                       
                    }
                    else
                    {
                        if (!currentObjective.GetComponent<CommonInterface>().isDestroyed())
                        {
                            if ((jugador.GetComponent<BasicDrone>().maxAmmo != GameConstants.INFINITE_AMMO && jugador.GetComponent<BasicDrone>().ammo > 0) || jugador.GetComponent<BasicDrone>().maxAmmo == GameConstants.INFINITE_AMMO) {
                                jugador.GetComponent<DroneInterface>().Attack(currentObjective);
                                agente.isStopped=true;                               
                            }
                            
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
