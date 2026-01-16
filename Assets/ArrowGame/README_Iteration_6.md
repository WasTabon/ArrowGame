# LockPoint Run - Итерация 6

## Система жизней

---

## Что добавлено

### Lives System:
1. **LivesConfig.cs** (`Scripts/Lives/`) - ScriptableObject с настройками жизней
2. **LivesManager.cs** (`Scripts/Lives/`) - Управление жизнями, регенерация, сохранение
3. **LivesDisplay.cs** (`Scripts/Lives/`) - UI отображение жизней и таймера
4. **GameStartController.cs** (`Scripts/Lives/`) - Контроллер старта игры с проверкой жизней

---

## Инструкция по установке

### Шаг 1: Добавить новые скрипты

```
Assets/ArrowGame/Scripts/
└── Lives/
    ├── LivesConfig.cs (NEW)
    ├── LivesManager.cs (NEW)
    ├── LivesDisplay.cs (NEW)
    └── GameStartController.cs (NEW)
```

### Шаг 2: Создать LivesConfig

1. ПКМ в папке `ArrowGame/Configs/`
2. Create → ArrowGame → Lives Config
3. Назови `LivesConfig`

### Шаг 3: Создать LivesManager на сцене

1. Создай пустой GameObject → назови `LivesManager`
2. Добавь компонент `LivesManager`
3. Назначь `LivesConfig` в поле Config

**ВАЖНО:** LivesManager использует `DontDestroyOnLoad` — он сохраняется между сценами.

### Шаг 4: Создать Lives Display UI

1. На Canvas создай контейнер → назови `LivesDisplay`
2. Добавь компонент `LivesDisplay`
3. Создай дочерние элементы:

**Вариант А: Текст**
- TextMeshPro с текстом "5/5"
- Назначь в поле **Lives Text**

**Вариант Б: Иконки**
- Создай 5 Image (иконки сердец/жизней)
- Назначь массив в поле **Life Icons**

**Таймер регенерации:**
- Создай контейнер `TimerContainer`
- Внутри: TextMeshPro для "19:59" и Image (Filled) для прогресса
- Назначь в поля **Timer Container**, **Timer Text**, **Timer Fill Bar**

### Шаг 5: Настроить цвета

| Параметр | Описание |
|----------|----------|
| Active Life Color | Цвет активной жизни (белый) |
| Empty Life Color | Цвет потраченной жизни (полупрозрачный) |

### Шаг 6: Создать GameStartController (для главного меню)

1. Создай пустой GameObject → назови `GameStartController`
2. Добавь компонент `GameStartController`
3. Назначь ссылки:
   - **Play Button** — кнопка начала игры
   - **No Lives Popup** — попап "Нет жизней" (опционально)
   - **Close Popup Button** — кнопка закрытия попапа (опционально)

### Шаг 7: Создать No Lives Popup (опционально)

1. На Canvas создай Panel → назови `NoLivesPopup`
2. Добавь `CanvasGroup` компонент
3. Внутри создай:
   - Text "No Lives!"
   - Text "Wait for regeneration or buy lives"
   - Button "Close"
   - Button "Buy Lives" (для IAP в следующей итерации)
4. **Отключи** NoLivesPopup (SetActive false)
5. Назначь в GameStartController

---

## Настройки LivesConfig

| Параметр | Значение | Описание |
|----------|----------|----------|
| Max Lives | 5 | Максимум жизней |
| Starting Lives | 5 | Начальное количество |
| Regeneration Time Minutes | 20 | Минут на регенерацию 1 жизни |

---

## Как работает система

### Использование жизни:
1. Игрок нажимает "Play"
2. `GameStartController.TryStartGame()` вызывается
3. Если есть жизни — тратится 1 жизнь, игра начинается
4. Если нет — показывается попап "Нет жизней"

### Регенерация:
1. Если жизней меньше максимума — запускается таймер
2. Каждые 20 минут (настраивается) +1 жизнь
3. Таймер показывает оставшееся время
4. При максимуме жизней — таймер скрыт

### Offline регенерация:
1. При закрытии/сворачивании приложения — время сохраняется
2. При возврате — рассчитывается сколько жизней восстановилось
3. Работает даже если приложение было закрыто

### Сохранение:
- Жизни сохраняются в `PlayerPrefs`
- Автосохранение при изменении, паузе, выходе
- Ключи: `ArrowGame_Lives`, `ArrowGame_RegenTime`

---

## Как тестировать

### Ожидаемый результат:

| Что проверить | Ожидание |
|---------------|----------|
| Запуск | Показывает 5/5 жизней |
| Начать игру | Жизни уменьшаются до 4/5 |
| Проиграть несколько раз | Жизни уменьшаются |
| 0 жизней + Play | Показывается попап "Нет жизней" |
| Подождать (или уменьшить время в конфиге) | Жизнь регенерируется |
| Таймер | Показывает обратный отсчёт до следующей жизни |
| Закрыть и открыть приложение | Жизни сохранены |

### Тест offline регенерации:

1. Потрать несколько жизней
2. В `LivesConfig` поставь `Regeneration Time Minutes = 0.1` (6 секунд)
3. Закрой Play Mode
4. Подожди 30 секунд
5. Запусти снова — жизни должны восстановиться

### Тест для дебага:

В `LivesManager` есть метод `ResetAllData()` — можно вызвать для сброса всех данных.

### Возможные проблемы:

| Проблема | Решение |
|----------|---------|
| Жизни не сохраняются | Проверь что PlayerPrefs работает (не WebGL с ограничениями) |
| Таймер не показывается | Проверь что Timer Container назначен и жизней < max |
| Жизнь не тратится | Проверь что GameStartController вызывает TryUseLife |
| Offline regen не работает | Проверь формат даты, попробуй увеличить время ожидания |

---

## Иерархия сцены

```
Game (Scene)
├── LivesManager (NEW, DontDestroyOnLoad)
├── GameManager
├── InputController
├── LevelGenerator
├── RingSpawner
├── RingSlowdownController
├── HitDetector
├── HitFeedbackManager
├── SpeedController
├── GameOverController
├── GameStartController (NEW)
├── Needle
├── Main Camera
└── Canvas
    ├── LivesDisplay (NEW)
    │   ├── LivesText / LifeIcons
    │   └── TimerContainer
    │       ├── TimerText
    │       └── TimerFillBar
    ├── NoLivesPopup (NEW, inactive)
    ├── SlowdownVignette
    ├── HitFlash
    ├── FadeOverlay
    ├── SpeedDisplay
    └── GameOverScreen
```

---

## Структура файлов итерации

```
ArrowGame/
├── Scripts/
│   └── Lives/
│       ├── LivesConfig.cs (NEW)
│       ├── LivesManager.cs (NEW)
│       ├── LivesDisplay.cs (NEW)
│       └── GameStartController.cs (NEW)
├── Configs/
│   └── LivesConfig.asset (создать)
```

---

## Следующая итерация

В следующей итерации добавим:
- Интеграция Unity IAP
- Покупка жизней
- Восстановление покупок
- Тестирование на iOS/Android
