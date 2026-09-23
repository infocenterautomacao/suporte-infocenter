# Suporte Infocenter

## Changelog
### Versão 1.3.4.0
- Modificação estrutural: Injetado App Manifest (pp.manifest) nativo de elevação de privilégios (equireAdministrator). O SuporteInfocenter.exe agora sempre exigirá e iniciará com permissões de Administrador pelo UAC do Windows automaticamente, sendo desnecessário usar a opção 'Executar como Administrador'.
- A aba 'Ações' teve seu layout refatorado para exibir o gerenciador em uma nova Janela Flutuante (Popup).
- Implementado botão separado para 'Reiniciar Serviço' (com lógica inteligente de parada e inicialização garantida).

### Versão 1.3.3.0
- O AssemblyVersion foi alinhado com a versão do app.
- Nova aba: Ações.
- Adicionado Gerenciador de Processos nativo via WMI (Windows Management Instrumentation) para monitorar e finalizar UpSystem.exe e ForcaDeVendas.exe em uso pelos usuários.
- Adicionado controle de Serviços (Start/Stop) para serviços do Firebird e Força de Vendas.

### Versão 1.3.2.0
- Nova rotina inserida: Duplicador de instância de Firebird e modificação de porta.
- Novo download adicionado: Firebird-UpSystem (Firebird-5.0.3.1683-0-windows-x64.exe) embutido nativamente na aba Downloads.

### Versão 1.3.1.0
- Renomeado executável principal de atualizador.exe para SuporteInfocenter.exe.
- Implementada transição suave de atualização: o instalador agora apaga o executável antigo (independente do nome original) e sempre instala a nova versão com o nome definitivo (SuporteInfocenter.exe).
- Correção definitiva de codificação (Mojibake) que impedia o Windows de ler os caracteres acentuados (UTF-8 com BOM).
- Textos da interface ajustados: O título principal foi alterado de Atualização de Sistema para SUPORTE.
- A versão mostrada no rodapé agora é espelhada nativamente da versão global no código fonte.

### Versão 1.3.0.0
- Nova Aba Rotinas (Scripts embutidos).
- Instalação inteligente do Força de Vendas via NSSM incorporado.
- Reposicionamento do layout e do botão de update do próprio programa.



