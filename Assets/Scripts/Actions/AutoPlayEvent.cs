using System;
using UnityEngine;
using UnityEngine.Events;

namespace Actions
{
    public class AutoPlayEvent : MonoBehaviour
    {
        public UnityEvent[] events;

        private void Start()
        {
            foreach (var unityEvent in events)
            {
                unityEvent.Invoke();
            }
        }
    }
}