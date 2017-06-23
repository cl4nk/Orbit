﻿using UnityEngine;
using UnityEngine.Events;

namespace Orbit.Entity
{
    public abstract class AUnitController : AEntityController
    {
        #region Static members
        public static UnityEvent DmgTakenEvent = new UnityEvent();
        #endregion

        #region Members
        public uint Price
        {
            get { return _price; }
        }
        [SerializeField]
        private uint _price;
        
        public uint CurrentPrice
        {
            get { return (uint)( Hp / MaxHP * Price ); }
        }

        [SerializeField]
        protected GameObject Head;

        protected bool FollowMouse = true;

        public uint Level
        {
            get { return _level; }
            protected set { _level = value; }
        }
        [SerializeField]
        private uint _level = 1;

        public bool IsSelected
        {
            get { return _bIsSelected; }
            protected set { _bIsSelected = value; }
        }
        private bool _bIsSelected = false;

        public GameCell Cell
        {
            get { if ( _gameCell == null)
                    _gameCell = GetComponent<GameCell>();
                return _gameCell; }
            protected set { _gameCell = value; }
        }
        private GameCell _gameCell = null;

        [SerializeField]
        private ParticleSystem _awakeParSysPrefab;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        public Sprite Icon
        {
            get { return _icon; }
        }
        [SerializeField]
        private Sprite _icon;
        #endregion

        #region Public functions
        public abstract void ExecuteOnClick( Vector3 target );
        #endregion

        #region Protected functions
        protected override void Start()
        {
            base.Start();

            if ( Cell == null )
                Debug.LogError( "AUnitController.Awake() - Cell is null, there's no GameCell component in object", this );

            _spriteRenderer = GetComponent<SpriteRenderer>();
            if ( _spriteRenderer == null )
                Debug.LogError( "AUnitController.Awake() - Sprite Renderer is null, there's no SpriteRenderer component in object", this );

            if ( _awakeParSysPrefab )
            {
                ParticleSystem particle = Instantiate( _awakeParSysPrefab, transform );
                particle.Play();
            }

            HpChanged += ModifyGrey;
            TriggerHit += DmgTakenEvent.Invoke;
            Cell.OnSelection += ModifySelected;
            Cell.OnActionLaunched += ExecuteOnClick;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if ( Cell )
            {
                Cell.OnSelection -= ModifySelected;
                Cell.OnActionLaunched -= ExecuteOnClick;
            }
        }

        protected override void UpdateAttackMode()
        {
            base.UpdateAttackMode();

            if ( IsSelected && FollowMouse && Head )
            {
                Vector3 target =
                    Camera.main.ScreenToWorldPoint( new Vector3( Input.mousePosition.x, Input.mousePosition.y,
                                                                 -Camera.main.transform.position.z ) );
                Head.transform.right = ( target - transform.position ).normalized;
            }
        }

        protected virtual void ModifySelected( bool selected )
        {
            IsSelected = selected;
        }
        #endregion

        #region Private functions
        private void ModifyGrey( uint hp )
        {
            _spriteRenderer.color = Color.Lerp( Color.black, Color.white, (float)hp / (float)MaxHP );
        }
        #endregion
    }
}