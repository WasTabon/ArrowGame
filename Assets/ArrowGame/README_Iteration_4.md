# LockPoint Run - Итерация 4

## Зоны попадания + Juice

---

## Что добавлено

### Новые скрипты:

**Hit System:**
1. **HitZone.cs** (`Scripts/Hit/`) - Enum зон попадания (Miss, Outer, Middle, Inner, Core)
2. **HitResult.cs** (`Scripts/Hit/`) - Структура результата попадания
3. **HitZoneConfig.cs** (`Scripts/Hit/`) - ScriptableObject с радиусами зон
4. **HitDetector.cs** (`Scripts/Hit/`) - Определяет в какую зону попала игла

**Feedback System:**
5. **HapticFeedback.cs** (`Scripts/Feedback/`) - Вибрация на мобилках (iOS/Android)
6. **HitFeedbackConfig.cs** (`Scripts/Feedback/`) - ScriptableObject с настройками feedback
7. **HitFeedbackManager.cs** (`Scripts/Feedback/`) - Управляет визуальным и тактильным feedback

**Editor:**
8. **HitZoneConfigEditor.cs** (`Scripts/Editor/`) - Визуализация зон в Scene View

---

## Инструкция по установке

### Шаг 1: Добавить новые скрипты

Скопируй файлы в структуру:
```
Assets/ArrowGame/Scripts/
├── Hit/
│   ├── HitZone.cs
│   ├── HitResult.cs
│   ├── HitZoneConfig.cs
│   └── HitDetector.cs
├── Feedback/
│   ├── HapticFeedback.cs
│   ├── HitFeedbackConfig.cs
│   └── HitFeedbackManager.cs
└── Editor/
    └── HitZoneConfigEditor.cs
```

### Шаг 2: Создать HitZoneConfig

1. ПКМ в папке `ArrowGame/Configs/`
2. Create → ArrowGame → Hit Zone Config
3. Назови `HitZoneConfig`
4. Настрой радиусы зон (или оставь по умолчанию)

### Шаг 3: Создать HitFeedbackConfig

1. ПКМ в папке `ArrowGame/Configs/`
2. Create → ArrowGame → Hit Feedback Config
3. Назови `HitFeedbackConfig`
4. Настрой цвета вспышек и силу тряски

### Шаг 4: Создать HitDetector на сцене

1. Создай пустой GameObject → назови `HitDetector`
2. Добавь компонент `HitDetector`
3. Перетащи `HitZoneConfig` в поле Config

### Шаг 5: Создать HitFeedbackManager на сцене

1. Создай пустой GameObject → назови `HitFeedbackManager`
2. Добавь компонент `HitFeedbackManager`
3. Перетащи `HitFeedbackConfig` в поле Config

### Шаг 6: Создать Flash Image для экранной вспышки

1. На Canvas создай Image → назови `HitFlash`
2. RectTransform: Anchor = Stretch/Stretch, все отступы = 0
3. Color: прозрачный (Alpha = 0)
4. Raycast Target: **OFF** (важно!)
5. Перетащи этот Image в поле Flash Image на HitFeedbackManager

---

## Настройки HitZoneConfig

| Параметр | Значение по умолчанию | Описание |
|----------|----------------------|----------|
| Core Radius | 0.3 | Радиус зоны Perfect Lock |
| Inner Radius | 0.7 | Радиус зоны Inner |
| Middle Radius | 1.2 | Радиус зоны Middle |
| Outer Radius | 1.8 | Радиус зоны Outer |

**Визуализация:** Выбери HitZoneConfig в Project, запусти Play — в Scene View увидишь цветные круги вокруг иглы.

---

## Настройки HitFeedbackConfig

### Screen Flash
| Параметр | Описание |
|----------|----------|
| Core Flash Color | Цвет вспышки для Perfect (желтый) |
| Inner Flash Color | Цвет для Inner (зелёный) |
| Middle Flash Color | Цвет для Middle (голубой) |
| Outer Flash Color | Цвет для Outer (белый) |
| Miss Flash Color | Цвет для промаха (красный) |
| Flash Duration | Длительность вспышки (0.15 сек) |

### Camera Shake
| Параметр | Описание |
|----------|----------|
| Core/Inner/Middle/Outer Shake Strength | Сила тряски для каждой зоны |
| Miss Shake Strength | Сила тряски при промахе |
| Shake Duration | Длительность тряски |

### Ring Scale Punch
| Параметр | Описание |
|----------|----------|
| Scale Punch Strength | Сила "пульса" кольца при попадании |
| Scale Punch Duration | Длительность пульса |

### Haptic
| Параметр | Описание |
|----------|----------|
| Haptic Enabled | Включить вибрацию на мобилках |

---

## Как тестировать

### Ожидаемый результат:

| Что проверить | Ожидание |
|---------------|----------|
| Пролети через центр кольца | Вспышка жёлтая, лог "Zone: Core" |
| Пролети чуть мимо центра | Вспышка зелёная, лог "Zone: Inner" |
| Пролети по краю | Вспышка белая/голубая, лог "Zone: Outer/Middle" |
| Совсем мимо | Вспышка красная, лог "Zone: Miss" |
| Camera shake | Камера трясётся при попадании |
| Ring punch | Кольцо "пульсирует" при прохождении |
| На мобилке | Вибрация при попадании |

### Проверка зон в редакторе:

1. Выбери `HitZoneConfig` в Project
2. Запусти Play Mode
3. Открой Scene View
4. Вокруг иглы отобразятся цветные круги зон

### Возможные проблемы:

| Проблема | Решение |
|----------|---------|
| Нет вспышки | Проверь что Flash Image назначен и Raycast Target = OFF |
| Нет лога | Проверь что HitDetector на сцене и Config назначен |
| Всегда Miss | Увеличь радиусы зон в HitZoneConfig |
| Нет тряски | Проверь что CameraController имеет метод ShakeCamera |
| Нет вибрации | Тестируй на реальном устройстве, в Editor не работает |

---

## Иерархия сцены

```
Game (Scene)
├── GameManager
├── InputController
├── LevelGenerator
├── RingSpawner
├── RingSlowdownController
├── HitDetector (NEW)
├── HitFeedbackManager (NEW)
├── Needle (+ NeedleController + RingPassDetector)
│   └── Visual
├── Main Camera (+ CameraController)
└── Canvas
    ├── SlowdownVignette
    └── HitFlash (NEW)
```

---

## Структура файлов итерации

```
ArrowGame/
├── Scripts/
│   ├── Hit/
│   │   ├── HitZone.cs (NEW)
│   │   ├── HitResult.cs (NEW)
│   │   ├── HitZoneConfig.cs (NEW)
│   │   └── HitDetector.cs (NEW)
│   ├── Feedback/
│   │   ├── HapticFeedback.cs (NEW)
│   │   ├── HitFeedbackConfig.cs (NEW)
│   │   └── HitFeedbackManager.cs (NEW)
│   └── Editor/
│       └── HitZoneConfigEditor.cs (NEW)
├── Configs/
│   ├── GameConfig.asset
│   ├── RingConfig.asset
│   ├── HitZoneConfig.asset (создать)
│   └── HitFeedbackConfig.asset (создать)
```

---

## Следующая итерация

В следующей итерации добавим:
- Динамическая скорость иглы
- Изменение скорости по результату попадания
- Game Over при скорости = 0
- Эффект замедления времени при провале
