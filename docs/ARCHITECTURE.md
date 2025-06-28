# Clean Architecture - Acervo Educacional

Este documento descreve a implementação da Clean Architecture no projeto Acervo Educacional.

## 🏗️ Visão Geral da Arquitetura

A Clean Architecture organiza o código em camadas concêntricas, onde as dependências apontam sempre para dentro, em direção ao núcleo do domínio.

```
┌─────────────────────────────────────────────────────────┐
│                    WebApi (Presentation)                │
│  ┌─────────────────────────────────────────────────┐   │
│  │              Infrastructure                     │   │
│  │  ┌─────────────────────────────────────────┐   │   │
│  │  │            Application                  │   │   │
│  │  │  ┌─────────────────────────────────┐   │   │   │
│  │  │  │           Domain                │   │   │   │
│  │  │  │                                 │   │   │   │
│  │  │  │  • Entities                     │   │   │   │
│  │  │  │  • Enums                        │   │   │   │
│  │  │  │  • Interfaces                   │   │   │   │
│  │  │  │                                 │   │   │   │
│  │  │  └─────────────────────────────────┘   │   │   │
│  │  │                                         │   │   │
│  │  │  • DTOs                                 │   │   │
│  │  │  • Services Interfaces                 │   │   │
│  │  │  • Use Cases                           │   │   │
│  │  │                                         │   │   │
│  │  └─────────────────────────────────────────┘   │   │
│  │                                                 │   │
│  │  • Repositories Implementation                  │   │
│  │  • External Services (AWS S3, Email)           │   │
│  │  • Database Context                             │   │
│  │                                                 │   │
│  └─────────────────────────────────────────────────┘   │
│                                                         │
│  • Controllers                                          │
│  • Middleware                                           │
│  • Configuration                                        │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

## 📁 Estrutura de Projetos

### 1. AcervoEducacional.Domain (Núcleo)
**Responsabilidade**: Contém as regras de negócio e entidades principais.

```
Domain/
├── Entities/
│   ├── BaseEntity.cs           # Entidade base com propriedades comuns
│   ├── Usuario.cs              # Entidade de usuário
│   ├── Curso.cs                # Entidade de curso
│   ├── Arquivo.cs              # Entidade de arquivo
│   └── OtherEntities.cs        # Outras entidades do sistema
├── Enums/
│   └── Enums.cs                # Enumerações do domínio
└── Interfaces/
    └── IRepositories.cs        # Interfaces de repositórios
```

**Características**:
- ❌ **Não possui dependências** externas
- ✅ **Contém regras de negócio** puras
- ✅ **Define contratos** (interfaces)
- ✅ **Entidades ricas** com comportamentos

### 2. AcervoEducacional.Application (Casos de Uso)
**Responsabilidade**: Orquestra o fluxo de dados e implementa casos de uso.

```
Application/
├── DTOs/
│   ├── Auth/                   # DTOs de autenticação
│   ├── Usuario/                # DTOs de usuário
│   ├── Curso/                  # DTOs de curso
│   ├── Arquivo/                # DTOs de arquivo
│   └── Common/                 # DTOs comuns
├── Interfaces/
│   └── IServices.cs            # Interfaces de serviços
├── Services/                   # Implementações de casos de uso
└── Mappings/                   # Configurações AutoMapper
```

**Características**:
- ✅ **Depende apenas** do Domain
- ✅ **Define interfaces** para serviços externos
- ✅ **Implementa casos de uso** de negócio
- ✅ **Valida dados** de entrada

### 3. AcervoEducacional.Infrastructure (Detalhes)
**Responsabilidade**: Implementa interfaces definidas nas camadas internas.

```
Infrastructure/
├── Data/
│   └── AcervoEducacionalContext.cs    # Contexto Entity Framework
├── Repositories/
│   ├── BaseRepository.cs              # Repositório base
│   ├── UsuarioRepository.cs           # Repositório de usuários
│   ├── CursoRepository.cs             # Repositório de cursos
│   ├── ArquivoRepository.cs           # Repositório de arquivos
│   ├── UnitOfWork.cs                  # Padrão Unit of Work
│   └── OtherRepositories.cs           # Outros repositórios
├── Services/
│   ├── AwsS3Service.cs                # Serviço AWS S3
│   └── EmailService.cs                # Serviço de email
└── Configurations/
    └── DependencyInjection.cs         # Configuração de DI
```

**Características**:
- ✅ **Implementa interfaces** do Domain e Application
- ✅ **Acessa recursos externos** (banco, APIs, arquivos)
- ✅ **Configura Entity Framework** e mapeamentos
- ✅ **Gerencia transações** e persistência

### 4. AcervoEducacional.WebApi (Interface)
**Responsabilidade**: Expõe funcionalidades via API REST.

```
WebApi/
├── Controllers/
│   ├── AuthController.cs              # Autenticação
│   ├── CursosController.cs            # Gestão de cursos
│   └── ArquivosController.cs          # Gestão de arquivos
├── Middleware/
│   └── ExceptionMiddleware.cs         # Tratamento de exceções
├── Extensions/
│   └── ServiceExtensions.cs          # Configurações de serviços
├── Program.cs                         # Configuração da aplicação
└── appsettings.json                   # Configurações
```

**Características**:
- ✅ **Depende de todas** as outras camadas
- ✅ **Configura injeção** de dependência
- ✅ **Trata requisições** HTTP
- ✅ **Serializa/deserializa** dados

## 🔄 Fluxo de Dados

### Requisição (Request Flow)
```
HTTP Request → Controller → Service → Repository → Database
```

### Resposta (Response Flow)
```
Database → Repository → Service → Controller → HTTP Response
```

### Exemplo Prático: Criar Curso
```csharp
// 1. Controller recebe requisição
[HttpPost]
public async Task<ActionResult<ApiResponse<CursoResponseDto>>> CreateCurso(
    [FromBody] CursoCreateDto createDto)
{
    // 2. Chama serviço de aplicação
    var result = await _cursoService.CreateCursoAsync(createDto, usuarioId);
    return Ok(result);
}

// 3. Serviço implementa caso de uso
public async Task<ApiResponse<CursoResponseDto>> CreateCursoAsync(
    CursoCreateDto createDto, Guid usuarioId)
{
    // 4. Cria entidade de domínio
    var curso = new Curso { /* propriedades */ };
    
    // 5. Usa repositório para persistir
    await _unitOfWork.Cursos.AddAsync(curso);
    await _unitOfWork.SaveChangesAsync();
    
    // 6. Retorna DTO
    return ApiResponse<CursoResponseDto>.SuccessResult(cursoDto);
}
```

## 🎯 Princípios Aplicados

### 1. Dependency Inversion Principle (DIP)
- **Abstrações** não dependem de detalhes
- **Detalhes** dependem de abstrações
- **Interfaces** definidas nas camadas internas

### 2. Single Responsibility Principle (SRP)
- **Cada classe** tem uma única responsabilidade
- **Separação clara** entre camadas
- **Coesão alta** dentro de cada módulo

### 3. Open/Closed Principle (OCP)
- **Extensível** para novos recursos
- **Fechado** para modificações
- **Interfaces** permitem novas implementações

### 4. Interface Segregation Principle (ISP)
- **Interfaces específicas** para cada contexto
- **Clientes** não dependem de métodos não utilizados
- **Contratos mínimos** e focados

### 5. Liskov Substitution Principle (LSP)
- **Implementações** são intercambiáveis
- **Comportamento** consistente entre implementações
- **Polimorfismo** bem definido

## 🔧 Padrões Implementados

### Repository Pattern
```csharp
public interface ICursoRepository : IBaseRepository<Curso>
{
    Task<Curso?> GetByCodigoAsync(string codigo);
    Task<IEnumerable<Curso>> GetByStatusAsync(StatusCurso status);
}
```

### Unit of Work Pattern
```csharp
public interface IUnitOfWork : IDisposable
{
    ICursoRepository Cursos { get; }
    IArquivoRepository Arquivos { get; }
    Task<int> SaveChangesAsync();
}
```

### Service Layer Pattern
```csharp
public interface ICursoService
{
    Task<ApiResponse<CursoResponseDto>> CreateCursoAsync(
        CursoCreateDto createDto, Guid usuarioId);
}
```

### DTO Pattern
```csharp
public class CursoCreateDto
{
    [Required]
    public string NomeCurso { get; set; }
    
    [Required]
    public StatusCurso Status { get; set; }
}
```

## 🧪 Testabilidade

### Vantagens para Testes
- **Isolamento** de dependências
- **Mocking** fácil de interfaces
- **Testes unitários** rápidos
- **Testes de integração** focados

### Exemplo de Teste
```csharp
[Test]
public async Task CreateCurso_ShouldReturnSuccess_WhenValidData()
{
    // Arrange
    var mockRepository = new Mock<ICursoRepository>();
    var service = new CursoService(mockRepository.Object);
    
    // Act
    var result = await service.CreateCursoAsync(createDto, usuarioId);
    
    // Assert
    Assert.IsTrue(result.Success);
}
```

## 📈 Benefícios Alcançados

### 1. Manutenibilidade
- ✅ **Código organizado** em camadas claras
- ✅ **Responsabilidades** bem definidas
- ✅ **Fácil localização** de funcionalidades

### 2. Testabilidade
- ✅ **Testes unitários** isolados
- ✅ **Mocking** simplificado
- ✅ **Cobertura** de código alta

### 3. Flexibilidade
- ✅ **Troca de implementações** sem impacto
- ✅ **Novos recursos** facilmente adicionados
- ✅ **Tecnologias** intercambiáveis

### 4. Escalabilidade
- ✅ **Arquitetura** preparada para crescimento
- ✅ **Performance** otimizável por camada
- ✅ **Deploy** independente de componentes

## 🚀 Próximos Passos

### Implementações Pendentes
- [ ] **Serviços de aplicação** completos
- [ ] **Validadores** FluentValidation
- [ ] **Mapeamentos** AutoMapper
- [ ] **Testes unitários** abrangentes

### Melhorias Futuras
- [ ] **CQRS** para separar comandos e consultas
- [ ] **Event Sourcing** para auditoria avançada
- [ ] **Domain Events** para comunicação entre agregados
- [ ] **Mediator Pattern** para desacoplamento

---

Esta implementação da Clean Architecture garante que o sistema seja **manutenível**, **testável** e **escalável**, seguindo as melhores práticas da engenharia de software moderna.

