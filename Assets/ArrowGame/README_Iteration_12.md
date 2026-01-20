# Итерация 12: Достижения

## Что добавлено

### Скрипты (Scripts/Achievements/)
| Файл | Описание |
|------|----------|
| **AchievementEnums.cs** | Категории и типы условий |
| **AchievementDefinition.cs** | ScriptableObject достижения |
| **AchievementsConfig.cs** | Список всех достижений |
| **AchievementManager.cs** | Управление прогрессом и разблокировкой |
| **AchievementTracker.cs** | Отслеживание во время игры |
| **AchievementPopup.cs** | Popup при разблокировке |
| **AchievementItemUI.cs** | Элемент списка |
| **AchievementsListUI.cs** | Панель списка достижений |

### Editor скрипт
- **AchievementsGenerator.cs** — генерирует 16 стандартных достижений

---

## Быстрый сетап

### 1. Сгенерировать достижения
**ArrowGame → Generate Default Achievements**

Создаст папку `Assets/ScriptableObjects/Achievements/` с:
- 16 AchievementDefinition файлов
- AchievementsConfig.asset

### 2. Создать менеджеры
1. GameObject **AchievementManager** + компонент
2. Присвоить **AchievementsConfig**
3. GameObject **AchievementTracker** + компонент

### 3. Создать Popup (опционально)
1. UI Panel на Canvas (вверху экрана)
2. CanvasGroup на панели
3. Icon (Image), Title (TMP), Description (TMP)
4. SkinUnlockBadge (GameObject с TMP для имени скина)
5. Компонент **AchievementPopup** → присвоить всё

### 4. Создать список достижений (опционально)
1. UI Panel на Canvas
2. Кнопки табов: All, Precision, Flow, Risk, Endurance
3. ScrollView с Content
4. Prefab элемента (AchievementItemUI)
5. Компонент **AchievementsListUI** → присвоить всё

---

## Категории достижений

| Категория | Описание |
|-----------|----------|
| **Precision** | Точность попаданий |
| **Flow** | Стрик и множитель |
| **Risk** | Выживание на низкой скорости |
| **Endurance** | Общий прогресс |

---

## Стандартные достижения

### Precision
- **Sharpshooter** — 5 Core подряд → скин "pulse"
- **Bullseye Master** — 10 Core подряд
- **Core Collector** — 100 Core всего
- **Perfectionist** — Рун без промахов → скин "razor"

### Flow
- **On Fire** — Стрик 25
- **Unstoppable** — Стрик 50 → скин "phase"
- **Maximum Power** — Достичь x5
- **Consistent** — 30 колец без Outer

### Risk
- **Living on Edge** — Выжить при скорости <5
- **Comeback King** — Восстановить 10 скорости

### Endurance
- **Ring Runner** — 500 колец всего
- **Ring Master** — 2000 колец → скин "void"
- **Score Hunter** — 10,000 очков за рун
- **High Scorer** — 50,000 очков за рун
- **Getting Started** — 10 игр
- **Dedicated** — 100 игр

---

## Скины, разблокируемые достижениями

| Скин | Достижение |
|------|------------|
| pulse | Sharpshooter (5 Core подряд) |
| razor | Perfectionist (рун без промахов) |
| phase | Unstoppable (стрик 50) |
| void | Ring Master (2000 колец) |

---

## Использование из кода

```csharp
// Проверить разблокировку
bool unlocked = AchievementManager.Instance.IsUnlocked("core_5");

// Получить прогресс
int progress = AchievementManager.Instance.GetProgress("core_5");
float percent = AchievementManager.Instance.GetProgressNormalized("core_5");

// Проверить скин
bool skinUnlocked = AchievementManager.Instance.IsSkinUnlocked("pulse");

// Показать список
AchievementsListUI.Instance.Show();
```

---

## Файлы в архиве

```
Scripts/
├── Achievements/
│   ├── AchievementEnums.cs
│   ├── AchievementDefinition.cs
│   ├── AchievementsConfig.cs
│   ├── AchievementManager.cs
│   ├── AchievementTracker.cs
│   ├── AchievementPopup.cs
│   ├── AchievementItemUI.cs
│   └── AchievementsListUI.cs
└── Editor/
    └── AchievementsGenerator.cs
```

---

## Следующая итерация

**Итерация 13: Скины иглы** — система скинов, выбор перед игрой
