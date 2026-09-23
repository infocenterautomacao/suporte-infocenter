$sourceFile = "AtualizadorGUI.cs"
$outputFile = "SuporteInfocenter.exe"
$iconFile = "icone.png"

Write-Host "Compilando $sourceFile com Recursos para $outputFile..."
& $env:windir\Microsoft.NET\Framework\v4.0.30319\csc.exe /target:winexe /out:$outputFile /win32icon:$iconFile /resource:logo.png /resource:icone.png /reference:System.Windows.Forms.dll,System.Drawing.dll,System.IO.Compression.FileSystem.dll,System.ServiceProcess.dll,Microsoft.VisualBasic.dll $sourceFile

if ($LASTEXITCODE -eq 0) {
    Write-Host "Compilado com sucesso: $outputFile" -ForegroundColor Green
    Copy-Item $outputFile "D:\projetos antigravity\exe atualizador up\SuporteInfocenter.exe" -Force
} else {
    Write-Host "Erro na compilação." -ForegroundColor Red
}
