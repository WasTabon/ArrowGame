# LockPoint Run - Итерация 5

## Система скорости + Game Over

---

## Что добавлено

### Speed System:
1. **SpeedConfig.cs** (`Scripts/Speed/`) - ScriptableObject с настройками изменения скорости
2. **SpeedController.cs** (`Scripts/Speed/`) - Управляет скоростью иглы по результатам попаданий
3. **SpeedDisplay.cs** (`Scripts/Speed/`) - UI отображение скорости (текст + полоска)

### Game Over System:
4. **GameOverConfig.cs** (`Scripts/GameOver/`) - ScriptableObject с настройками Game Over эффектов
5. **GameOverController.cs** (`Scripts/GameOver/`) - Управляет последовательностью Game Over
6. **GameOverUI.cs** (`Scripts/GameOver/`) - Кнопки на экране Game Over

---

## ВАЖНО: Правка в NeedleController.cs

В **NeedleController.cs** замени метод `SetSpeed` на:

```csharp
public void SetSpeed(float newSpeed)
{
    currentSpeed = newSpeed;
}
```

Убери DOTween из этого метода — анимация скорости теперь делается в SpeedController.

---

## Инструкция по установке

### Шаг 1: Добавить новые скрипты

```
Assets/ArrowGame/Scripts/
├── Speed/
│   ├── SpeedConfig.cs (NEW)
│   ├── SpeedController.cs (NEW)
│   └── SpeedDisplay.cs (NEW)
└── GameOver/
    ├── GameOverConfig.cs (NEW)
    ├── GameOverController.cs (NEW)
    └── GameOverUI.cs (NEW)
```

### Шаг 2: Создать SpeedConfig

1. ПКМ в папке `ArrowGame/Configs/`
2. Create → ArrowGame → Speed Config
3. Назови `SpeedConfig`

### Шаг 3: Создать GameOverConfig

1. ПКМ в папке `ArrowGame/Configs/`
2. Create → ArrowGame → Game Over Config
3. Назови `GameOverConfig`

### Шаг 4: Создать SpeedController на сцене

1. Создай пустой GameObject → назови `SpeedController`
2. Добавь компонент `SpeedController`
3. Назначь `SpeedConfig` в поле Config

### Шаг 5: Создать GameOverController на сцене

1. Создай пустой GameObject → назови `GameOverController`
2. Добавь компонент `GameOverController`
3. Назначь `GameOverConfig` в поле Config

### Шаг 6: Создать Fade Image

1. На Canvas создай Image → назови `FadeOverlay`
2. RectTransform: Anchor = Stretch/Stretch, все отступы = 0
3. Color: чёрный с Alpha = 0
4. Raycast Target: **OFF**
5. Перетащи в поле **Fade Image** на GameOverController

### Шаг 7: Создать Game Over Screen

1. На Canvas создай пустой GameObject → назови `GameOverScreen`
2. Добавь компонент `CanvasGroup`
3. Добавь компонент `GameOverUI`
4. Создай дочерние элементы:
   - Panel (фон)
   - Text "GAME OVER"
   - Button "Restart"
   - Button "Main Menu" (опционально)
5. Назначь кнопки в поля компонента `GameOverUI`
6. Перетащи `GameOverScreen` в поле на GameOverController
7. **Отключи** GameOverScreen (SetActive false)

### Шаг 8: Создать Speed Display (опционально)

1. На Canvas создай элементы для отображения скорости:
   - TextMeshPro для числа скорости
   - Image (Filled) для полоски скорости
2. Создай пустой GameObject → назови `SpeedDisplay`
3. Добавь компонент `SpeedDisplay`
4. Назначь ссылки на Text и Image

---

## Настройки SpeedConfig

| Параметр | Значение | Описание |
|----------|----------|----------|
| Start Speed | 10 | Начальная скорость |
| Min Speed | 0 | Минимальная (0 = Game Over) |
| Max Speed | 30 | Максимальная скорость |
| Core Speed Bonus | +2 | Бонус за Perfect |
| Inner Speed Bonus | +1 | Бонус за Inner |
| Middle Speed Bonus | +0.5 | Бонус за Middle |
| Outer Speed Penalty | -1 | Штраф за Outer |
| Miss Speed Penalty | -3 | Штраф за промах |
| Speed Change Duration | 0.3 | Длительность анимации изменения |

---

## Настройки GameOverConfig

| Параметр | Значение | Описание |
|----------|----------|----------|
| Slow Motion Time Scale | 0.2 | Скорость времени при провале (20%) |
| Slow Motion Duration | 1.5 | Длительность замедления |
| Fade To Black Duration | 0.5 | Длительность затемнения |
| Delay Before Screen | 0.5 | Задержка перед показом экрана |

---

## Как тестировать

### Ожидаемый результат:

| Что проверить | Ожидание |
|---------------|----------|
| Попадание в Core | Скорость увеличивается (+2) |
| Попадание в Inner | Скорость увеличивается (+1) |
| Попадание в Outer | Скорость уменьшается (-1) |
| Промах (Miss) | Скорость уменьшается (-3) |
| Скорость достигла 0 | Slow-motion → затемнение → Game Over экран |
| Кнопка Restart | Перезапуск сцены |
| Speed Display | Показывает текущую скорость, меняет цвет |

### Тест Game Over:

1. Запусти игру
2. Специально промахивайся мимо колец
3. После нескольких промахов скорость упадёт до 0
4. Должен сработать эффект замедления времени
5. Экран затемняется
6. Появляется Game Over экран
7. Кнопка Restart перезапускает игру

### Возможные проблемы:

| Проблема | Решение |
|----------|---------|
| Скорость не меняется | Проверь что SpeedController на сцене и подписан на HitDetector |
| Game Over не срабатывает | Проверь что GameOverController подписан на SpeedController |
| Время не восстанавливается | Time.timeScale должен вернуться к 1 после рестарта |
| Кнопки не работают | Проверь что назначены в GameOverUI |

---

## Иерархия сцены

```
Game (Scene)
├── GameManager
├── InputController
├── LevelGenerator
├── RingSpawner
├── RingSlowdownController
├── HitDetector
├── HitFeedbackManager
├── SpeedController (NEW)
├── GameOverController (NEW)
├── Needle
├── Main Camera
└── Canvas
    ├── SlowdownVignette
    ├── HitFlash
    ├── FadeOverlay (NEW)
    ├── SpeedDisplay (NEW, optional)
    └── GameOverScreen (NEW, inactive)
        ├── Panel
        ├── Text "GAME OVER"
        └── Button "Restart"
```

---

## Структура файлов итерации

```
ArrowGame/
├── Scripts/
│   ├── Speed/
│   │   ├── SpeedConfig.cs (NEW)
│   │   ├── SpeedController.cs (NEW)
│   │   └── SpeedDisplay.cs (NEW)
│   └── GameOver/
│       ├── GameOverConfig.cs (NEW)
│       ├── GameOverController.cs (NEW)
│       └── GameOverUI.cs (NEW)
├── Configs/
│   ├── SpeedConfig.asset (создать)
│   └── GameOverConfig.asset (создать)
```

---

## Следующая итерация

В следующей итерации добавим:
- Система жизней (5 жизней max)
- Регенерация жизней (1 / 20 минут)
- Сохранение/загрузка состояния
- UI таймера регенерации
