using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace ArrowGame.Editor
{
    public class GameUIGenerator : EditorWindow
    {
        private Canvas targetCanvas;
        private GameObject gameHUD;
        
        private bool createCountdown = true;
        private bool createPause = true;
        private bool createResults = true;
        private bool createScreenFader = true;

        [MenuItem("ArrowGame/Generate Game UI (Iteration 9)")]
        public static void ShowWindow()
        {
            GetWindow<GameUIGenerator>("Game UI Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Game UI Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            targetCanvas = (Canvas)EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true);
            gameHUD = (GameObject)EditorGUILayout.ObjectField("Game HUD (for Pause Button)", gameHUD, typeof(GameObject), true);

            EditorGUILayout.Space(10);
            GUILayout.Label("What to create:", EditorStyles.boldLabel);
            createCountdown = EditorGUILayout.Toggle("Countdown Panel", createCountdown);
            createPause = EditorGUILayout.Toggle("Pause Panel", createPause);
            createResults = EditorGUILayout.Toggle("Results Screen", createResults);
            createScreenFader = EditorGUILayout.Toggle("Screen Fader", createScreenFader);

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Generate UI", GUILayout.Height(40)))
            {
                GenerateUI();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox(
                "Creates:\n" +
                "• CountdownPanel (3, 2, 1, GO!)\n" +
                "• PausePanel (Resume, Restart, Menu buttons)\n" +
                "• ResultsPanel (Score, Stats, Buttons)\n" +
                "• ScreenFader (Black fade overlay)\n\n" +
                "All components will be auto-configured.",
                MessageType.Info);
        }

        private void GenerateUI()
        {
            if (targetCanvas == null)
            {
                targetCanvas = FindObjectOfType<Canvas>();
                if (targetCanvas == null)
                {
                    Debug.LogError("No Canvas found! Create a Canvas first.");
                    return;
                }
            }

            Undo.SetCurrentGroupName("Generate Game UI");
            int undoGroup = Undo.GetCurrentGroup();

            if (createCountdown) CreateCountdownPanel();
            if (createPause) CreatePausePanel();
            if (createResults) CreateResultsPanel();
            if (createScreenFader) CreateScreenFader();

            Undo.CollapseUndoOperations(undoGroup);

            Debug.Log("Game UI generated successfully!");
        }

        private void CreateCountdownPanel()
        {
            GameObject panel = CreatePanel("CountdownPanel", targetCanvas.transform);
            panel.SetActive(false);
            
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.5f);

            CanvasGroup cg = panel.AddComponent<CanvasGroup>();

            GameObject countdownText = CreateTextObject("CountdownText", panel.transform, "3", 200, Color.white);
            RectTransform textRect = countdownText.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = new Vector2(400, 250);
            
            TextMeshProUGUI tmp = countdownText.GetComponent<TextMeshProUGUI>();
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;

            UI.CountdownController controller = panel.AddComponent<UI.CountdownController>();
            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("countdownPanel").objectReferenceValue = panel;
            so.FindProperty("countdownText").objectReferenceValue = tmp;
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.ApplyModifiedProperties();
        }

        private void CreatePausePanel()
        {
            GameObject panel = CreatePanel("PausePanel", targetCanvas.transform);
            panel.SetActive(false);
            
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.7f);

            CanvasGroup cg = panel.AddComponent<CanvasGroup>();

            GameObject window = CreatePanel("Window", panel.transform);
            RectTransform windowRect = window.GetComponent<RectTransform>();
            windowRect.anchorMin = new Vector2(0.5f, 0.5f);
            windowRect.anchorMax = new Vector2(0.5f, 0.5f);
            windowRect.pivot = new Vector2(0.5f, 0.5f);
            windowRect.anchoredPosition = Vector2.zero;
            windowRect.sizeDelta = new Vector2(400, 450);
            
            Image windowImage = window.GetComponent<Image>();
            windowImage.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

            GameObject title = CreateTextObject("Title", window.transform, "PAUSED", 48, Color.white);
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0, -30);
            titleRect.sizeDelta = new Vector2(350, 60);
            title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            Button resumeBtn = CreateButton("ResumeButton", window.transform, "RESUME", new Vector2(0, 30));
            Button restartBtn = CreateButton("RestartButton", window.transform, "RESTART", new Vector2(0, -50));
            Button menuBtn = CreateButton("MainMenuButton", window.transform, "MAIN MENU", new Vector2(0, -130));

            Button pauseBtn = null;
            if (gameHUD != null)
            {
                GameObject pauseBtnObj = CreateUIObject("PauseButton", gameHUD.transform);
                RectTransform pauseBtnRect = pauseBtnObj.GetComponent<RectTransform>();
                pauseBtnRect.anchorMin = new Vector2(1f, 1f);
                pauseBtnRect.anchorMax = new Vector2(1f, 1f);
                pauseBtnRect.pivot = new Vector2(1f, 1f);
                pauseBtnRect.anchoredPosition = new Vector2(-20, -20);
                pauseBtnRect.sizeDelta = new Vector2(60, 60);

                Image pauseBtnImage = pauseBtnObj.AddComponent<Image>();
                pauseBtnImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
                
                pauseBtn = pauseBtnObj.AddComponent<Button>();
                pauseBtn.targetGraphic = pauseBtnImage;

                GameObject pauseIcon = CreateTextObject("Icon", pauseBtnObj.transform, "||", 32, Color.white);
                RectTransform iconRect = pauseIcon.GetComponent<RectTransform>();
                iconRect.anchorMin = Vector2.zero;
                iconRect.anchorMax = Vector2.one;
                iconRect.offsetMin = Vector2.zero;
                iconRect.offsetMax = Vector2.zero;
                pauseIcon.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            }

            UI.PauseController controller = panel.AddComponent<UI.PauseController>();
            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("pausePanel").objectReferenceValue = panel;
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.FindProperty("resumeButton").objectReferenceValue = resumeBtn;
            so.FindProperty("restartButton").objectReferenceValue = restartBtn;
            so.FindProperty("mainMenuButton").objectReferenceValue = menuBtn;
            if (pauseBtn != null)
            {
                so.FindProperty("pauseButton").objectReferenceValue = pauseBtn;
            }
            so.ApplyModifiedProperties();
        }

        private void CreateResultsPanel()
        {
            GameObject panel = CreatePanel("ResultsPanel", targetCanvas.transform);
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
            windowRect.sizeDelta = new Vector2(450, 700);
            
            Image windowImage = window.GetComponent<Image>();
            windowImage.color = new Color(0.12f, 0.12f, 0.12f, 0.95f);

            GameObject title = CreateTextObject("Title", window.transform, "GAME OVER", 42, Color.white);
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0, -20);
            titleRect.sizeDelta = new Vector2(400, 50);
            title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            GameObject finalScore = CreateTextObject("FinalScoreText", window.transform, "0", 72, Color.yellow);
            RectTransform finalScoreRect = finalScore.GetComponent<RectTransform>();
            finalScoreRect.anchorMin = new Vector2(0.5f, 1f);
            finalScoreRect.anchorMax = new Vector2(0.5f, 1f);
            finalScoreRect.pivot = new Vector2(0.5f, 1f);
            finalScoreRect.anchoredPosition = new Vector2(0, -75);
            finalScoreRect.sizeDelta = new Vector2(400, 80);
            TextMeshProUGUI finalScoreTMP = finalScore.GetComponent<TextMeshProUGUI>();
            finalScoreTMP.alignment = TextAlignmentOptions.Center;
            finalScoreTMP.fontStyle = FontStyles.Bold;

            GameObject highScore = CreateTextObject("HighScoreText", window.transform, "BEST: 0", 28, new Color(0.7f, 0.7f, 0.7f));
            RectTransform highScoreRect = highScore.GetComponent<RectTransform>();
            highScoreRect.anchorMin = new Vector2(0.5f, 1f);
            highScoreRect.anchorMax = new Vector2(0.5f, 1f);
            highScoreRect.pivot = new Vector2(0.5f, 1f);
            highScoreRect.anchoredPosition = new Vector2(0, -155);
            highScoreRect.sizeDelta = new Vector2(300, 35);
            highScore.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            GameObject newHighScoreBadge = CreateTextObject("NewHighScoreBadge", window.transform, "NEW RECORD!", 24, Color.green);
            RectTransform badgeRect = newHighScoreBadge.GetComponent<RectTransform>();
            badgeRect.anchorMin = new Vector2(0.5f, 1f);
            badgeRect.anchorMax = new Vector2(0.5f, 1f);
            badgeRect.pivot = new Vector2(0.5f, 1f);
            badgeRect.anchoredPosition = new Vector2(0, -190);
            badgeRect.sizeDelta = new Vector2(200, 30);
            newHighScoreBadge.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            newHighScoreBadge.SetActive(false);

            GameObject statsContainer = CreateUIObject("StatsContainer", window.transform);
            RectTransform statsRect = statsContainer.GetComponent<RectTransform>();
            statsRect.anchorMin = new Vector2(0f, 0.5f);
            statsRect.anchorMax = new Vector2(1f, 0.5f);
            statsRect.pivot = new Vector2(0.5f, 0.5f);
            statsRect.anchoredPosition = new Vector2(0, -30);
            statsRect.sizeDelta = new Vector2(0, 250);

            float yPos = 100;
            float yStep = 32;
            
            GameObject bestStreak = CreateStatRow("BestStreakText", statsContainer.transform, "Best Streak:", "0", yPos);
            yPos -= yStep;
            GameObject coreHits = CreateStatRow("CoreHitsText", statsContainer.transform, "Core:", "0", yPos);
            yPos -= yStep;
            GameObject innerHits = CreateStatRow("InnerHitsText", statsContainer.transform, "Inner:", "0", yPos);
            yPos -= yStep;
            GameObject middleHits = CreateStatRow("MiddleHitsText", statsContainer.transform, "Middle:", "0", yPos);
            yPos -= yStep;
            GameObject outerHits = CreateStatRow("OuterHitsText", statsContainer.transform, "Outer:", "0", yPos);
            yPos -= yStep;
            GameObject misses = CreateStatRow("MissesText", statsContainer.transform, "Misses:", "0", yPos);
            yPos -= yStep;
            GameObject totalRings = CreateStatRow("TotalRingsText", statsContainer.transform, "Total Rings:", "0", yPos);
            yPos -= yStep;
            GameObject accuracy = CreateStatRow("AccuracyText", statsContainer.transform, "Accuracy:", "0%", yPos);

            Button restartBtn = CreateButton("RestartButton", window.transform, "PLAY AGAIN", new Vector2(0, -290));
            Button menuBtn = CreateButton("MainMenuButton", window.transform, "MAIN MENU", new Vector2(0, -370));

            UI.ResultsScreen resultsScreen = panel.AddComponent<UI.ResultsScreen>();
            SerializedObject so = new SerializedObject(resultsScreen);
            so.FindProperty("resultsPanel").objectReferenceValue = panel;
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.FindProperty("finalScoreText").objectReferenceValue = finalScoreTMP;
            so.FindProperty("highScoreText").objectReferenceValue = highScore.GetComponent<TextMeshProUGUI>();
            so.FindProperty("newHighScoreBadge").objectReferenceValue = newHighScoreBadge;
            so.FindProperty("bestStreakText").objectReferenceValue = bestStreak.transform.Find("Value").GetComponent<TextMeshProUGUI>();
            so.FindProperty("coreHitsText").objectReferenceValue = coreHits.transform.Find("Value").GetComponent<TextMeshProUGUI>();
            so.FindProperty("innerHitsText").objectReferenceValue = innerHits.transform.Find("Value").GetComponent<TextMeshProUGUI>();
            so.FindProperty("middleHitsText").objectReferenceValue = middleHits.transform.Find("Value").GetComponent<TextMeshProUGUI>();
            so.FindProperty("outerHitsText").objectReferenceValue = outerHits.transform.Find("Value").GetComponent<TextMeshProUGUI>();
            so.FindProperty("missesText").objectReferenceValue = misses.transform.Find("Value").GetComponent<TextMeshProUGUI>();
            so.FindProperty("totalRingsText").objectReferenceValue = totalRings.transform.Find("Value").GetComponent<TextMeshProUGUI>();
            so.FindProperty("accuracyText").objectReferenceValue = accuracy.transform.Find("Value").GetComponent<TextMeshProUGUI>();
            so.FindProperty("restartButton").objectReferenceValue = restartBtn;
            so.FindProperty("mainMenuButton").objectReferenceValue = menuBtn;
            so.ApplyModifiedProperties();
        }

        private void CreateScreenFader()
        {
            GameObject faderObj = CreateUIObject("ScreenFader", targetCanvas.transform);
            
            RectTransform rect = faderObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = faderObj.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0);
            image.raycastTarget = false;

            UI.ScreenFader fader = faderObj.AddComponent<UI.ScreenFader>();
            SerializedObject so = new SerializedObject(fader);
            so.FindProperty("fadeImage").objectReferenceValue = image;
            so.ApplyModifiedProperties();

            faderObj.transform.SetAsLastSibling();
        }

        private GameObject CreateStatRow(string name, Transform parent, string label, string value, float yPos)
        {
            GameObject row = CreateUIObject(name, parent);
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0, 0.5f);
            rowRect.anchorMax = new Vector2(1, 0.5f);
            rowRect.pivot = new Vector2(0.5f, 0.5f);
            rowRect.anchoredPosition = new Vector2(0, yPos);
            rowRect.sizeDelta = new Vector2(0, 30);

            GameObject labelObj = CreateTextObject("Label", row.transform, label, 22, new Color(0.7f, 0.7f, 0.7f));
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(0.6f, 1);
            labelRect.offsetMin = new Vector2(40, 0);
            labelRect.offsetMax = Vector2.zero;
            labelObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Left;

            GameObject valueObj = CreateTextObject("Value", row.transform, value, 22, Color.white);
            RectTransform valueRect = valueObj.GetComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(0.6f, 0);
            valueRect.anchorMax = new Vector2(1, 1);
            valueRect.offsetMin = Vector2.zero;
            valueRect.offsetMax = new Vector2(-40, 0);
            valueObj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;

            return row;
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

            GameObject textObj = CreateTextObject("Text", btnObj.transform, text, 28, Color.white);
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
