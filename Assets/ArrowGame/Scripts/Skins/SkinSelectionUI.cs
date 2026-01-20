using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

namespace ArrowGame.Skins
{
    public class SkinSelectionUI : MonoBehaviour
    {
        public static SkinSelectionUI Instance { get; private set; }

        [Header("Panel")]
        [SerializeField] private GameObject panel;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Header")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI unlockedCountText;

        [Header("Preview")]
        [SerializeField] private Image previewImage;
        [SerializeField] private TextMeshProUGUI previewNameText;
        [SerializeField] private TextMeshProUGUI previewDescriptionText;

        [Header("Content")]
        [SerializeField] private Transform content;
        [SerializeField] private GameObject skinItemPrefab;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button selectButton;

        [Header("Animation")]
        [SerializeField] private float fadeIn = 0.25f;
        [SerializeField] private float fadeOut = 0.2f;

        private List<GameObject> spawnedItems = new List<GameObject>();
        private NeedleSkinDefinition previewedSkin;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (panel != null)
                panel.SetActive(false);
        }

        private void Start()
        {
            SetupButtons();
        }

        private void SetupButtons()
        {
            closeButton?.onClick.AddListener(Hide);
            selectButton?.onClick.AddListener(OnSelectClicked);
        }

        public void Show()
        {
            if (panel == null) return;

            panel.SetActive(true);
            Refresh();

            if (SkinManager.Instance?.CurrentSkin != null)
            {
                ShowPreview(SkinManager.Instance.CurrentSkin);
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.DOFade(1f, fadeIn).SetUpdate(true);
            }
        }

        public void Hide()
        {
            if (panel == null) return;

            if (canvasGroup != null)
            {
                canvasGroup.DOFade(0f, fadeOut)
                    .SetUpdate(true)
                    .OnComplete(() => panel.SetActive(false));
            }
            else
            {
                panel.SetActive(false);
            }
        }

        private void Refresh()
        {
            ClearItems();
            UpdateHeader();

            if (SkinManager.Instance == null || skinItemPrefab == null || content == null) return;

            var skins = SkinManager.Instance.GetAllSkins();
            string selectedId = SkinManager.Instance.CurrentSkin?.id ?? "";

            foreach (var skin in skins)
            {
                bool isUnlocked = SkinManager.Instance.IsSkinUnlocked(skin.id);
                bool isSelected = skin.id == selectedId;

                var obj = Instantiate(skinItemPrefab, content);
                spawnedItems.Add(obj);

                var itemUI = obj.GetComponent<SkinItemUI>();
                if (itemUI != null)
                {
                    itemUI.Setup(skin, isSelected, isUnlocked, OnSkinItemClicked);
                }
            }
        }

        private void ClearItems()
        {
            foreach (var obj in spawnedItems)
                Destroy(obj);
            spawnedItems.Clear();
        }

        private void UpdateHeader()
        {
            if (unlockedCountText == null || SkinManager.Instance == null) return;

            int unlocked = SkinManager.Instance.GetUnlockedSkins().Count;
            int total = SkinManager.Instance.GetAllSkins().Count;
            unlockedCountText.text = $"{unlocked}/{total}";
        }

        private void OnSkinItemClicked(NeedleSkinDefinition skin)
        {
            ShowPreview(skin);
        }

        private void ShowPreview(NeedleSkinDefinition skin)
        {
            previewedSkin = skin;

            if (previewImage != null)
            {
                previewImage.sprite = skin.icon;
                previewImage.gameObject.SetActive(skin.icon != null);
            }

            if (previewNameText != null)
                previewNameText.text = skin.displayName;

            if (previewDescriptionText != null)
                previewDescriptionText.text = skin.description;

            bool isUnlocked = SkinManager.Instance?.IsSkinUnlocked(skin.id) ?? false;
            bool isSelected = SkinManager.Instance?.CurrentSkin?.id == skin.id;

            if (selectButton != null)
            {
                selectButton.interactable = isUnlocked && !isSelected;

                var buttonText = selectButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    if (isSelected)
                        buttonText.text = "EQUIPPED";
                    else if (isUnlocked)
                        buttonText.text = "EQUIP";
                    else
                        buttonText.text = "LOCKED";
                }
            }
        }

        private void OnSelectClicked()
        {
            if (previewedSkin == null || SkinManager.Instance == null) return;

            if (SkinManager.Instance.SelectSkin(previewedSkin.id))
            {
                Feedback.HapticFeedback.Success();
                Refresh();
                ShowPreview(previewedSkin);
            }
        }
    }
}
