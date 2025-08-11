#!/bin/bash

# Android AAB Build Script for .NET MAUI
# Uses dotnet msbuild -t:Bundle to create .aab file
# Copies output to ./artifacts/ directory
# Exits with non-zero code on failure

set -e  # Exit immediately if any command fails

echo "?? Starting Android AAB build for .NET MAUI project..."

# Define variables
PROJECT_FILE="KesifUygulamasiTemplate.csproj"
TARGET_FRAMEWORK="net8.0-android"
CONFIGURATION="Release"
ARTIFACTS_DIR="./artifacts"
AAB_SEARCH_PATTERN="*.aab"

# Validate project file exists
if [ ! -f "$PROJECT_FILE" ]; then
    echo "? Error: Project file '$PROJECT_FILE' not found!"
    exit 1
fi

# Create artifacts directory
echo "?? Creating artifacts directory..."
mkdir -p "$ARTIFACTS_DIR"

# Clean any existing AAB files in artifacts
echo "?? Cleaning existing artifacts..."
rm -f "$ARTIFACTS_DIR"/*.aab

echo "?? Building Android App Bundle (.aab)..."
echo "   Project: $PROJECT_FILE"
echo "   Framework: $TARGET_FRAMEWORK"
echo "   Configuration: $CONFIGURATION"
echo "   Package Format: AAB"

# Build command with msbuild Bundle target
dotnet msbuild -t:Bundle \
    -f:$TARGET_FRAMEWORK \
    -p:Configuration=$CONFIGURATION \
    -p:AndroidPackageFormat=aab \
    "$PROJECT_FILE"

# Check if build was successful
if [ $? -ne 0 ]; then
    echo "? Error: msbuild Bundle command failed!"
    exit 1
fi

echo "? Build completed successfully!"

# Find generated .aab file(s)
echo "?? Searching for generated .aab files..."
AAB_FILES=$(find . -name "$AAB_SEARCH_PATTERN" -type f)

if [ -z "$AAB_FILES" ]; then
    echo "? Error: No .aab files found after build!"
    echo "Expected to find files matching pattern: $AAB_SEARCH_PATTERN"
    exit 1
fi

# Copy .aab file(s) to artifacts directory
echo "?? Copying .aab files to artifacts directory..."
for aab_file in $AAB_FILES; do
    filename=$(basename "$aab_file")
    destination="$ARTIFACTS_DIR/$filename"
    
    echo "   Copying: $aab_file ? $destination"
    cp "$aab_file" "$destination"
    
    # Verify copy was successful
    if [ ! -f "$destination" ]; then
        echo "? Error: Failed to copy $aab_file to $destination"
        exit 1
    fi
    
    # Get file size for verification
    file_size=$(stat -c%s "$destination" 2>/dev/null || stat -f%z "$destination" 2>/dev/null || echo "unknown")
    echo "   ? Copied successfully (Size: $file_size bytes)"
done

# List final artifacts
echo "?? Final artifacts in $ARTIFACTS_DIR:"
ls -la "$ARTIFACTS_DIR"/*.aab

# Set output for CI/CD usage
AAB_COUNT=$(ls -1 "$ARTIFACTS_DIR"/*.aab | wc -l)
FIRST_AAB=$(ls -1 "$ARTIFACTS_DIR"/*.aab | head -n 1)

echo "? Android AAB build completed successfully!"
echo "   ?? Generated $AAB_COUNT .aab file(s)"
echo "   ?? Artifacts directory: $ARTIFACTS_DIR"
echo "   ?? Primary AAB: $FIRST_AAB"

# Output for GitHub Actions
if [ -n "$GITHUB_OUTPUT" ]; then
    echo "aab_count=$AAB_COUNT" >> "$GITHUB_OUTPUT"
    echo "artifacts_dir=$ARTIFACTS_DIR" >> "$GITHUB_OUTPUT"
    echo "primary_aab=$FIRST_AAB" >> "$GITHUB_OUTPUT"
fi

echo "?? Build script completed successfully!"