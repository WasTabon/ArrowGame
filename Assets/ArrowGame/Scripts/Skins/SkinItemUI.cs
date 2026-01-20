using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace ArrowGame.Skins
{
    public class SkinItemUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private GameObject selectedBadge;
        [SerializeField] private GameObject lockedOverlay;
        [SerializeField] private TextMeshProUGUI unlockHintText;
        [SerializeField] private Button selectButton;

        [Header("Colors")]
        [SerializeField] private Color selectedColor = new Color(0.2f, 0.35f, 0.2f);
        [SerializeField] private Color unlockedColor = new Color(0.15f, 0.15f, 0.15f);
        [SerializeField] private Color lockedColor = new Color(0.1f, 0.1f, 0.1f);

        private NeedleSkinDefinition skinData;
        private Action<NeedleSkinDefinition> onSelectCallback;

        private void Awake()
        {
            if (selectButton != null)
            {
                selectButton.onClick.AddListener(OnClick);
            }
        }

        public void Setup(NeedleSkinDefinition skin, bool isSelected, bool isUnlocked, Action<NeedleSkinDefinition> onSelect)
        {
            skinData = skin;
            onSelectCallback = onSelect;

            if (nameText != null)
                nameText.text = skin.displayName;

            if (descriptionText != null)
                descriptionText.text = skin.description;

            if (iconImage != null)
            {
                iconImage.sprite = skin.icon;
                iconImage.gameObject.SetActive(skin.icon != null);
                iconImage.color = isUnlocked ? Color.white : new Color(0.3f, 0.3f, 0.3f);
            }

            if (selectedBadge != null)
                selectedBadge.SetActive(isSelected);

            if (lockedOverlay != null)
                lockedOverlay.SetActive(!isUnlocked);

            if (unlockHintText != null)
            {
                if (!isUnlocked && !string.IsNullOrEmpty(skin.unlockedByAchievementId))
                {
                    unlockHintText.text = GetUnlockHint(skin.unlockedByAchievementId);
                    unlockHintText.gameObject.SetActive(true);
                }
                else
                {
                    unlockHintText.gameObject.SetActive(false);
                }
            }

            if (backgroundImage != null)
            {
                if (isSelected)
                    backgroundImage.color = selectedColor;
                else if (isUnlocked)
                    backgroundImage.color = unlockedColor;
                else
                    backgroundImage.color = lockedColor;
            }

            if (selectButton != null)
                selectButton.interactable = isUnlocked;
        }

        private string GetUnlockHint(string achievementId)
        {
            if (Achievements.AchievementManager.Instance != null)
            {
                var achievement = Achievements.AchievementManager.Instance.Config?.GetById(achievementId);
                if (achievement != null)
                {
                    return $"Unlock: {achievement.title}";
                }
            }
            return "Locked";
        }

        private void OnClick()
        {
            onSelectCallback?.Invoke(skinData);
        }
    }
}
