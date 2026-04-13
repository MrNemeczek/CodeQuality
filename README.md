# LibraryProject

Prosty projekt API do biblioteki oparty o `ASP.NET Core`, `PostgreSQL`, `Clean Architecture` i `CQRS`.

## Co zawiera projekt

- `src/Library.Api` - warstwa prezentacji, kontrolery, Swagger, middleware bledow
- `src/Library.Application` - komendy, zapytania, handlery, DTO, walidacja
- `src/Library.Domain` - encje i reguly domenowe
- `src/Library.Infrastructure` - EF Core, PostgreSQL, repozytoria, migracje, seed danych
- `tests/Library.UnitTests` - testy logiki domenowej i handlerow
- `tests/Library.IntegrationTests` - test przeplywu API end-to-end

## Moduly MVP

- ksiazki
- czytelnicy
- wypozyczenia

## Reguly biznesowe

- nie mozna wypozyczyc ksiazki, jesli nie ma dostepnych egzemplarzy
- nie mozna oddac wypozyczenia drugi raz
- nie mozna usunac ksiazki, jesli ma aktywne wypozyczenia
- liczba egzemplarzy nie moze byc mniejsza od liczby aktywnych wypozyczen

## Uruchomienie lokalne

1. Uruchom PostgreSQL:

```bash
docker compose up -d
```

2. Uruchom API:

```bash
dotnet run --project src/Library.Api
```

3. Otworz Swagger:

- `http://localhost:5118/swagger`
- `https://localhost:7092/swagger`

## Konfiguracja bazy

Domyslny connection string jest w `src/Library.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "LibraryDatabase": "Host=localhost;Port=5432;Database=library_db;Username=postgres;Password=postgres"
  }
}
```

Przy starcie aplikacja automatycznie:

- wykonuje migracje EF Core
- dodaje dane startowe, jesli baza jest pusta

## Przydatne komendy

```bash
dotnet build LibraryProject.slnx
dotnet test LibraryProject.slnx
dotnet ef database update --project src/Library.Infrastructure --startup-project src/Library.Api
```

## Endpointy

### Ksiazki

- `POST /api/books`
- `GET /api/books`
- `GET /api/books/{id}`
- `PUT /api/books/{id}`
- `DELETE /api/books/{id}`

### Czytelnicy

- `POST /api/readers`
- `GET /api/readers`
- `GET /api/readers/{id}`
- `GET /api/readers/{id}/loans`

### Wypozyczenia

- `POST /api/loans`
- `GET /api/loans`
- `POST /api/loans/{id}/return`

## CQRS w projekcie

- zapis jest obslugiwany przez komendy (`CreateBook`, `RegisterReader`, `BorrowBook`, `ReturnBook`)
- odczyt jest obslugiwany przez zapytania (`GetBooks`, `GetReaders`, `GetActiveLoans`)
- handlery sa rozdzielone od kontrolerow i infrastruktury

## Testy

- testy jednostkowe sprawdzaja reguly domenowe oraz obsluge konfliktu przy wypozyczaniu
- test integracyjny sprawdza pelny scenariusz: utworzenie ksiazki, rejestracja czytelnika, wypozyczenie i zwrot
