# Clean .resx backup files and ensure minimal valid .resx format
# Usage: pwsh -NoProfile -ExecutionPolicy Bypass -File .\scripts\clean-resx.ps1
Set-PSDebug -Strict -ErrorAction Stop
$root = Resolve-Path "$(Join-Path (Get-Location) '.')"
$deleted = @()
$fixed = @()
$modifiedCsproj = @()

Write-Output "Root: $root"

# 1) Delete empty .resx files (0 bytes)
$emptyResx = Get-ChildItem -Path $root -Recurse -Filter *.resx -ErrorAction SilentlyContinue | Where-Object { $_.Length -eq 0 }
foreach ($f in $emptyResx) {
    Try {
        Remove-Item -LiteralPath $f.FullName -Force -ErrorAction Stop
        $deleted += $f.FullName
        Write-Output "Deleted empty resx: $($f.FullName)"
    } Catch {
        Write-Warning "Failed to delete $($f.FullName): $_"
    }
}

# 2) Ensure remaining AppResources.*.resx files are valid XML and contain <root>
$resxFiles = Get-ChildItem -Path $root -Recurse -Filter "AppResources*.resx" -ErrorAction SilentlyContinue
foreach ($resx in $resxFiles) {
    $isValid = $true
    Try {
        $xml = [xml](Get-Content -LiteralPath $resx.FullName -ErrorAction Stop -Raw)
        if (-not $xml.root) { $isValid = $false }
    } Catch {
        $isValid = $false
    }
    if (-not $isValid) {
        # backup and write minimal template
        $bak = $resx.FullName + ".bak"
        Try {
            Copy-Item -LiteralPath $resx.FullName -Destination $bak -Force -ErrorAction SilentlyContinue
        } Catch { }
        $template = @"
<?xml version="1.0" encoding="utf-8"?>
<root>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
</root>
"@
        Try {
            Set-Content -LiteralPath $resx.FullName -Value $template -Encoding utf8 -Force -ErrorAction Stop
            $fixed += $resx.FullName
            Write-Output "Replaced invalid resx with minimal template: $($resx.FullName) (backup: $bak)"
        } Catch {
            Write-Warning "Failed to replace $($resx.FullName): $_"
        }
    }
}

# 3) Remove EmbeddedResource nodes in csproj that reference missing files
$csprojs = Get-ChildItem -Path $root -Recurse -Filter *.csproj -ErrorAction SilentlyContinue
foreach ($proj in $csprojs) {
    $projChanged = $false
    Try {
        [xml]$xml = Get-Content -LiteralPath $proj.FullName -ErrorAction Stop
    } Catch {
        Write-Warning "Failed to load csproj $($proj.FullName): $_"
        continue
    }
    # Find EmbeddedResource nodes
    $namespaceManager = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
    $nodes = $xml.SelectNodes('//EmbeddedResource')
    foreach ($n in $nodes) {
        $attr = $n.GetAttribute('Include')
        if (-not $attr) { $attr = $n.GetAttribute('Remove') }
        if ($attr) {
            $candidate = Resolve-Path -Path (Join-Path (Split-Path $proj.FullName) $attr) -ErrorAction SilentlyContinue
            if (-not $candidate) {
                # remove node
                $parent = $n.ParentNode
                $parent.RemoveChild($n) | Out-Null
                $projChanged = $true
                Write-Output "Removed EmbeddedResource reference to missing file in $($proj.FullName): $attr"
            }
        }
    }
    if ($projChanged) {
        Try {
            $xml.Save($proj.FullName)
            $modifiedCsproj += $proj.FullName
        } Catch {
            Write-Warning "Failed to save modified csproj $($proj.FullName): $_"
        }
    }
}

# Summary
Write-Output "\nSummary:"
Write-Output "Deleted empty resx files: $($deleted.Count)"
$deleted | ForEach-Object { Write-Output "  $_" }
Write-Output "Fixed invalid AppResources resx files: $($fixed.Count)"
$fixed | ForEach-Object { Write-Output "  $_" }
Write-Output "Modified csproj files: $($modifiedCsproj.Count)"
$modifiedCsproj | ForEach-Object { Write-Output "  $_" }

if (($deleted.Count -eq 0) -and ($fixed.Count -eq 0) -and ($modifiedCsproj.Count -eq 0)) {
    Write-Output "Nothing to do."
} else {
    Write-Output "Cleanup completed."
}
