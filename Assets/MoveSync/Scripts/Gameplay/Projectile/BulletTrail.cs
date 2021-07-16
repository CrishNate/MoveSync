using System;
using System.Collections;
using MoveSync.ModelData;
using UnityEngine;
using UnityEngine.Serialization;

namespace MoveSync
{
    public class BulletTrail : BulletProjectile
    {
        [FormerlySerializedAs("_animator")]
        [Header("Bullet Trail")]
        [SerializeField] private Animator _animatorTrail;

        private static readonly int Appear = Animator.StringToHash("appear");
        private static readonly int Duration = Animator.StringToHash("duration");
        private static readonly float AppearSpeed = 5f;

        private static readonly int Count = 4;
        
        public override void Init(ProjectileParam initParam)
        {
            base.Init(initParam);
            
            transform.localScale *= initParam.size;
            
            // perfect sync
            _animatorTrail.speed = LevelSequencer.instance.toBPM;
            _animatorTrail.SetFloat(Duration, 1.0f / Count);
            _animatorTrail.gameObject.SetActive(false);
            
            StartCoroutine(AddTrail());
        }
        
        IEnumerator AddTrail()
        {
            // Sync with rythm
            yield return new WaitForSeconds((1f - 1f / AppearSpeed) * LevelSequencer.instance.toTime);
            
            while (dDisappear < 0)
            {
                var transformInst = new GameObject("TrailInstance");
                transformInst.transform.position = transform.position;
                
                Animator animTrailInst = Instantiate<Animator>(_animatorTrail, transformInst.transform);
                animTrailInst.gameObject.SetActive(true);
                animTrailInst.speed = LevelSequencer.instance.toBPM;
                animTrailInst.SetFloat(Duration, 1.0f / Count);
                animTrailInst.SetFloat(Appear, AppearSpeed);

                animTrailInst.gameObject.transform.parent = transformInst.transform;
                transformInst.transform.localScale = transform.localScale;

                Destroy(transformInst, LevelSequencer.instance.toTime * Count);
                
                yield return new WaitForSeconds(LevelSequencer.instance.toTime);
            }
        }
    }
}