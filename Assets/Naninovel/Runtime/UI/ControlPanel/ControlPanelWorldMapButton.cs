// Copyright 2023 ReWaffle LLC. All rights reserved.


namespace Naninovel.UI
{
    public class ControlPanelWorldMapButton : ScriptableLabeledButton
    {
        private IUIManager uiManager;

        protected override void Awake ()
        {
            base.Awake();

            uiManager = Engine.GetService<IUIManager>();
        }

        protected override void Start ()
        {
            base.Start();

            var worldMapUI = uiManager.GetUI<IWorldMapUI>();
            if (worldMapUI is null)
                gameObject.SetActive(false);
        }

        protected override void OnButtonClick ()
        {
            uiManager.GetUI<IPauseUI>()?.Hide();
            var worldMapUI = uiManager.GetUI<IWorldMapUI>();
            ((WorldMapPanel)worldMapUI).TakeOffAll();
            worldMapUI?.Show();
        }
    } 
}
