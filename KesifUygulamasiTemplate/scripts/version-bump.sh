#!/bin/bash

# Version Bump Script for .NET MAUI Projects
# Updates ApplicationDisplayVersion and ApplicationVersion in .csproj files
# Creates git tag and pushes with [skip ci] protection

set -e

CSPROJ_FILE="KesifUygulamasiTemplate.csproj"

# Check if csproj file exists
if [ ! -f "$CSPROJ_FILE" ]; then
    echo "? Error: $CSPROJ_FILE not found!"
    exit 1
fi

echo "?? Reading current version from $CSPROJ_FILE..."

# Extract current ApplicationDisplayVersion
CURRENT_VERSION=$(grep -o '<ApplicationDisplayVersion>[^<]*' "$CSPROJ_FILE" | cut -d'>' -f2)

if [ -z "$CURRENT_VERSION" ]; then
    echo "? Error: ApplicationDisplayVersion not found in $CSPROJ_FILE"
    exit 1
fi

echo "?? Current version: $CURRENT_VERSION"

# Parse version components (assuming semantic versioning x.y.z)
IFS='.' read -r MAJOR MINOR PATCH <<< "$CURRENT_VERSION"

# Validate version format
if [ -z "$MAJOR" ] || [ -z "$MINOR" ] || [ -z "$PATCH" ]; then
    echo "? Error: Invalid version format. Expected x.y.z, got: $CURRENT_VERSION"
    exit 1
fi

# Increment patch version
NEW_PATCH=$((PATCH + 1))
NEW_VERSION="$MAJOR.$MINOR.$NEW_PATCH"

echo "?? Bumping version to: $NEW_VERSION"

# Update ApplicationDisplayVersion in csproj
sed -i.bak "s/<ApplicationDisplayVersion>$CURRENT_VERSION<\/ApplicationDisplayVersion>/<ApplicationDisplayVersion>$NEW_VERSION<\/ApplicationDisplayVersion>/" "$CSPROJ_FILE"

# Update ApplicationVersion (integer) - increment by 1
CURRENT_APP_VERSION=$(grep -o '<ApplicationVersion>[^<]*' "$CSPROJ_FILE" | cut -d'>' -f2)
NEW_APP_VERSION=$((CURRENT_APP_VERSION + 1))

sed -i.bak "s/<ApplicationVersion>$CURRENT_APP_VERSION<\/ApplicationVersion>/<ApplicationVersion>$NEW_APP_VERSION<\/ApplicationVersion>/" "$CSPROJ_FILE"

# Clean up backup file
rm -f "$CSPROJ_FILE.bak"

echo "? Version updated in $CSPROJ_FILE:"
echo "   ApplicationDisplayVersion: $CURRENT_VERSION ? $NEW_VERSION"
echo "   ApplicationVersion: $CURRENT_APP_VERSION ? $NEW_APP_VERSION"

# Configure git
git config --local user.email "action@github.com"
git config --local user.name "GitHub Action"

# Commit changes
git add "$CSPROJ_FILE"
git commit -m "ci: bump version to $NEW_VERSION [skip ci]"

# Create and push tag
TAG_NAME="v$NEW_VERSION"
git tag -a "$TAG_NAME" -m "ci: release $NEW_VERSION [skip ci]"

echo "???  Created tag: $TAG_NAME"

# Push changes and tag
git push origin HEAD --tags

echo "? Version bump completed successfully!"
echo "::set-output name=new_version::$NEW_VERSION"
echo "::set-output name=tag_name::$TAG_NAME"