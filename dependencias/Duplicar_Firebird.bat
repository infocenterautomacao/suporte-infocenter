@echo off
setlocal EnableDelayedExpansion

:: Define cor do terminal para facilitar a leitura (Verde)
color 0A

echo ====================================================================
echo =       DUPLICAR INSTANCIA DO FIREBIRD COM PORTA ADICIONAL         =
echo ====================================================================
echo.

:: Verifica privilegios de Administrador (necessario para registrar o servico e copiar para Arquivos de Programas)
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo [ERRO] O script precisa ser executado como Administrador.
    echo Por favor, clique com o botao direito no arquivo .bat e selecione "Executar como Administrador".
    echo.
    pause
    exit /b
)

echo Preencha as informacoes abaixo:
echo.

set /p SOURCE_DIR="1. Diretorio de origem do Firebird (ex: C:\Program Files\Firebird\Firebird_5_0): "
:: Remove aspas duplas se o usuario as tiver digitado
set SOURCE_DIR=%SOURCE_DIR:"=%
if not exist "%SOURCE_DIR%" (
    echo.
    echo [ERRO] O diretorio de origem nao existe.
    pause
    exit /b
)

set /p TARGET_DIR="2. Diretorio de destino (ex: C:\Program Files\Firebird\Firebird_5_0_123456789000123): "
set TARGET_DIR=%TARGET_DIR:"=%

set /p NEW_PORT="3. Numero da nova porta (ex: 3051): "

set /p INSTANCE_NAME="4. Nome da nova instancia (ex: Firebird_5_0_123456789000123_3051): "
set INSTANCE_NAME=%INSTANCE_NAME:"=%

echo.
echo ====================================================================
echo Resumo da Operacao:
echo Origem:      %SOURCE_DIR%
echo Destino:     %TARGET_DIR%
echo Porta:       %NEW_PORT%
echo Instancia:   %INSTANCE_NAME%
echo ====================================================================
echo.
echo Pressione qualquer tecla para iniciar a duplicacao ou feche a janela para cancelar.
pause > nul

echo.
echo [1/4] Copiando arquivos de "%SOURCE_DIR%" para "%TARGET_DIR%"...
if not exist "%TARGET_DIR%" mkdir "%TARGET_DIR%"
xcopy "%SOURCE_DIR%\*" "%TARGET_DIR%\" /E /I /H /Y /Q > nul
if %errorLevel% neq 0 (
    echo [ERRO] Falha ao copiar os arquivos.
    echo Certifique-se de que nenhum arquivo do Firebird de origem esta sendo bloqueado.
    pause
    exit /b
)
echo - Copia concluida.

echo.
echo [2/4] Alterando configuracoes no arquivo firebird.conf...
set "CONF_FILE=%TARGET_DIR%\firebird.conf"
if not exist "%CONF_FILE%" (
    echo [ERRO] Arquivo firebird.conf nao encontrado no diretorio de destino.
    pause
    exit /b
)

:: Usa o PowerShell para atualizar a porta
powershell -Command "(Get-Content -Path '%CONF_FILE%') -replace '(?im)^\s*#?\s*RemoteServicePort\s*=.*$', 'RemoteServicePort = %NEW_PORT%' | Set-Content -Path '%CONF_FILE%'"
echo - Porta alterada para %NEW_PORT%.

:: Usa o PowerShell para atualizar DataTypeCompatibility
powershell -Command "(Get-Content -Path '%CONF_FILE%') -replace '(?im)^\s*#?\s*DataTypeCompatibility\s*=.*$', 'DataTypeCompatibility = 3.0' | Set-Content -Path '%CONF_FILE%'"
echo - DataTypeCompatibility alterado para 3.0.

echo.
echo [3/4] Registrando o novo servico (%INSTANCE_NAME%)...
cd /d "%TARGET_DIR%"
instsvc.exe install -name "%INSTANCE_NAME%"
if %errorLevel% neq 0 (
    echo [ERRO] Falha ao instalar o servico. Pode ser que o nome da instancia ja exista.
    pause
    exit /b
)
echo - Servico instalado.

echo.
echo [4/4] Iniciando o servico (%INSTANCE_NAME%)...
instsvc.exe start -name "%INSTANCE_NAME%"
if %errorLevel% neq 0 (
    echo [ERRO] Falha ao iniciar o servico. Verifique o visualizador de eventos do Windows.
    pause
    exit /b
)
echo - Servico iniciado.

echo.
echo ====================================================================
echo =                 OPERACAO CONCLUIDA COM SUCESSO!                  =
echo ====================================================================
echo O Firebird esta rodando na porta %NEW_PORT% com a instancia %INSTANCE_NAME%.
echo.
pause
