using System;
using Frame;
using UnityEngine;

namespace UI
{
    public class Title : MonoBehaviour
    {
        private Animator _animator;

        public GameObject seekRoadCanvas;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            EventCenter.AddListener(GameEvent.TitleFadeEvent, OnTitleFade);
        }

        private void OnDisable()
        {
            EventCenter.RemoveListener(GameEvent.TitleFadeEvent, OnTitleFade);
        }

        private void OnTitleFade()
        {
            _animator.SetTrigger(Settings.AnimatorParameters.ON_FADE);
        }

        public void AfterTitle()
        {
            EventCenter.Broadcast(GameEvent.AfterTitleEvent);
            seekRoadCanvas.SetActive(true);
        }
        
        public void AfterAnimation()
        {
            gameObject.SetActive(false);
        }
    }
}
