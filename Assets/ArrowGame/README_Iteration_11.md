# Итерация 11: Advanced Game Feel

## Что добавлено

### Новые скрипты (папка Scripts/Feedback/)
- **ScreenShake.cs** — Тряска камеры при попаданиях
- **HitStop.cs** — Микро-пауза времени при попаданиях
- **HitParticles.cs** — Партиклы при попаданиях (пулинг)
- **NeedleTrail.cs** — Trail за иглой (цвет по стрику)
- **GameFeelManager.cs** — Связывает все эффекты

### Новые скрипты (папка Scripts/Audio/)
- **AudioSlowMotion.cs** — Эффект "под водой" при замедлении

---

## Настройка в Unity

### 1. GameFeelManager
1. Создай GameObject: **GameFeelManager**
2. Добавь компонент **GameFeelManager**
3. Настрой какие эффекты включены

### 2. ScreenShake
1. Создай GameObject: **ScreenShake**
2. Добавь компонент **ScreenShake**
3. Присвой **Camera Transform** (Main Camera)

### 3. HitStop
1. Создай GameObject: **HitStop**
2. Добавь компонент **HitStop**
3. Настрой длительность для каждой зоны

### 4. HitParticles
1. Создай GameObject: **HitParticles**
2. Добавь компонент **HitParticles**
3. Создай Particle System для каждой зоны:
   - Core Particles (золотой burst)
   - Inner Particles (зелёный burst)
   - Middle Particles (голубой burst)
   - Outer Particles (белый burst)
   - Miss Particles (красный burst)
4. Присвой их в компонент

### 5. NeedleTrail
1. На объект Needle добавь **Trail Renderer**
2. Добавь компонент **NeedleTrail**
3. Настрой цвета для каждого множителя

### 6. AudioSlowMotion
1. Создай GameObject: **AudioSlowMotion**
2. Добавь компонент **AudioSlowMotion**
3. Он автоматически найдёт AudioSource и добавит Low Pass Filter

---

## Как работает

### Screen Shake
- Core → средняя тряска
- Inner → лёгкая тряска
- Miss → сильная тряска

### Hit Stop
- Core → 0.08 сек пауза
- Inner → 0.04 сек пауза
- Miss → 0.12 сек пауза

### Particles
- Пул партиклов для каждой зоны
- Автоматически играют на позиции иглы
- Цвет соответствует зоне

### Trail
- Ширина и длина зависят от скорости
- Цвет меняется по множителю стрика

### Audio Slow Motion
- При зажатии экрана (замедление кольца):
  - Low Pass Filter снижает частоту до 800Hz
  - Pitch опускается до 0.7
- При отпускании — возврат к нормальному звуку

---

## Настройки в GameFeelManager

| Параметр | Описание |
|----------|----------|
| Enable Screen Shake | Вкл/выкл тряску |
| Enable Hit Stop | Вкл/выкл хитстопы |
| Enable Particles | Вкл/выкл партиклы |
| Enable Slow Mo Audio | Вкл/выкл эффект звука при замедлении |
| Shake On Core/Inner/Miss | Какие зоны трясут камеру |
| Hit Stop On Core/Inner/Miss | Какие зоны делают хитстоп |

---

## Particle System рекомендации

Для каждой зоны создай Particle System:

**Core Particles:**
- Duration: 0.3
- Looping: OFF
- Start Lifetime: 0.3
- Start Speed: 5
- Start Size: 0.2
- Start Color: Gold/Yellow
- Emission: Burst 20 particles
- Shape: Sphere, Radius 0.1

**Miss Particles:**
- Duration: 0.5
- Start Color: Red
- Emission: Burst 30 particles
- Start Speed: 3
- Более "разбросанные"

---

## Файлы в архиве

```
Scripts/
├── Feedback/
│   ├── ScreenShake.cs
│   ├── HitStop.cs
│   ├── HitParticles.cs
│   ├── NeedleTrail.cs
│   └── GameFeelManager.cs
└── Audio/
    └── AudioSlowMotion.cs
```

---

## Следующая итерация

**Итерация 12: Достижения** — система достижений, разблокировка скинов
