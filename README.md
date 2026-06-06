# 🚀 Prompt Processing System

Asynchroniczna aplikacja do przetwarzania promptow użytkownika.

## 🛠️ Tech Stack

- **Frontend:** React, Vite, Axios
- **Backend:** C#, .NET 10.0.8, Entity Framework Core
- **Messaging:** RabbitMQ, MassTransit
- **Testing:** xUnit, Moq, In-Memory DB

## 🧠 The Architecture

1. **Frontend Submission:** Interfejs w Reakcie pakuje prompty i wysyła wszystkie na raz do API.
2. **API & Database:** Kontroler w .NET odbiera DTO, zapisuje go w bazie ze statusem Pending (Oczekujące) i przez MassTransita wrzuca powiadomienie PromptCreated na kolejke. API od razu rzuca na frontend status 200
3. **RabbitMQ:** Nasz prompt ląduje w RabbitMQ. RabbitMQ trzyma je w kolejce i czeka, aż Worker ją przejmie. (Ustawione specjalniew wokerze, 1 zadanie na raz)
4. **Background Worker:** PromptWorker zgarnia zadanie z kolejki RabbitMQ. Podmienia status w bazie na Processing (Przetwarzanie), czeka 5 sekund (żeby zasymulować opóźnienie AI), a dopiero potem uderza do API Gemini.
   (Tutaj mialo byc API Gemini, ale niestety od ostatniego projektu Google zablokowalo darmowe prompty, zostawilem wiec kod do gemini service jako wykomentowany, wiecej o tym pozniej)
5. **Wyniki i bledy:** Kiedy Gemini w końcu wypluje odpowiedź, Worker zapisuje ją w bazie i zmienia status na Completed.
6. **Frontend Polling:** Przez cały ten czas apka w Reakcie co sekundę, puka do API po najnowsze dane (GET /prompts). Dzięki temu na ekranie wszystko zmienia się płynnie i na żywo (Pending ➔ Processing ➔ Completed / Failed).

## ✨ Key Features

- **Procesowanie:** UI nigdy nie "zamarza", nawet gdy przetwarzanie na backendzie trwa kilka lub kilkanaście sekund.
- **Batching:** Użytkownik może wysłać wiele promptów jednocześnie (na froncie obsłużone za pomocą `Promise.allSettled`).
- **Error handling:** Mozliwość przetestowania (wysłanie dokładnie słowa `"error"` wyrzuci w backendzie błąd, który pokaże normalnie nie użyty status `Failed`)
- **Real-time UX:** Czytelne i dynamiczne statusy zrealizowane za pomocą dedykowanego komponentu `StatusBadge`.

## AI Integration

Niestety mimo prób okazało się, przynajmniej z tego co znalazłem, że gemini zaostrzyło dostęp do modeli, i mimo odpowiednik API kluczy oraz szukania najłatwiejszego modelu i zrobienia kodu, nie udało mi się uzyskać response, mimo, że kod pukał i działał.
![Zrzut ekranu](images/screen.png)
Probowałem też nowej rzeczy na modelu z którym nigdy nie pracowałem i w aplikacji, z której też średnio korzystałem :D.<br>
Chciałem postawić lokalnego LLMa Ollamę, ale po zobaczeniu ze sam silnik bez modelu posiada 3,5GB stwierdziłem, że raczej nie chcielibyście tego specjalnie pobierac żeby odpalić aplikacje. <br>
myślałem też na zrobieniu flagi do pobrania ollamy lub korzystania z mocka, ale nie chciałem przekombinowywać więc zostałem przy mock Response, jednocześnie zostawiając kod GeminiService z fallbackiem w przypadku braku klucza. <br>
Zrobiłem już to na potrzeby tego projektu, po co ma się marnować?<br>
W przypadku posiadania klucza API Gemini, wystarczy podmienić klucz cleanApiKey w Development/appsettings.Development.json

```bash
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "cleanApiKey": "Your_Gemini_API_Key_Here"
}
```

## 🚀 Jak uruchomić lokalnie

Zdecydowałem się na użycie Dockera, ze względu na to, że wydaje się to łatwiejsze dla użytkowników niż instalowanie osobno RabbitMQ czy PostgreSQL. Co ciekawe jak już wcześniej wspominałem, średnio miałem okazję go używać, ale w sumie fajna sprawa :D

### Wymagania

- [.NET 10.0 SDK] (lub nowszy)
- [Node.js]
- [Docker Desktop] (do postawienia bazy danych i kolejki RabbitMQ)

### 1. Uruchom infrastrukturę (RabbitMQ & PostgreSQL)

Dzięki Docker Compose nie trzeba wpisywać długich komend czy instalowac osobno aplikacji. W głównym folderze projektu (tam gdzie znajduje się plik `docker-compose.yml`) odpal:

```bash
docker compose up -d
```

### Haslo do DB

Dodaj plik .env w Backend/ i wpisz w nim hasło do bazy, w tym przypadku na potrzeby projektu:

```bash
DB_PASSWORD=SecretPassword123!
```

### 2. Uruchom Backend

Przejdź do folderu backendu i uruchom aplikację. Migracje bazy danych (PostgreSQL) wykonają się automatycznie przy starcie i resetują sie z każdym resetem backendu:

```bash
cd Backend
dotnet run
```

### 3. Uruchom Frontend

Przejdź do folderu frontendowego, zainstaluj zależności i odpal serwer deweloperski Vite:

```bash
cd Backend
npm intall
npm run dev
```

### 4. Przejdź na Frontend

Wpisz w przeglądarkę: http://localhost:5173/

Wszystko powinno działać! (Przetestowałem na innym kompie od zera ;D)

## 🔮 Co planuję poprawić (Future Improvements)

- **Integracja AI:** Połączenie z prawdziwym AI
- **Unit Tests Expansion:** Większa ilość testów
- **Logging:** Można by dodać zewnętrzna logowania, z reguły robiłem to w Azure insights
- **Containerization:** Przygotowanie `Dockerfile` dla samej aplikacji, aby można ją było uruchomić w 100% w kontenerach. Były próby ogarnięcia tego, ale za dużo czasu szło na problemy Linuxowe z WSL, więc stwierdziłem ze na potrzeby projektu to pominę.

## Notatnik

Chcesz zobaczyć, jak wyglądała droga od zera do działającego systemu? W pliku [DEVELOPMENT_LOG.md](DEVELOPMENT_LOG.md) prowadziłem zapiski z procesu tworzenia backendu!
Na przestrzeni projektu jak i notatek mozna sie spotkać rownież z komentarzami w języku polskim i angielskm, po prostu zapisywalem w jakim języku najsprawniej mi cos wpadło do głowy, stwierdzilem ze nie bede ujednolicał, myślę, że nie ma to większego znaczenia

### Na potrzeby tego projektu przedstawiam readme w dosyć niekonwencjonalnym stylu (nie dość że po polsku, to jeszcze luźno. Profesjonalne Readme zrobiłbym zupełnie inaczej!)
