# LockPoint Run - Итерация 1

## Основа — Игла и Камера + DOTween

---

## Что добавлено

### Скрипты:
1. **GameManager.cs** (`Scripts/Core/`) - Менеджер состояний игры (Boot, MainMenu, Run, Pause, GameOver, Results)
2. **GameConfig.cs** (`Scripts/Core/`) - ScriptableObject с настройками игры
3. **NeedleController.cs** (`Scripts/Needle/`) - Контроллер иглы: движение вперёд + синусоидальный дрейф
4. **CameraController.cs** (`Scripts/Camera/`) - Контроллер камеры: плавное следование за иглой

---

## Инструкция по установке

### Шаг 1: Установить DOTween

1. Открой Window → Asset Store (или Package Manager)
2. Найди **DOTween (HOTween v2)** - это бесплатный ассет
3. Импортируй в проект
4. После импорта появится окно DOTween Setup
5. Нажми **Setup DOTween...**
6. В открывшемся окне нажми **Create ASMDEF...**  (опционально)
7. Нажми **Apply**

Альтернатива: скачай с https://dotween.demigiant.com/download.php

### Шаг 2: Создать структуру папок

```
Assets/
└── ArrowGame/
    ├── Scripts/
    │   ├── Core/
    │   ├── Needle/
    │   └── Camera/
    ├── Prefabs/
    ├── Scenes/
    └── Configs/
```

### Шаг 3: Добавить скрипты

Скопируй файлы из архива:
- `Scripts/Core/GameManager.cs`
- `Scripts/Core/GameConfig.cs`
- `Scripts/Needle/NeedleController.cs`
- `Scripts/Camera/CameraController.cs`

### Шаг 4: Создать GameConfig

1. ПКМ в папке `ArrowGame/Configs/`
2. Create → ArrowGame → Game Config
3. Назови файл `GameConfig`

### Шаг 5: Создать сцену

1. Создай новую сцену: File → New Scene
2. Сохрани как `ArrowGame/Scenes/Game.unity`

### Шаг 6: Создать GameManager

1. Создай пустой GameObject: ПКМ в Hierarchy → Create Empty
2. Назови его `GameManager`
3. Добавь компонент `GameManager` (ArrowGame.Core)

### Шаг 7: Создать иглу (Needle)

1. Создай пустой GameObject, назови `Needle`
2. Добавь дочерний объект для визуала:
   - ПКМ на Needle → 3D Object → Capsule
   - Или Cylinder для простоты
3. Поверни визуал так, чтобы он смотрел вперёд (по Z)
   - Rotation: (90, 0, 0)
4. Уменьши Scale визуала: (0.2, 0.5, 0.2)
5. На родительский `Needle` добавь компонент `NeedleController`
6. Перетащи `GameConfig` в поле Config

### Шаг 8: Настроить камеру

1. Выбери Main Camera
2. Добавь компонент `CameraController`
3. Перетащи `GameConfig` в поле Config
4. Перетащи `Needle` в поле Target
5. Позиция камеры: (0, 2, -8)
6. Rotation камеры: (10, 0, 0) — небольшой наклон вниз

### Шаг 9: Добавить пол (опционально, для ориентира)

1. Create → 3D Object → Plane
2. Scale: (10, 1, 100)
3. Position: (0, -1, 50)

---

## Настройки GameConfig

| Параметр | Значение по умолчанию | Описание |
|----------|----------------------|----------|
| Base Needle Speed | 10 | Начальная скорость иглы |
| Min Needle Speed | 0 | Минимальная скорость (0 = Game Over) |
| Max Needle Speed | 30 | Максимальная скорость |
| Float Amplitude X | 0.3 | Амплитуда дрейфа по X |
| Float Amplitude Y | 0.2 | Амплитуда дрейфа по Y |
| Float Frequency X | 1.5 | Частота дрейфа по X |
| Float Frequency Y | 2 | Частота дрейфа по Y |
| Camera Offset | (0, 2, -8) | Смещение камеры от иглы |
| Camera Smooth Speed | 5 | Скорость сглаживания камеры |
| Camera Look Ahead | 3 | Опережение камеры по Z |

---

## Как тестировать

### Ожидаемый результат:

1. **Запусти сцену** (Play)

2. **Игла движется вперёд:**
   - Игла должна непрерывно двигаться вперёд по оси Z
   - Скорость примерно 10 единиц в секунду

3. **Плавающий дрейф:**
   - Игла слегка покачивается влево-вправо и вверх-вниз
   - Движение плавное, синусоидальное
   - Не резкое, не дёрганое

4. **Камера следует:**
   - Камера плавно следует за иглой
   - Есть небольшое отставание (лаг)
   - Камера смотрит немного вперёд (опережение)

5. **Остановка:**
   - При выходе из Play mode всё сбрасывается

### Проверка компонентов:

- В Inspector у Needle должен быть NeedleController с заполненным Config
- В Inspector у Main Camera должен быть CameraController с заполненными Config и Target
- GameManager должен существовать в сцене

### Возможные проблемы:

| Проблема | Решение |
|----------|---------|
| Ошибка "DOTween not found" | Установи DOTween через Asset Store |
| Игла не движется | Проверь что GameConfig назначен |
| Камера не следует | Проверь что Target назначен |
| Namespace errors | Проверь структуру папок |

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
│   └── Camera/
│       └── CameraController.cs
├── Configs/
│   └── GameConfig.asset (создать вручную)
└── Scenes/
    └── Game.unity (создать вручную)
```

---

## Следующая итерация

В следующей итерации добавим:
- Спавнер колец
- Кольца появляются впереди иглы
- Кольца вращаются
- Игла проходит сквозь кольца
