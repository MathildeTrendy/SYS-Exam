# SYS-Exam

Dette projekt bruges som eksempel på teststrategi i et microservices-setup.

# Oversigt

Projektet består af to services og tilhørende tests:

* ProductService

- ASP.NET minimal API med in-memory produkter og lagerbeholdning.

- Endpoint: POST /products/{id}/reserve (Modtager en ReserveRequest 
med feltet Quantity og Returnerer en ReserveResponse med felterne Success og Reason)

* OrderService

- ASP.NET minimal API der opretter ordrer.

- Kalder ProductService via HttpClient for at reservere lager.

- Endpoint: POST /orders

- Modtager productId og quantity

- Returnerer enten en oprettet ordre eller en fejl (fx OUT_OF_STOCK)

* OrderService.UnitTests

- xUnit unit tests for fejlhåndtering i OrderService, blandt andet ErrorMapper.

* ProductService.ContractTests

- xUnit-baserede contract tests for ProductService.

- Tester POST /products/p1/reserve for både succes- og OUT_OF_STOCK-scenarier.

# Krav

* .NET 9 SDK

* Postman

# Kørsel af services

1. Fra projektroden (mappen SYS-Exam):

* Start ProductService

* cd ProductService

* dotnet run

* Lytter på http://localhost:5203

2. Start OrderService i en ny terminal

* cd OrderService

* dotnet run

* Lytter på http://localhost:5102

# Manuelle end-to-end tests (fx i Postman)

Succesfuld ordre:

* Metode: POST

* URL: http://localhost:5102/orders

* Body (JSON):

- productId: "p1"

- quantity: 2

- Forventet svar: status 200 OK og en ordre med status "CREATED".

OUT_OF_STOCK-fejl:

* Metode: POST

* URL: http://localhost:5102/orders

* Body (JSON):

- productId: "p1"

- quantity: 999

* Forventet svar: status 400 Bad Request og et svar med error = "OUT_OF_STOCK".

# Kørsel af tests

Fra projektroden (SYS-Exam):

* Unit tests for OrderService:

* dotnet test OrderService.UnitTests/OrderService.UnitTests.csproj

* Contract tests for ProductService:

* dotnet test ProductService.ContractTests/ProductService.ContractTests.csproj

