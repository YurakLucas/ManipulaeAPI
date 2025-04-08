# Manipulaê Video 

Este projeto desenvolvido consiste em uma API em .NET 8 para consulta, inserção, atualização e exclusão (soft delete) de dados obtidos a partir da API de vídeos do YouTube. A solução adota os princípios de Clean Architecture e Domain-Driven Design (DDD) e utiliza JWT para proteger os endpoints.

## Desenho da Arquitetura

A arquitetura do projeto está organizada em camadas, conforme ilustrado abaixo:

                       +--------------------------------+
                       |       Manipulaê API            |
                       |  (Manipulae Video )            |
                       +--------------------------------+
                                      /     \
                                     /       \
                     +---------------+     +-------------------+
                     | AuthController |   | VideosController |
                     +---------------+     +-------------------+
                                 \           /
                                  \         /
                         +---------------------+
                         |  Application Layer  |
                         |  (DTOs, Interfaces) |
                         +---------------------+
                                  |
                                  v
                         +---------------------+
                         |   Domain Layer      |
                         |   (Entities: Video) |
                         +---------------------+
                                  |
                                  v
                         +---------------------+
                         | Infrastructure Layer|
                         | (EF Core, Services) |
                         +---------------------+
                                  |
                                  v
                         +---------------------+
                         |  SQLite Database    |
                         +---------------------+



## Tecnologias Utilizadas

- **.NET 8**: Plataforma de desenvolvimento.
- **C#**: Linguagem de programação.
- **Entity Framework Core com SQLite**: Banco de dados local para persistência dos dados.
- **JWT**: Implementado vpara proteger os endpoints.
- **HttpClient e System.Text.Json**: Para realizar requisições REST à API do YouTube e interpretar as respostas JSON sem dependências externas.
- **Swagger**: Para documentação e teste interativo dos endpoints.
- **xUnit e Moq**: Para testes unitários.

## Como Funciona

A API realiza as seguintes operações:
1. **Busca e Persistência de Vídeos**: 
   - Consome a API do YouTube para obter vídeos relacionados à manipulação de medicamentos, filtrados para o ano de 2022 e vídeos brasileiros.
   - Os dados são persistidos em um banco SQLite utilizando o EF Core.
   - A duração dos vídeos é obtida por uma chamada adicional para o endpoint de detalhes da API do YouTube e convertida do formato ISO 8601 para um formato legível.

2. **Endpoints CRUD**:
   - **Filtrar**: Permite filtrar os vídeos por título, duração, autor, data de publicação e um parâmetro “q” para busca em título, descrição e nome do canal.
   - **Inserir**: Insere novos registros de vídeos no banco.
   - **Atualizar**: Atualiza dados de vídeos já existentes.
   - **Excluir (Soft Delete)**: Marca o registro como excluído sem removê-lo fisicamente do banco.

## Instruções para Compilar/Executar

1. **Clone o repositório**:
git clone <URL-do-repositório> cd ManipulaeAPI

2. **Abrir a solução**:
- Abra o arquivo `ManipulaeVideo.sln` no Visual Studio 2022 ou em sua IDE preferida.

3. **Restaurar os pacotes NuGet**:
- O Visual Studio geralmente faz isso automaticamente, ou execute:
  ```
  dotnet restore
  ```

4. **Configurar as variáveis de ambiente**:
- Defina a variável de ambiente `YOUTUBE_API_KEY` com sua chave da API do YouTube.
- No arquivo `appsettings.json` da API (Manipulae.API), ajuste as configurações conforme necessário (por exemplo, credenciais de Basic Auth para gerar o token do JWT).

5. **Compilar a Solução**:
dotnet build


6. **Executar a API**:
- Defina o projeto de inicialização (Manipulae.API).
- Execute a aplicação (F5 ou `dotnet run`).
- A aplicação utilizará o EF Core para criar automaticamente o banco SQLite (se ainda não existir) e suas tabelas.

7. **Acessar a Documentação via Swagger**:
- Por padrão, a API expõe o Swagger em `/swagger`. Exemplo:

  ```https://localhost:5001/swagger```

- Use o Swagger para testar os endpoints.
Uso Básico dos Endpoints

Manipulação de Vídeos (Manipulae.API)

**POST /api/videos/fetch-youtube**
Descrição: Busca vídeos do YouTube e insere os registros no banco.
Exemplo de Payload: (não é necessário enviar dados, a chamada inicia o processo)
Resposta: Lista dos vídeos inseridos.

**GET /api/videos/filter**
Descrição: Filtra os vídeos persistidos com base em parâmetros opcionais (título, duração, autor, data, q).

```Exemplo: /api/videos/filter?title=exemplo&q=medicamentos```

**POST /api/videos/insert**
Descrição: Insere um novo vídeo manualmente.
Exemplo de Payload:

Json
```
{
  "Title": "Exemplo de Vídeo",
  "Description": "Descrição do vídeo",
  "Channel": "Canal Exemplo",
  "Duration": "PT10M3S",
  "PublishDate": "2022-05-01T00:00:00Z"
}
```

**PUT /api/videos/update**
Descrição: Atualiza os dados de um vídeo existente.

```Exemplo de Payload: Similar ao de inserção, mas deve conter o ID (gerado automaticamente).```

**DELETE /api/videos/{id}**

```Descrição: Realiza o soft delete do vídeo (marca como excluído).```



8. **Executar os Testes Unitários**:
- Dentro da pasta `tests/`, execute:
  ```
  dotnet test
  ```
- Ou utilize o Test Explorer do Visual Studio para visualizar os resultados.

## Possíveis Evoluções Futuras

- **Mensageria Assíncrona**: Implementar um broker (como RabbitMQ ou Kafka) para desacoplar o processo de ingestão de dados do YouTube da operação CRUD.
- **Persistência em Banco Relacional**: Utilizar SQL Server ou PostgreSQL para produção, garantindo histórico persistente e maior robustez.
- **Cache Distribuído (Redis)**: Implementar caching no endpoint de consolidação para melhorar a performance em cenários de alta carga.
- **Monitoramento e Observabilidade**: Integrar com ferramentas como Application Insights, Prometheus e Grafana para monitoramento de logs, métricas e alertas.

## Resumo

Este projeto demonstra a aplicação de boas práticas de Clean Architecture e DDD, com uma separação clara de camadas e responsabilidades. A API permite gerenciar dados obtidos da API do YouTube, utilizando SQLite para persistência, e está protegida por JWT. Com testes unitários e documentação via Swagger, a solução fornece uma base sólida para evoluções futuras e para a implementação de melhorias em ambientes de produção.

---
Qualquer dúvida ou sugestão, sinta-se à vontade para abrir uma issue ou enviar um pull request.
