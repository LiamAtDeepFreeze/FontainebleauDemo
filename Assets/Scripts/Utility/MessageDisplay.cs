using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utility
{
    public class MessageDisplay : MonoBehaviour
    {
        private enum State
        {
            Idle,
            FadeIn,
            Active,
            FadeOut
        }
        
        public TextMeshProUGUI messageDisplay;
        public TextMeshProUGUI subtitleDisplay;
        public CanvasGroup canvasGroup;
        public RectTransform rectTransform;

        public float fadeInTime = 0.5f;

        private float _hideTime;
        private State _state = State.Idle;

        private Message _activeMessage;
        private Queue<Message> _messages = new Queue<Message>();

        private void Start()
        {
            canvasGroup.alpha = 0;
        }

        public void ShowMessage(string message, string submessage = null, float fadeTime = 5)
        {
            var newMessage = new Message(message, submessage, fadeTime);
            if (_state != State.Idle)
            {
                _messages.Enqueue(newMessage);
            }
            
            DisplayMessage(newMessage);
        }

        private void DisplayMessage(Message message)
        {
            _activeMessage = message;
            messageDisplay.SetText(message.message);
            if (string.IsNullOrWhiteSpace(message.submessage))
            {
                subtitleDisplay.gameObject.SetActive(false);
            }
            else
            {
                subtitleDisplay.gameObject.SetActive(true);
                subtitleDisplay.SetText(message.submessage);
            }
            
            _hideTime = Time.time + message.shownTime + fadeInTime;
            StartCoroutine(Fade(1f));
        }

        private void Update()
        {
            if (_state == State.Active)
            {
                if (Time.time > _hideTime)
                {
                    StartCoroutine(Fade(0f));
                }
            }
        }

        private IEnumerator Fade(float target)
        {
            _state = target > 0 ? State.FadeIn : State.FadeOut;
            var start = canvasGroup.alpha;
            var startTime = Time.time;
            
            var multiplier = start;
            if (_state == State.FadeIn)
            {
                while (multiplier < target)
                {
                    multiplier = Mathf.SmoothStep(start, target, (Time.time - startTime) / fadeInTime);
                    canvasGroup.alpha = multiplier;
                    rectTransform.localScale = Vector3.one * multiplier;
                    
                    yield return null;
                }
            }
            else
            {
                while (multiplier > target)
                {
                    multiplier = Mathf.SmoothStep(start, target, (Time.time - startTime) / fadeInTime);
                    canvasGroup.alpha = multiplier;
                    rectTransform.localScale = Vector3.one * multiplier;
                    
                    yield return null;
                }
            }
            

            if (target > 0f)
            {
                _state = State.Active;
            }
            else
            {
                _state = State.Idle;
                _activeMessage = null;
                if (_messages.Count > 0)
                {
                    DisplayMessage(_messages.Dequeue());
                }
            }
        }

        [Serializable]
        private class Message
        {
            public string message;
            public string submessage;
            public float shownTime;

            public Message(string message, string submessage, float shownTime = 5f)
            {
                this.message = message;
                this.submessage = submessage;
                this.shownTime = shownTime;
            }
        }
    }
}