@echo off
setlocal enabledelayedexpansion

:: Arquivos de configuracao
set "CONF_INI=C:\UpSystem\CONF.INI"
set "CONFIG_ARQUIVOS_INI=C:\UpSystem\CONFIGARQUIVOS.INI"

set "SRV="
set "DIRETORIO_ORIGEM="

echo ===================================================
echo     SCRIPT DE TRAZER CERTIFICADOS E IMAGENS
echo ===================================================
echo.

:: --- NOVIDADE AQUI: Solicitando diretorio de destino ---
set "DESTINO_PADRAO=C:\UpSystem\"
set "DESTINO="

echo Digite o diretorio de destino ou pressione ENTER para usar o padrao:
echo [%DESTINO_PADRAO%]
set /p "DESTINO=> "

:: Se o usuario apenas apertou ENTER (variavel vazia), usa o padrao
if "!DESTINO!"=="" set "DESTINO=!DESTINO_PADRAO!"

:: Garante que o diretorio de destino termine sempre com barra invertida (\)
if not "!DESTINO:~-1!"=="\" set "DESTINO=!DESTINO!\"

:: Cria a pasta de destino caso o usuario tenha digitado um caminho que ainda nao existe
if not exist "!DESTINO!" (
    echo.
    echo [INFO] Criando diretorio de destino !DESTINO!...
    mkdir "!DESTINO!"
)

echo.
:: 1. Busca o IP no CONF.INI
if exist "%CONF_INI%" (
    for /f "tokens=1,* delims==" %%a in ('type "%CONF_INI%" ^| findstr /b /i "SRV="') do (
        set "SRV=%%b"
    )
) else (
    echo [ERRO] Arquivo %CONF_INI% nao encontrado.
    pause
    exit /b
)

:: Limpa possiveis espacos em branco do valor capturado
set "SRV=%SRV: =%"

:: 2. Verifica se o servidor e localhost
if /i "%SRV%"=="localhost" (
    echo [INFO] Servidor local detectado. Buscando caminho no CONFIGARQUIVOS.INI...
    if exist "%CONFIG_ARQUIVOS_INI%" (
        for /f "tokens=1,* delims==" %%a in ('type "%CONFIG_ARQUIVOS_INI%" ^| findstr /b /i "exeremoto="') do (
            set "LINHA_EXE=%%b"
            :: Remove espacos em branco da linha caso existam
            set "LINHA_EXE=!LINHA_EXE: =!"
            :: Extrai dinamicamente apenas a pasta do executavel
            for %%I in ("!LINHA_EXE!") do set "DIRETORIO_ORIGEM=%%~dpI"
        )
    ) else (
        echo [ERRO] Arquivo %CONFIG_ARQUIVOS_INI% nao encontrado.
        pause
        exit /b
    )
) else (
    :: Se nao for localhost, monta o caminho de rede padrao
    echo [INFO] Servidor de rede detectado: %SRV%
    set "DIRETORIO_ORIGEM=\\%SRV%\upsystem\"
)

:: Verifica se a origem foi definida corretamente
if "!DIRETORIO_ORIGEM!"=="" (
    echo [ERRO] Nao foi possivel determinar o diretorio de origem.
    pause
    exit /b
)

echo.
echo [ORIGEM]  !DIRETORIO_ORIGEM!
echo [DESTINO] !DESTINO!
echo.
echo Iniciando a copia dos arquivos .pfx, .png, .jpg e .jpeg...
echo.

:: 3. Copia todos os arquivos da origem para o destino
xcopy /Y /D /C "!DIRETORIO_ORIGEM!*.pfx" "!DESTINO!" 2>nul
xcopy /Y /D /C "!DIRETORIO_ORIGEM!*.png" "!DESTINO!" 2>nul
xcopy /Y /D /C "!DIRETORIO_ORIGEM!*.jpg" "!DESTINO!" 2>nul
xcopy /Y /D /C "!DIRETORIO_ORIGEM!*.jpeg" "!DESTINO!" 2>nul

echo.
echo [SUCESSO] Varredura e copia concluidas.
echo.
pause