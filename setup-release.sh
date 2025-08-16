#!/bin/bash

# Setup Release Script for Android Keystore
# Converts Base64 encoded keystore to file and sets environment variables
# Usage: ./setup-release.sh

set -e  # Exit immediately if any command fails

echo "?? Setting up Android release signing..."

# Check if ANDROID_SIGNING_KEY environment variable is set
if [ -z "$ANDROID_SIGNING_KEY" ]; then
    echo "? Error: ANDROID_SIGNING_KEY environment variable is not set!"
    echo "   Please ensure the Base64 encoded keystore is available in secrets."
    exit 1
fi

# Define keystore file name
KEYSTORE_FILE="android-release.keystore"

echo "?? Converting Base64 keystore to file..."
echo "   Target file: $KEYSTORE_FILE"

# Decode Base64 keystore and save to file
if echo "$ANDROID_SIGNING_KEY" | base64 -d > "$KEYSTORE_FILE"; then
    echo "? Keystore file created successfully"
else
    echo "? Error: Failed to decode Base64 keystore!"
    exit 1
fi

# Verify keystore file was created and has content
if [ ! -f "$KEYSTORE_FILE" ]; then
    echo "? Error: Keystore file was not created!"
    exit 1
fi

# Check if file has content (should be > 0 bytes)
KEYSTORE_SIZE=$(stat -c%s "$KEYSTORE_FILE" 2>/dev/null || stat -f%z "$KEYSTORE_FILE" 2>/dev/null || echo "0")
if [ "$KEYSTORE_SIZE" -eq 0 ]; then
    echo "? Error: Keystore file is empty!"
    rm -f "$KEYSTORE_FILE"
    exit 1
fi

echo "?? Keystore file size: $KEYSTORE_SIZE bytes"

# Set environment variable for the keystore path
echo "?? Setting keystore environment variables..."

# For GitHub Actions, write to GITHUB_ENV
if [ -n "$GITHUB_ENV" ]; then
    echo "ANDROID_KEYSTORE_PATH=$PWD/$KEYSTORE_FILE" >> "$GITHUB_ENV"
    echo "ANDROID_KEYSTORE_FILE=$KEYSTORE_FILE" >> "$GITHUB_ENV"
    echo "? Environment variables written to GITHUB_ENV"
else
    # For local development, export variables
    export ANDROID_KEYSTORE_PATH="$PWD/$KEYSTORE_FILE"
    export ANDROID_KEYSTORE_FILE="$KEYSTORE_FILE"
    echo "? Environment variables exported for current session"
    echo "   ANDROID_KEYSTORE_PATH=$ANDROID_KEYSTORE_PATH"
    echo "   ANDROID_KEYSTORE_FILE=$ANDROID_KEYSTORE_FILE"
fi

# Set file permissions (read-only for owner only)
chmod 600 "$KEYSTORE_FILE"
echo "?? Keystore file permissions set to 600 (owner read-write only)"

# Optional: Verify keystore with keytool if available
if command -v keytool >/dev/null 2>&1; then
    echo "?? Verifying keystore integrity..."
    if keytool -list -keystore "$KEYSTORE_FILE" -storepass "${ANDROID_SIGNING_PASSWORD:-android}" >/dev/null 2>&1; then
        echo "? Keystore verification successful"
    else
        echo "??  Warning: Keystore verification failed (this might be normal if password is different)"
    fi
else
    echo "??  keytool not available, skipping keystore verification"
fi

echo "?? Android release setup completed successfully!"
echo "   Keystore file: $KEYSTORE_FILE"
echo "   File size: $KEYSTORE_SIZE bytes"
echo "   Ready for release build"