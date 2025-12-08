#!/bin/bash

# DesignPatternSamples - Automated Quality Assurance Script
# .NET 8 version with modern tooling

set -e  # Exit on any error

# Configuration
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SRC_DIR="$PROJECT_ROOT/src"
OUTPUT_DIR="$PROJECT_ROOT/coverage"
DOTNET_VERSION="8.0"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🚀 DesignPatternSamples - Quality Assurance Pipeline${NC}"
echo -e "${BLUE}=====================================================${NC}"

# Function to print colored messages
print_step() {
    echo -e "${BLUE}📋 $1${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Check if .NET 8 is installed
print_step "Checking .NET version..."
if ! command -v dotnet &> /dev/null; then
    print_error ".NET CLI not found. Please install .NET 8 SDK."
    exit 1
fi

INSTALLED_VERSION=$(dotnet --version)
print_success ".NET CLI version: $INSTALLED_VERSION"

# Navigate to source directory
cd "$SRC_DIR"

# Clean previous builds
print_step "Cleaning previous builds..."
dotnet clean --configuration Release --verbosity quiet
print_success "Clean completed"

# Restore packages
print_step "Restoring NuGet packages..."
dotnet restore --verbosity quiet
print_success "Packages restored"

# Format code
print_step "Checking code formatting..."
if dotnet format --verify-no-changes --verbosity quiet; then
    print_success "Code formatting is correct"
else
    print_warning "Code formatting issues found. Running auto-format..."
    dotnet format --verbosity quiet
    print_success "Code formatted successfully"
fi

# Build solution
print_step "Building solution in Release mode..."
dotnet build --configuration Release --no-restore --verbosity quiet
print_success "Build completed successfully"

# Run static analysis
print_step "Running static code analysis..."
dotnet build --configuration Release --verbosity minimal --property WarningsAsErrors="" > /tmp/build.log 2>&1 || true

WARNING_COUNT=$(grep -c "warning" /tmp/build.log || echo "0")
ERROR_COUNT=$(grep -c "error" /tmp/build.log || echo "0")

if [ "$ERROR_COUNT" -gt "0" ]; then
    print_error "Found $ERROR_COUNT error(s) in static analysis"
    cat /tmp/build.log
    exit 1
else
    print_success "Static analysis completed - $WARNING_COUNT warning(s) found"
fi

# Create output directory
mkdir -p "$OUTPUT_DIR"

# Run tests with coverage
print_step "Running tests with coverage analysis..."
dotnet test \
    --configuration Release \
    --no-build \
    --verbosity minimal \
    --collect:"XPlat Code Coverage" \
    --results-directory "$OUTPUT_DIR" \
    --logger "trx;LogFileName=test-results.trx"

print_success "Tests completed successfully"

# Generate coverage report
print_step "Generating coverage report..."
if command -v reportgenerator &> /dev/null; then
    # Find the latest coverage file
    COVERAGE_FILE=$(find "$OUTPUT_DIR" -name "coverage.cobertura.xml" -o -name "*.coverage" | head -n 1)
    
    if [ -n "$COVERAGE_FILE" ]; then
        reportgenerator \
            -reports:"$COVERAGE_FILE" \
            -targetdir:"$OUTPUT_DIR/report" \
            -reporttypes:"Html;JsonSummary;Badges" \
            -verbosity:Warning
        
        print_success "Coverage report generated at: $OUTPUT_DIR/report"
        
        # Extract coverage percentage
        if [ -f "$OUTPUT_DIR/report/Summary.json" ]; then
            COVERAGE_PERCENT=$(grep -o '"linecoverage":[^,]*' "$OUTPUT_DIR/report/Summary.json" | cut -d':' -f2)
            print_success "Line Coverage: ${COVERAGE_PERCENT}%"
        fi
    else
        print_warning "No coverage file found. Coverage report not generated."
    fi
else
    print_warning "ReportGenerator not installed. Install with: dotnet tool install -g dotnet-reportgenerator-globaltool"
fi

# Security scan
print_step "Running security analysis..."
dotnet list package --vulnerable --include-transitive > /tmp/vuln.log 2>&1 || true
dotnet list package --deprecated --include-transitive > /tmp/deprecated.log 2>&1 || true

VULN_COUNT=$(grep -c "has the following vulnerable dependencies" /tmp/vuln.log || echo "0")
DEPRECATED_COUNT=$(grep -c "is deprecated" /tmp/deprecated.log || echo "0")

if [ "$VULN_COUNT" -gt "0" ]; then
    print_warning "Found vulnerable dependencies:"
    cat /tmp/vuln.log
else
    print_success "No vulnerable dependencies found"
fi

if [ "$DEPRECATED_COUNT" -gt "0" ]; then
    print_warning "Found deprecated dependencies:"
    cat /tmp/deprecated.log
else
    print_success "No deprecated dependencies found"
fi

# Summary
echo -e "\n${BLUE}📊 Quality Assurance Summary${NC}"
echo -e "${BLUE}=============================${NC}"
print_success "✅ Build: Successful"
print_success "✅ Tests: All Passed"

if [ "$WARNING_COUNT" -gt "0" ]; then
    print_warning "⚠️  Static Analysis: $WARNING_COUNT warnings"
else
    print_success "✅ Static Analysis: No warnings"
fi

if [ "$VULN_COUNT" -gt "0" ] || [ "$DEPRECATED_COUNT" -gt "0" ]; then
    print_warning "⚠️  Security: Issues found (see details above)"
else
    print_success "✅ Security: No issues found"
fi

echo -e "\n${GREEN}🎉 Quality assurance pipeline completed successfully!${NC}"

# Open coverage report if available
if [ -f "$OUTPUT_DIR/report/index.html" ]; then
    echo -e "\n${BLUE}🌐 Coverage report available at: file://$OUTPUT_DIR/report/index.html${NC}"
    
    # Try to open the report in the default browser (Linux)
    if command -v xdg-open &> /dev/null; then
        echo -e "${YELLOW}💡 Opening coverage report in default browser...${NC}"
        xdg-open "$OUTPUT_DIR/report/index.html" 2>/dev/null &
    fi
fi

exit 0