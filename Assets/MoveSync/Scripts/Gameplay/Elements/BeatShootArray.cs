using System.Collections;
using MoveSync.ModelData;
using UnityEngine;

namespace MoveSync
{
    public class BeatShootArray : BeatShootBullet
    {
        [SerializeField] private bool _asBullet;
        private static readonly float TimeBetweenShootsBPM = 0.1f;
        
        public override void Init(BeatObjectData beatObjectData)
        {
            shootOnAwake = false;
            
            base.Init(beatObjectData);

            StartCoroutine(BeginShootArray());
        }

        IEnumerator BeginShootArray()
        {
            for (int i = 0; i < count; i++)
            {
                Shoot(transform.position + transform.up * ((i - (count - 1) / 2.0f) * size * 1.5f), transform.rotation,
                    LevelSequencer.instance.timeBPM + appear);

                yield return new WaitForSeconds(LevelSequencer.instance.toTime * TimeBetweenShootsBPM);
            }
        }
        
        protected override float GetDestroyTime()
        {
            return beatObjectData.time + count * TimeBetweenShootsBPM;
        }

        protected override Quaternion GetRotationByTargetState()
        {
            if (_asBullet)
            {
                Quaternion rotation = beatObjectData.getModel<ROTATION>().value;
                Quaternion unrolledRotation = Quaternion.LookRotation(rotation * Vector3.forward);
                Quaternion rolledLocalRotation = Quaternion.Inverse(rotation) * unrolledRotation;

                Quaternion rotateTo = base.GetRotationByTargetState();
                rotateTo = Quaternion.AngleAxis(-rolledLocalRotation.eulerAngles.z, rotateTo * Vector3.forward) * rotateTo;

                return rotateTo;
            }
            
            return beatObjectData.getModel<ROTATION>().value;
        }

        protected override Vector3 GetSpawnPosition()
        {
            if (_asBullet)
            {
                return base.GetSpawnPosition();
            }
            
            return position;
        }
    }
}