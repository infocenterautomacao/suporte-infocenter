@echo off
setlocal enabledelayedexpansion

:: =============================================================
:: CodigoUP - Vinicius Borges - 2026
:: PRODUCAO - Forca de Vendas Servico Windows
:: =============================================================

:: -------------------------------
:: AUTO-ELEVACAO PARA ADMINISTRADOR
:: Elevacao garantida pelo instalador Inno Setup.
:: -------------------------------
net session >nul 2>&1
if %errorlevel% neq 0 (
    powershell -Command "Start-Process -FilePath '%~f0' -ArgumentList '%~1' -Verb RunAs -Wait"
    exit /b 0
)

cls
echo.
echo  =====================================================
echo    CodigoUP - Instalador de Servico Windows
echo    Forca de Vendas - Vinicius Borges 2026
echo  =====================================================
echo.

:: -------------------------------
:: VALIDAR PARAMETRO
:: Recebe o caminho da pasta onde estao os executaveis
:: -------------------------------
set "APP_PATH=%~1"

if "%APP_PATH%"=="" (
    echo  [ERRO] Caminho da pasta nao informado.
    echo.
    echo  Uso: install_service_nssm.bat "C:\UpSystem"
    echo.
    pause
    exit /b 1
)

:: Remove barra final se houver
if "%APP_PATH:~-1%"=="\" set "APP_PATH=%APP_PATH:~0,-1%"

:: Valida se a pasta informada existe
if not exist "%APP_PATH%" (
    echo  [ERRO] Pasta nao encontrada: %APP_PATH%
    echo.
    pause
    exit /b 1
)

:: -------------------------------
:: VARIAVEIS PRINCIPAIS
:: Monta caminhos com base na pasta recebida por parametro
:: -------------------------------
set "SERVICO=ForcaDeVendas"
set "APP=%APP_PATH%\ServidorForcaVendas.exe"
set "NSSM=%APP_PATH%\nssm.exe"
set "ERROS=0"

echo  [INFO] Pasta alvo    : %APP_PATH%
echo  [INFO] Executavel    : %APP%
echo  [INFO] Nome servico  : %SERVICO%
echo.

:: -------------------------------
:: VALIDAR NSSM
:: O nssm.exe deve estar na pasta informada
:: Download: https://nssm.cc/download
:: -------------------------------
echo  [1/4] Verificando NSSM...
if not exist "%NSSM%" (
    echo        nssm.exe nao encontrado em %APP_PATH% ... FALHA
    echo.
    echo  [DICA] Baixe o nssm.exe em: https://nssm.cc/download
    echo         e coloque-o na pasta: %APP_PATH%
    pause
    exit /b 1
)
echo        nssm.exe encontrado ... OK
echo.

:: -------------------------------
:: VALIDAR EXECUTAVEL DO SERVICO
:: O ServidorForcaVendas.exe deve estar na pasta informada
:: -------------------------------
echo  [2/4] Verificando ServidorForcaVendas.exe...
if not exist "%APP%" (
    echo        ServidorForcaVendas.exe nao encontrado em %APP_PATH% ... FALHA
    pause
    exit /b 1
)
echo        ServidorForcaVendas.exe encontrado ... OK
echo.

:: -------------------------------
:: VALIDAR NOME DO SERVICO
:: Verifica se ja existe um servico com o mesmo nome.
:: Se existir, solicita um novo nome ate encontrar um disponivel.
:: -------------------------------
echo  [3/4] Verificando disponibilidade do nome do servico...
:VALIDAR_NOME
sc query "%SERVICO%" >nul 2>&1
if %errorlevel%==0 (
    echo.
    echo  [ATENCAO] Servico "%SERVICO%" ja existe.
    set /p SERVICO=  Digite um novo nome para o servico: 
    if "!SERVICO!"=="" (
        echo  [ERRO] Nome invalido. Tente novamente.
        goto VALIDAR_NOME
    )
    goto VALIDAR_NOME
)
echo        Nome disponivel: %SERVICO% ... OK
echo.

:: -------------------------------
:: INSTALACAO E CONFIGURACAO DO SERVICO
:: Instala via NSSM e define:
::   - AppDirectory : pasta de trabalho do executavel
::   - Start        : inicializacao automatica com o Windows
:: -------------------------------
echo  [4/4] Instalando e configurando servico...

"%NSSM%" install "%SERVICO%" "%APP%" >nul 2>&1
if %errorlevel% neq 0 (
    echo        Falha ao instalar servico ... FALHA
    pause
    exit /b 1
)

"%NSSM%" set "%SERVICO%" AppDirectory "%APP_PATH%" >nul 2>&1
"%NSSM%" set "%SERVICO%" Start SERVICE_AUTO_START >nul 2>&1

echo        Servico instalado e configurado ... OK
echo.

:: -------------------------------
:: INICIALIZACAO DO SERVICO
:: Inicia imediatamente apos a instalacao
:: -------------------------------
echo  [INFO] Iniciando servico...
net start "%SERVICO%" >nul 2>&1
if %errorlevel% neq 0 (
    echo.
    echo  =====================================================
    echo    STATUS: ERRO - Falha ao iniciar o servico.
    echo    Verifique permissoes ou configuracoes do sistema.
    echo  =====================================================
    echo.
    pause
    exit /b 1
)

:: -------------------------------
:: FINALIZACAO
:: Fecha automaticamente em 3 segundos se tudo ocorreu bem
:: -------------------------------
echo  =====================================================
echo    STATUS: SUCESSO - Servico instalado e em execucao!
echo    Nome do servico : %SERVICO%
echo    Pasta           : %APP_PATH%
echo  =====================================================
echo.
echo    Fechando automaticamente em 3 segundos...
echo.
timeout /t 3 /nobreak >nul
exit /b 0