<p align="center">
  <img src="https://capsule-render.vercel.app/api?type=waving&amp;color=0:FFF0F5,50:F0B5CF,100:D77EA5&amp;height=280&amp;section=header&amp;text=atm%20banking%20api&amp;fontSize=65&amp;fontColor=672344&amp;fontAlignY=38&amp;desc=BANKING%20OPERATIONS%20SYSTEM&amp;descSize=18&amp;descAlignY=58" alt="ATM Banking API" />
</p>

<p align="center">
  <img src="https://img.shields.io/badge/C%23-963B67?style=for-the-badge" alt="C#" />
  <img src="https://img.shields.io/badge/.NET_9-AC4D77?style=for-the-badge&amp;logo=dotnet&amp;logoColor=white" alt=".NET 9" />
  <img src="https://img.shields.io/badge/ASP.NET_Core-B74D70?style=for-the-badge&amp;logo=dotnet&amp;logoColor=white" alt="ASP.NET Core" />
  <img src="https://img.shields.io/badge/xUnit-672344?style=for-the-badge" alt="xUnit" />
  <img src="https://img.shields.io/badge/SOLID-963B67?style=for-the-badge" alt="SOLID" />
  <img src="https://img.shields.io/badge/Hexagonal_Architecture-AC4D77?style=for-the-badge" alt="Hexagonal Architecture" />
</p>

# ATM Banking API

## ✧ О проекте

Backend-приложение на C# для моделирования работы банкомата: управление банковскими счетами, выполнение денежных операций и хранение истории транзакций. HTTP API предоставляет пользовательские и административные сценарии, а бизнес-логика отделена от механизмов хранения данных.

## ✧ Функциональные возможности

- Создание банковского счёта через административную сессию.
- Аутентификация клиента по номеру счёта и PIN-коду.
- Создание пользовательских и административных сессий с ключом `Guid`.
- Просмотр баланса, пополнение и снятие средств.
- Валидация суммы операции и проверка достаточности средств.
- История успешных пополнений и снятий с отметками времени.
- Проверка роли сессии и обработка некорректных запросов.

## ✧ Технологический стек

| Категория | Технологии |
| --- | --- |
| Язык | C# |
| Платформа | .NET 9, ASP.NET Core Web API |
| Архитектура | Ports & Adapters, многослойная структура |
| Внедрение зависимостей | Microsoft.Extensions.DependencyInjection |
| Хранение | In-memory repositories |
| Тестирование | xUnit, собственные fake-репозитории |

## ✧ Архитектура

Проект построен на принципах гексагональной архитектуры (Ports & Adapters) с разделением доменной логики, прикладных сервисов, инфраструктуры и слоя представления.

Взаимодействие между компонентами организовано через абстракции и внедрение зависимостей (Dependency Injection). Реализация хранилищ изолирована от бизнес-логики.

| Модуль | Ответственность |
| --- | --- |
| Core | Доменные модели банковских счетов, сессий и операций. |
| Application | Прикладные сервисы, бизнес-логика и контракты репозиториев. |
| Infrastructure | Хранение счетов, сессий и истории операций в памяти. |
| WebApi | HTTP-контроллеры, DTO, конфигурация и регистрация зависимостей. |

В текущей реализации контроллер создания счёта обращается к портам напрямую.
## ✧ Инженерные решения

- **Repository Pattern** — прикладные сервисы работают с интерфейсами хранилищ.
- **Dependency Injection** — модули регистрируют зависимости через методы расширения.
- **Factory** — отдельная фабрика создаёт пользовательские и административные сессии.
- **Encapsulation** — изменение баланса выполняется через методы доменного объекта.
- **Separation of Concerns** — бизнес-правила отделены от HTTP и реализации хранилищ.

## ✧ HTTP API

| Метод | Endpoint | Назначение |
| --- | --- | --- |
| POST | `/sessions/admin` | Создание административной сессии |
| POST | `/sessions/user` | Создание пользовательской сессии |
| POST | `/accounts/create` | Создание счёта |
| GET | `/accounts/balance` | Просмотр баланса |
| POST | `/accounts/deposit` | Пополнение |
| POST | `/accounts/withdraw` | Снятие средств |
| GET | `/accounts/history` | История операций |

Для пользовательских операций используется ключ пользовательской сессии; для создания счёта — ключ административной сессии. Примеры запросов находятся в [`AtmSystem.WebApi.http`](src/AtmSystem.WebApi/AtmSystem.WebApi.http).

## ✧ Структура проекта

```text
src/
├── AtmSystem.Core/            # Доменная модель
├── AtmSystem.Application/     # Сервисы и порты
├── AtmSystem.Infrastructure/  # In-memory хранилища
└── AtmSystem.WebApi/          # HTTP API

tests/
└── AtmSystem.Tests/           # Модульные тесты
```

## ✧ Запуск

Требуется .NET SDK 9.0 (для `.slnx` — версия 9.0.200 или новее в ветке 9.0).

```bash
dotnet restore AtmSystem.slnx
dotnet build AtmSystem.slnx
dotnet test AtmSystem.slnx
```

Для локального запуска на macOS/Linux задай собственный системный пароль через переменную окружения и запусти API:

```bash
export SystemPassword__Value='replace-with-your-local-password'
dotnet run --project src/AtmSystem.WebApi --no-launch-profile --urls http://localhost:5071
```

Используй только тестовые данные. Для демонстрации отправь запросы из `.http`-файла по порядку (вставь локальный пароль и полученные ключи сессий в соответствующие переменные). После создания счёта с балансом 1000, пополнения на 250 и снятия 100 ожидаемый баланс — 1150.

## ✧ Тестирование

Четыре исходных теста проверяют пополнение, снятие средств и отказ при недостаточном балансе. Три дополнительные проверки покрывают создание административной сессии, неверный пароль и пустую настройку пароля. Тестовые двойники изолируют прикладной слой от реального хранилища и HTTP API.

```bash
dotnet test AtmSystem.slnx --collect:"XPlat Code Coverage"
```

## ✧ Ограничения

Проект демонстрационный: данные сбрасываются после перезапуска, PIN-коды хранятся в исходном виде в памяти, сессии не имеют срока действия. Нет полноценного аудита, атомарности финансовых операций при конкурентных запросах и промышленной системы безопасности. История отражает пополнения и снятия, а не все обращения к счёту. Не предназначен для реальных финансовых данных или публичного развёртывания.

*Учебный проект по архитектуре backend-приложений и объектно-ориентированному проектированию.*

![Footer](https://capsule-render.vercel.app/api?type=waving&color=0:FFF0F5,50:F0B5CF,100:D77EA5&height=110&section=footer&text=%E2%99%A1&fontSize=38&fontColor=672344&fontAlign=50&fontAlignY=72)
