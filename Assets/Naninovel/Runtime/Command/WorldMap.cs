// Copyright 2023 ReWaffle LLC. All rights reserved.

using UnityEngine;

namespace Naninovel.Commands
{
    /// <summary>
    /// Shows an input field UI where user can enter an arbitrary text.
    /// Upon submit the entered text will be assigned to the specified custom variable.
    /// </summary>
    /// <remarks>
    /// Check out this [video guide](https://youtu.be/F9meuMzvGJw) on usage example.
    /// <br/><br/>
    /// To assign a display name for a character using this command consider [binding the name to a custom variable](/guide/characters.html#display-names).
    /// </remarks>
    [CommandAlias("worldMap")]
    public class WorldMap : Command, Command.ILocalizable
    {
        /// <summary>
        /// Name of a custom variable to which the entered text will be assigned.
        /// </summary>
        [ParameterAlias(NamelessParameterAlias), RequiredParameter]
        public StringParameter VariableName;
        /// <summary>
        /// An optional summary text to show along with input field.
        /// When the text contain spaces, wrap it in double quotes (`"`). 
        /// In case you wish to include the double quotes in the text itself, escape them.
        /// </summary>
        public LocalizableTextParameter Summary;
        /// <summary>
        /// A predefined value to set for the input field.
        /// </summary>
        [ParameterAlias("value")]
        public LocalizableTextParameter PredefinedValue;
        /// <summary>
        /// Whether to automatically resume script playback when user submits the input form.
        /// </summary>
        [ParameterAlias("play"), ParameterDefaultValue("true")]
        public BooleanParameter PlayOnSubmit = true;

        [ParameterAlias("isInteractableLocation1"), ParameterDefaultValue("true")]
        public BooleanParameter IsInteractableLocation1 = true;
        [ParameterAlias("isInteractableLocation2"), ParameterDefaultValue("true")]
        public BooleanParameter IsInteractableLocation2 = true;
        [ParameterAlias("isInteractableLocation3"), ParameterDefaultValue("true")]
        public BooleanParameter IsInteractableLocation3 = true;
        
        
        public override UniTask ExecuteAsync (AsyncToken asyncToken = default)
        {
            var inputUI = Engine.GetService<IUIManager>().GetUI<UI.IWorldMapUI>();
            inputUI?.Show(VariableName, Summary, PredefinedValue, PlayOnSubmit,
                IsInteractableLocation1,
                IsInteractableLocation2,
                IsInteractableLocation3
                );

            return UniTask.CompletedTask;
        }
    }
}
