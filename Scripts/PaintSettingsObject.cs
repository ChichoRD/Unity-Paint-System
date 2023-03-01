using UnityEngine;

namespace PaintSystem
{
    [CreateAssetMenu(fileName = "New Paint Settings Object", menuName = "Scriptable Objects/Paint Settings Object")]
    public class PaintSettingsObject : ScriptableObject
    {
        [field: SerializeField][field: Min(0)] public float BrushRadius { get; private set; } = 1f;
        [field: SerializeField][field: Range(0f, 1f)] public float BrushHardness { get; private set; } = 0.5f;
        [field: SerializeField][field: Range(0f, 1f)] public float BrushStrength { get; private set; } = 0.5f;
        [field: SerializeField] public Color PaintColor { get; private set; } = Color.white;
        [field: SerializeField][field: Range(0f, 1f)] public float PaintMetallic { get; private set; } = 0.5f;
        [field: SerializeField][field: Range(0f, 1f)] public float PaintSmoothness { get; private set; } = 0.5f;
        [field: SerializeField] public Texture2D PaintTexture { get; private set; } = null;
        [field: SerializeField] public Vector3 PaintTextureRotation { get; private set; } = Vector3.zero;
        [field: SerializeField] public Vector2 PaintTextureScale { get; private set; } = Vector2.one;
        [field: SerializeField] public Vector2 PaintTextureOffset { get; private set; } = Vector2.zero;
    }
}
