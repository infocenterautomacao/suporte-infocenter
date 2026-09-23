# Suporte Infocenter

## Changelog
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
