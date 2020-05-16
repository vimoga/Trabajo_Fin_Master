using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DroneState { ATTACK, PATROL, ALERT, CAPTURED,DESTROYED };

/// <summary>
/// Constants used in the game
/// </summary>
static class GameConstants
{
    public static float TERRAIN_HEIGHT_CORRECTION = 4;
    public static float SEPARATION_TERRAIN_AERIAL = 3;
    public static float CAPTURE_LIMIT = 80;
    public static float INFINITE_AMMO = -1;
    public static float MAX_CPU_POWER = 8;
    public static float ALERT_TIME = 5f;
    public static float WAYPOINT_STOP_AVOID = 1;
    public static float TUTORIAL_TIME = 5;


    public static Vector3 spawnPoint;
    public static float currentCPUPower = 0;
    public static string currentLevel = "MainLevel2";

    public static ArrayList radarCaptured = new ArrayList();
    public static ArrayList postCaptured = new ArrayList();
    public static GameObject playerTemp;

}
