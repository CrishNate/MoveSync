using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using Random = UnityEngine.Random;

namespace MoveSync
{
    public class PlayerBehaviour : Singleton<PlayerBehaviour>
    {
        [SerializeField] private GameObject _fullSizeCollider;
        [SerializeField] private XRRig _xrRig;
        
        public UnityEvent<int> onHit;
            
        private float _invincibilityTimeStamp;
        private static readonly float _invincibilityTime = 2.0f;
        private static readonly int _maxHealth = 3;
        private int _health;

        private void Start()
        {
            Restart();
        }

        void Update()
        {
            if (_invincibilityTimeStamp > 0 && Time.time > _invincibilityTimeStamp)
            {
                _invincibilityTimeStamp = 0.0f;
                
                OnRestore();
            }

            if (_fullSizeCollider)
            {
                _fullSizeCollider.transform.rotation = Quaternion.identity;
            }
        }

        private void OnRestore()
        {
            LowerSound.instance.BeginHigher();
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("DamageObjects"))
            {
                OnHit(collider.gameObject);
            }
        }

        public void OnHit(GameObject instigator)
        {
            if (_invincibilityTimeStamp > Time.time) 
                return;
            
            onHit.Invoke(--_health);

            if (_health == 0)
            {
                Death();
                return;
            }

            if (_health == 1)
            {
                StartCoroutine(Regen());
            }

            _invincibilityTimeStamp = Time.time + _invincibilityTime;
            
            LowerSound.instance.BeginLower();
        }

        IEnumerator Regen()
        {
            yield return new WaitForSeconds(5f);
            _health += 1;
        }
        
        static Vector3 GetPlayerPositionScale(float height)
        {
            float y = instance._xrRig.cameraFloorOffsetObject.transform.position.y;
            Vector3 pos = instance.transform.position;
            return new Vector3(pos.x, y, pos.z + height);
        }
        
        public static Vector3 GetPlayerPositionHalfScale()
        {
            return GetPlayerPositionScale(instance._xrRig.cameraYOffset * 0.5f);
        }
        
        public static Vector3 GetPlayerPositionFullScale()
        {
            return GetPlayerPositionScale(instance._xrRig.cameraYOffset);
        }
        
        public static Vector3 GetRandomPointNearPlayer()
        {
            return instance.transform.position + Random.insideUnitSphere * 0.2f;
        }

        void Death()
        {
            LevelSequencer.instance.BeginRestart();
        }

        public void Restart()
        {
            _health = _maxHealth;
        }

        public int health => _health;
    }
}