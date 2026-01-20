using UnityEngine;

namespace ArrowGame.Skins
{
    [CreateAssetMenu(fileName = "NeedleSkin", menuName = "ArrowGame/Needle Skin")]
    public class NeedleSkinDefinition : ScriptableObject
    {
        [Header("Info")]
        public string id;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Unlock")]
        public bool unlockedByDefault;
        public string unlockedByAchievementId;

        [Header("Visuals - Mesh")]
        public Mesh mesh;
        public Material material;

        [Header("Visuals - Particles")]
        public GameObject particlesPrefab;

        [Header("Visuals - Glow")]
        public bool hasGlow;
        public Color glowColor = Color.white;
        public float glowIntensity = 1f;
    }
}
