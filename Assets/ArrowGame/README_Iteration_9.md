# Итерация 9: Базовый UI (переходы, пауза, countdown)

## Что добавлено

### Новые скрипты (папка Scripts/UI/)
- **ScreenFader.cs** — Плавные fade-переходы между экранами
- **CountdownController.cs** — Отсчёт "3, 2, 1, GO!" перед стартом
- **PauseController.cs** — Экран паузы с кнопками Resume/Restart/Menu
- **ResultsScreen.cs** — Детальная статистика после Game Over
- **UIAnimator.cs** — Компонент для анимации появления UI элементов

### Обновленные скрипты
- **UIManager.cs** — Добавлены анимированные переходы
- **GameStartController.cs** — Интеграция с countdown

---

## Настройка в Unity

### 1. Screen Fader (плавные переходы)

1. В Canvas создай Image: **FadeImage**
   - Цвет: черный
   - Alpha: 0
   - Stretch на весь экран
   - Raycast Target: OFF
   - Должен быть **последним** в иерархии (поверх всего)

2. Создай пустой GameObject: **ScreenFader**
3. Добавь компонент **ScreenFader**
4. Присвой Fade Image → FadeImage

---

### 2. Countdown Controller (отсчёт перед стартом)

1. В Canvas создай Panel: **CountdownPanel**
   - Полупрозрачный фон (опционально)
   - Добавь CanvasGroup

2. Внутри создай:
   - **CountdownText** (TextMeshPro) — большой текст по центру
     - Font Size: 150+
     - Alignment: Center

3. Добавь компонент **CountdownController** на CountdownPanel
4. Присвой:
   - Countdown Panel → CountdownPanel
   - Countdown Text → CountdownText
   - Canvas Group → CanvasGroup на CountdownPanel

5. **Выключи** CountdownPanel

---

### 3. Pause Controller (экран паузы)

1. В Canvas создай Panel: **PausePanel**
   - Добавь CanvasGroup
   - Полупрозрачный затемняющий фон

2. Внутри создай:
   ```
   PausePanel
   ├── Background (Image, темный, alpha 0.7)
   ├── Window (Panel)
   │   ├── Title (TMP): "PAUSED"
   │   ├── ResumeButton (Button): "Resume"
   │   ├── RestartButton (Button): "Restart"
   │   └── MainMenuButton (Button): "Main Menu"
   ```

3. В GameHUD создай кнопку паузы: **PauseButton** (иконка ||)

4. Добавь компонент **PauseController** на PausePanel
5. Присвой:
   - Pause Panel → PausePanel
   - Canvas Group → CanvasGroup на PausePanel
   - Pause Button → PauseButton (в GameHUD)
   - Resume Button → ResumeButton
   - Restart Button → RestartButton
   - Main Menu Button → MainMenuButton

6. **Выключи** PausePanel

---

### 4. Results Screen (статистика)

1. В Canvas создай Panel: **ResultsPanel**
   - Добавь CanvasGroup

2. Внутри создай:
   ```
   ResultsPanel
   ├── Background
   ├── Window
   │   ├── Title (TMP): "GAME OVER"
   │   ├── FinalScoreText (TMP): "0"
   │   ├── HighScoreText (TMP): "BEST: 0"
   │   ├── NewHighScoreBadge (GameObject): "NEW!" (выключен)
   │   ├── StatsContainer
   │   │   ├── BestStreakText (TMP): "Best Streak: 0"
   │   │   ├── CoreHitsText (TMP): "Core: 0"
   │   │   ├── InnerHitsText (TMP): "Inner: 0"
   │   │   ├── MiddleHitsText (TMP): "Middle: 0"
   │   │   ├── OuterHitsText (TMP): "Outer: 0"
   │   │   ├── MissesText (TMP): "Misses: 0"
   │   │   ├── TotalRingsText (TMP): "Total: 0"
   │   │   └── AccuracyText (TMP): "Accuracy: 0%"
   │   ├── RestartButton (Button)
   │   └── MainMenuButton (Button)
   ```

3. Добавь компонент **ResultsScreen** на ResultsPanel
4. Присвой все текстовые поля и кнопки

5. **Выключи** ResultsPanel

---

### 5. UI Animator (анимации появления)

Добавь компонент **UIAnimator** на любой UI элемент для анимации появления:

| Параметр | Описание |
|----------|----------|
| Animation Type | Тип анимации (Fade, Scale, Slide, FadeAndScale) |
| Duration | Длительность |
| Delay | Задержка перед началом |
| Ease Type | Тип easing |
| Play On Enable | Автоматически играть при включении |

**Пример использования:**
- Добавь UIAnimator на каждую кнопку в меню
- Установи разные Delay (0, 0.1, 0.2, 0.3)
- Animation Type: SlideFromBottom или FadeAndScale

---

### 6. Обновить GameStartController

В компоненте **GameStartController**:
- **Use Countdown**: true (включить отсчёт перед стартом)

---

## Иерархия Canvas

```
Canvas
├── MainMenuPanel + CanvasGroup
│   ├── Title
│   ├── PlayButton
│   └── ... (другие элементы с UIAnimator)
├── GameHUD + CanvasGroup
│   ├── PauseButton ←── PauseController.pauseButton
│   ├── SpeedDisplay
│   ├── LivesDisplay
│   ├── ScoreContainer
│   └── StreakContainer
├── PausePanel (выключен) + PauseController
├── ResultsPanel (выключен) + ResultsScreen
├── NoLivesPopup (выключен)
├── LivesShopPanel (выключен)
├── CountdownPanel (выключен) + CountdownController
├── StreakEffects
└── FadeImage ←── ScreenFader.fadeImage (последний!)
```

---

## Как работает

### Flow игры:
1. **MainMenu** → нажать Play
2. MainMenu скрывается
3. **Countdown**: 3... 2... 1... GO!
4. Игра запускается, **GameHUD** показывается
5. Нажать паузу или ESC → **PausePanel**
6. Game Over → **ResultsPanel** с анимированной статистикой

### Пауза:
- Нажать кнопку паузы или ESC
- Time.timeScale = 0
- Кнопки: Resume / Restart / Main Menu

### Results:
- Показывается после Game Over
- Анимированный подсчёт статистики
- Показывает: очки, лучший стрик, попадания по зонам, accuracy
- Badge "NEW!" при новом рекорде

---

## Как тестировать

### Countdown:
1. Нажми Play
2. Должен появиться отсчёт: 3, 2, 1, GO!
3. После "GO!" игра начинается

### Пауза:
1. Во время игры нажми кнопку паузы или ESC
2. Игра останавливается, появляется меню паузы
3. Resume — продолжить
4. Restart — перезапуск
5. Main Menu — выход в меню

### Results:
1. Доиграй до Game Over (скорость = 0)
2. Появится экран результатов
3. Статистика анимированно заполняется
4. Если новый рекорд — появится badge "NEW!"

### UI Animator:
1. Добавь UIAnimator на элементы меню
2. При включении панели элементы анимируются

---

## Файлы в архиве

```
Scripts/
├── UI/
│   ├── ScreenFader.cs
│   ├── CountdownController.cs
│   ├── PauseController.cs
│   ├── ResultsScreen.cs
│   ├── UIAnimator.cs
│   └── UIManager.cs (обновлен)
└── Lives/
    └── GameStartController.cs (обновлен)
```

---

## Следующая итерация

**Итерация 10: Аудио система** — музыка, звуки попаданий, слои по стрику
