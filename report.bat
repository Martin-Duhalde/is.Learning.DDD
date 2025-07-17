@echo off
REM Eliminar carpetas dentro de .\TestResults\
for /d %%i in (.\TestResults\*) do rd /s /q "%%i"

REM Eliminar archivos dentro de coveragereport\
if exist coveragereport (
    rd /s /q coveragereport
)

REM Ejecutar tests y generar reporte
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

start coveragereport\index.html

pause
