using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

namespace ArrowGame.Achievements
{
    public class AchievementsListUI : MonoBehaviour
    {
        public static AchievementsListUI Instance { get; private set; }

        [Header("Panel")]
        [SerializeField] private GameObject panel;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Header")]
        [SerializeField] private TextMeshProUGUI progressText;

        [Header("Content")]
        [SerializeField] private Transform content;
        [SerializeField] private GameObject itemPrefab;

        [Header("Tabs")]
        [SerializeField] private Button tabAll;
        [SerializeField] private Button tabPrecision;
        [SerializeField] private Button tabFlow;
        [SerializeField] private Button tabRisk;
        [SerializeField] private Button tabEndurance;

        [Header("Buttons")]
        [SerializeField] private Button closeButton;

        [Header("Animation")]
        [SerializeField] private float fadeIn = 0.25f;
        [SerializeField] private float fadeOut = 0.2f;

        private AchievementCategory? currentFilter;
        private List<GameObject> spawnedItems = new List<GameObject>();

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
            tabAll?.onClick.AddListener(() => SetFilter(null));
            tabPrecision?.onClick.AddListener(() => SetFilter(AchievementCategory.Precision));
            tabFlow?.onClick.AddListener(() => SetFilter(AchievementCategory.Flow));
            tabRisk?.onClick.AddListener(() => SetFilter(AchievementCategory.Risk));
            tabEndurance?.onClick.AddListener(() => SetFilter(AchievementCategory.Endurance));
        }

        public void Show()
        {
            if (panel == null) return;

            panel.SetActive(true);
            currentFilter = null;
            Refresh();

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

        private void SetFilter(AchievementCategory? category)
        {
            currentFilter = category;
            Refresh();
        }

        private void Refresh()
        {
            ClearItems();
            UpdateHeader();

            if (AchievementManager.Instance == null || itemPrefab == null || content == null) return;

            var list = AchievementManager.Instance.GetAllAchievements();

            foreach (var def in list)
            {
                if (currentFilter.HasValue && def.category != currentFilter.Value)
                    continue;

                if (def.isHidden && !AchievementManager.Instance.IsUnlocked(def.id))
                    continue;

                var obj = Instantiate(itemPrefab, content);
                spawnedItems.Add(obj);

                var itemUI = obj.GetComponent<AchievementItemUI>();
                if (itemUI != null)
                    itemUI.Setup(def);
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
            if (progressText == null || AchievementManager.Instance == null) return;

            int unlocked = AchievementManager.Instance.GetUnlockedCount();
            int total = AchievementManager.Instance.GetTotalCount();
            progressText.text = $"{unlocked}/{total}";
        }
    }
}
