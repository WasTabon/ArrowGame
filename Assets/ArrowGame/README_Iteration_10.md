# Итерация 10: Аудио система

## Что добавлено

### Новые скрипты (папка Scripts/Audio/)
- **AudioConfig.cs** — ScriptableObject с настройками всех звуков
- **AudioManager.cs** — Центральный менеджер звука (музыка, SFX, слои)
- **ButtonClickSound.cs** — Компонент для звука клика на кнопку
- **AudioSettingsUI.cs** — UI для mute кнопок музыки/SFX

### Обновлённые скрипты
- **CountdownController.cs** — добавлены звуки отсчёта

---

## Настройка в Unity

### 1. Создать AudioConfig
1. **Right-click → Create → ArrowGame → Audio Config**
2. Назови `AudioConfig`
3. Присвой звуки (можно оставить пустыми — не будет ошибок)

### 2. Создать AudioManager
1. Создай пустой GameObject: **AudioManager**
2. Добавь компонент **AudioManager**
3. Присвой **AudioConfig**

AudioManager автоматически создаст AudioSource для музыки, SFX и UI.

---

## Звуки в AudioConfig

| Категория | Поле | Когда играет |
|-----------|------|--------------|
| **Music** | Menu Music | В главном меню |
| | Game Music | Во время игры |
| **Hit Sounds** | Core Hit Sound | Попадание в Core |
| | Inner Hit Sound | Попадание в Inner |
| | Middle Hit Sound | Попадание в Middle |
| | Outer Hit Sound | Попадание в Outer |
| | Miss Sound | Промах |
| **UI Sounds** | Button Click Sound | Клик по кнопке |
| | Countdown Tick Sound | 3, 2, 1 |
| | Countdown Go Sound | GO! |
| | Game Over Sound | Game Over |
| | New High Score Sound | Новый рекорд |
| **Streak** | Streak Up Sound | Стрик растёт |
| | Streak Break Sound | Стрик сломался |
| | Multiplier Up Sound | Множитель увеличился |
| **Layers** | Streak Layers[] | Музыкальные слои по стрику |

---

## Streak Music Layers

Система добавляет музыкальные слои при росте множителя:
- **x1** — только основная музыка
- **x2** — + Layer 0
- **x3** — + Layer 1
- **x4** — + Layer 2
- **x5** — + Layer 3

Слои должны быть синхронизированы с основной музыкой (одинаковая длина и темп).

---

## Добавить звук клика на кнопки

На любую кнопку добавь компонент **ButtonClickSound** — при клике будет играть звук.

---

## Mute кнопки (опционально)

1. Создай кнопки Music Toggle и SFX Toggle
2. Добавь компонент **AudioSettingsUI**
3. Присвой:
   - Music Toggle Button
   - Music Icon On / Off (Image)
   - SFX Toggle Button
   - SFX Icon On / Off (Image)

Состояние mute сохраняется в PlayerPrefs.

---

## Как работает

### Автоматически:
- Музыка меню при MainMenu state
- Музыка игры при Run state
- Звуки попаданий по зонам
- Звуки стрика (рост, сброс, множитель)
- Звук Game Over
- Звук нового рекорда
- Звуки countdown (3, 2, 1, GO!)

### Вручную (если нужно):
```csharp
AudioManager.Instance.PlaySound(clip, volume);
AudioManager.Instance.PlayButtonClick();
AudioManager.Instance.ToggleMusic();
AudioManager.Instance.ToggleSfx();
```

---

## Файлы в архиве

```
Scripts/
├── Audio/
│   ├── AudioConfig.cs
│   ├── AudioManager.cs
│   ├── ButtonClickSound.cs
│   └── AudioSettingsUI.cs
└── UI/
    └── CountdownController.cs (обновлён)
```

---

## Рекомендуемые звуки

Для тестирования можно использовать бесплатные звуки:
- **freesound.org**
- **opengameart.org**
- **kenney.nl/assets** (Game Audio)

Форматы: .wav, .ogg, .mp3

---

## Следующая итерация

**Итерация 11: Advanced Game Feel** — screen shake, хитстопы, particles, trails
