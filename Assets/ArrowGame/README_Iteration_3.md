# LockPoint Run - Итерация 3

## Управление — замедление вращения

---

## Что добавлено

### Новые скрипты:

1. **InputController.cs** (`Scripts/Input/`) - Обработка ввода (тач + мышь)
2. **RingSlowdownController.cs** (`Scripts/Ring/`) - Управление замедлением вращения колец

---

## Инструкция по установке

### Шаг 1: Добавить новые скрипты

Скопируй файлы в структуру:
```
Assets/ArrowGame/Scripts/
├── Input/
│   └── InputController.cs (NEW)
└── Ring/
    └── RingSlowdownController.cs (NEW)
```

### Шаг 2: Создать InputController на сцене

1. Создай пустой GameObject → назови `InputController`
2. Добавь компонент `InputController`

### Шаг 3: Создать RingSlowdownController на сцене

1. Создай пустой GameObject → назови `RingSlowdownController`
2. Добавь компонент `RingSlowdownController`
3. Настрой параметры:
   - **Slowdown Rate**: 2 (скорость замедления)
   - **Min Speed Multiplier**: 0 (0 = полная остановка)
   - **Speed Recovery Duration**: 0.3 (время восстановления скорости)

---

## Настройки RingSlowdownController

| Параметр | Значение по умолчанию | Описание |
|----------|----------------------|----------|
| Slowdown Rate | 2 | Как быстро замедляется кольцо (в секунду) |
| Min Speed Multiplier | 0 | Минимальный множитель скорости (0 = полная остановка) |
| Speed Recovery Duration | 0.3 | Время плавного восстановления скорости после отпускания |

---

## Как тестировать

### Ожидаемый результат:

| Что проверить | Ожидание |
|---------------|----------|
| Запусти Play | Игра стартует, кольца вращаются |
| Зажми ЛКМ / тач | Ближайшее кольцо начинает замедляться |
| Держи зажатым | Кольцо замедляется до полной остановки (~0.5 сек) |
| Отпусти | Кольцо плавно восстанавливает скорость |
| Пролети кольцо | Следующее кольцо становится целью замедления |
| Мышь в Editor | Работает ЛКМ |
| Тач на устройстве | Работает касание экрана |

### Детали поведения:

1. **Замедление влияет только на ближайшее кольцо** — то, которое впереди иглы
2. **При пролёте через кольцо** — автоматически переключается на следующее
3. **Если отпустить до полной остановки** — скорость плавно восстанавливается
4. **Если зажать снова** — замедление продолжается с текущей скорости

### Возможные проблемы:

| Проблема | Решение |
|----------|---------|
| Замедление не работает | Проверь что InputController и RingSlowdownController на сцене |
| Не реагирует на клик | Проверь что GameState = Run |
| Кольцо не восстанавливает скорость | Проверь Speed Recovery Duration > 0 |
| Слишком быстро/медленно замедляется | Настрой Slowdown Rate |

---

## Иерархия сцены

```
Game (Scene)
├── GameManager
├── InputController (NEW)
├── LevelGenerator
├── RingSpawner
├── RingSlowdownController (NEW)
├── Needle (+ NeedleController + RingPassDetector)
│   └── Visual
└── Main Camera (+ CameraController)
```

---

## Структура файлов итерации

```
ArrowGame/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs
│   │   └── GameConfig.cs
│   ├── Needle/
│   │   └── NeedleController.cs
│   ├── Camera/
│   │   └── CameraController.cs
│   ├── Input/
│   │   └── InputController.cs (NEW)
│   ├── Level/
│   │   └── LevelGenerator.cs
│   └── Ring/
│       ├── RingConfig.cs
│       ├── RingController.cs
│       ├── RingSpawner.cs
│       ├── RingPassDetector.cs
│       └── RingSlowdownController.cs (NEW)
```

---

## Следующая итерация

В следующей итерации добавим:
- Структура кольца с зонами (Outer, Middle, Inner, Core)
- Определение зоны попадания
- Визуальный feedback: вспышки, масштабирование
- Haptic feedback при попадании
