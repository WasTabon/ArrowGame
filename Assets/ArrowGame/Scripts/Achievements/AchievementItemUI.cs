using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ArrowGame.Achievements
{
    public class AchievementItemUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image progressFill;
        [SerializeField] private GameObject completedBadge;
        [SerializeField] private GameObject skinBadge;
        [SerializeField] private TextMeshProUGUI skinNameText;

        [Header("Colors")]
        [SerializeField] private Color unlockedBgColor = new Color(0.15f, 0.25f, 0.15f);
        [SerializeField] private Color lockedBgColor = new Color(0.12f, 0.12f, 0.12f);
        [SerializeField] private Color unlockedIconColor = Color.white;
        [SerializeField] private Color lockedIconColor = new Color(0.4f, 0.4f, 0.4f);

        public void Setup(AchievementDefinition def)
        {
            bool unlocked = AchievementManager.Instance != null && AchievementManager.Instance.IsUnlocked(def.id);

            if (titleText != null)
                titleText.text = def.title;

            if (descriptionText != null)
                descriptionText.text = def.description;

            if (iconImage != null)
            {
                iconImage.sprite = def.icon;
                iconImage.gameObject.SetActive(def.icon != null);
                iconImage.color = unlocked ? unlockedIconColor : lockedIconColor;
            }

            if (backgroundImage != null)
                backgroundImage.color = unlocked ? unlockedBgColor : lockedBgColor;

            if (completedBadge != null)
                completedBadge.SetActive(unlocked);

            bool hasSkin = !string.IsNullOrEmpty(def.unlocksSkinId);
            if (skinBadge != null)
                skinBadge.SetActive(hasSkin);
            if (skinNameText != null && hasSkin)
                skinNameText.text = def.unlocksSkinId;

            UpdateProgress(def, unlocked);
        }

        private void UpdateProgress(AchievementDefinition def, bool unlocked)
        {
            int current = unlocked ? def.targetValue : (AchievementManager.Instance?.GetProgress(def.id) ?? 0);
            int target = def.targetValue;

            if (progressText != null)
                progressText.text = $"{current}/{target}";

            if (progressFill != null)
            {
                float fill = target > 0 ? Mathf.Clamp01((float)current / target) : 0f;
                progressFill.fillAmount = fill;
            }
        }
    }
}
