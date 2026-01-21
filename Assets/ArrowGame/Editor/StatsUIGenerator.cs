using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace ArrowGame.Editor
{
    public class StatsUIGenerator : EditorWindow
    {
        private Canvas targetCanvas;
        private GameObject mainMenuPanel;

        [MenuItem("ArrowGame/Generate Statistics UI")]
        public static void ShowWindow()
        {
            GetWindow<StatsUIGenerator>("Statistics UI Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Statistics UI Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            targetCanvas = (Canvas)EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true);
            mainMenuPanel = (GameObject)EditorGUILayout.ObjectField("Main Menu Panel", mainMenuPanel, typeof(GameObject), true);

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Generate Statistics UI", GUILayout.Height(40)))
            {
                Generate();
            }
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

            Undo.SetCurrentGroupName("Generate Statistics UI");
            int undoGroup = Undo.GetCurrentGroup();

            CreateStatisticsManager();
            CreateStatsPanel();
            CreateStatsButton();

            Undo.CollapseUndoOperations(undoGroup);
            Debug.Log("Statistics UI generated!");
        }

        private void CreateStatisticsManager()
        {
            if (FindObjectOfType<Stats.StatisticsManager>() != null) return;

            GameObject manager = new GameObject("StatisticsManager");
            manager.AddComponent<Stats.StatisticsManager>();
            Undo.RegisterCreatedObjectUndo(manager, "Create StatisticsManager");
        }

        private void CreateStatsPanel()
        {
            GameObject panel = new GameObject("StatisticsPanel");
            panel.transform.SetParent(targetCanvas.transform, false);
            panel.SetActive(false);

            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.9f);

            CanvasGroup cg = panel.AddComponent<CanvasGroup>();

            GameObject window = new GameObject("Window");
            window.transform.SetParent(panel.transform, false);
            RectTransform windowRect = window.AddComponent<RectTransform>();
            windowRect.anchorMin = new Vector2(0.5f, 0.5f);
            windowRect.anchorMax = new Vector2(0.5f, 0.5f);
            windowRect.pivot = new Vector2(0.5f, 0.5f);
            windowRect.sizeDelta = new Vector2(420, 550);
            Image windowImage = window.AddComponent<Image>();
            windowImage.color = new Color(0.1f, 0.1f, 0.1f, 0.98f);

            GameObject title = CreateText("Title", window.transform, "STATISTICS", 28, Color.white, TextAlignmentOptions.Center);
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, -15);
            titleRect.sizeDelta = new Vector2(0, 40);

            Button closeBtn = CreateButton("CloseButton", window.transform, "X", new Color(0.5f, 0.2f, 0.2f));
            RectTransform closeBtnRect = closeBtn.GetComponent<RectTransform>();
            closeBtnRect.anchorMin = new Vector2(1, 1);
            closeBtnRect.anchorMax = new Vector2(1, 1);
            closeBtnRect.pivot = new Vector2(1, 1);
            closeBtnRect.anchoredPosition = new Vector2(-10, -10);
            closeBtnRect.sizeDelta = new Vector2(40, 40);

            float yPos = -70;
            float spacing = 32;

            var gamesPlayed = CreateStatRow(window.transform, "Games Played", ref yPos, spacing);
            var totalScore = CreateStatRow(window.transform, "Total Score", ref yPos, spacing);
            var highScore = CreateStatRow(window.transform, "High Score", ref yPos, spacing);
            var playTime = CreateStatRow(window.transform, "Play Time", ref yPos, spacing);

            yPos -= 15;
            var totalRings = CreateStatRow(window.transform, "Rings Passed", ref yPos, spacing);
            var coreHits = CreateStatRow(window.transform, "Core Hits", ref yPos, spacing);
            var innerHits = CreateStatRow(window.transform, "Inner Hits", ref yPos, spacing);
            var middleHits = CreateStatRow(window.transform, "Middle Hits", ref yPos, spacing);
            var outerHits = CreateStatRow(window.transform, "Outer Hits", ref yPos, spacing);
            var misses = CreateStatRow(window.transform, "Misses", ref yPos, spacing);
            var accuracy = CreateStatRow(window.transform, "Accuracy", ref yPos, spacing);

            yPos -= 15;
            var longestStreak = CreateStatRow(window.transform, "Longest Streak", ref yPos, spacing);
            var highestMult = CreateStatRow(window.transform, "Highest Multiplier", ref yPos, spacing);
            var perfectRuns = CreateStatRow(window.transform, "Perfect Runs", ref yPos, spacing);

            Stats.StatisticsUI statsUI = panel.AddComponent<Stats.StatisticsUI>();
            SerializedObject so = new SerializedObject(statsUI);
            so.FindProperty("panel").objectReferenceValue = panel;
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.FindProperty("gamesPlayedText").objectReferenceValue = gamesPlayed;
            so.FindProperty("totalScoreText").objectReferenceValue = totalScore;
            so.FindProperty("highScoreText").objectReferenceValue = highScore;
            so.FindProperty("playTimeText").objectReferenceValue = playTime;
            so.FindProperty("totalRingsText").objectReferenceValue = totalRings;
            so.FindProperty("coreHitsText").objectReferenceValue = coreHits;
            so.FindProperty("innerHitsText").objectReferenceValue = innerHits;
            so.FindProperty("middleHitsText").objectReferenceValue = middleHits;
            so.FindProperty("outerHitsText").objectReferenceValue = outerHits;
            so.FindProperty("missesText").objectReferenceValue = misses;
            so.FindProperty("accuracyText").objectReferenceValue = accuracy;
            so.FindProperty("longestStreakText").objectReferenceValue = longestStreak;
            so.FindProperty("highestMultiplierText").objectReferenceValue = highestMult;
            so.FindProperty("perfectRunsText").objectReferenceValue = perfectRuns;
            so.FindProperty("closeButton").objectReferenceValue = closeBtn;
            so.ApplyModifiedProperties();

            Undo.RegisterCreatedObjectUndo(panel, "Create Statistics Panel");
        }

        private TextMeshProUGUI CreateStatRow(Transform parent, string label, ref float yPos, float spacing)
        {
            GameObject row = new GameObject($"Row_{label.Replace(" ", "")}");
            row.transform.SetParent(parent, false);
            RectTransform rowRect = row.AddComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0, 1);
            rowRect.anchorMax = new Vector2(1, 1);
            rowRect.pivot = new Vector2(0.5f, 1);
            rowRect.anchoredPosition = new Vector2(0, yPos);
            rowRect.sizeDelta = new Vector2(-40, 28);

            GameObject labelObj = CreateText("Label", row.transform, label, 16, new Color(0.7f, 0.7f, 0.7f), TextAlignmentOptions.Left);
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(0.6f, 1);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            GameObject valueObj = CreateText("Value", row.transform, "0", 16, Color.white, TextAlignmentOptions.Right);
            RectTransform valueRect = valueObj.GetComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(0.6f, 0);
            valueRect.anchorMax = new Vector2(1, 1);
            valueRect.offsetMin = Vector2.zero;
            valueRect.offsetMax = Vector2.zero;

            yPos -= spacing;

            return valueObj.GetComponent<TextMeshProUGUI>();
        }

        private void CreateStatsButton()
        {
            if (mainMenuPanel == null) return;

            GameObject btnObj = new GameObject("StatsButton");
            btnObj.transform.SetParent(mainMenuPanel.transform, false);

            RectTransform btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0, 1);
            btnRect.anchorMax = new Vector2(0, 1);
            btnRect.pivot = new Vector2(0, 1);
            btnRect.anchoredPosition = new Vector2(160, -20);
            btnRect.sizeDelta = new Vector2(60, 60);

            Image btnImage = btnObj.AddComponent<Image>();
            btnImage.color = new Color(0.3f, 0.3f, 0.3f, 0.9f);

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = btnImage;

            GameObject iconText = CreateText("Icon", btnObj.transform, "📊", 28, Color.white, TextAlignmentOptions.Center);
            RectTransform iconRect = iconText.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            Stats.StatisticsUI statsUI = FindObjectOfType<Stats.StatisticsUI>();
            if (statsUI != null)
            {
                UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, statsUI.Show);
            }

            Undo.RegisterCreatedObjectUndo(btnObj, "Create Stats Button");
        }

        private GameObject CreateText(string name, Transform parent, string text, int fontSize, Color color, TextAlignmentOptions alignment)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.AddComponent<RectTransform>();

            TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = alignment;
            tmp.raycastTarget = false;

            return obj;
        }

        private Button CreateButton(string name, Transform parent, string text, Color bgColor)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            btnObj.AddComponent<RectTransform>();

            Image btnImage = btnObj.AddComponent<Image>();
            btnImage.color = bgColor;

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = btnImage;

            GameObject textObj = CreateText("Text", btnObj.transform, text, 20, Color.white, TextAlignmentOptions.Center);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            return btn;
        }
    }
}
