﻿using System.Collections;
using UnityEngine;

namespace Orbit.Entity
{
    public abstract class ALivingEntity : ABaseEntity
    {
        #region Events
        public delegate void DelegateTrigger();

        public event DelegateTrigger TriggerDeath;
        public event DelegateTrigger TriggerDestroy;
        public event DelegateTrigger TriggerHit;

        public delegate void DelegateUint( uint value );

        public event DelegateUint HpChanged;
        public event DelegateUint MaxHpChanged;
        #endregion

        #region Music

        [Header("Sounds")]

        [SerializeField]
        private AudioClip _hitClip;

        [SerializeField]
        private AudioClip _deathClip;

        #endregion
        #region Members
        public int Hp
        {
            get { return _healthPoints; }
            protected set
            {
                bool damageTaken = _healthPoints > value;

                _healthPoints = value;

                if ( _healthPoints > MaxHP )
                {
                    _healthPoints = (int)MaxHP;
                    return;
                }

                if ( _healthPoints < 0 )
                    _healthPoints = 0;
                if ( _healthPoints == 0 )
                {
                    if ( TriggerDeath != null )
                        TriggerDeath();
                    return;
                }

                if ( HpChanged != null )
                    HpChanged( (uint)_healthPoints );

                if ( damageTaken && TriggerHit != null )
                    TriggerHit();
            }
        }
        private int _healthPoints = 0;

        public uint MaxHP
        {
            get { return _maxHealthPoints; }
            protected set
            {
                _maxHealthPoints = value;
                if ( MaxHpChanged != null )
                    MaxHpChanged( _maxHealthPoints );
            }
        }

        [Header("HP")]

        [SerializeField]
        private uint _maxHealthPoints = 1;

        private SpriteRenderer _spriteRenderer;

        public SpriteRenderer OwnSpriteRenderer
        {
            get
            {
                if (_spriteRenderer == null)
                    _spriteRenderer = GetComponent<SpriteRenderer>();
                return _spriteRenderer;
            }
        }

        [Header("Visual feedbacks")]

        [SerializeField]
        private ParticleSystem _deathParSysPrefab;

        [SerializeField]
        private Color _hitColor = Color.red;

        [SerializeField]
        private float _hitColorTime = 0.2f;
        #endregion

        #region Public functions
        public void ReceiveHeal( int power )
        {
            Hp += power;
        }

        public void ReceiveDamages( int power )
        {
            Hp -= power;
            PlaySound(_hitClip);
        }
        #endregion

        #region Protected functions
        protected override void Awake()
        {
            base.Awake();

            TriggerDeath += OnDeath;

            Hp = (int)MaxHP;

            if (OwnSpriteRenderer == null)
                Debug.LogError("ALivingEntity.Awake() - Sprite Renderer is null, there's no SpriteRenderer component in object", this);

            HpChanged += HpChangedCallback;
        }

        protected virtual void OnDeath()
        {
            if ( _deathParSysPrefab )
            {
                ParticleSystem particle = Instantiate( _deathParSysPrefab, transform.position, transform.rotation );
                particle.Play();
                Destroy(particle.gameObject, particle.main.duration);
            }
            PlaySound( _deathClip );
            Destroy( gameObject );
        }

        protected override void OnDestroy()
        {
            if ( TriggerDestroy != null )
                TriggerDestroy();

            base.OnDestroy();
        }
        #endregion

        #region Private Functions

        private void HpChangedCallback( uint hp )
        {
            StartCoroutine( ColorCoroutine( hp ) );
        }

        private void ModifyGrey(uint hp)
        {
            OwnSpriteRenderer.color = Color.Lerp(Color.black, Color.white, (float)hp / (float)MaxHP);
        }

        IEnumerator ColorCoroutine(uint hp)
        {
            OwnSpriteRenderer.color = _hitColor;
            yield return  new WaitForSeconds( _hitColorTime );
            ModifyGrey( hp );
        }
        #endregion
    }
}