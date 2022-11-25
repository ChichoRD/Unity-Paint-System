using UnityEngine;

namespace PaintSystem
{
    [CreateAssetMenu(fileName = "New Paint Settings Object", menuName = "Scriptable Objects/Paint Settings Object")]
    public class PaintSettingsObject : ScriptableObject
    {
        [field: SerializeField][field: Min(0)] public float Radius { get; private set; } = 1f;
        [field: SerializeField][field: Range(0f, 1f)] public float Hardness { get; private set; } = 0.5f;
        [field: SerializeField][field: Range(0f, 1f)] public float Strength { get; private set; } = 0.5f;
        [field: SerializeField] public Color Color { get; private set; } = Color.white;
    }
}
