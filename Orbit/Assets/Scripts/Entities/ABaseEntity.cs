﻿using UnityEngine;

namespace Orbit.Entity
{
    public class ABaseEntity : MonoBehaviour
    {
        #region  Sound
        private AudioSource _source;
        public AudioSource Source
        {
            get
            {
                if ( _source == null )
                    _source = GetComponent<AudioSource>();
                if ( _source == null )
                    _source = gameObject.AddComponent<AudioSource>();
                return _source;
            }
        }

        protected void PlaySound( AudioClip clip )
        {
            if ( clip )
            {
                Source.clip = clip;
                Source.Play();
            }
        }
        #endregion

        #region Events
        protected delegate void DelegateUpdate();

        protected event DelegateUpdate OnUpdate = () => { };

        public delegate void DelegateTrigger();

        public event DelegateTrigger TriggerDestroy;
        #endregion

        #region Protected functions
        protected virtual void Awake()
        {
            GameManager.Instance.OnAttackMode.AddListener( OnAttackMode );
            GameManager.Instance.OnBuildMode.AddListener( OnBuildMode );
            GameManager.Instance.OnPause.AddListener( OnPause );
            GameManager.Instance.OnPlay.AddListener( OnPlay );
        }

        protected virtual void Start()
        {
            if ( GameManager.Instance.CurrentGameState == GameManager.GameState.Play )
                OnPlay();
            Source.volume = MusicManager.Instance.SoundVolume;
        }

        protected virtual void OnDestroy()
        {
            if ( TriggerDestroy != null )
                TriggerDestroy();

            if ( GameManager.Instance )
            {
                GameManager.Instance.OnAttackMode.RemoveListener( OnAttackMode );
                GameManager.Instance.OnBuildMode.RemoveListener( OnBuildMode );
                GameManager.Instance.OnPause.RemoveListener( OnPause );
                GameManager.Instance.OnPlay.RemoveListener( OnPlay );
            }
        }

        protected void Update()
        {
            OnUpdate();
        }

        protected virtual void UpdateAttackMode()
        {
            // Update Attack Mode
        }

        protected virtual void UpdateBuildMode()
        {
            // Update Build Mode
        }

        #region Private functions
        private void OnAttackMode()
        {
            OnUpdate = UpdateAttackMode;
        }

        private void OnBuildMode()
        {
            OnUpdate = UpdateBuildMode;
        }

        private void OnPause()
        {
            OnUpdate = () => { };
        }

        private void OnPlay()
        {
            if ( GameManager.Instance.CurrentGameMode == GameManager.GameMode.Attacking )
                OnAttackMode();
            else if ( GameManager.Instance.CurrentGameMode == GameManager.GameMode.Building )
                OnBuildMode();
            else
                Debug.LogWarning( "ABaseEntity.OnPlay() - CurrentGameMode is None." );
        }
        #endregion
        #endregion
    }
}