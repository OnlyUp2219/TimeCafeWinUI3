# Docker TODO для TimeCafeWinUI3

## Что уже сделано:

- ✅ Redis добавлен в Docker

## Что нужно сделать:

### 1. Добавить SQL Server в Docker

```bash
docker run -d --name sqlserver -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Password123!" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Создать docker-compose.yml

Файл для управления всеми контейнерами (Redis + SQL Server)

### 3. Настроить строку подключения

Изменить в `appsettings.json` или коде:

- **Было**: `Server=localhost;Database=TimeCafe;Trusted_Connection=true;`
- **Станет**: `Server=localhost,1433;Database=TimeCafe;User Id=sa;Password=Password123!;TrustServerCertificate=true;`

### 4. Проверить подключение

Убедиться, что WinUI3 приложение может подключиться к базе данных в Docker

## Архитектура после настройки:

```
WinUI3 App → Core Library → Entity Framework → SQL Server (в Docker)
                                    ↓
                                Redis (в Docker)
```

## Важные моменты:

- WinUI3 остается настольным приложением
- Core Library не меняется
- Только строка подключения к базе данных
- Все работает через localhost (порты 1433 и 6379)

## Команды для управления:

```bash
# Запуск всех контейнеров
docker-compose up -d

# Остановка всех контейнеров
docker-compose down

# Просмотр логов
docker-compose logs

# Перезапуск конкретного контейнера
docker restart sqlserver
docker restart redis
```

---

## ДОПОЛНИТЕЛЬНЫЕ ВОПРОСЫ ПО АРХИТЕКТУРЕ:

### 1. Разделение Core на Domain и Infrastructure

- Как правильно разделить существующий Core проект?
- Что должно быть в Domain, что в Infrastructure?
- Как изменить структуру папок?

### 2. Переход с библиотеки на Web API

- Как создать Web API проект в существующем решении?
- Как настроить DI контейнер для API?
- Как создать контроллеры для существующих сервисов?
- Как WinUI3 будет обращаться к API?

### 3. CQRS и MediatR

- Нужно ли сначала перейти на полноценный CQRS (каждая команда в отдельном файле)?
- Можно ли использовать MediatR с текущей структурой (гибридный подход)?
- Как правильно структурировать команды и запросы?
- Какие преимущества и недостатки у каждого подхода?

### 4. Архитектурные улучшения

- Clean Architecture - как применить к проекту?
- Event-Driven Architecture - стоит ли добавлять?
- Микросервисы - когда и как переходить?
