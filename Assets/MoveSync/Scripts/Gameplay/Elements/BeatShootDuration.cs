namespace MoveSync
{
    public class BeatShootDuration : BeatShoot
    {
        protected override float GetDestroyTime()
        {
            return beatObjectData.time + projectile.Duration + projectile.GetDisappearTime();
        }
    }
}