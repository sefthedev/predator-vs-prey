using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimulationRules 
{
    public static float xBorder = 40;
    public static float yBorder = 40;
    public static int predatorPopulationMaxSize = 180;
    public static int preyPopulationMaxSize = 800;
    public static float timeToSplit = 7f;
    public static int eatenPreyToSplit = 4;
    public static readonly float maxEnergy = 15f;
}
