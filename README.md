# Clean Architecture API — Template .NET

Um template de API ASP.NET Core com foco em Clean Architecture, para iniciar novos projetos de forma consistente e produtiva.

## Pré-requisitos

- .NET SDK 8.0 ou superior instalado (verifique com `dotnet --version`)
- Acesso ao terminal/PowerShell

## Instalação do template

Você pode instalar o template diretamente a partir da pasta do projeto (local) ou via pacote NuGet (quando publicado).

### 1) Instalar a partir do código-fonte (local)

No diretório onde está o template (a pasta que contém `.template.config/template.json`):
`dotnet new install .`

Para atualizar após mudanças no template, desinstale e instale novamente:
`dotnet new uninstall . dotnet new install .`

### 2) Instalar via pacote NuGet (opcional, quando publicado)

Se o template estiver empacotado e publicado como `.nupkg`:

- Instalando a partir do arquivo `.nupkg` local: `dotnet new install caminho/para/SeuTemplate.nupkg`
- Instalando a partir de um feed NuGet (por exemplo, nuget.org): `dotnet new install Fillipe.CleanArchitecture.Api`
- Para desinstalar: `dotnet new uninstall Fillipe.CleanArchitecture.Api`

## Verificar instalação

Liste os templates instalados e procure por “Clean Architecture API” ou pelo short name: `dotnet new list`

Você deverá ver algo como:
- Name: Clean Architecture API
- Short Name: cleanapi

## Criar um novo projeto a partir do template

Crie uma nova solução/projeto informando o nome do seu projeto: `dotnet new cleanapi -n MinhaApi`

Por padrão, o template direciona para .NET 8.0. Se desejar especificar o framework: `dotnet new cleanapi -n MinhaApi --Framework net8.0`

Entre na pasta gerada e restaure/execute: `cd MinhaApi dotnet restore dotnet build dotnet run`


A API deverá iniciar e exibir a URL base no console (normalmente https://localhost:<porta> e/ou http://localhost:<porta>).

## Parâmetros suportados

- --Framework: Define o Target Framework do projeto (padrão: net8.0).
  - Exemplo: `--Framework net8.0`

Exemplo completo: `dotnet new cleanapi -n MinhaApi --Framework net8.0`


## Atualização do template

Se você instalou via diretório local e alterou o template, rode: `dotnet new uninstall . dotnet new install .`

Se instalou via pacote: `dotnet new uninstall Fillipe.CleanArchitecture.Api dotnet new install Fillipe.CleanArchitecture.Api`


## Dicas

- Use o short name do template para agilizar: `dotnet new cleanapi`.
- Após gerar o projeto, ajuste namespaces, organização de camadas e pipelines conforme as necessidades do seu time/organização.

## Suporte

Em caso de problemas ou sugestões, abra uma issue no repositório deste template ou entre em contato com o autor.
