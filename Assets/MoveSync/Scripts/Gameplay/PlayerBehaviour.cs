using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MoveSync
{
    public class PlayerBehaviour : MonoBehaviour
    {
        [HideInInspector] public static PlayerBehaviour instance = null;

        [Header("Events")] [SerializeField] private UnityEvent _onDeath;
        [SerializeField] private UnityEvent _onHit;
        [SerializeField] private UnityEvent _onInvincibilityEnd;

        [Header("Health")] public int maxHealth;
        [HideInInspector] public int health;

        [SerializeField] private float _invincibilityTime;

        private float _invincibilityTimeStamp;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance == this)
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            health = maxHealth;
        }

        void Update()
        {
            if (_invincibilityTimeStamp > 0
                && _invincibilityTimeStamp < Time.time)
            {
                _invincibilityTimeStamp = 0.0f;
                _onInvincibilityEnd.Invoke();
            }
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
            if (health == 0) return;
            if (_invincibilityTimeStamp > Time.time) return;

            health--;

            _invincibilityTimeStamp = Time.time + _invincibilityTime;

            if (health == 0)
            {
                Death();
            }

            _onHit.Invoke();
        }

        void Death()
        {
            LevelSequencer.instance.Restart();

            _onDeath.Invoke();
        }

        public void Restart()
        {
            health = maxHealth;
        }

        public bool isMaxHealth => health == maxHealth;
    }
}