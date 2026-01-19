using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace ArrowGame.Editor
{
    public class ScoreUIGenerator : EditorWindow
    {
        private Canvas targetCanvas;
        private GameObject gameHUD;
        
        private Color scoreColor = Color.white;
        private Color highScoreColor = new Color(0.7f, 0.7f, 0.7f);
        private Color streakColor = Color.white;
        private Color multiplierColor = Color.yellow;
        private Color popupColor = Color.yellow;
        
        private int scoreFontSize = 72;
        private int highScoreFontSize = 28;
        private int streakFontSize = 48;
        private int multiplierFontSize = 36;
        private int popupFontSize = 32;

        [MenuItem("ArrowGame/Generate Score UI")]
        public static void ShowWindow()
        {
            GetWindow<ScoreUIGenerator>("Score UI Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Score & Streak UI Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            targetCanvas = (Canvas)EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true);
            gameHUD = (GameObject)EditorGUILayout.ObjectField("Game HUD (optional)", gameHUD, typeof(GameObject), true);

            EditorGUILayout.Space(10);
            GUILayout.Label("Colors", EditorStyles.boldLabel);
            scoreColor = EditorGUILayout.ColorField("Score Color", scoreColor);
            highScoreColor = EditorGUILayout.ColorField("High Score Color", highScoreColor);
            streakColor = EditorGUILayout.ColorField("Streak Color", streakColor);
            multiplierColor = EditorGUILayout.ColorField("Multiplier Color", multiplierColor);
            popupColor = EditorGUILayout.ColorField("Popup Color", popupColor);

            EditorGUILayout.Space(10);
            GUILayout.Label("Font Sizes", EditorStyles.boldLabel);
            scoreFontSize = EditorGUILayout.IntField("Score", scoreFontSize);
            highScoreFontSize = EditorGUILayout.IntField("High Score", highScoreFontSize);
            streakFontSize = EditorGUILayout.IntField("Streak", streakFontSize);
            multiplierFontSize = EditorGUILayout.IntField("Multiplier", multiplierFontSize);
            popupFontSize = EditorGUILayout.IntField("Popup", popupFontSize);

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Generate Score UI", GUILayout.Height(40)))
            {
                GenerateUI();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox(
                "This will create:\n" +
                "• ScoreContainer (ScoreText, HighScoreText, PointsPopup)\n" +
                "• StreakContainer (StreakText, MultiplierText, StreakFillBar, GlowImage)\n" +
                "• StreakEffects (ScreenPulseImage, StreakVignetteImage)\n\n" +
                "Components ScoreDisplay, StreakDisplay, StreakVisualEffects will be added automatically.",
                MessageType.Info);
        }

        private void GenerateUI()
        {
            if (targetCanvas == null)
            {
                targetCanvas = FindObjectOfType<Canvas>();
                if (targetCanvas == null)
                {
                    Debug.LogError("No Canvas found! Please create a Canvas first or assign one.");
                    return;
                }
            }

            Transform parent = gameHUD != null ? gameHUD.transform : targetCanvas.transform;

            Undo.SetCurrentGroupName("Generate Score UI");
            int undoGroup = Undo.GetCurrentGroup();

            CreateScoreContainer(parent);
            CreateStreakContainer(parent);
            CreateStreakEffects(targetCanvas.transform);

            Undo.CollapseUndoOperations(undoGroup);

            Debug.Log("Score UI generated successfully!");
        }

        private void CreateScoreContainer(Transform parent)
        {
            GameObject container = CreateUIObject("ScoreContainer", parent);
            RectTransform containerRect = container.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 1f);
            containerRect.anchorMax = new Vector2(0.5f, 1f);
            containerRect.pivot = new Vector2(0.5f, 1f);
            containerRect.anchoredPosition = new Vector2(0, -20);
            containerRect.sizeDelta = new Vector2(400, 150);

            GameObject scoreTextObj = CreateTextObject("ScoreText", container.transform, "0", scoreFontSize, scoreColor);
            RectTransform scoreRect = scoreTextObj.GetComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0.5f, 1f);
            scoreRect.anchorMax = new Vector2(0.5f, 1f);
            scoreRect.pivot = new Vector2(0.5f, 1f);
            scoreRect.anchoredPosition = new Vector2(0, 0);
            scoreRect.sizeDelta = new Vector2(400, 80);
            TextMeshProUGUI scoreTMP = scoreTextObj.GetComponent<TextMeshProUGUI>();
            scoreTMP.alignment = TextAlignmentOptions.Center;
            scoreTMP.fontStyle = FontStyles.Bold;

            GameObject highScoreTextObj = CreateTextObject("HighScoreText", container.transform, "BEST: 0", highScoreFontSize, highScoreColor);
            RectTransform highScoreRect = highScoreTextObj.GetComponent<RectTransform>();
            highScoreRect.anchorMin = new Vector2(0.5f, 1f);
            highScoreRect.anchorMax = new Vector2(0.5f, 1f);
            highScoreRect.pivot = new Vector2(0.5f, 1f);
            highScoreRect.anchoredPosition = new Vector2(0, -80);
            highScoreRect.sizeDelta = new Vector2(300, 40);
            TextMeshProUGUI highScoreTMP = highScoreTextObj.GetComponent<TextMeshProUGUI>();
            highScoreTMP.alignment = TextAlignmentOptions.Center;

            GameObject popupTextObj = CreateTextObject("PointsPopup", container.transform, "+100", popupFontSize, popupColor);
            RectTransform popupRect = popupTextObj.GetComponent<RectTransform>();
            popupRect.anchorMin = new Vector2(0.5f, 0.5f);
            popupRect.anchorMax = new Vector2(0.5f, 0.5f);
            popupRect.pivot = new Vector2(0.5f, 0.5f);
            popupRect.anchoredPosition = new Vector2(0, -30);
            popupRect.sizeDelta = new Vector2(200, 50);
            TextMeshProUGUI popupTMP = popupTextObj.GetComponent<TextMeshProUGUI>();
            popupTMP.alignment = TextAlignmentOptions.Center;
            popupTMP.alpha = 0f;

            Score.ScoreDisplay scoreDisplay = container.AddComponent<Score.ScoreDisplay>();
            SerializedObject so = new SerializedObject(scoreDisplay);
            so.FindProperty("scoreText").objectReferenceValue = scoreTMP;
            so.FindProperty("highScoreText").objectReferenceValue = highScoreTMP;
            so.FindProperty("pointsPopupText").objectReferenceValue = popupTMP;
            so.ApplyModifiedProperties();
        }

        private void CreateStreakContainer(Transform parent)
        {
            GameObject container = CreateUIObject("StreakContainer", parent);
            RectTransform containerRect = container.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0, 0);
            containerRect.anchorMax = new Vector2(0, 0);
            containerRect.pivot = new Vector2(0, 0);
            containerRect.anchoredPosition = new Vector2(20, 20);
            containerRect.sizeDelta = new Vector2(200, 100);

            GameObject glowObj = CreateUIObject("GlowImage", container.transform);
            Image glowImage = glowObj.AddComponent<Image>();
            glowImage.color = new Color(1f, 1f, 0f, 0f);
            glowImage.raycastTarget = false;
            RectTransform glowRect = glowObj.GetComponent<RectTransform>();
            glowRect.anchorMin = new Vector2(0, 0.5f);
            glowRect.anchorMax = new Vector2(0, 0.5f);
            glowRect.pivot = new Vector2(0, 0.5f);
            glowRect.anchoredPosition = new Vector2(-10, 20);
            glowRect.sizeDelta = new Vector2(120, 80);

            GameObject fillBarBg = CreateUIObject("StreakFillBarBg", container.transform);
            Image fillBarBgImage = fillBarBg.AddComponent<Image>();
            fillBarBgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            fillBarBgImage.raycastTarget = false;
            RectTransform fillBarBgRect = fillBarBg.GetComponent<RectTransform>();
            fillBarBgRect.anchorMin = new Vector2(0, 0);
            fillBarBgRect.anchorMax = new Vector2(1, 0);
            fillBarBgRect.pivot = new Vector2(0, 0);
            fillBarBgRect.anchoredPosition = new Vector2(0, 0);
            fillBarBgRect.sizeDelta = new Vector2(0, 10);

            GameObject fillBarObj = CreateUIObject("StreakFillBar", fillBarBg.transform);
            Image fillBarImage = fillBarObj.AddComponent<Image>();
            fillBarImage.color = Color.cyan;
            fillBarImage.type = Image.Type.Filled;
            fillBarImage.fillMethod = Image.FillMethod.Horizontal;
            fillBarImage.fillAmount = 0f;
            fillBarImage.raycastTarget = false;
            RectTransform fillBarRect = fillBarObj.GetComponent<RectTransform>();
            fillBarRect.anchorMin = Vector2.zero;
            fillBarRect.anchorMax = Vector2.one;
            fillBarRect.offsetMin = Vector2.zero;
            fillBarRect.offsetMax = Vector2.zero;

            GameObject streakTextObj = CreateTextObject("StreakText", container.transform, "0", streakFontSize, streakColor);
            RectTransform streakRect = streakTextObj.GetComponent<RectTransform>();
            streakRect.anchorMin = new Vector2(0, 0);
            streakRect.anchorMax = new Vector2(0, 0);
            streakRect.pivot = new Vector2(0, 0);
            streakRect.anchoredPosition = new Vector2(0, 15);
            streakRect.sizeDelta = new Vector2(80, 60);
            TextMeshProUGUI streakTMP = streakTextObj.GetComponent<TextMeshProUGUI>();
            streakTMP.alignment = TextAlignmentOptions.Left;
            streakTMP.fontStyle = FontStyles.Bold;

            GameObject multiplierTextObj = CreateTextObject("MultiplierText", container.transform, "x1", multiplierFontSize, multiplierColor);
            RectTransform multiplierRect = multiplierTextObj.GetComponent<RectTransform>();
            multiplierRect.anchorMin = new Vector2(0, 0);
            multiplierRect.anchorMax = new Vector2(0, 0);
            multiplierRect.pivot = new Vector2(0, 0);
            multiplierRect.anchoredPosition = new Vector2(85, 20);
            multiplierRect.sizeDelta = new Vector2(80, 50);
            TextMeshProUGUI multiplierTMP = multiplierTextObj.GetComponent<TextMeshProUGUI>();
            multiplierTMP.alignment = TextAlignmentOptions.Left;
            multiplierTMP.fontStyle = FontStyles.Bold;

            Score.StreakDisplay streakDisplay = container.AddComponent<Score.StreakDisplay>();
            SerializedObject so = new SerializedObject(streakDisplay);
            so.FindProperty("streakText").objectReferenceValue = streakTMP;
            so.FindProperty("multiplierText").objectReferenceValue = multiplierTMP;
            so.FindProperty("streakFillBar").objectReferenceValue = fillBarImage;
            so.FindProperty("glowImage").objectReferenceValue = glowImage;
            so.ApplyModifiedProperties();
        }

        private void CreateStreakEffects(Transform canvasTransform)
        {
            GameObject container = CreateUIObject("StreakEffects", canvasTransform);
            RectTransform containerRect = container.GetComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = Vector2.zero;
            containerRect.offsetMax = Vector2.zero;

            GameObject pulseObj = CreateUIObject("ScreenPulseImage", container.transform);
            Image pulseImage = pulseObj.AddComponent<Image>();
            pulseImage.color = new Color(1f, 1f, 1f, 0f);
            pulseImage.raycastTarget = false;
            RectTransform pulseRect = pulseObj.GetComponent<RectTransform>();
            pulseRect.anchorMin = Vector2.zero;
            pulseRect.anchorMax = Vector2.one;
            pulseRect.offsetMin = Vector2.zero;
            pulseRect.offsetMax = Vector2.zero;

            GameObject vignetteObj = CreateUIObject("StreakVignetteImage", container.transform);
            Image vignetteImage = vignetteObj.AddComponent<Image>();
            vignetteImage.color = new Color(1f, 1f, 1f, 0f);
            vignetteImage.raycastTarget = false;
            RectTransform vignetteRect = vignetteObj.GetComponent<RectTransform>();
            vignetteRect.anchorMin = Vector2.zero;
            vignetteRect.anchorMax = Vector2.one;
            vignetteRect.offsetMin = Vector2.zero;
            vignetteRect.offsetMax = Vector2.zero;

            Score.StreakVisualEffects visualEffects = container.AddComponent<Score.StreakVisualEffects>();
            SerializedObject so = new SerializedObject(visualEffects);
            so.FindProperty("screenPulseImage").objectReferenceValue = pulseImage;
            so.FindProperty("streakVignetteImage").objectReferenceValue = vignetteImage;
            so.ApplyModifiedProperties();

            container.transform.SetAsLastSibling();
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
            
            RectTransform rect = obj.AddComponent<RectTransform>();
            
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
