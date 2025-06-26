# Dockerfile para AcervoEducacional.WebApi - Clean Architecture

# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["AcervoEducacional.WebApi.csproj", "AcervoEducacional.WebApi/"]
COPY ["../AcervoEducacional.Application/AcervoEducacional.Application.csproj", "AcervoEducacional.Application/"]
COPY ["../AcervoEducacional.Infrastructure/AcervoEducacional.Infrastructure.csproj", "AcervoEducacional.Infrastructure/"]
COPY ["../AcervoEducacional.Domain/AcervoEducacional.Domain.csproj", "AcervoEducacional.Domain/"]

# Restaurar dependências
RUN dotnet restore "AcervoEducacional.WebApi/AcervoEducacional.WebApi.csproj"

# Copiar código fonte
COPY . .
COPY ../AcervoEducacional.Application/ AcervoEducacional.Application/
COPY ../AcervoEducacional.Infrastructure/ AcervoEducacional.Infrastructure/
COPY ../AcervoEducacional.Domain/ AcervoEducacional.Domain/

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

