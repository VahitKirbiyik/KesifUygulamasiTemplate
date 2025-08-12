#!/bin/bash
# debug-tests.sh

echo "?? Test Environment Debug Script"
echo "================================"

# Sistem bilgileri
echo "?? System Info:"
uname -a
echo ""

# Android SDK kontrolü
echo "?? Android SDK:"
if command -v adb &> /dev/null; then
    echo "? ADB found"
    adb version
    echo "Connected devices:"
    adb devices
else
    echo "? ADB not found"
fi
echo ""

# .NET bilgileri
echo "?? .NET Info:"
dotnet --info
echo ""
echo "Installed workloads:"
dotnet workload list
echo ""

# Test projeleri
echo "?? Test Projects:"
find . -name "*.Tests.csproj" -type f
echo ""

# Emulator durumu (Android)
echo "?? Emulator Status:"
if command -v emulator &> /dev/null; then
    echo "? Emulator command found"
    emulator -list-avds 2>/dev/null || echo "No AVDs found"
else
    echo "? Emulator command not found"
fi
echo ""

# Memory ve CPU
echo "?? System Resources:"
free -h 2>/dev/null || echo "free command not available"
nproc 2>/dev/null || echo "nproc command not available"
echo ""

echo "Debug completed! ?"