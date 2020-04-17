using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DroneState { ATTACK, PATROL, ALERT, CAPTURED };

/// <summary>
/// Constants used in the game
/// </summary>
static class GameConstants
{
   public static float SEPARATION_TERRAIN_AERIAL = 3;
   public static float CAPTURE_LIMIT = 100;
   public static float INFINITE_AMMO = -1;
   public static float MAX_CPU_POWER = 8;
   public static float ALERT_TIME = 5f;
   public static float WAYPOINT_STOP_AVOID = 1;
}
