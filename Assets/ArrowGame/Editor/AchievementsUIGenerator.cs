using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace ArrowGame.Editor
{
    public class AchievementsUIGenerator : EditorWindow
    {
        private Canvas targetCanvas;
        private GameObject mainMenuPanel;

        private bool createPopup = true;
        private bool createListPanel = true;
        private bool createTracker = true;

        [MenuItem("ArrowGame/Generate Achievements UI")]
        public static void ShowWindow()
        {
            GetWindow<AchievementsUIGenerator>("Achievements UI Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Achievements UI Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            targetCanvas = (Canvas)EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true);
            mainMenuPanel = (GameObject)EditorGUILayout.ObjectField("Main Menu (for Achievements Button)", mainMenuPanel, typeof(GameObject), true);

            EditorGUILayout.Space(10);
            GUILayout.Label("What to create:", EditorStyles.boldLabel);
            createPopup = EditorGUILayout.Toggle("Achievement Popup", createPopup);
            createListPanel = EditorGUILayout.Toggle("Achievements List Panel", createListPanel);
            createTracker = EditorGUILayout.Toggle("Achievement Tracker", createTracker);

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Generate Achievements UI", GUILayout.Height(40)))
            {
                Generate();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox(
                "Creates:\n" +
                "• Achievement Popup (top of screen)\n" +
                "• Achievements List Panel (with tabs)\n" +
                "• Achievement Item Prefab\n" +
                "• Achievement Tracker GameObject\n" +
                "• Achievements Button in Main Menu",
                MessageType.Info);
        }

        private void Generate()
        {
            if (targetCanvas == null)
            {
                targetCanvas = FindObjectOfType<Canvas>();
                if (targetCanvas == null)
                {
                    Debug.LogError("No Canvas found!");
                    return;
                }
            }

            Undo.SetCurrentGroupName("Generate Achievements UI");
            int undoGroup = Undo.GetCurrentGroup();

            GameObject itemPrefab = null;
            if (createListPanel)
            {
                itemPrefab = CreateItemPrefab();
            }

            if (createPopup)
            {
                CreatePopup();
            }

            if (createListPanel)
            {
                CreateListPanel(itemPrefab);
            }

            if (createTracker)
            {
                CreateTracker();
            }

            CreateAchievementsButton();

            Undo.CollapseUndoOperations(undoGroup);
            Debug.Log("Achievements UI generated!");
        }

        private GameObject CreateItemPrefab()
        {
            string prefabPath = "Assets/Prefabs/UI/AchievementItem.prefab";
            string folderPath = "Assets/Prefabs/UI";

            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            if (!AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder("Assets/Prefabs", "UI");

            GameObject item = new GameObject("AchievementItem");
            RectTransform itemRect = item.AddComponent<RectTransform>();
            itemRect.sizeDelta = new Vector2(400, 80);

            Image bg = item.AddComponent<Image>();
            bg.color = new Color(0.12f, 0.12f, 0.12f, 0.95f);

            HorizontalLayoutGroup hlg = item.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 10;
            hlg.padding = new RectOffset(10, 10, 10, 10);
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;

            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(item.transform, false);
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(60, 60);
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.color = new Color(0.5f, 0.5f, 0.5f);

            GameObject infoContainer = new GameObject("Info");
            infoContainer.transform.SetParent(item.transform, false);
            RectTransform infoRect = infoContainer.AddComponent<RectTransform>();
            infoRect.sizeDelta = new Vector2(220, 60);
            VerticalLayoutGroup vlg = infoContainer.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 2;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            GameObject titleObj = CreateText("Title", infoContainer.transform, "Achievement Title", 18, Color.white);
            titleObj.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            LayoutElement titleLE = titleObj.AddComponent<LayoutElement>();
            titleLE.preferredHeight = 24;

            GameObject descObj = CreateText("Description", infoContainer.transform, "Description here", 14, new Color(0.7f, 0.7f, 0.7f));
            LayoutElement descLE = descObj.AddComponent<LayoutElement>();
            descLE.preferredHeight = 18;

            GameObject progressContainer = new GameObject("ProgressContainer");
            progressContainer.transform.SetParent(infoContainer.transform, false);
            RectTransform progRect = progressContainer.AddComponent<RectTransform>();
            progRect.sizeDelta = new Vector2(0, 16);
            LayoutElement progLE = progressContainer.AddComponent<LayoutElement>();
            progLE.preferredHeight = 16;

            GameObject progressBg = new GameObject("ProgressBg");
            progressBg.transform.SetParent(progressContainer.transform, false);
            RectTransform progBgRect = progressBg.AddComponent<RectTransform>();
            progBgRect.anchorMin = new Vector2(0, 0.5f);
            progBgRect.anchorMax = new Vector2(0.7f, 0.5f);
            progBgRect.pivot = new Vector2(0, 0.5f);
            progBgRect.anchoredPosition = Vector2.zero;
            progBgRect.sizeDelta = new Vector2(0, 8);
            Image progBgImage = progressBg.AddComponent<Image>();
            progBgImage.color = new Color(0.2f, 0.2f, 0.2f);

            GameObject progressFill = new GameObject("ProgressFill");
            progressFill.transform.SetParent(progressBg.transform, false);
            RectTransform progFillRect = progressFill.AddComponent<RectTransform>();
            progFillRect.anchorMin = Vector2.zero;
            progFillRect.anchorMax = Vector2.one;
            progFillRect.offsetMin = Vector2.zero;
            progFillRect.offsetMax = Vector2.zero;
            Image progFillImage = progressFill.AddComponent<Image>();
            progFillImage.color = new Color(0.3f, 0.7f, 1f);
            progFillImage.type = Image.Type.Filled;
            progFillImage.fillMethod = Image.FillMethod.Horizontal;
            progFillImage.fillAmount = 0.5f;

            GameObject progressText = CreateText("ProgressText", progressContainer.transform, "5/10", 12, Color.white);
            RectTransform progTextRect = progressText.GetComponent<RectTransform>();
            progTextRect.anchorMin = new Vector2(1, 0.5f);
            progTextRect.anchorMax = new Vector2(1, 0.5f);
            progTextRect.pivot = new Vector2(1, 0.5f);
            progTextRect.anchoredPosition = Vector2.zero;
            progTextRect.sizeDelta = new Vector2(50, 16);
            progressText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;

            GameObject completedBadge = new GameObject("CompletedBadge");
            completedBadge.transform.SetParent(item.transform, false);
            RectTransform badgeRect = completedBadge.AddComponent<RectTransform>();
            badgeRect.sizeDelta = new Vector2(30, 30);
            Image badgeImage = completedBadge.AddComponent<Image>();
            badgeImage.color = Color.green;

            GameObject checkText = CreateText("Check", completedBadge.transform, "✓", 20, Color.white);
            RectTransform checkRect = checkText.GetComponent<RectTransform>();
            checkRect.anchorMin = Vector2.zero;
            checkRect.anchorMax = Vector2.one;
            checkRect.offsetMin = Vector2.zero;
            checkRect.offsetMax = Vector2.zero;
            checkText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            GameObject skinBadge = new GameObject("SkinBadge");
            skinBadge.transform.SetParent(item.transform, false);
            RectTransform skinRect = skinBadge.AddComponent<RectTransform>();
            skinRect.sizeDelta = new Vector2(50, 20);
            Image skinImage = skinBadge.AddComponent<Image>();
            skinImage.color = new Color(0.8f, 0.6f, 0.2f);

            GameObject skinText = CreateText("SkinName", skinBadge.transform, "skin", 10, Color.white);
            RectTransform skinTextRect = skinText.GetComponent<RectTransform>();
            skinTextRect.anchorMin = Vector2.zero;
            skinTextRect.anchorMax = Vector2.one;
            skinTextRect.offsetMin = Vector2.zero;
            skinTextRect.offsetMax = Vector2.zero;
            skinText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            Achievements.AchievementItemUI itemUI = item.AddComponent<Achievements.AchievementItemUI>();
            SerializedObject so = new SerializedObject(itemUI);
            so.FindProperty("backgroundImage").objectReferenceValue = bg;
            so.FindProperty("iconImage").objectReferenceValue = iconImage;
            so.FindProperty("titleText").objectReferenceValue = titleObj.GetComponent<TextMeshProUGUI>();
            so.FindProperty("descriptionText").objectReferenceValue = descObj.GetComponent<TextMeshProUGUI>();
            so.FindProperty("progressText").objectReferenceValue = progressText.GetComponent<TextMeshProUGUI>();
            so.FindProperty("progressFill").objectReferenceValue = progFillImage;
            so.FindProperty("completedBadge").objectReferenceValue = completedBadge;
            so.FindProperty("skinBadge").objectReferenceValue = skinBadge;
            so.FindProperty("skinNameText").objectReferenceValue = skinText.GetComponent<TextMeshProUGUI>();
            so.ApplyModifiedProperties();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(item, prefabPath);
            DestroyImmediate(item);

            Debug.Log($"Created prefab at {prefabPath}");
            return prefab;
        }

        private void CreatePopup()
        {
            GameObject popup = CreatePanel("AchievementPopup", targetCanvas.transform);
            popup.SetActive(false);

            RectTransform popupRect = popup.GetComponent<RectTransform>();
            popupRect.anchorMin = new Vector2(0.5f, 1f);
            popupRect.anchorMax = new Vector2(0.5f, 1f);
            popupRect.pivot = new Vector2(0.5f, 1f);
            popupRect.anchoredPosition = new Vector2(0, -20);
            popupRect.sizeDelta = new Vector2(350, 100);

            Image popupImage = popup.GetComponent<Image>();
            popupImage.color = new Color(0.1f, 0.15f, 0.1f, 0.95f);

            CanvasGroup cg = popup.AddComponent<CanvasGroup>();

            HorizontalLayoutGroup hlg = popup.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 15;
            hlg.padding = new RectOffset(15, 15, 15, 15);
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;

            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(popup.transform, false);
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(70, 70);
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.color = Color.white;
            LayoutElement iconLE = iconObj.AddComponent<LayoutElement>();
            iconLE.preferredWidth = 70;
            iconLE.preferredHeight = 70;

            GameObject infoContainer = new GameObject("Info");
            infoContainer.transform.SetParent(popup.transform, false);
            RectTransform infoRect = infoContainer.AddComponent<RectTransform>();
            infoRect.sizeDelta = new Vector2(230, 70);
            VerticalLayoutGroup vlg = infoContainer.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 5;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            LayoutElement infoLE = infoContainer.AddComponent<LayoutElement>();
            infoLE.preferredWidth = 230;

            GameObject header = CreateText("Header", infoContainer.transform, "ACHIEVEMENT UNLOCKED", 12, new Color(0.5f, 0.8f, 0.5f));
            header.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            LayoutElement headerLE = header.AddComponent<LayoutElement>();
            headerLE.preferredHeight = 16;

            GameObject title = CreateText("Title", infoContainer.transform, "Achievement Name", 18, Color.white);
            title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            LayoutElement titleLE = title.AddComponent<LayoutElement>();
            titleLE.preferredHeight = 22;

            GameObject desc = CreateText("Description", infoContainer.transform, "Description", 13, new Color(0.7f, 0.7f, 0.7f));
            LayoutElement descLE = desc.AddComponent<LayoutElement>();
            descLE.preferredHeight = 18;

            GameObject skinBadge = new GameObject("SkinUnlockBadge");
            skinBadge.transform.SetParent(infoContainer.transform, false);
            RectTransform skinRect = skinBadge.AddComponent<RectTransform>();
            skinRect.sizeDelta = new Vector2(100, 16);
            LayoutElement skinLE = skinBadge.AddComponent<LayoutElement>();
            skinLE.preferredHeight = 16;

            GameObject skinText = CreateText("SkinName", skinBadge.transform, "Skin: name", 11, new Color(1f, 0.8f, 0.3f));
            RectTransform skinTextRect = skinText.GetComponent<RectTransform>();
            skinTextRect.anchorMin = Vector2.zero;
            skinTextRect.anchorMax = Vector2.one;
            skinTextRect.offsetMin = Vector2.zero;
            skinTextRect.offsetMax = Vector2.zero;

            Achievements.AchievementPopup popupComponent = popup.AddComponent<Achievements.AchievementPopup>();
            SerializedObject so = new SerializedObject(popupComponent);
            so.FindProperty("popupPanel").objectReferenceValue = popup;
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.FindProperty("iconImage").objectReferenceValue = iconImage;
            so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
            so.FindProperty("descriptionText").objectReferenceValue = desc.GetComponent<TextMeshProUGUI>();
            so.FindProperty("skinUnlockBadge").objectReferenceValue = skinBadge;
            so.FindProperty("skinNameText").objectReferenceValue = skinText.GetComponent<TextMeshProUGUI>();
            so.ApplyModifiedProperties();

            Undo.RegisterCreatedObjectUndo(popup, "Create Achievement Popup");
        }

        private void CreateListPanel(GameObject itemPrefab)
        {
            GameObject panel = CreatePanel("AchievementsListPanel", targetCanvas.transform);
            panel.SetActive(false);

            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.9f);

            CanvasGroup cg = panel.AddComponent<CanvasGroup>();

            GameObject window = CreatePanel("Window", panel.transform);
            RectTransform windowRect = window.GetComponent<RectTransform>();
            windowRect.anchorMin = new Vector2(0.5f, 0.5f);
            windowRect.anchorMax = new Vector2(0.5f, 0.5f);
            windowRect.pivot = new Vector2(0.5f, 0.5f);
            windowRect.anchoredPosition = Vector2.zero;
            windowRect.sizeDelta = new Vector2(500, 600);
            window.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.98f);

            GameObject header = new GameObject("Header");
            header.transform.SetParent(window.transform, false);
            RectTransform headerRect = header.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.anchoredPosition = Vector2.zero;
            headerRect.sizeDelta = new Vector2(0, 60);

            GameObject title = CreateText("Title", header.transform, "ACHIEVEMENTS", 32, Color.white);
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0);
            titleRect.anchorMax = new Vector2(0.7f, 1);
            titleRect.offsetMin = new Vector2(20, 0);
            titleRect.offsetMax = Vector2.zero;
            title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;
            title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

            GameObject progress = CreateText("Progress", header.transform, "0/16", 24, new Color(0.7f, 0.7f, 0.7f));
            RectTransform progressRect = progress.GetComponent<RectTransform>();
            progressRect.anchorMin = new Vector2(0.7f, 0);
            progressRect.anchorMax = new Vector2(1, 1);
            progressRect.offsetMin = Vector2.zero;
            progressRect.offsetMax = new Vector2(-60, 0);
            progress.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;

            Button closeBtn = CreateButton("CloseButton", header.transform, "X", new Vector2(-25, -30));
            RectTransform closeBtnRect = closeBtn.GetComponent<RectTransform>();
            closeBtnRect.anchorMin = new Vector2(1, 1);
            closeBtnRect.anchorMax = new Vector2(1, 1);
            closeBtnRect.pivot = new Vector2(1, 1);
            closeBtnRect.sizeDelta = new Vector2(40, 40);
            closeBtn.GetComponent<Image>().color = new Color(0.5f, 0.2f, 0.2f);

            GameObject tabsContainer = new GameObject("Tabs");
            tabsContainer.transform.SetParent(window.transform, false);
            RectTransform tabsRect = tabsContainer.AddComponent<RectTransform>();
            tabsRect.anchorMin = new Vector2(0, 1);
            tabsRect.anchorMax = new Vector2(1, 1);
            tabsRect.pivot = new Vector2(0.5f, 1);
            tabsRect.anchoredPosition = new Vector2(0, -65);
            tabsRect.sizeDelta = new Vector2(0, 35);

            HorizontalLayoutGroup tabsHlg = tabsContainer.AddComponent<HorizontalLayoutGroup>();
            tabsHlg.spacing = 5;
            tabsHlg.padding = new RectOffset(10, 10, 0, 0);
            tabsHlg.childControlWidth = true;
            tabsHlg.childControlHeight = true;
            tabsHlg.childForceExpandWidth = true;

            Button tabAll = CreateTabButton("TabAll", tabsContainer.transform, "All");
            Button tabPrecision = CreateTabButton("TabPrecision", tabsContainer.transform, "Precision");
            Button tabFlow = CreateTabButton("TabFlow", tabsContainer.transform, "Flow");
            Button tabRisk = CreateTabButton("TabRisk", tabsContainer.transform, "Risk");
            Button tabEndurance = CreateTabButton("TabEndurance", tabsContainer.transform, "Endurance");

            GameObject scrollView = new GameObject("ScrollView");
            scrollView.transform.SetParent(window.transform, false);
            RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0, 0);
            scrollRect.anchorMax = new Vector2(1, 1);
            scrollRect.offsetMin = new Vector2(10, 60);
            scrollRect.offsetMax = new Vector2(-10, -105);

            ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;

            Image scrollBg = scrollView.AddComponent<Image>();
            scrollBg.color = new Color(0.08f, 0.08f, 0.08f);
            
            Mask mask = scrollView.AddComponent<Mask>();
            mask.showMaskGraphic = true;

            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform, false);
            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0, 0);

            VerticalLayoutGroup contentVlg = content.AddComponent<VerticalLayoutGroup>();
            contentVlg.spacing = 8;
            contentVlg.padding = new RectOffset(5, 5, 5, 5);
            contentVlg.childControlWidth = true;
            contentVlg.childControlHeight = false;
            contentVlg.childForceExpandWidth = true;

            ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.viewport = viewportRect;
            scroll.content = contentRect;

            Achievements.AchievementsListUI listUI = panel.AddComponent<Achievements.AchievementsListUI>();
            SerializedObject so = new SerializedObject(listUI);
            so.FindProperty("panel").objectReferenceValue = panel;
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.FindProperty("progressText").objectReferenceValue = progress.GetComponent<TextMeshProUGUI>();
            so.FindProperty("content").objectReferenceValue = content.transform;
            so.FindProperty("itemPrefab").objectReferenceValue = itemPrefab;
            so.FindProperty("tabAll").objectReferenceValue = tabAll;
            so.FindProperty("tabPrecision").objectReferenceValue = tabPrecision;
            so.FindProperty("tabFlow").objectReferenceValue = tabFlow;
            so.FindProperty("tabRisk").objectReferenceValue = tabRisk;
            so.FindProperty("tabEndurance").objectReferenceValue = tabEndurance;
            so.FindProperty("closeButton").objectReferenceValue = closeBtn;
            so.ApplyModifiedProperties();

            Undo.RegisterCreatedObjectUndo(panel, "Create Achievements List Panel");
        }

        private void CreateTracker()
        {
            if (FindObjectOfType<Achievements.AchievementTracker>() != null)
            {
                Debug.Log("AchievementTracker already exists.");
                return;
            }

            GameObject tracker = new GameObject("AchievementTracker");
            tracker.AddComponent<Achievements.AchievementTracker>();
            Undo.RegisterCreatedObjectUndo(tracker, "Create AchievementTracker");
        }

        private void CreateAchievementsButton()
        {
            if (mainMenuPanel == null)
            {
                Debug.LogWarning("Main Menu Panel not assigned. Achievements button not created.");
                return;
            }

            GameObject btnObj = new GameObject("AchievementsButton");
            btnObj.transform.SetParent(mainMenuPanel.transform, false);
            Undo.RegisterCreatedObjectUndo(btnObj, "Create AchievementsButton");

            RectTransform btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0, 1);
            btnRect.anchorMax = new Vector2(0, 1);
            btnRect.pivot = new Vector2(0, 1);
            btnRect.anchoredPosition = new Vector2(20, -20);
            btnRect.sizeDelta = new Vector2(60, 60);

            Image btnImage = btnObj.AddComponent<Image>();
            btnImage.color = new Color(0.3f, 0.3f, 0.3f, 0.9f);

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = btnImage;

            GameObject iconText = CreateText("Icon", btnObj.transform, "🏆", 32, Color.white);
            RectTransform iconRect = iconText.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            iconText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            Achievements.AchievementsListUI listUI = FindObjectOfType<Achievements.AchievementsListUI>();
            if (listUI != null)
            {
                UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, listUI.Show);
            }
        }

        private Button CreateTabButton(string name, Transform parent, string text)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            btnObj.AddComponent<RectTransform>();

            Image btnImage = btnObj.AddComponent<Image>();
            btnImage.color = new Color(0.2f, 0.2f, 0.2f);

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = btnImage;

            ColorBlock colors = btn.colors;
            colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f);
            colors.pressedColor = new Color(0.25f, 0.25f, 0.25f);
            colors.selectedColor = new Color(0.3f, 0.5f, 0.3f);
            btn.colors = colors;

            GameObject textObj = CreateText("Text", btnObj.transform, text, 12, Color.white);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            textObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            return btn;
        }

        private GameObject CreatePanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            panel.AddComponent<RectTransform>();
            panel.AddComponent<Image>();
            return panel;
        }

        private Button CreateButton(string name, Transform parent, string text, Vector2 position)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);

            RectTransform btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchoredPosition = position;
            btnRect.sizeDelta = new Vector2(40, 40);

            Image btnImage = btnObj.AddComponent<Image>();
            btnImage.color = new Color(0.25f, 0.25f, 0.25f);

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = btnImage;

            GameObject textObj = CreateText("Text", btnObj.transform, text, 20, Color.white);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            textObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            return btn;
        }

        private GameObject CreateText(string name, Transform parent, string text, int fontSize, Color color)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.AddComponent<RectTransform>();

            TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.raycastTarget = false;

            return obj;
        }
    }
}
