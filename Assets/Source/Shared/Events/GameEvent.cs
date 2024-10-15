using System;
using UnityEngine;

namespace Deadfront.Shared.Events
{
    /// <summary>
    ///     Base class for a game event
    ///     Game events can be invoked from anywhere in the codebase, and can be subscribed to by any class
    ///     This allows for a decoupled way of communicating between different parts of the codebase
    /// </summary>
    /// <typeparam name="T">The type of the event, uses the curiously recurring template pattern</typeparam>
    [Serializable]
    public abstract class GameEvent<T> : IGameEvent where T : class
    {
        /// <summary>
        ///     Helper method to raise the event from an instance of the event
        /// </summary>
        public virtual void Raise()
        {
            Broadcast(this as T);
        }

        /// <summary>
        ///     Internal event that is invoked when the event is broadcast
        /// </summary>
        private static event Action<T> OnEvent;

        /// <summary>
        ///     Subscribe to the game event and run the given action/method when the event is broadcast
        /// </summary>
        /// <param name="action">The action/method to run when the event is broadcast</param>
        public static void Subscribe(Action<T> action)
        {
            Debug.Log($"Subscribing to event of type {typeof(T).Name}");
            
            OnEvent += action;
        }

        /// <summary>
        ///     Unsubscribe from the game event, and clean up the given action/method
        /// </summary>
        /// <param name="action">The action/method to clean up</param>
        public static void Unsubscribe(Action<T> action)
        {
            Debug.Log($"Unsubscribing from event of type {typeof(T).Name}");
            
            OnEvent -= action;
        }

        /// <summary>
        ///     Broadcast the game event to all subscribers
        /// </summary>
        /// <param name="data">The data to broadcast with the event</param>
        public static void Broadcast(T data)
        {
            Debug.Log($"Broadcasting event of type {typeof(T).Name}");
            
            OnEvent?.Invoke(data);
        }
      
        protected GameEvent()
        {
            Debug.Log($"Created event of type {GetType().Name}");
        }
    }
}