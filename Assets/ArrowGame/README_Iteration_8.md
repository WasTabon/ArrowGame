# Итерация 8: Очки и Streak

## Что добавлено

### Новые скрипты (папка Scripts/Score/)
- **ScoreConfig.cs** — ScriptableObject с настройками очков и стрика
- **ScoreManager.cs** — Управление очками, хайскор
- **StreakManager.cs** — Управление стриком (серией попаданий)
- **ScoreDisplay.cs** — UI отображение очков
- **StreakDisplay.cs** — UI отображение стрика и множителя
- **StreakVisualEffects.cs** — Визуальные эффекты от стрика (пульсации, виньетка)

---

## Настройка в Unity

### 1. Создать ScoreConfig
1. **Right-click** в Project → **Create → ArrowGame → Score Config**
2. Назови `ScoreConfig`
3. Настрой параметры:

| Параметр | Значение по умолчанию | Описание |
|----------|----------------------|----------|
| Core Points | 100 | Очки за Core зону |
| Inner Points | 50 | Очки за Inner зону |
| Middle Points | 25 | Очки за Middle зону |
| Outer Points | 10 | Очки за Outer зону |
| Streak For Multiplier 2x | 5 | Стрик для x2 |
| Streak For Multiplier 3x | 10 | Стрик для x3 |
| Streak For Multiplier 4x | 20 | Стрик для x4 |
| Streak For Multiplier 5x | 35 | Стрик для x5 |
| Outer Breaks Streak | false | Outer ломает стрик? |
| Outer Weakens Streak | true | Outer ослабляет стрик? |
| Outer Streak Penalty | 2 | Сколько вычитать из стрика при Outer |

---

### 2. Создать ScoreManager
1. Создай пустой GameObject: **ScoreManager**
2. Добавь компонент **ScoreManager**
3. Присвой **ScoreConfig**

---

### 3. Создать StreakManager
1. Создай пустой GameObject: **StreakManager**
2. Добавь компонент **StreakManager**
3. Присвой тот же **ScoreConfig**

---

### 4. Настроить UI — Score Display

В GameHUD создай:

```
ScoreContainer
├── ScoreText (TextMeshPro) — "0"
├── HighScoreText (TextMeshPro) — "BEST: 0"
└── PointsPopup (TextMeshPro) — "+100" (для popup очков)
```

1. Добавь компонент **ScoreDisplay** на ScoreContainer
2. Присвой:
   - Score Text → ScoreText
   - High Score Text → HighScoreText
   - Points Popup Text → PointsPopup

---

### 5. Настроить UI — Streak Display

В GameHUD создай:

```
StreakContainer
├── StreakText (TextMeshPro) — "0"
├── MultiplierText (TextMeshPro) — "x1"
├── StreakFillBar (Image, Fill) — полоска прогресса
└── GlowImage (Image) — свечение за текстом (опционально)
```

1. Добавь компонент **StreakDisplay** на StreakContainer
2. Присвой:
   - Streak Text → StreakText
   - Multiplier Text → MultiplierText
   - Streak Fill Bar → StreakFillBar (Image Type = Filled)
   - Glow Image → GlowImage (опционально)

---

### 6. Настроить визуальные эффекты стрика

В Canvas создай:

```
StreakEffects
├── ScreenPulseImage (Image, Stretch) — полноэкранный цветной оверлей
└── StreakVignetteImage (Image, Stretch) — цветная виньетка
```

1. Создай пустой GameObject: **StreakEffects**
2. Добавь компонент **StreakVisualEffects**
3. Создай ScreenPulseImage:
   - Image, белый цвет, Raycast Target = OFF
   - Stretch на весь экран
   - Alpha = 0
4. Создай StreakVignetteImage:
   - Image с текстурой виньетки (или белый)
   - Stretch на весь экран
   - Alpha = 0
5. Присвой в StreakVisualEffects:
   - Screen Pulse Image → ScreenPulseImage
   - Streak Vignette Image → StreakVignetteImage

---

## Иерархия GameHUD

```
GameHUD
├── SpeedDisplay (из предыдущих итераций)
├── LivesDisplay (из предыдущих итераций)
├── ScoreContainer + ScoreDisplay
│   ├── ScoreText
│   ├── HighScoreText
│   └── PointsPopup
├── StreakContainer + StreakDisplay
│   ├── StreakText
│   ├── MultiplierText
│   ├── StreakFillBar
│   └── GlowImage
└── StreakEffects + StreakVisualEffects
    ├── ScreenPulseImage
    └── StreakVignetteImage
```

---

## Как работает система

### Очки
- При попадании в зону начисляются очки
- Очки умножаются на текущий множитель стрика
- Хайскор сохраняется автоматически

### Стрик
- Core/Inner/Middle попадания увеличивают стрик на 1
- Outer ослабляет стрик (вычитает 2 по умолчанию)
- Miss ломает стрик полностью
- Стрик определяет множитель очков:
  - 0-4: x1
  - 5-9: x2
  - 10-19: x3
  - 20-34: x4
  - 35+: x5

### Визуальные эффекты
- При достижении нового множителя — пульсация экрана
- Цветная виньетка усиливается с ростом множителя
- При сбросе стрика — красная вспышка

---

## Как тестировать

1. Запусти игру
2. Попадай в Core/Inner/Middle — стрик растёт
3. Смотри как меняется множитель (x1 → x2 → x3...)
4. Очки умножаются на множитель
5. Попробуй попасть в Outer — стрик уменьшится
6. Промахнись (Miss) — стрик сбросится

### Ожидаемый результат

| Действие | Результат |
|----------|-----------|
| Попадание Core | +100 × множитель, стрик +1 |
| Попадание Inner | +50 × множитель, стрик +1 |
| Попадание Middle | +25 × множитель, стрик +1 |
| Попадание Outer | +10 × множитель, стрик -2 |
| Miss | Очков нет, стрик = 0 |
| Стрик 5+ | Множитель x2, цвет cyan |
| Стрик 10+ | Множитель x3, цвет green |
| Стрик 20+ | Множитель x4, цвет yellow |
| Стрик 35+ | Множитель x5, цвет orange |

---

## Файлы в архиве

```
Scripts/
└── Score/
    ├── ScoreConfig.cs
    ├── ScoreManager.cs
    ├── StreakManager.cs
    ├── ScoreDisplay.cs
    ├── StreakDisplay.cs
    └── StreakVisualEffects.cs
```

---

## Следующая итерация

**Итерация 9: Базовый UI** — HUD, меню, экраны, плавные переходы
