# Patient Management System

## 1. Visão Geral e Arquitetura

### Visão Geral

Esta solução foi concebida para ser robusta, escalável e de fácil manutenção, adotando boas práticas de Clean Code, SOLID, Domain-Driven Design (DDD) e Clean Architecture. O principal desafio é garantir que o sistema de gestão de pacientes e históricos médicos seja capaz de lidar com grandes volumes de dados, mantendo consultas performáticas e organizando os dados de forma que a escalabilidade futura seja garantida.

### Arquitetura DDD e Clean Code

O projeto segue os princípios da Clean Architecture e está dividido em:

- **Domain:** Entidades e regras de negócio.
- **Application:** DTOs e interfaces que representam os casos de uso.
- **Infrastructure:** Implementações dos serviços, acesso a dados (EF Core com SQLite) e integração com APIs externas.
- **API:** Camada de apresentação com endpoints RESTful e configuração de autenticação JWT.

### Diagrama Arquitetural

```
                    +---------------------------+
                    |        Patient.API        |
                    | (Controllers + JWT Auth) |
                    +-------------+-------------+
                                  |
              +------------------+------------------+
              |                                     |
     +--------v--------+                   +--------v--------+
     |   Application   |                   |   Infrastructure |
     | DTOs + Interfaces|                 |  EF Core, APIs     |
     +--------+--------+                   +--------+--------+
              |                                     |
     +--------v--------+                   +--------v--------+
     |     Domain      |                   |     Database     |
     |  Entidades DDD  |                   |   SQLite (EF)    |
     +-----------------+                   +------------------+
```

---

## 2. Estrutura do Projeto

```
PatientSolution/
├── Patient.API/                       # Camada de apresentação (Controllers + Auth + Swagger)
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ExternalExamsController.cs
│   │   ├── MedicalHistoryController.cs
│   │   └── PatientsController.cs
│   ├── appsettings.json
│   ├── Program.cs
│   └── Program.Public.cs
│
├── Patient.Application/              # Casos de uso e contratos
│   ├── DTOs/
│   │   ├── CreateMedicalHistoryDto.cs
│   │   ├── CreatePatientDto.cs
│   │   ├── ExamDto.cs
│   │   ├── MedicalHistoryDto.cs
│   │   ├── PatientDto.cs
│   │   ├── UpdateMedicalHistoryDto.cs
│   │   └── UpdatePatientDto.cs
│   └── Interfaces/
│       ├── IExamsService.cs
│       ├── IMedicalHistoryService.cs
│       └── IPatientService.cs
│
├── Patient.Domain/                   # Entidades do domínio
│   └── Entities/
│       ├── MedicalHistory.cs
│       └── Patient.cs
│
├── Patient.Infrastructure/          # Acesso a dados e integrações externas
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Models/
│   │   └── BRCepResponse.cs
│   └── Services/
│       ├── ExamsService.cs
│       ├── MedicalHistoryService.cs
│       └── PatientService.cs
│
├── tests/
│   ├── Patient.API.IntegrationTests/
│   │   └── PatientsControllerIntegrationTests.cs
│   ├── Patient.API.Tests/
│   │   ├── ExternalExamsControllerTests.cs
│   │   ├── MedicalHistoryControllerTests.cs
│   │   └── PatientsControllerTests.cs
│   └── Patient.Infrastructure.Tests/
│       ├── ExamsServiceTests.cs
│       ├── MedicalHistoryServiceTests.cs
│       └── PatientServiceTests.cs
```

---

## 3. Tecnologias e Boas Práticas

- **.NET 8**, **C#**, **Entity Framework Core** com **SQLite**
- **DDD** (Domain-Driven Design)
- **Autenticação JWT**
- **Swagger/OpenAPI** para documentação
- **xUnit** + **Moq** para testes unitários
- **WebApplicationFactory** para testes de integração

---

## 4. Como Executar

```bash
git clone <url-do-repositorio>
cd PatientSolution
dotnet restore
dotnet build
dotnet run --project src/Patient.API
```

Abra no navegador:
```
https://localhost:<porta>/swagger
```

---

## 5. Funcionalidades

- CRUD de Pacientes
- CRUD de Histórico Médico
- Consulta externa de exames (mock ou BrasilAPI)
- Autenticação JWT (com endpoint de login)
- Soft Delete de registros
- Testes automatizados (unitários e integração)

---

## 6. Evoluções Futuras

- Integração com RabbitMQ ou Kafka
- Migração para PostgreSQL ou SQL Server em produção
- Cache distribuído com Redis
- Dashboard com Prometheus e Grafana
- Logs estruturados com Serilog
- Suporte a Multi-Tenant

---

## 7. Testes

```bash
dotnet test
```

Ou rode os testes pelo Test Explorer no Visual Studio.

---

## 8. Autenticação

1. Endpoint de login:
```http
POST /api/auth/login
```

2. Adicione o token retornado no header:
```http
Authorization: Bearer {seu_token}
```

---

## 9. Swagger

O Swagger está habilitado em:
```
https://localhost:<porta>/swagger
```

Permite testar endpoints protegidos com JWT diretamente via UI.

---

## 10. Arquitetura e Separação de Responsabilidades
Camadas Independentes:
A aplicação está dividida em quatro camadas distintas:

**Domain:** Contém as entidades e as regras de negócio.

**Application:** Define DTOs, interfaces e casos de uso.

**Infrastructure:** Gerencia o acesso aos dados (EF Core com SQLite) e integra serviços externos (ex.: API para consulta de exames).

**API**: Exposição dos endpoints RESTful.
Essa separação facilita a manutenção e a escalabilidade, permitindo que mudanças em uma camada (ex.: troca de banco de dados) não afetem a lógica de negócio.

**Microservices / Modularização:**
Em um cenário de grandes volumes e alta demanda, a aplicação pode ser desmembrada em microservices independentes para cada domínio (por exemplo, serviço de pacientes e serviço de histórico médico). Essa abordagem facilita o escalonamento horizontal, possibilitando a replicação de componentes que são mais críticos.

## 11. Boas Práticas de Modelagem de Dados
**Entidades e Relacionamentos:**
As entidades foram modeladas de forma a refletir o domínio real. O uso de relacionamentos (ex.: Paciente e seu Históricos Médicos) é feito considerando os conceitos do DDD para garantir que a modelagem seja fiel ao negócio.

**Normalização e Índices:**

**Normalização:**
Os dados são normalizados para evitar redundâncias e manter a integridade.

**Índices:**
Índices apropriados são criados nas colunas frequentemente consultadas (ex.: CPF, Data de Nascimento, e campos utilizados em filtros e joins) para acelerar as consultas, principalmente com grandes volumes de registros.

**Soft Delete:**
Para exclusões, é utilizada a estratégia de soft delete, marcando registros como deletados sem removê-los fisicamente. Isso possibilita auditoria e mantém a integridade referencial mesmo quando os dados crescem em quantidade.

## 12. Consultas Performáticas
**Otimização de Consultas com Entity Framework:**

Uso adequado de Include e ThenInclude para carregamento de relacionamentos apenas quando necessário.

Uso de projeções (select) para retornar apenas os campos essenciais.

Análise dos planos de execução e otimização de consultas quando houver necessidade.

**Cache Distribuído:**
Implementar cache (por exemplo, via Redis) para armazenar resultados de consultas frequentes, como listagens e relatórios consolidados. Isso diminui a carga sobre o banco de dados e melhora o tempo de resposta.

**Paginação e Filtros:**
Para grandes volumes de dados, o endpoint de listagem utiliza paginação e filtros eficientes, garantindo que apenas subconjuntos dos dados sejam retornados por consulta.

## 13. Organização dos Dados para Grandes Volumes
**Particionamento (Partitioning) e Sharding:**
Em cenários de produção, os bancos de dados relacionais podem utilizar particionamento para distribuir a carga dos dados em várias partições. Quando a escala exigir, pode-se optar por sharding para dividir os dados em vários bancos de dados, permitindo escalonamento horizontal.

**Armazenamento e Arquivamento:**
Dados históricos que não são acessados frequentemente podem ser movidos para tabelas de arquivo ou sistemas de armazenamento diferenciados, melhorando o desempenho das consultas sobre os dados ativos.

## 14. Estratégias de Escalabilidade
**Escalabilidade Horizontal:**
A aplicação pode ser containerizada com Docker e orquestrada via Kubernetes, permitindo o escalonamento horizontal de microservices conforme a demanda. Os endpoints mais críticos podem ter réplicas adicionais.

**Mensageria e Processamento Assíncrono:**
Em cenários onde a ingestão de dados externos (por exemplo, consulta de exames) pode ser intensiva, o uso de uma fila de mensageria (RabbitMQ, Kafka) permite que o processamento seja distribuído e desacoplado do fluxo principal da API.

**Monitoramento e Observabilidade:**
Integração com ferramentas de monitoramento (como Application Insights, Prometheus e Grafana) para visualizar métricas, identificar gargalos e ajustar a escalabilidade dinamicamente.

## 15. Principais Decisões Técnicas

**Adotar Clean Architecture e DDD:**
Garante que as regras de negócio estejam isoladas na camada de domínio, facilitando a manutenção e a evolução do sistema sem impacto em outras camadas.

**Uso do Entity Framework Core com SQLite:**
Para este MVP, o SQLite foi escolhido pela simplicidade. No entanto, a arquitetura permite uma fácil migração para bancos de dados mais robustos em produção.

**Implementação de JWT para Autenticação:**
Garante segurança e escalabilidade nas operações, facilitando a integração com outros serviços e a gestão de credenciais de forma centralizada.

**Modularização e Microservices:**
A arquitetura modular permite que cada componente (ex.: gerenciamento de pacientes, histórico médico, consulta externa) seja escalado de forma independente.

## 16. Conclusão

Com essas práticas e decisões técnicas, a solução foi projetada para ser escalável, performática e de fácil manutenção. A separação em camadas e a utilização de boas práticas de modelagem e indexação, juntamente com estratégias de cache e escalonamento horizontal, garantem que o sistema poderá ser adaptado para lidar com grandes volumes de dados e alta demanda. Essa arquitetura serve como base sólida para evolução e implantação em ambientes de produção, mantendo a integridade e a performance dos dados ao longo do tempo.
