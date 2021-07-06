using UnityEngine;

namespace MoveSync.Data
{
    [CreateAssetMenu(fileName = "ColorData", menuName = "MoveSync/ColorData", order = 0)]
    public class ColorData : ScriptableObject
    {
        [Header("UI")]
        [SerializeField] public Color DefaultUIBeatObject;
        [SerializeField] public Color SelectedUIBeatObject;
        [SerializeField] public Color SelectedPropertiesUIBeatObject;
    }
}