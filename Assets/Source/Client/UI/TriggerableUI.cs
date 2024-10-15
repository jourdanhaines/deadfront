using System;
using Deadfront.Shared.Events;
using UnityEngine;

namespace Deadfront.Client.UI
{
    /// <summary>
    /// Base class for UI elements that can be triggered by events
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class TriggerableUI : MonoBehaviour, IViewable
    {
        [SerializeField]
        protected bool closeOthers = true; 
            
        [SerializeField] protected Canvas canvas;
        
        protected CanvasGroup canvasGroup;

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GetComponentInParent<Canvas>();

            if (canvas == null)
            {
                throw new MissingComponentException("Trigger UI cannot find canvas in parent component");
            }

            Hide();
        }
        
        /// <summary>
        /// Show the UI element
        /// </summary>
        public virtual void Show()
        {
            if (closeOthers)
            {
                UIManager.Instance.HideAllTriggerableUI();
            }
            
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        
        /// <summary>
        /// Hide the UI element
        /// </summary>
        public virtual void Hide()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        /// <summary>
        /// Check if the UI element can block raycasts
        /// </summary>
        /// <returns>True if the UI element can block raycasts, false otherwise</returns>
        public virtual bool IsBlocking()
        {
            return canvasGroup.interactable;
        }
    }
    
    /// <summary>
    /// Base class for UI elements that can be triggered by events
    ///
    /// Subscribes to the game event on enable and forwards the event data to the inheriting class.
    ///
    /// To use this class, inherit from it and supply the `GameEvent` you want to react to, and implement the `OnEventTriggered` method.
    /// </summary>
    /// <typeparam name="T">The type of event data that the UI element will react to</typeparam>
    public abstract class TriggerableUI<T> : TriggerableUI where T : GameEvent<T>
    {
        /// <summary>
        /// Called after the event is raised, ensuring the UI element has confirmed the event data.
        ///
        /// Must be overridden to allow the UI element to react to specific event data.
        /// </summary>
        /// <param name="e">The event data that was raised</param>
        protected abstract void OnEventTriggered(T e);
        
        /// <summary>
        /// Called when the event is first triggered by the event system.
        ///
        /// Can be overridden to allow the UI element to react to specific event data, before acting on it.
        /// </summary>
        /// <param name="e">The event data that was raised</param>
        protected virtual void OnEventRaised(T e)
        {
            Show();
            
            OnEventTriggered(e);
        }
        
        /// <summary>
        /// Called after the event is complete and the UI element has finished reacting to the event data.
        ///
        /// Must be called from the inheriting class to ensure the UI element is hidden after the event is complete.
        ///
        /// Can be overridden to add additional functionality after the event is complete.
        /// </summary>
        protected virtual void OnEventComplete()
        {
            Hide();
        }

        /// <summary>
        /// Subscribes to the event when the UI element is enabled.
        /// </summary>
        private void OnEnable()
        {
            GameEvent<T>.Subscribe(OnEventRaised);
        }

        /// <summary>
        /// Unsubscribes from the event when the UI element is disabled.
        /// </summary>
        private void OnDisable()
        {
            GameEvent<T>.Unsubscribe(OnEventRaised);
        }
    }
}