﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orbit.Entity
{
    public abstract class AOpponentController : AEntityController,
                                                IMovingEntity,
                                                IDropResources
    {
        protected List<Vector3> WayPoints = new List<Vector3>();

        protected int currentWayPoint = 0;

        [SerializeField]
        private float _secondsBeforeRegister = 1.5f;

        #region Members
        public float Speed
        {
            get { return _speed; }
            protected set { _speed = value; }
        }
        [SerializeField, Range(0, 50)]
        protected float _speed = 2;

        public float RotateSpeed
        {
            get { return _rotateSpeed; }
            protected set { _rotateSpeed = value; }
        }
        [SerializeField, Range(0, 100)]
        protected float _rotateSpeed = 20;

        public uint ResourcesToDrop
        {
            get { return _resourcesToDrop; }
            protected set { _resourcesToDrop = value; }
        }
        [SerializeField]
        private uint _resourcesToDrop;
        #endregion

        #region Protected functions
        protected override void UpdateAttackMode()
        {
            // Do not override UpdateBuildMode, because should never have an active opponent when in building mode
            base.UpdateAttackMode();

            if ( currentWayPoint < WayPoints.Count )
            {
                Vector3 target = WayPoints[currentWayPoint];
                Vector3 moveDirection = target - transform.position;
                //move towards waypoint
                if ( moveDirection.magnitude < 1 )
                {
                    currentWayPoint++;
                }
                else
                {
                    Vector3 delta = target - transform.position;
                    delta.Normalize();
                    float moveSpeed = _speed * Time.deltaTime;
                    transform.position = transform.position + ( delta * moveSpeed );
                    //Rotate Towards
                    Vector3 direction = ( target - transform.position ).normalized;
                    transform.up = Vector3.Slerp( transform.up, direction, Time.deltaTime * _rotateSpeed );
                }
            }
            else
            {
                Destroy( gameObject );
            }
        }

        protected override void OnDeath()
        {
            DropResources();

            base.OnDeath();
        }

        protected virtual void OnBecameVisible()
        {
            StartCoroutine( RegisterToOpponentManager() );
        }
        #endregion

        private void OnBecameInvisible()
        {
            Destroy( gameObject );
        }

        public void DropResources()
        {
            GameManager.Instance.ResourcesCount += ResourcesToDrop;
        }

        private IEnumerator RegisterToOpponentManager()
        {
            yield return new WaitForSeconds( _secondsBeforeRegister );

            OpponentManager.Instance.RegisterOpponent( this );
        }
    }
}