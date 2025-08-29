#!/bin/bash

# iOS IPA Build Script
# Bu script, IPA dosyasını oluşturur ve imza doğrulama işlemlerini gerçekleştirir.

# IPA dosyasının yolu
IPA_PATH="out/iOS/app-release.ipa"

# IPA dosyasının varlığını kontrol et
if [ -f "$IPA_PATH" ]; then
    echo "✅ IPA dosyası bulundu: $IPA_PATH"

    # IPA dosyasının imzasını doğrula
    echo "🔍 IPA imzası doğrulanıyor..."
    codesign -v "$IPA_PATH"

    if [ $? -eq 0 ]; then
        echo "✅ IPA imzası geçerli."
    else
        echo "❌ IPA imzası geçersiz!"
        exit 1
    fi

    # Geçerli kimlikleri listele
    echo "🔍 Geçerli kimlikler kontrol ediliyor..."
    security find-identity -v -p codesigning
else
    echo "❌ IPA dosyası bulunamadı: $IPA_PATH"
    exit 1
fi
