using System.Collections.Generic;
using UnityEngine;
using Naninovel;

namespace Naninovel
{
    [EditInProjectSettings]
    public class MiniGameConfiguration : Configuration
    {
        [Tooltip("Prefab of the mini-game.")] 
        public GameObject MiniGamePrefab;
        
        [Tooltip("The list of variables to initialize by default. Global variables (names starting with `G_` or `g_`) are initialized on first application start, and others on each state reset.")]
        public List<CustomVariable> MiniGamesList = new List<CustomVariable>();

    }
}