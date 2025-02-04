using Naninovel;
using UnityEngine;

namespace Naninovel
{
    [InitializeAtRuntime]
    public class MiniGameEngineService : IEngineService
    {
        private GameObject miniGamePrefab;


        public async UniTask StartMiniGameAsync(string movesVar, string timeVar)
        {
            var config = Engine.GetConfiguration<MiniGameConfiguration>();
            if (config == null)
            {
                Debug.LogError("MiniGameConfiguration is NULL");
                return;
            }

            miniGamePrefab = config.MiniGamePrefab;
            
            if (miniGamePrefab == null)
            {
                Debug.LogError("MiniGame prefab is not assigned in Naninovel Configuration.");
                return;
            }

            var miniGameObj = Object.Instantiate(miniGamePrefab);
            var memoryGame = miniGameObj.GetComponent<MemoryGame>();

            GameResult result = await RunGame(memoryGame);
            
            Object.DestroyImmediate(miniGameObj);
            
            Engine.GetService<CustomVariableManager>().SetVariableValue(movesVar, result.Moves.ToString());
            Engine.GetService<CustomVariableManager>().SetVariableValue(timeVar, result.TimeSpent.TotalSeconds.ToString("F2"));
        }

        private UniTask<GameResult> RunGame(MemoryGame game)
        {
            var tcs = new UniTaskCompletionSource<GameResult>();
            game.StartGame((GameResult result) => tcs.TrySetResult(result)); // Явное указание типа
            return tcs.Task;
        }

        public UniTask InitializeServiceAsync()
        {
            return new UniTask();
        }

        public void ResetService()
        {
        }

        public void DestroyService()
        {
        }
    }
}