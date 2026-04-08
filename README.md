# gloBUS Music

Простое настольное музыкальное приложение на WPF.

## Как запустить

### 1. Склонировать репозиторий
```bash
git clone https://github.com/Too4er/gloBUS-Music.git
2. Открыть проект

Открой файл gloBUS Music.slnx в Visual Studio.

3. Восстановить зависимости

После открытия проекта дождись восстановления NuGet-пакетов.

4. Проверить подключение к базе данных

Открой файл:

gloBUS Music/Data/gloBUS_MusicDbContext.cs

По умолчанию используется такая строка подключения:

Server=localhost;Database=gloBUS_Music;Trusted_Connection=True;TrustServerCertificate=True;

Если у тебя другой сервер SQL Server, измени строку подключения под себя.

5. Применить миграции

Открой Package Manager Console в Visual Studio и выполни команду:

Update-Database
6. Запустить приложение

Выбери проект gloBUS Music как стартовый и нажми F5 или кнопку Start.

Что нужно для запуска
Windows
Visual Studio 2022
.NET 10 SDK
SQL Server
Если приложение не запускается

Проверь, что:

установлен .NET 10 SDK
SQL Server запущен
строка подключения указана правильно
миграции успешно применились
