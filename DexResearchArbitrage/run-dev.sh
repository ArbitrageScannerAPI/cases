#!/bin/bash
set -e

# Сборка приложения Blazor WASM

dotnet publish -c Release -o ./out

# Запуск dotnet-serve с path-base

dotnet-serve -d ./out/wwwroot --path-base /cases/DexResearchArbitrage
