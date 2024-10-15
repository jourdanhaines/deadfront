using System.Collections.Generic;
using UnityEngine;

namespace Deadfront.Client.UI
{
    public class UIManager : Singleton<UIManager>
    {
        private List<TriggerableUI> uiList = new();
        
        private void Awake()
        {
            var triggerableUis = FindObjectsByType<TriggerableUI>(FindObjectsSortMode.None);

            Debug.Log($"Registering {triggerableUis.Length} UI Listeners");
            
            foreach (var ui in triggerableUis)
            {
                Debug.Log($"Adding UI {ui.GetType().Name}");
                uiList.Add(ui);
            }
        }
        
        public void HideAllTriggerableUI()
        {
            foreach (var ui in uiList)
            {
                ui.Hide();
            }
        }
    }
}