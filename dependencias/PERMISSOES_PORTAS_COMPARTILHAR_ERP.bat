title by: Yago Rocha - Infocenter Automacao
@echo off
chcp 65001
setlocal

:: Defina o caminho da pasta
set "Pasta=C:\UpSystem"

:: Verifique se a pasta existe
if not exist "%Pasta%" (
    echo [ERRO] A pasta %Pasta% não existe.
    pause
    exit /b
)

:: Conceda controle total para os grupos e usuários especificados
echo Concedendo controle total para "Todos"...
icacls "%Pasta%" /grant "Todos:(OI)(CI)F" /T /C >nul 2>&1

echo Concedendo controle total para "Usuários"...
icacls "%Pasta%" /grant "Usuários:(OI)(CI)F" /T /C >nul 2>&1

echo Concedendo controle total para "Rede"...
icacls "%Pasta%" /grant "Rede:(OI)(CI)F" /T /C >nul 2>&1

echo Concedendo controle total para "Administrador"...
icacls "%Pasta%" /grant "Administrador:(OI)(CI)F" /T /C >nul 2>&1

echo Concedendo controle total para "Administradores"...
icacls "%Pasta%" /grant "Administradores:(OI)(CI)F" /T /C >nul 2>&1

echo Concedendo controle total para "SISTEMA"...
icacls "%Pasta%" /grant "SISTEMA:(OI)(CI)F" /T /C >nul 2>&1

echo Concedendo controle total para "Usuários autenticados"...
icacls "%Pasta%" /grant "Usuários autenticados:(OI)(CI)F" /T /C >nul 2>&1

:: Remova a herança de permissões
echo Removendo herança de permissões da pasta %Pasta%...
icacls "%Pasta%" /inheritance:r

:: Verifique o status da execução
if errorlevel 1 (
    echo [ERRO] Falha ao aplicar permissões.
) else (
    echo Permissões aplicadas com sucesso.
)

echo --------------------------------------------------------------------------------------

Echo Compartilhando pasta do sistema

net share UpSystem=C:\UpSystem /grant:Todos,FULL

echo --------------------------------------------------------------------------------------


:: Adiciona regras do firewall
echo Adicionando regras do firewall...

for %%P in (3050 8082 2018 7079 1080) do (
    netsh advfirewall firewall add rule name="up-TCP-%%P" action=allow protocol=TCP dir=in localport=%%P >nul 2>&1
    netsh advfirewall firewall add rule name="up-UDP-%%P" action=allow protocol=UDP dir=in localport=%%P >nul 2>&1
    netsh advfirewall firewall add rule name="up-TCP-%%P" action=allow protocol=TCP dir=out localport=%%P >nul 2>&1
    netsh advfirewall firewall add rule name="up-UDP-%%P" action=allow protocol=UDP dir=out localport=%%P >nul 2>&1
)

echo Regras do firewall adicionadas com sucesso.


echo --------------------------------------------------------------------------------------


:: Adiciona exclusões ao Windows Defender
echo Adicionando exclusões ao Windows Defender...

powershell -Command Add-MpPreference -ExclusionPath 'C:\Program Files (x86)\MasterRemote','C:\ProgramData\MasterRemote','C:\UpSystem'

echo Exclusões do Windows Defender adicionadas com sucesso.


echo --------------------------------------------------------------------------------------


:: Configurações de falha dos serviços
echo Configurando ações de falha dos serviços...

:: Defina os nomes dos serviços em uma lista
set "services=FirebirdServerDefaultInstance Spooler"

:: Loop para aplicar as mesmas configurações de falha para todos os serviços
for %%S in (%services%) do (
    echo Configurando ações de falha para %%S...
    sc failure "%%S" reset= 86400 actions= restart/60000/restart/60000/restart/60000
)

echo Configurações de falha aplicadas com sucesso para todos os serviços.


echo --------------------------------------------------------------------------------------




echo Após a execução do comando, verifique manualmente as permissões na pasta do sistema para garantir que todas as permissões foram aplicadas conforme esperado.
echo Lembre também de abrir o suporte.exe se caso tenha instalado o sistema nesse instante.



echo --------------------------------------------------------------------------------------


pause
endlocal
