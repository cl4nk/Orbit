﻿using System.Collections;
using UnityEngine;

namespace Orbit.Entity.Unit
{
    public class Reparator : AUnitController,
                             IRepairingEntity
    {
        #region Members
        protected DelegateTrigger OnRepairedUnitDeath
        {
            get { return _onRepairedUnitDeath; }
            set { _onRepairedUnitDeath = value; }
        }
        private DelegateTrigger _onRepairedUnitDeath;

        protected AUnitController RepairedUnit
        {
            get { return _repairedUnit; }
            set { _repairedUnit = value; }
        }
        private AUnitController _repairedUnit;

        protected ParticleSystem ReparatorParticles
        {
            get { return _reparatorParticles; }
            set { _reparatorParticles = value; }
        }

        [SerializeField]
        private ParticleSystem _reparatorParticles;

        public float RepairSpeed
        {
            get { return _repairSpeed; }
            protected set { _repairSpeed = value; }
        }
        [SerializeField]
        private float _repairSpeed;

        public bool CanRepair
        {
            get { return _canRepair; }
            protected set { _canRepair = value; }
        }
        private bool _canRepair;

        public float RepairTimer
        {
            get { return _repairTimer; }
            set
            {
                _repairTimer = value;

                if ( _repairTimer >= _repairSpeed )
                {
                    _repairTimer = 0.0f;
                    _canRepair = true;
                }
            }
        }
        private float _repairTimer;
        #endregion

        #region Protected functions
        protected override void Awake()
        {
            base.Awake();

            OnRepairedUnitDeath = () =>
            {
                RepairedUnit.TriggerDeath -= OnRepairedUnitDeath;
                RepairedUnit = null;
                FollowMouse = true;
            };

            ReparatorParticles = GetComponentInChildren<ParticleSystem>();
            if ( ReparatorParticles == null )
                Debug.LogWarning( "Reparator.Awake() - could not find ParticleSystem in children" );

            GameManager.Instance.OnAttackMode.AddListener( ResetCooldown );
        }

        protected override void UpdateAttackMode()
        {
            base.UpdateAttackMode();

            if ( CanRepair )
                Repair();
            else
                RepairTimer += Time.deltaTime;
        }

        protected override void OnDestroy()
        {
            if ( RepairedUnit )
                OnRepairedUnitDeath.Invoke();

			if (GameManager.Instance) 
			{
				GameManager.Instance.OnAttackMode.RemoveListener (ResetCooldown);
			}
            base.OnDestroy();
        }
        #endregion

        public override void ExecuteOnClick( Vector3 target )
        {
            GameCell targetCell = GameGrid.Instance.GetCellFromWorldPoint( target );

            if ( Cell.IsConnectedTo( targetCell ) )
            {
                if ( RepairedUnit == targetCell.Unit )
                    return;

                if ( RepairedUnit != null )
                    RepairedUnit.TriggerDeath -= OnRepairedUnitDeath;

                RepairedUnit = targetCell.Unit;
                RepairedUnit.TriggerDeath += OnRepairedUnitDeath;

                Cell.Selected = false;
            }
        }

        protected override void ModifySelected( bool selected )
        {
            base.ModifySelected( selected );

            if ( selected )
                FollowMouse = true;
            else if ( RepairedUnit != null )
            {
                FollowMouse = false;
                if ( Head )
                    Head.transform.right = ( RepairedUnit.transform.position - transform.position ).normalized;
            }
        }

        public void Repair()
        {
            if ( RepairedUnit == null || !CanRepair )
                return;

            RepairedUnit.ReceiveHeal( ( int )Power );
            if ( ReparatorParticles != null )
                ReparatorParticles.Play();

            CanRepair = false;
        }

        private void ResetCooldown()
        {
            CanRepair = false;
            RepairTimer = 0.0f;
        }
    }
}