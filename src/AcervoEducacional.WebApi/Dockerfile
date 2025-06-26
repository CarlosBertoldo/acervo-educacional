# Dockerfile para AcervoEducacional.WebApi - Clean Architecture

# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto na ordem correta (dependências primeiro)
COPY ["src/AcervoEducacional.Domain/AcervoEducacional.Domain.csproj", "AcervoEducacional.Domain/"]
COPY ["src/AcervoEducacional.Application/AcervoEducacional.Application.csproj", "AcervoEducacional.Application/"]
COPY ["src/AcervoEducacional.Infrastructure/AcervoEducacional.Infrastructure.csproj", "AcervoEducacional.Infrastructure/"]
COPY ["src/AcervoEducacional.WebApi/AcervoEducacional.WebApi.csproj", "AcervoEducacional.WebApi/"]

# Restaurar dependências
RUN dotnet restore "AcervoEducacional.WebApi/AcervoEducacional.WebApi.csproj"

# Copiar todo o código fonte
COPY src/AcervoEducacional.Domain/ AcervoEducacional.Domain/
COPY src/AcervoEducacional.Application/ AcervoEducacional.Application/
COPY src/AcervoEducacional.Infrastructure/ AcervoEducacional.Infrastructure/
COPY src/AcervoEducacional.WebApi/ AcervoEducacional.WebApi/

# Build da aplicação
WORKDIR "/src/AcervoEducacional.WebApi"
RUN dotnet build "AcervoEducacional.WebApi.csproj" -c Release -o /app/build

# Publicar aplicação
FROM build AS publish
RUN dotnet publish "AcervoEducacional.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalar dependências para health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copiar aplicação publicada
COPY --from=publish /app/publish .

# Criar diretórios necessários
RUN mkdir -p /app/logs /app/uploads

# Configurar usuário não-root
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Expor porta
EXPOSE 80

# Configurar variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Ponto de entrada
ENTRYPOINT ["dotnet", "AcervoEducacional.WebApi.dll"]

