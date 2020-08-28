namespace MoveSync
{
    public class BeatShootDuration : BeatShoot
    {
        protected override float GetDestroyTime()
        {
            return beatObjectData.time + duration + projectile.GetDisappearTime();
        }
    }
}