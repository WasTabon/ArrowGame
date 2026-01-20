using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace ArrowGame.Editor
{
    public class SkinsUIGenerator : EditorWindow
    {
        private Canvas targetCanvas;
        private GameObject mainMenuPanel;

        [MenuItem("ArrowGame/Generate Skins UI")]
        public static void ShowWindow()
        {
            GetWindow<SkinsUIGenerator>("Skins UI Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Skins UI Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            targetCanvas = (Canvas)EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true);
            mainMenuPanel = (GameObject)EditorGUILayout.ObjectField("Main Menu (for Skins Button)", mainMenuPanel, typeof(GameObject), true);

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Generate Skins UI", GUILayout.Height(40)))
            {
                Generate();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox(
                "Creates:\n" +
                "• Skin Selection Panel\n" +
                "• Skin Item Prefab\n" +
                "• Skins Button in Main Menu\n" +
                "• SkinManager GameObject",
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

            Undo.SetCurrentGroupName("Generate Skins UI");
            int undoGroup = Undo.GetCurrentGroup();

            CreateSkinManager();
            GameObject itemPrefab = CreateSkinItemPrefab();
            CreateSelectionPanel(itemPrefab);
            CreateSkinsButton();

            Undo.CollapseUndoOperations(undoGroup);
            Debug.Log("Skins UI generated!");
        }

        private void CreateSkinManager()
        {
            if (FindObjectOfType<Skins.SkinManager>() != null)
            {
                Debug.Log("SkinManager already exists.");
                return;
            }

            GameObject manager = new GameObject("SkinManager");
            manager.AddComponent<Skins.SkinManager>();
            Undo.RegisterCreatedObjectUndo(manager, "Create SkinManager");
        }

        private GameObject CreateSkinItemPrefab()
        {
            string prefabPath = "Assets/Prefabs/UI/SkinItem.prefab";
            string folderPath = "Assets/Prefabs/UI";

            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            if (!AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder("Assets/Prefabs", "UI");

            GameObject item = new GameObject("SkinItem");
            RectTransform itemRect = item.AddComponent<RectTransform>();
            itemRect.sizeDelta = new Vector2(120, 150);

            Image bg = item.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.15f);

            Button btn = item.AddComponent<Button>();
            btn.targetGraphic = bg;

            VerticalLayoutGroup vlg = item.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 5;
            vlg.padding = new RectOffset(8, 8, 8, 8);
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;

            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(item.transform, false);
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(80, 80);
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.color = Color.white;
            LayoutElement iconLE = iconObj.AddComponent<LayoutElement>();
            iconLE.preferredHeight = 80;

            GameObject nameObj = CreateText("Name", item.transform, "Skin Name", 14, Color.white);
            nameObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            LayoutElement nameLE = nameObj.AddComponent<LayoutElement>();
            nameLE.preferredHeight = 20;

            GameObject descObj = CreateText("Description", item.transform, "Description", 10, new Color(0.6f, 0.6f, 0.6f));
            descObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            LayoutElement descLE = descObj.AddComponent<LayoutElement>();
            descLE.preferredHeight = 28;

            GameObject selectedBadge = new GameObject("SelectedBadge");
            selectedBadge.transform.SetParent(item.transform, false);
            RectTransform badgeRect = selectedBadge.AddComponent<RectTransform>();
            badgeRect.anchorMin = new Vector2(1, 1);
            badgeRect.anchorMax = new Vector2(1, 1);
            badgeRect.pivot = new Vector2(1, 1);
            badgeRect.anchoredPosition = new Vector2(-5, -5);
            badgeRect.sizeDelta = new Vector2(24, 24);
            Image badgeImage = selectedBadge.AddComponent<Image>();
            badgeImage.color = Color.green;
            
            GameObject checkText = CreateText("Check", selectedBadge.transform, "✓", 16, Color.white);
            RectTransform checkRect = checkText.GetComponent<RectTransform>();
            checkRect.anchorMin = Vector2.zero;
            checkRect.anchorMax = Vector2.one;
            checkRect.offsetMin = Vector2.zero;
            checkRect.offsetMax = Vector2.zero;
            checkText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            GameObject lockedOverlay = new GameObject("LockedOverlay");
            lockedOverlay.transform.SetParent(item.transform, false);
            RectTransform lockedRect = lockedOverlay.AddComponent<RectTransform>();
            lockedRect.anchorMin = Vector2.zero;
            lockedRect.anchorMax = Vector2.one;
            lockedRect.offsetMin = Vector2.zero;
            lockedRect.offsetMax = Vector2.zero;
            Image lockedImage = lockedOverlay.AddComponent<Image>();
            lockedImage.color = new Color(0, 0, 0, 0.7f);

            GameObject lockIcon = CreateText("LockIcon", lockedOverlay.transform, "🔒", 32, Color.white);
            RectTransform lockIconRect = lockIcon.GetComponent<RectTransform>();
            lockIconRect.anchorMin = new Vector2(0.5f, 0.5f);
            lockIconRect.anchorMax = new Vector2(0.5f, 0.5f);
            lockIconRect.pivot = new Vector2(0.5f, 0.5f);
            lockIconRect.anchoredPosition = Vector2.zero;
            lockIconRect.sizeDelta = new Vector2(50, 50);
            lockIcon.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            GameObject unlockHint = CreateText("UnlockHint", lockedOverlay.transform, "Unlock hint", 10, new Color(0.8f, 0.8f, 0.8f));
            RectTransform hintRect = unlockHint.GetComponent<RectTransform>();
            hintRect.anchorMin = new Vector2(0, 0);
            hintRect.anchorMax = new Vector2(1, 0);
            hintRect.pivot = new Vector2(0.5f, 0);
            hintRect.anchoredPosition = new Vector2(0, 10);
            hintRect.sizeDelta = new Vector2(0, 20);
            unlockHint.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            Skins.SkinItemUI itemUI = item.AddComponent<Skins.SkinItemUI>();
            SerializedObject so = new SerializedObject(itemUI);
            so.FindProperty("backgroundImage").objectReferenceValue = bg;
            so.FindProperty("iconImage").objectReferenceValue = iconImage;
            so.FindProperty("nameText").objectReferenceValue = nameObj.GetComponent<TextMeshProUGUI>();
            so.FindProperty("descriptionText").objectReferenceValue = descObj.GetComponent<TextMeshProUGUI>();
            so.FindProperty("selectedBadge").objectReferenceValue = selectedBadge;
            so.FindProperty("lockedOverlay").objectReferenceValue = lockedOverlay;
            so.FindProperty("unlockHintText").objectReferenceValue = unlockHint.GetComponent<TextMeshProUGUI>();
            so.FindProperty("selectButton").objectReferenceValue = btn;
            so.ApplyModifiedProperties();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(item, prefabPath);
            DestroyImmediate(item);

            return prefab;
        }

        private void CreateSelectionPanel(GameObject itemPrefab)
        {
            GameObject panel = CreatePanel("SkinSelectionPanel", targetCanvas.transform);
            panel.SetActive(false);

            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            panel.GetComponent<Image>().color = new Color(0, 0, 0, 0.9f);
            CanvasGroup cg = panel.AddComponent<CanvasGroup>();

            GameObject window = CreatePanel("Window", panel.transform);
            RectTransform windowRect = window.GetComponent<RectTransform>();
            windowRect.anchorMin = new Vector2(0.5f, 0.5f);
            windowRect.anchorMax = new Vector2(0.5f, 0.5f);
            windowRect.pivot = new Vector2(0.5f, 0.5f);
            windowRect.anchoredPosition = Vector2.zero;
            windowRect.sizeDelta = new Vector2(500, 550);
            window.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.98f);

            GameObject header = new GameObject("Header");
            header.transform.SetParent(window.transform, false);
            RectTransform headerRect = header.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.anchoredPosition = Vector2.zero;
            headerRect.sizeDelta = new Vector2(0, 50);

            GameObject title = CreateText("Title", header.transform, "NEEDLE SKINS", 28, Color.white);
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0);
            titleRect.anchorMax = new Vector2(0.7f, 1);
            titleRect.offsetMin = new Vector2(20, 0);
            titleRect.offsetMax = Vector2.zero;
            title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

            GameObject countText = CreateText("Count", header.transform, "1/5", 20, new Color(0.7f, 0.7f, 0.7f));
            RectTransform countRect = countText.GetComponent<RectTransform>();
            countRect.anchorMin = new Vector2(0.7f, 0);
            countRect.anchorMax = new Vector2(1, 1);
            countRect.offsetMin = Vector2.zero;
            countRect.offsetMax = new Vector2(-60, 0);
            countText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;

            Button closeBtn = CreateButton("CloseButton", header.transform, "X", new Vector2(-25, -25));
            RectTransform closeBtnRect = closeBtn.GetComponent<RectTransform>();
            closeBtnRect.anchorMin = new Vector2(1, 1);
            closeBtnRect.anchorMax = new Vector2(1, 1);
            closeBtnRect.pivot = new Vector2(1, 1);
            closeBtnRect.sizeDelta = new Vector2(40, 40);
            closeBtn.GetComponent<Image>().color = new Color(0.5f, 0.2f, 0.2f);

            GameObject preview = new GameObject("Preview");
            preview.transform.SetParent(window.transform, false);
            RectTransform previewRect = preview.AddComponent<RectTransform>();
            previewRect.anchorMin = new Vector2(0, 1);
            previewRect.anchorMax = new Vector2(1, 1);
            previewRect.pivot = new Vector2(0.5f, 1);
            previewRect.anchoredPosition = new Vector2(0, -55);
            previewRect.sizeDelta = new Vector2(0, 100);

            GameObject previewIcon = new GameObject("PreviewIcon");
            previewIcon.transform.SetParent(preview.transform, false);
            RectTransform previewIconRect = previewIcon.AddComponent<RectTransform>();
            previewIconRect.anchorMin = new Vector2(0, 0.5f);
            previewIconRect.anchorMax = new Vector2(0, 0.5f);
            previewIconRect.pivot = new Vector2(0, 0.5f);
            previewIconRect.anchoredPosition = new Vector2(20, 0);
            previewIconRect.sizeDelta = new Vector2(80, 80);
            Image previewImage = previewIcon.AddComponent<Image>();
            previewImage.color = Color.white;

            GameObject previewName = CreateText("PreviewName", preview.transform, "Skin Name", 24, Color.white);
            RectTransform previewNameRect = previewName.GetComponent<RectTransform>();
            previewNameRect.anchorMin = new Vector2(0, 0.5f);
            previewNameRect.anchorMax = new Vector2(1, 1);
            previewNameRect.offsetMin = new Vector2(120, 0);
            previewNameRect.offsetMax = new Vector2(-20, -10);
            previewName.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

            GameObject previewDesc = CreateText("PreviewDesc", preview.transform, "Description here", 14, new Color(0.7f, 0.7f, 0.7f));
            RectTransform previewDescRect = previewDesc.GetComponent<RectTransform>();
            previewDescRect.anchorMin = new Vector2(0, 0);
            previewDescRect.anchorMax = new Vector2(1, 0.5f);
            previewDescRect.offsetMin = new Vector2(120, 10);
            previewDescRect.offsetMax = new Vector2(-20, 0);
            previewDesc.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

            GameObject scrollView = new GameObject("ScrollView");
            scrollView.transform.SetParent(window.transform, false);
            RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0, 0);
            scrollRect.anchorMax = new Vector2(1, 1);
            scrollRect.offsetMin = new Vector2(10, 70);
            scrollRect.offsetMax = new Vector2(-10, -160);

            ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
            scroll.horizontal = true;
            scroll.vertical = false;
            Image scrollBg = scrollView.AddComponent<Image>();
            scrollBg.color = new Color(0.08f, 0.08f, 0.08f);
            Mask mask = scrollView.AddComponent<Mask>();
            mask.showMaskGraphic = true;

            GameObject content = new GameObject("Content");
            content.transform.SetParent(scrollView.transform, false);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 0);
            contentRect.anchorMax = new Vector2(0, 1);
            contentRect.pivot = new Vector2(0, 0.5f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0, 0);

            HorizontalLayoutGroup hlg = content.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 10;
            hlg.padding = new RectOffset(10, 10, 10, 10);
            hlg.childControlWidth = false;
            hlg.childControlHeight = true;
            hlg.childForceExpandHeight = true;

            ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.content = contentRect;

            Button selectBtn = CreateButton("SelectButton", window.transform, "EQUIP", new Vector2(0, -230));
            RectTransform selectBtnRect = selectBtn.GetComponent<RectTransform>();
            selectBtnRect.sizeDelta = new Vector2(200, 50);

            Skins.SkinSelectionUI selectionUI = panel.AddComponent<Skins.SkinSelectionUI>();
            SerializedObject so = new SerializedObject(selectionUI);
            so.FindProperty("panel").objectReferenceValue = panel;
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.FindProperty("unlockedCountText").objectReferenceValue = countText.GetComponent<TextMeshProUGUI>();
            so.FindProperty("previewImage").objectReferenceValue = previewImage;
            so.FindProperty("previewNameText").objectReferenceValue = previewName.GetComponent<TextMeshProUGUI>();
            so.FindProperty("previewDescriptionText").objectReferenceValue = previewDesc.GetComponent<TextMeshProUGUI>();
            so.FindProperty("content").objectReferenceValue = content.transform;
            so.FindProperty("skinItemPrefab").objectReferenceValue = itemPrefab;
            so.FindProperty("closeButton").objectReferenceValue = closeBtn;
            so.FindProperty("selectButton").objectReferenceValue = selectBtn;
            so.ApplyModifiedProperties();

            Undo.RegisterCreatedObjectUndo(panel, "Create Skin Selection Panel");
        }

        private void CreateSkinsButton()
        {
            if (mainMenuPanel == null)
            {
                Debug.LogWarning("Main Menu Panel not assigned.");
                return;
            }

            GameObject btnObj = new GameObject("SkinsButton");
            btnObj.transform.SetParent(mainMenuPanel.transform, false);
            Undo.RegisterCreatedObjectUndo(btnObj, "Create SkinsButton");

            RectTransform btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0, 1);
            btnRect.anchorMax = new Vector2(0, 1);
            btnRect.pivot = new Vector2(0, 1);
            btnRect.anchoredPosition = new Vector2(90, -20);
            btnRect.sizeDelta = new Vector2(60, 60);

            Image btnImage = btnObj.AddComponent<Image>();
            btnImage.color = new Color(0.3f, 0.3f, 0.3f, 0.9f);

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = btnImage;

            GameObject iconText = CreateText("Icon", btnObj.transform, "🎨", 28, Color.white);
            RectTransform iconRect = iconText.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            iconText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            Skins.SkinSelectionUI selectionUI = FindObjectOfType<Skins.SkinSelectionUI>();
            if (selectionUI != null)
            {
                UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, selectionUI.Show);
            }
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
            btnRect.anchorMin = new Vector2(0.5f, 0.5f);
            btnRect.anchorMax = new Vector2(0.5f, 0.5f);
            btnRect.pivot = new Vector2(0.5f, 0.5f);
            btnRect.anchoredPosition = position;
            btnRect.sizeDelta = new Vector2(40, 40);

            Image btnImage = btnObj.AddComponent<Image>();
            btnImage.color = new Color(0.25f, 0.25f, 0.25f);

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = btnImage;

            GameObject textObj = CreateText("Text", btnObj.transform, text, 18, Color.white);
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
