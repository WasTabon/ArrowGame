using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace ArrowGame.Editor
{
    public class SettingsUIGenerator : EditorWindow
    {
        private Canvas targetCanvas;
        private GameObject mainMenuPanel;

        [MenuItem("ArrowGame/Generate Settings UI")]
        public static void ShowWindow()
        {
            GetWindow<SettingsUIGenerator>("Settings UI Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Settings UI Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            targetCanvas = (Canvas)EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true);
            mainMenuPanel = (GameObject)EditorGUILayout.ObjectField("Main Menu Panel (for Settings Button)", mainMenuPanel, typeof(GameObject), true);

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Generate Settings UI", GUILayout.Height(40)))
            {
                GenerateUI();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox(
                "Creates:\n" +
                "• SettingsPanel with sliders and toggles\n" +
                "• Settings Button in Main Menu\n" +
                "• SettingsManager GameObject\n\n" +
                "Settings:\n" +
                "• Music Volume (slider)\n" +
                "• SFX Volume (slider)\n" +
                "• Vibration (toggle)\n" +
                "• Post Processing (toggle)\n" +
                "• Reset to Defaults button",
                MessageType.Info);
        }

        private void GenerateUI()
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

            Undo.SetCurrentGroupName("Generate Settings UI");
            int undoGroup = Undo.GetCurrentGroup();

            CreateSettingsManager();
            GameObject settingsPanel = CreateSettingsPanel();
            CreateSettingsButton(settingsPanel);

            Undo.CollapseUndoOperations(undoGroup);

            Debug.Log("Settings UI generated successfully!");
        }

        private void CreateSettingsManager()
        {
            if (FindObjectOfType<Settings.SettingsManager>() != null)
            {
                Debug.Log("SettingsManager already exists, skipping.");
                return;
            }

            GameObject managerObj = new GameObject("SettingsManager");
            managerObj.AddComponent<Settings.SettingsManager>();
            Undo.RegisterCreatedObjectUndo(managerObj, "Create SettingsManager");
        }

        private GameObject CreateSettingsPanel()
        {
            GameObject panel = CreatePanel("SettingsPanel", targetCanvas.transform);
            panel.SetActive(false);

            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.85f);

            CanvasGroup cg = panel.AddComponent<CanvasGroup>();

            GameObject window = CreatePanel("Window", panel.transform);
            RectTransform windowRect = window.GetComponent<RectTransform>();
            windowRect.anchorMin = new Vector2(0.5f, 0.5f);
            windowRect.anchorMax = new Vector2(0.5f, 0.5f);
            windowRect.pivot = new Vector2(0.5f, 0.5f);
            windowRect.anchoredPosition = Vector2.zero;
            windowRect.sizeDelta = new Vector2(450, 550);

            Image windowImage = window.GetComponent<Image>();
            windowImage.color = new Color(0.12f, 0.12f, 0.12f, 0.95f);

            GameObject title = CreateTextObject("Title", window.transform, "SETTINGS", 42, Color.white);
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0, -25);
            titleRect.sizeDelta = new Vector2(400, 50);
            title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            float yPos = -100;
            float yStep = 90;

            var musicSlider = CreateSliderRow("MusicSlider", window.transform, "Music", yPos);
            yPos -= yStep;

            var sfxSlider = CreateSliderRow("SfxSlider", window.transform, "Sound Effects", yPos);
            yPos -= yStep;

            var vibrationToggle = CreateToggleRow("VibrationToggle", window.transform, "Vibration", yPos);
            yPos -= 70;

            var postProcessingToggle = CreateToggleRow("PostProcessingToggle", window.transform, "Post Processing", yPos);
            yPos -= 80;

            Button resetBtn = CreateButton("ResetButton", window.transform, "RESET TO DEFAULTS", new Vector2(0, yPos));
            RectTransform resetRect = resetBtn.GetComponent<RectTransform>();
            resetRect.sizeDelta = new Vector2(280, 50);

            Button closeBtn = CreateButton("CloseButton", window.transform, "X", new Vector2(185, 235));
            RectTransform closeRect = closeBtn.GetComponent<RectTransform>();
            closeRect.sizeDelta = new Vector2(50, 50);
            closeBtn.GetComponent<Image>().color = new Color(0.6f, 0.2f, 0.2f, 1f);

            Settings.SettingsUI settingsUI = panel.AddComponent<Settings.SettingsUI>();
            SerializedObject so = new SerializedObject(settingsUI);
            so.FindProperty("settingsPanel").objectReferenceValue = panel;
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.FindProperty("musicSlider").objectReferenceValue = musicSlider.slider;
            so.FindProperty("musicFillImage").objectReferenceValue = musicSlider.fill;
            so.FindProperty("sfxSlider").objectReferenceValue = sfxSlider.slider;
            so.FindProperty("sfxFillImage").objectReferenceValue = sfxSlider.fill;
            so.FindProperty("vibrationToggle").objectReferenceValue = vibrationToggle.toggle;
            so.FindProperty("vibrationCheckmark").objectReferenceValue = vibrationToggle.checkmark;
            so.FindProperty("postProcessingToggle").objectReferenceValue = postProcessingToggle.toggle;
            so.FindProperty("postProcessingCheckmark").objectReferenceValue = postProcessingToggle.checkmark;
            so.FindProperty("closeButton").objectReferenceValue = closeBtn;
            so.FindProperty("resetButton").objectReferenceValue = resetBtn;
            so.ApplyModifiedProperties();

            return panel;
        }

        private void CreateSettingsButton(GameObject settingsPanel)
        {
            if (mainMenuPanel == null)
            {
                Debug.LogWarning("Main Menu Panel not assigned. Settings button not created.");
                return;
            }

            GameObject btnObj = CreateUIObject("SettingsButton", mainMenuPanel.transform);
            RectTransform btnRect = btnObj.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(1f, 1f);
            btnRect.anchorMax = new Vector2(1f, 1f);
            btnRect.pivot = new Vector2(1f, 1f);
            btnRect.anchoredPosition = new Vector2(-20, -20);
            btnRect.sizeDelta = new Vector2(60, 60);

            Image btnImage = btnObj.AddComponent<Image>();
            btnImage.color = new Color(0.3f, 0.3f, 0.3f, 0.9f);

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = btnImage;

            ColorBlock colors = btn.colors;
            colors.highlightedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            colors.pressedColor = new Color(0.25f, 0.25f, 0.25f, 1f);
            btn.colors = colors;

            GameObject iconText = CreateTextObject("Icon", btnObj.transform, "⚙", 36, Color.white);
            RectTransform iconRect = iconText.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            iconText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            Settings.SettingsUI settingsUI = settingsPanel.GetComponent<Settings.SettingsUI>();
            if (settingsUI != null)
            {
                UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, settingsUI.Show);
            }
        }

        private (Slider slider, Image fill) CreateSliderRow(string name, Transform parent, string label, float yPos)
        {
            GameObject row = CreateUIObject(name + "Row", parent);
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0, 1f);
            rowRect.anchorMax = new Vector2(1, 1f);
            rowRect.pivot = new Vector2(0.5f, 1f);
            rowRect.anchoredPosition = new Vector2(0, yPos);
            rowRect.sizeDelta = new Vector2(0, 80);

            GameObject labelObj = CreateTextObject("Label", row.transform, label, 24, Color.white);
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0.5f);
            labelRect.anchorMax = new Vector2(1, 1f);
            labelRect.offsetMin = new Vector2(40, 0);
            labelRect.offsetMax = new Vector2(-40, -5);
            labelObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

            GameObject sliderObj = CreateUIObject(name, row.transform);
            RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0, 0);
            sliderRect.anchorMax = new Vector2(1, 0.5f);
            sliderRect.offsetMin = new Vector2(40, 10);
            sliderRect.offsetMax = new Vector2(-40, 0);

            GameObject background = CreateUIObject("Background", sliderObj.transform);
            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            GameObject fillArea = CreateUIObject("Fill Area", sliderObj.transform);
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = new Vector2(5, 5);
            fillAreaRect.offsetMax = new Vector2(-5, -5);

            GameObject fill = CreateUIObject("Fill", fillArea.transform);
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = new Color(0.3f, 0.7f, 1f, 1f);

            GameObject handleArea = CreateUIObject("Handle Slide Area", sliderObj.transform);
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(10, 0);
            handleAreaRect.offsetMax = new Vector2(-10, 0);

            GameObject handle = CreateUIObject("Handle", handleArea.transform);
            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 0);
            Image handleImage = handle.AddComponent<Image>();
            handleImage.color = Color.white;

            Slider slider = sliderObj.AddComponent<Slider>();
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;

            return (slider, fillImage);
        }

        private (Toggle toggle, Image checkmark) CreateToggleRow(string name, Transform parent, string label, float yPos)
        {
            GameObject row = CreateUIObject(name + "Row", parent);
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0, 1f);
            rowRect.anchorMax = new Vector2(1, 1f);
            rowRect.pivot = new Vector2(0.5f, 1f);
            rowRect.anchoredPosition = new Vector2(0, yPos);
            rowRect.sizeDelta = new Vector2(0, 50);

            GameObject labelObj = CreateTextObject("Label", row.transform, label, 24, Color.white);
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(0.7f, 1f);
            labelRect.offsetMin = new Vector2(40, 0);
            labelRect.offsetMax = Vector2.zero;
            labelObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

            GameObject toggleObj = CreateUIObject(name, row.transform);
            RectTransform toggleRect = toggleObj.GetComponent<RectTransform>();
            toggleRect.anchorMin = new Vector2(1f, 0.5f);
            toggleRect.anchorMax = new Vector2(1f, 0.5f);
            toggleRect.pivot = new Vector2(1f, 0.5f);
            toggleRect.anchoredPosition = new Vector2(-40, 0);
            toggleRect.sizeDelta = new Vector2(50, 50);

            GameObject background = CreateUIObject("Background", toggleObj.transform);
            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            GameObject checkmark = CreateUIObject("Checkmark", background.transform);
            RectTransform checkRect = checkmark.GetComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0.5f, 0.5f);
            checkRect.anchorMax = new Vector2(0.5f, 0.5f);
            checkRect.pivot = new Vector2(0.5f, 0.5f);
            checkRect.anchoredPosition = Vector2.zero;
            checkRect.sizeDelta = new Vector2(30, 30);
            Image checkImage = checkmark.AddComponent<Image>();
            checkImage.color = new Color(0.3f, 0.8f, 0.3f, 1f);

            Toggle toggle = toggleObj.AddComponent<Toggle>();
            toggle.targetGraphic = bgImage;
            toggle.graphic = checkImage;
            toggle.isOn = true;

            return (toggle, checkImage);
        }

        private GameObject CreatePanel(string name, Transform parent)
        {
            GameObject panel = CreateUIObject(name, parent);
            Image image = panel.AddComponent<Image>();
            image.color = Color.white;
            return panel;
        }

        private Button CreateButton(string name, Transform parent, string text, Vector2 position)
        {
            GameObject btnObj = CreateUIObject(name, parent);
            RectTransform btnRect = btnObj.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, 0.5f);
            btnRect.anchorMax = new Vector2(0.5f, 0.5f);
            btnRect.pivot = new Vector2(0.5f, 0.5f);
            btnRect.anchoredPosition = position;
            btnRect.sizeDelta = new Vector2(280, 60);

            Image btnImage = btnObj.AddComponent<Image>();
            btnImage.color = new Color(0.25f, 0.25f, 0.25f, 1f);

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = btnImage;

            ColorBlock colors = btn.colors;
            colors.highlightedColor = new Color(0.35f, 0.35f, 0.35f, 1f);
            colors.pressedColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            btn.colors = colors;

            GameObject textObj = CreateTextObject("Text", btnObj.transform, text, 24, Color.white);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            textObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            return btn;
        }

        private GameObject CreateUIObject(string name, Transform parent)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.AddComponent<RectTransform>();
            Undo.RegisterCreatedObjectUndo(obj, "Create " + name);
            return obj;
        }

        private GameObject CreateTextObject(string name, Transform parent, string text, int fontSize, Color color)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.AddComponent<RectTransform>();

            TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.raycastTarget = false;

            Undo.RegisterCreatedObjectUndo(obj, "Create " + name);
            return obj;
        }
    }
}
