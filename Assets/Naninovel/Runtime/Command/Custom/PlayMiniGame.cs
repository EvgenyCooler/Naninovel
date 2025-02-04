using System.Collections.Generic;
using UnityEngine;

namespace Naninovel.Commands
{
    [CommandAlias("minigame")]
    public class PlayMiniGame : Command
    {
        [ParameterAlias(NamelessParameterAlias), RequiredParameter, ResourceContext(MoviesConfiguration.DefaultPathPrefix)]
        public StringParameter GameName;
        
        [Tooltip("The list of variables to initialize by default. Global variables (names starting with `G_` or `g_`) are initialized on first application start, and others on each state reset.")]
        public List<CustomVariable> PredefinedVariables = new List<CustomVariable>();

        public override async UniTask ExecuteAsync (AsyncToken asyncToken = default)
        {
            var service = Engine.GetService<MiniGameEngineService>();
            if (service == null)
            {
                Debug.LogError("MiniGameEngineService is NULL");
                return;
            }
                
            await service.StartMiniGameAsync("moveCount","gameTime");
        }
    }
}
