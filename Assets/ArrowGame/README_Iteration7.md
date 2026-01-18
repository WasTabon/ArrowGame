# Итерация 7: IAP — Покупка жизней

## Что добавлено

### Новые скрипты (папка Scripts/IAP/)
- **IAPConfig.cs** — ScriptableObject с конфигурацией продуктов (ID, название, количество жизней)
- **IAPManager.cs** — Основной менеджер покупок (инициализация, покупка, восстановление)
- **IAPButton.cs** — Компонент для кнопки покупки в UI
- **LivesShopUI.cs** — UI панель магазина жизней

### Обновленные скрипты
- **UIManager.cs** — Добавлена интеграция с магазином жизней

---

## Установка Unity IAP

### Шаг 1: Установить пакет
1. Открой **Window → Package Manager**
2. Нажми **+** → **Add package by name**
3. Введи: `com.unity.purchasing`
4. Нажми **Add**

### Шаг 2: Включить In-App Purchasing
1. Открой **Edit → Project Settings → Services**
2. Залогинься в Unity Dashboard если нужно
3. Включи **In-App Purchasing**

### Шаг 3: Настроить продукты (для реального релиза)
1. Перейди в **Services → In-App Purchasing → Configure**
2. Добавь продукты с такими ID:
   - `com.arrowgame.lives_1` — 1 жизнь
   - `com.arrowgame.lives_5` — 5 жизней
   - `com.arrowgame.lives_10` — 10 жизней

---

## Настройка в Unity

### 1. Создать IAPConfig
1. **Right-click** в Project → **Create → ArrowGame → IAP Config**
2. Назови `IAPConfig`
3. Настрой продукты (по умолчанию уже есть 3 продукта)

### 2. Создать GameObject для IAPManager
1. Создай пустой GameObject: `IAPManager`
2. Добавь компонент: **ArrowGame → IAP → IAPManager**
3. Присвой **IAPConfig** в поле Config

### 3. Создать UI магазина жизней

#### Панель магазина:
1. В Canvas создай Panel: `LivesShopPanel`
2. Добавь **CanvasGroup** компонент
3. Добавь компонент **LivesShopUI**

#### Внутри панели:
1. **Заголовок** (TextMeshPro): "Get More Lives"
2. **Кнопка закрытия** (Button): "X" или "Close"
3. **Кнопки покупки** (3 штуки):
   - Создай Button для каждого продукта
   - Добавь компонент **IAPButton**
   - Укажи Product ID: `com.arrowgame.lives_1`, `com.arrowgame.lives_5`, `com.arrowgame.lives_10`
   - Присвой IAPConfig
   - Опционально добавь текстовые поля для цены, названия, количества жизней
4. **Кнопка восстановления** (только iOS): "Restore Purchases"
5. **Текст статуса** (TextMeshPro): для сообщений об ошибках/успехе

#### Настройка LivesShopUI:
- Shop Panel → LivesShopPanel
- Close Button → кнопка закрытия
- Restore Button → кнопка восстановления
- Status Text → текст статуса

### 4. Подключить к UIManager
- В UIManager присвой Lives Shop Panel

---

## Структура UI магазина (пример)

```
LivesShopPanel (Panel + CanvasGroup + LivesShopUI)
├── Background (Image, темный полупрозрачный)
├── Window (Panel, основное окно)
│   ├── Title (TextMeshPro): "Need More Lives?"
│   ├── CloseButton (Button): "X"
│   ├── ProductsContainer (Vertical Layout)
│   │   ├── Product1 (Button + IAPButton)
│   │   │   ├── LivesIcon (Image)
│   │   │   ├── LivesAmount (TextMeshPro): "+1"
│   │   │   └── Price (TextMeshPro): "$0.99"
│   │   ├── Product5 (Button + IAPButton)
│   │   │   ├── LivesIcon (Image)
│   │   │   ├── LivesAmount (TextMeshPro): "+5"
│   │   │   └── Price (TextMeshPro): "$2.99"
│   │   └── Product10 (Button + IAPButton)
│   │       ├── LivesIcon (Image)
│   │       ├── LivesAmount (TextMeshPro): "+10"
│   │       └── Price (TextMeshPro): "$4.99"
│   ├── RestoreButton (Button): "Restore Purchases"
│   └── StatusText (TextMeshPro)
```

---

## Как тестировать

### В Editor (без Unity IAP):
1. Запусти игру
2. Потрать все жизни (играй пока не закончатся)
3. При попытке начать игру без жизней появится магазин
4. **В Editor** нажатие на кнопку покупки **симулирует покупку** — жизни добавятся без реальной транзакции
5. Проверь что жизни добавились

### На устройстве (с Unity IAP):
1. Настрой продукты в App Store Connect / Google Play Console
2. Используй Sandbox/Test аккаунты для тестирования
3. Проверь покупку и восстановление покупок (iOS)

---

## Ожидаемый результат

### При нажатии Play в Editor:
1. Игра запускается нормально
2. Когда жизней 0, при нажатии Play появляется магазин
3. Нажатие на кнопку покупки добавляет жизни (симуляция в Editor)
4. Магазин закрывается автоматически после успешной покупки
5. Кнопка закрытия работает

### Логи в Console:
- `IAP: Unity Purchasing not enabled` — если пакет не установлен (нормально для тестирования в Editor)
- `IAP [EDITOR]: Simulated purchase - Added X lives` — симуляция покупки в Editor
- `IAP: Store initialized successfully` — если пакет установлен и инициализирован

---

## Файлы в архиве

```
Scripts/
├── IAP/
│   ├── IAPConfig.cs
│   ├── IAPManager.cs
│   ├── IAPButton.cs
│   └── LivesShopUI.cs
└── UI/
    └── UIManager.cs (обновленный)
```

---

## Важные заметки

1. **Без Unity IAP пакета** — в Editor работает симуляция покупок для тестирования
2. **Product ID** — должны совпадать с ID в App Store Connect / Google Play Console
3. **Restore Purchases** — обязателен для iOS (требование Apple)
4. **Consumable продукты** — жизни являются consumable (можно покупать многократно)
5. **DontDestroyOnLoad** — IAPManager сохраняется между сценами

---

## Следующая итерация

**Итерация 8: Очки и Streak** — подсчёт очков, множители, визуальная интенсивность от стрика
