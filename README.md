# RTS

Real-time strategy с процедурной генерацией карт и объектов на ней, а также физикой и модификацией юнитов (гравитация, параметры проходимости terrain'а и проч.). Возможно потом будет введена глобальный карта (World map) с пошаговыми боями. Проект скорее всего PvE-only. 
- Windows

## GDD

### Концепция
Игровой процесс после начала боя включает в себя управление юнитами посредством назначения приказов, основная задача - выполнение objective'ов, определяемых в зависимости от сгенерированной карты и настроек боя.

### Визуальный стиль
Интерфейс функциональный и минималистичный, модели юнитов и анимации Hand made или заимствованые из других игр, стиль реалистичный.

### Жизненный цикл
1. Выбор настроек боя (включая режимы игры, фрацкции, количество игроков, ресурсов и т.д.)
2. Выбор параметров случайной генерации карты
3. Классический бой с ботами по правилам жанра rts, но при различных (случайных) условиях карты и objective'ах, которые нужно выполнить как боту, так и игроку (могут быть как симметричные так и ассиметричные задачи).
4. Предоставление результатов боя с информацией о продвижении пр рангу (званию). Ранг сбрасывается/скрывается после долгого перерыва между игровыми сессиями.
5. Модификация юнитов в "Ангаре".
6. Возвращаемся к пункту 1.
### Механики и фичи
- Core
  - Процедурная генерация карты, объектов, модификаторов, objective'ов.
  - Игровая физика.
  - Различные фракции с поддержкой модификаций юнитов.
  - Искусственный интеллект для ботов.
  - Уровни сложности
  - Система званий для игрока.
  - Мини карта и боевой интерфейс.
- Additional
  - World map (пошаговая)
- Juice
  - Multiplayer

### Распределение задач
- Nshtk: решение задач, связанных с процедурной генерацией карт, физикой объектов, созданием ИИ, разработкой режимов игры и внутриигровых задач. Пример внутриигровых задач: ассиметричная - для одного игрока защита определенного здания/объекта, для другого - разрушение этого объекта; симметричная: для обоих игроков необходимость уничтожить их главный юнит; ассиметричная - для одного игрока защита заводов, рандомно расставленных по карте и производящих юниты, для другого - активация бомбы; и т.д. (прототип - игра Into the Breach). Заданий для каждого игрока в одном бою может быть несколько, за каждое выполненное начисляются очки.
- Mikhail: создание игровых фракций (Разработка уникальных особенностей и характеристик для различных фракций. Балансировка игровых параметров фракций и полная настройка юнитов. Интеграция возможностей модификации юнитов в игровой процесс.), мини-карты (Разработка функциональной и понятной мини-карты для управления и навигации. Создание эффективного боевого интерфейса, обеспечивающего игроку необходимую информацию в реальном времени.), системы прогрессии игрока, UI (Создание функционального и минималистичного пользовательского интерфейса. Интеграция элементов UI, обеспечивающих легкость управления и информативность.), системы модификаций юнитов (Разработка системы, позволяющей игрокам модифицировать своих юнитов в "Ангаре". Балансировка и интеграция модификаций в общую механику игры.).

### Итог 1 семестра
Возможность проведения полноценного боя с определяемым игрком количеством ботов, возможность выполнения сгенерированных заданий на различных типах сгенерированных карт для победы в бою, начисление очков по системе прогрессии игрока, выбор из 2-х доступных фракций, выбор нескольких режимов игры (минимум 1).

## Похожие игры
Серия Command & Conquer, Men of War (геймплей), Into the breach (генерация карты и объектов).
![ ](https://cncseries.ru/wp-content/uploads/2017/02/tw-screen45.jpg)

## Билд
[Build](https://drive.google.com/drive/folders/1ELiILyNOlEFE2lCO078aJKK19IEXCZgR?usp=sharing)

## Инструкция по запуску
При запуске игры пользователя встречает меню с 4-мя кнопками: "Начать игру", "Настройки боя", "Настройки" и "Выход из игры". На данном этапе для начала боя пользователю требуется нажать на кнопку "Начать игру", в меню "Настройки" пользователь может изменить параметры графики.
