namespace MoveSync
{
    public class BeatShootDuration : BeatShootPlayer
    {
        protected override float GetDestroyTime()
        {
            return beatObjectData.time + duration + projectile.GetDisappearTime();
        }
    }
}