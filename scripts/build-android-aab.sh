#!/usr/bin/env bash
set -euo pipefail
IFS=$'\n\t'

########################################
# Orta Seviye Güvenli .NET MAUI AAB Build Script
# Kullanım: ./scripts/build-android-aab.sh
########################################

# Varsayılanlar (CI ortamında override edilebilir)
SOLUTION="${SOLUTION:-YourSolution.sln}"   # <- kendi .sln dosyanla değiştir
CONFIGURATION="${CONFIGURATION:-Release}"
OUTPUT_DIR="${OUTPUT_DIR:-artifacts/android}"
FRAMEWORK="${FRAMEWORK:-net8.0-android}"

# Keystore parametreleri (opsiyonel)
KEYSTORE_PATH="${KEYSTORE_PATH:-}"
KEYSTORE_ALIAS="${KEYSTORE_ALIAS:-}"
KEYSTORE_PASSWORD="${KEYSTORE_PASSWORD:-}"

# Renkler
C_GREEN='\033[0;32m'
C_RED='\033[0;31m'
C_YELLOW='\033[1;33m'
C_BLUE='\033[0;34m'
C_NC='\033[0m'

log() { echo -e "${C_BLUE}[$(date '+%H:%M:%S')]${C_NC} $1"; }
log_success() { echo -e "${C_GREEN}✔ $1${C_NC}"; }
log_warn() { echo -e "${C_YELLOW}⚠ $1${C_NC}"; }
log_error() { echo -e "${C_RED}✖ $1${C_NC}"; }

log "📦 Android AAB build başlatılıyor..."
log "🔧 Solution: $SOLUTION"
log "🔧 Configuration: $CONFIGURATION"
log "📁 Output: $OUTPUT_DIR"

mkdir -p "$OUTPUT_DIR"

########################################
# Versiyon Bilgisi (Git'ten al)
########################################
VERSION_NAME="1.0.0"
VERSION_CODE="1"
if command -v git >/dev/null && git rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  GIT_TAG=$(git describe --tags --exact-match 2>/dev/null || echo "")
  [[ -n "$GIT_TAG" ]] && VERSION_NAME="${GIT_TAG#v}"
  VERSION_CODE=$(git rev-list --count HEAD)
fi
log "📌 Versiyon: $VERSION_NAME (Code: $VERSION_CODE)"

########################################
# Keystore Kontrol & Doğrulama
########################################
SIGNING_PARAMS=""
if [[ -f "$KEYSTORE_PATH" && -n "$KEYSTORE_ALIAS" && -n "$KEYSTORE_PASSWORD" ]]; then
  log "🔐 Keystore bulundu, imzalama aktif."
  if command -v keytool >/dev/null; then
    if keytool -list -keystore "$KEYSTORE_PATH" -storepass "$KEYSTORE_PASSWORD" -alias "$KEYSTORE_ALIAS" >/dev/null 2>&1; then
      log_success "Keystore doğrulandı."
    else
      log_error "Keystore doğrulaması başarısız!"
      exit 1
    fi
  else
    log_warn "keytool bulunamadı, doğrulama atlandı."
  fi
  SIGNING_PARAMS="/p:AndroidKeyStore=true \
                  /p:AndroidSigningKeyStore=$KEYSTORE_PATH \
                  /p:AndroidSigningKeyAlias=$KEYSTORE_ALIAS \
                  /p:AndroidSigningKeyPass=$KEYSTORE_PASSWORD \
                  /p:AndroidSigningStorePass=$KEYSTORE_PASSWORD"
else
  log_warn "Keystore eksik, imzalama atlanıyor."
fi

########################################
# Publish işlemi (optimizasyonlarla)
########################################
dotnet publish "$SOLUTION" \
  -c "$CONFIGURATION" \
  -f "$FRAMEWORK" \
  -o "$OUTPUT_DIR" \
  /p:ApplicationDisplayVersion="$VERSION_NAME" \
  /p:ApplicationVersion="$VERSION_CODE" \
  /p:AndroidPackageFormat=aab \
  /p:AndroidLinkTool=r8 \
  /p:AndroidLinkMode=SdkOnly \
  /p:EnableLLVM=true \
  /p:PublishTrimmed=true \
  /p:AndroidStripILAfterAOT=true \
  /p:AndroidEnableMultiDex=true \
  $SIGNING_PARAMS \
  --no-restore

########################################
# Çıktı kontrolü
########################################
AAB_FILE=$(find "$OUTPUT_DIR" -name "*.aab" -print -quit)
if [[ -n "$AAB_FILE" ]]; then
  log_success "AAB bulundu: $AAB_FILE"
  if command -v unzip >/dev/null && unzip -tq "$AAB_FILE"; then
    log_success "AAB bütünlük kontrolü geçti."
  else
    log_warn "AAB bütünlük kontrolü atlandı."
  fi
else
  log_error "AAB dosyası bulunamadı!"
  exit 1
fi

########################################
# CI/CD Output (GitHub Actions)
########################################
if [[ -n "${GITHUB_OUTPUT:-}" ]]; then
  {
    echo "aab_path=$AAB_FILE"
    echo "version_name=$VERSION_NAME"
    echo "version_code=$VERSION_CODE"
    echo "signed=$( [[ -n "$SIGNING_PARAMS" ]] && echo true || echo false )"
  } >> "$GITHUB_OUTPUT"
  log_success "GitHub Actions output değişkenleri ayarlandı."
fi

log_success "✅ Build tamamlandı."
