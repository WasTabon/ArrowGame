using UnityEngine;

namespace ArrowGame.Skins
{
    public class NeedleSkinApplier : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Transform particlesParent;

        private GameObject currentParticles;
        private NeedleSkinDefinition appliedSkin;

        private void Start()
        {
            if (SkinManager.Instance != null)
            {
                SkinManager.Instance.OnSkinChanged += ApplySkin;

                if (SkinManager.Instance.CurrentSkin != null)
                {
                    ApplySkin(SkinManager.Instance.CurrentSkin);
                }
            }
        }

        private void OnEnable()
        {
            if (SkinManager.Instance != null)
            {
                SkinManager.Instance.OnSkinChanged += ApplySkin;
            }
        }

        private void OnDisable()
        {
            if (SkinManager.Instance != null)
            {
                SkinManager.Instance.OnSkinChanged -= ApplySkin;
            }
        }

        public void ApplySkin(NeedleSkinDefinition skin)
        {
            if (skin == null) return;

            appliedSkin = skin;

            ApplyMesh(skin);
            ApplyMaterial(skin);
            ApplyParticles(skin);
        }

        private void ApplyMesh(NeedleSkinDefinition skin)
        {
            if (meshFilter != null && skin.mesh != null)
            {
                meshFilter.mesh = skin.mesh;
            }
        }

        private void ApplyMaterial(NeedleSkinDefinition skin)
        {
            if (meshRenderer != null && skin.material != null)
            {
                meshRenderer.material = skin.material;
            }
        }

        private void ApplyParticles(NeedleSkinDefinition skin)
        {
            if (currentParticles != null)
            {
                Destroy(currentParticles);
                currentParticles = null;
            }

            if (skin.particlesPrefab != null && particlesParent != null)
            {
                currentParticles = Instantiate(skin.particlesPrefab, particlesParent);
                currentParticles.transform.localPosition = Vector3.zero;
                currentParticles.transform.localRotation = Quaternion.identity;
            }
        }

        public void RefreshSkin()
        {
            if (SkinManager.Instance != null && SkinManager.Instance.CurrentSkin != null)
            {
                ApplySkin(SkinManager.Instance.CurrentSkin);
            }
        }
    }
}
