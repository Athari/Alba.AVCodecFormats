param (
    [Parameter(Mandatory=$true)]
    [string] $DirPath
)

$PSStyle.OutputRendering = [Management.Automation.OutputRendering]::PlainText

$Dirs = Get-ChildItem -LiteralPath $DirPath -Recurse -Force -Directory -ErrorAction SilentlyContinue
if ($Dirs -ne $null) {
    [array]::Reverse($Dirs)
    foreach ($Dir in $Dirs) {
        if ((Get-Childitem -LiteralPath $Dir.FullName -Recurse -Force -File) -eq $null) {
            Remove-Item -LiteralPath $Dir.FullName -Recurse -Force
        }
    }
}