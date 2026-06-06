prerequisites:
.net10
docker
rabbitmq
masstransit


myslalem nad uzyciem 8, tak samo jak w typescripcie nie używamy najnowszej technologii, jednak wsparcie konczy się pod koniec 2026.
password123
SecretPassword123!

Uzyje dockera, zamiast pobierania wszystkiego osobno

1. tworzenie poliku docker compose

dotnew new gitignore

dotnet new sln -n PromptProcessingSystem
dotnet new webapi -n Backend -f net10.0
dotnet sln add Backend
tworzenie initial projektu .net

stworzenie enumow promptstatus oraz prompt(co będzie w bazie)

dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
(c# to sql compiler)
ulatwia pracowanie na tabeli, tłumaczy bezpośrednio na SQL

dotnet add package Microsoft.EntityFrameworkCore.Design
(migracja enum prompt do postgre)
Ulatwia migracje tabeli, nie trzeba create table

dodajemy plik appDBContext.cs

:DBContext -> dziedziczymy ta klase dla tłumaczenia c# do sql za pomocą EF Core
DbContextOptions<AppDbContext> options -> conf package, informacje z appsettings.json, PostgreSQL i localhost:5432

public DbSet<Prompt> Prompts => Set<Prompt>(); -> Tworzenie bazy na podstawie enuma Prompt, 

.HasConversion<string>() -> konwersja, faktycznie string statusy zamiast liczb. musimy to zrobić tylko dla promptstatus, bo to w enumie Prompt jedyne pole z moim własnym enumem, który z automat w myśl ogolnej zasady ef core by przypisal 0,1,2,3


pogram.cs updated, its using appdbcontext, using postgresql, looking for adress and credentials from appsettings and passes it to database

dotnet ef migrations add InitialCreate -> Stworzyl templaty z prompt i prompt status do uzycia

dotnet ef database update -> aplikuje w.w plany na baze

instalowanie SQLTools w vs code i sql drivers
polaczenie z baza

<screen>

przeniesienie hasla do bazy dynamicznie z .env
bezpośrednio z .env do appsettings nie dzialalo, fetching przeniesione do program.cs 


Dodanie DTO
Dodanie CORS dla backendu

tworzenie kontrolerow
rozszerza controllerBase -> program.cs wylapie za pomoca builder.Services.AddControllers()

dodanie masstransit do program.cs 
testowanie postmanem, blad System.ArgumentException: Message types must not be anonymous types

Rozwiazanie, dodanie messages/promptcreated.cs 

postman do testow

dodanie get do zobaczenia bazy

RabbitMQ
Dlaczego masstransit wrzuca cos do rabbitMQ. skoro potem z niego wyciąga? czemu kontroler nie może od razu przekazac do workera?
Traffic handling 
bez rabbit mq kontroler leciałby od razu do workera, i każdy użytkownik czekalby kolejno coraz dluzej, rabbitMQ działa jak poczekalnia pozwalajaca na zakolejkowanie dużej ilości promptow. a worker powoli wyciąga prompty

backend działa, az dziwne

frontend otrzymuje status jako int mimo modelBuilder.Entity<Prompt>().Property(p => p.Status).HasConversion<string>();

builder.Services.AddControllers().AddJsonOptions(options =>
    {
        // allows sending strings instead of numbers for enums, for better readability in API responses
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }); // Rejestruje kontrolery

po prawie godzinie stwierdzam, ze Gemini nie daje już darmowych requestow dla jakiegokolwiek modelu. blad 429
zostawiam kod do gemini, robie mocka

pare testy zrobione, proba ogarniecie dockera
Problemy z WSL2, pomijamy i zostawiamy Docker-compose







