using UnityEngine;

namespace MoveSync.Data
{
    [CreateAssetMenu(fileName = "ColorData", menuName = "MoveSync", order = 0)]
    public class ColorData : ScriptableObject
    {
        [SerializeField] public Color FarBeatObjectColor;
        [SerializeField] public Color NearBeatObjectColor;
    }
}