@echo off
title ANDROID ASSETS FIX & BUILD
echo ================================
echo MAUI Android Build Fix & Compile
echo ================================
echo.

:: 1. Proje kök dizinine git
cd /d "%~dp0"

:: 2. Eski obj ve bin klasörlerini sil
echo [1/5] Eski obj ve bin klasörleri siliniyor...
rd /s /q obj
rd /s /q bin

:: 3. Assets klasörünü oluştur ve dummy dosya ekle
echo [2/5] Assets klasörü hazırlanıyor...
mkdir Platforms\Android\assets >nul 2>&1
echo dummy > Platforms\Android\assets\dummy.txt

:: 4. Dotnet temizleme ve geri yükleme
echo [3/5] dotnet clean...
dotnet clean

echo [4/5] dotnet restore...
dotnet restore

:: 5. Build başlat
echo [5/5] Build başlıyor...
dotnet build -f net9.0-android

echo.
echo ================================
echo İŞLEM TAMAMLANDI!
echo ================================
pause
