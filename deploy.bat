@echo off
setlocal ENABLEDELAYEDEXPANSION

REM ------------------------------------------------------------
REM CoreBankingSystem - Deploy helper (Windows)
REM Usage:
REM   deploy.bat            -> build & start containers (default)
REM   deploy.bat up         -> build & start containers
REM   deploy.bat down       -> stop and remove containers
REM   deploy.bat reset      -> down + remove volumes, then up
REM   deploy.bat logs       -> follow logs
REM   deploy.bat ps         -> list container status
REM   deploy.bat restart    -> restart running containers
REM ------------------------------------------------------------

REM Pick compose file (prefer root docker-compose.yml)
set COMPOSE_FILE=docker-compose.yml
if not exist "%COMPOSE_FILE%" (
  if exist ".docker\compose.yml" (
    set COMPOSE_FILE=.docker\compose.yml
  ) else (
    echo [ERROR] No docker compose file found. Expected docker-compose.yml or .docker\compose.yml
    exit /b 1
  )
)

echo Using compose file: %COMPOSE_FILE%

REM Determine compose command (Docker Compose v2 preferred)
set COMPOSE_CMD=docker compose
for /f "tokens=1" %%v in ('docker compose version 2^>NUL') do (
  set FOUND_COMPOSE=1
)
if not defined FOUND_COMPOSE (
  for /f "tokens=1" %%v in ('docker-compose --version 2^>NUL') do (
    set COMPOSE_CMD=docker-compose
    set FOUND_COMPOSE=1
  )
)
if not defined FOUND_COMPOSE (
  echo [ERROR] Docker Compose not found. Install Docker Desktop and ensure docker is on PATH.
  exit /b 1
)

REM Verify Docker is running
for /f "tokens=*" %%i in ('docker info 2^>NUL ^| findstr /I "Server Version"') do set DOCKER_OK=1
if not defined DOCKER_OK (
  echo [ERROR] Docker engine is not running. Please start Docker Desktop.
  exit /b 1
)

set ACTION=%1
if "%ACTION%"=="" set ACTION=up

if /I "%ACTION%"=="up" goto :UP
if /I "%ACTION%"=="down" goto :DOWN
if /I "%ACTION%"=="reset" goto :RESET
if /I "%ACTION%"=="logs" goto :LOGS
if /I "%ACTION%"=="ps" goto :PS
if /I "%ACTION%"=="restart" goto :RESTART

echo Unknown action: %ACTION%
echo Usage: deploy.bat [up^|down^|reset^|logs^|ps^|restart]
exit /b 1

:UP
  echo Building and starting containers...
  %COMPOSE_CMD% -f "%COMPOSE_FILE%" up -d --build
  if errorlevel 1 (
    echo [ERROR] Failed to start containers.
    exit /b 1
  )
  echo.
  echo Services are starting. Endpoints:
  echo   Web: http://localhost:8081
  echo   API: http://localhost:8080
  echo   SQL: localhost,1433  user: sa  password: Your_password123
  echo.
  %COMPOSE_CMD% -f "%COMPOSE_FILE%" ps
  echo.
  echo To follow logs: ^> deploy.bat logs
  exit /b 0

:DOWN
  echo Stopping and removing containers...
  %COMPOSE_CMD% -f "%COMPOSE_FILE%" down
  exit /b %errorlevel%

:RESET
  echo WARNING: This will remove volumes and database data.
  set /p CONFIRM=Type YES to continue: 
  if /I not "%CONFIRM%"=="YES" (
    echo Aborted.
    exit /b 1
  )
  %COMPOSE_CMD% -f "%COMPOSE_FILE%" down -v
  if errorlevel 1 exit /b 1
  %COMPOSE_CMD% -f "%COMPOSE_FILE%" up -d --build
  exit /b %errorlevel%

:LOGS
  %COMPOSE_CMD% -f "%COMPOSE_FILE%" logs -f
  exit /b %errorlevel%

:PS
  %COMPOSE_CMD% -f "%COMPOSE_FILE%" ps
  exit /b %errorlevel%

:RESTART
  %COMPOSE_CMD% -f "%COMPOSE_FILE%" restart
  exit /b %errorlevel%
