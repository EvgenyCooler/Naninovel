// Copyright 2023 ReWaffle LLC. All rights reserved.

using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Naninovel.UI
{
    public class WorldMapPanel : CustomUI, IWorldMapUI
    {
        [Serializable]
        public new class GameState
        {
            public string VariableName;
            public LocalizableText SummaryText;
            //public string InputFieldText;
            public bool PlayOnSubmit;
        }

        protected virtual LocalizableText Summary { get; private set; }
        //protected virtual TMP_InputField InputField => inputField;
        //protected virtual Button SubmitButton => submitButton;
        protected virtual bool ActivateOnShow => activateOnShow;
        protected virtual bool SubmitOnInput => submitOnInput;
        protected virtual GameObject SummaryContainer => summaryContainer;

        //[SerializeField] private TMP_InputField inputField;
        //[SerializeField] private Button submitButton;
        [Tooltip("Whether to automatically select and activate input field when the UI is shown.")]
        [SerializeField] private bool activateOnShow = true;
        [Tooltip("Whether to attempt submit input field value when a `Submit` input is activated.")]
        [SerializeField] private bool submitOnInput = true;
        [Tooltip("When assigned, the game object will be de-/activated based on whether summary is assigned.")]
        [SerializeField] private GameObject summaryContainer;
        [SerializeField] private StringUnityEvent onSummaryChanged;
        [SerializeField] private StringUnityEvent onPredefinedValueChanged;

        [SerializeField] private Button buttonLocation1;
        [SerializeField] private Button buttonLocation2;
        [SerializeField] private Button buttonLocation3;
        
        private IScriptPlayer scriptPlayer;
        private ICustomVariableManager variableManager;
        private IStateManager stateManager;
        private IInputSampler submitInput;
        private string variableName = "worldMapLocation";
        private bool playOnSubmit = true;

        private string selectedLocation = ".Location1";
        //private readonly CancellationTokenSource cts = new CancellationTokenSource();
        
        // ReSharper disable Unity.PerformanceAnalysis
        public virtual void Show (string variableName, LocalizableText summary, 
            LocalizableText predefinedValue, bool playOnSubmit,
            bool isInteractableLocation1,
            bool isInteractableLocation2,
            bool isInteractableLocation3
            )
        {
            this.variableName = variableName;
            this.playOnSubmit = playOnSubmit;
            SetSummary(summary);
            SetPredefinedValue(predefinedValue);

            //TakeOffAll();
            
            Show();

            if (ActivateOnShow)
            {
                // InputField.Select();
                // InputField.ActivateInputField();
            }

            buttonLocation1.interactable = isInteractableLocation1;
            buttonLocation2.interactable = isInteractableLocation2;
            buttonLocation3.interactable = isInteractableLocation3;
        }

        public override void Show()
        {
            base.Show();
            TakeOffAll();
        }
        
        public async void TakeOffAll()
        {
            
            var textPrinterManager = Engine.GetService<ITextPrinterManager>();
            if (textPrinterManager is null)
                return;
            var alltTextPrinterActors = textPrinterManager.GetAllActors();
            foreach (var actor in alltTextPrinterActors)
                actor.Visible = false;
            
            var characterManager = Engine.GetService<ICharacterManager>();
            if (characterManager is null)
                return;
            var allCharacterActors = characterManager.GetAllActors();
            foreach (var actor in allCharacterActors)
                actor.Visible = false;

            var backgroundManager = Engine.GetService<IBackgroundManager>();
            if (backgroundManager is null)
                return;
            var allBackgroundActors = backgroundManager.GetAllActors();
            foreach (var actor in allBackgroundActors)
                actor.Visible = false;

            var backgroundMusicManager = Engine.GetService<IAudioManager>();
            if (backgroundMusicManager is null)
                return;
            await backgroundMusicManager.StopAllBgmAsync(0);
        }

        protected override void Awake ()
        {
            base.Awake();
            //this.AssertRequiredObjects(InputField, SubmitButton);

            scriptPlayer = Engine.GetService<IScriptPlayer>();
            variableManager = Engine.GetService<ICustomVariableManager>();
            stateManager = Engine.GetService<IStateManager>();
            submitInput = Engine.GetService<IInputManager>().GetSubmit();

            //SubmitButton.interactable = false;
            buttonLocation1.interactable = true;
            buttonLocation2.interactable = true;
            buttonLocation3.interactable = true;
        }

        protected override void OnEnable ()
        {
            base.OnEnable();

            buttonLocation1.onClick.AddListener(() => selectedLocation = ".Location1");
            buttonLocation2.onClick.AddListener(() => selectedLocation = ".Location2");
            buttonLocation3.onClick.AddListener(() => selectedLocation = ".Location3");

            
            buttonLocation1.onClick.AddListener(HandleSubmit);
            buttonLocation2.onClick.AddListener(HandleSubmit);
            buttonLocation3.onClick.AddListener(HandleSubmit);


            if (submitInput != null && SubmitOnInput)
                submitInput.OnStart += HandleSubmit;
        }

        protected override void OnDisable ()
        {
            base.OnDisable();

            buttonLocation1.onClick.RemoveAllListeners();
            buttonLocation2.onClick.RemoveAllListeners();
            buttonLocation3.onClick.RemoveAllListeners();

            if (submitInput != null && SubmitOnInput)
                submitInput.OnStart -= HandleSubmit;
        }

        protected override void SerializeState (GameStateMap stateMap)
        {
            base.SerializeState(stateMap);

            var state = new GameState {
                VariableName = variableName,
                SummaryText = Summary,
                //InputFieldText = InputField.text,
                PlayOnSubmit = playOnSubmit
            };
            stateMap.SetState(state);
        }

        protected override async UniTask DeserializeState (GameStateMap stateMap)
        {
            await base.DeserializeState(stateMap);

            var state = stateMap.GetState<GameState>();
            if (state is null) return;

            variableName = state.VariableName;
            SetSummary(state.SummaryText);
            //InputField.text = state.InputFieldText;
            playOnSubmit = state.PlayOnSubmit;
        }

        protected virtual void SetSummary (LocalizableText value)
        {
            Summary = value;
            onSummaryChanged?.Invoke(value);
            if (SummaryContainer)
                SummaryContainer.SetActive(!value.IsEmpty);
        }

        protected virtual void SetPredefinedValue (LocalizableText value)
        {
            onPredefinedValueChanged?.Invoke(value);
        }

        protected virtual void HandleInputChanged (string text)
        {
            //SubmitButton.interactable = !string.IsNullOrWhiteSpace(text);
        }

        private void SetSelectedLocation(int value)
        {
            selectedLocation = value.ToString();
        }
        
        protected async virtual void HandleSubmit ()
        {
            if (!Visible) return;

            stateManager.PeekRollbackStack()?.AllowPlayerRollback();

            variableManager.SetVariableValue(variableName, selectedLocation);


            Debug.Log($"NEXT COMMAND - {scriptPlayer.PlayedIndex}");

            
            using (var cts = new CancellationTokenSource())
            {
                stateManager.OnRollbackStarted += cts.Cancel;
                
                var scriptText = "@goto {worldMapLocation}";
                //var scriptText = "@char BlackRose.angry visible:true";
                try { await scriptPlayer.PlayTransient($"scriptGoTo", scriptText, cts.Token); }
                catch (OperationCanceledException) { return; }
                finally
                {
                    if (stateManager != null)
                        stateManager.OnRollbackStarted -= cts.Cancel;
                    cts.Dispose();
                }
            }

            //Debug.Log($"NEXT COMMAND - {scriptPlayer.PlayedIndex}");

            ClearFocus();
            Hide();

            scriptPlayer.SetWaitingForInputEnabled(false);
            scriptPlayer.Play(scriptPlayer.PlayedIndex);
            //scriptPlayer.SetWaitingForInputEnabled(true);
            

            // if (playOnSubmit)
            // {
            //     // Attempt to select and play next command.
            //     Debug.Log($"NEXT COMMAND - {scriptPlayer.PlayedIndex}/{scriptPlayer.Playing}");
            //     scriptPlayer.PlayedCommand.Log("HZ");
            //     var nextIndex = scriptPlayer.PlayedIndex + 1;
            //
            //      
            //     scriptPlayer.Play(nextIndex);
            //
            //     Debug.Log($"NEXT COMMAND - {scriptPlayer.PlayedIndex}/{scriptPlayer.Playing}/{scriptPlayer.PlayedCommand}");
            //     scriptPlayer.PlayedCommand.Log("HZ");
            // }
        }
    }
}
