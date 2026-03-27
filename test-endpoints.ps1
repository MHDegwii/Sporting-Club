# Test Script for Sporting Club API
# Make sure the API is running on localhost:5244 before running this script

$baseUrl = "http://localhost:5244"
$apiVersion = "v1"

Write-Host "=== Sporting Club API - Endpoint Tests ===" -ForegroundColor Cyan
Write-Host ""

# Test 1: Health Check
Write-Host "1. Testing Health Check..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/$apiVersion/health" -UseBasicParsing -ErrorAction Stop
    Write-Host "   ? Status: $($response.StatusCode) - Health Check OK" -ForegroundColor Green
} catch {
    Write-Host "   ? Health Check Failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 2: Register User
Write-Host "2. Testing User Registration..." -ForegroundColor Yellow
$registerData = @{
    email = "testuser@example.com"
    password = "Test@123456"
    fullName = "Test User"
} | ConvertTo-Json

try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/$apiVersion/auth/register" `
        -Method POST `
        -ContentType "application/json" `
        -Body $registerData `
        -UseBasicParsing -ErrorAction Stop

    Write-Host "   ? Status: $($response.StatusCode) - User Registered Successfully" -ForegroundColor Green
    Write-Host "   Response: $($response.Content | ConvertFrom-Json | ConvertTo-Json)" -ForegroundColor Cyan
} catch {
    Write-Host "   ? Registration Failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "   Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 3: Login
Write-Host "3. Testing Login..." -ForegroundColor Yellow
$loginData = @{
    email = "testuser@example.com"
    password = "Test@123456"
} | ConvertTo-Json

try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/$apiVersion/auth/login" `
        -Method POST `
        -ContentType "application/json" `
        -Body $loginData `
        -UseBasicParsing -ErrorAction Stop

    Write-Host "   ? Status: $($response.StatusCode) - Login Successful" -ForegroundColor Green
    $loginResponse = $response.Content | ConvertFrom-Json
    Write-Host "   Access Token: $($loginResponse.accessToken.Substring(0, 20))..." -ForegroundColor Cyan
} catch {
    Write-Host "   ? Login Failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Tests Complete ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "For more endpoints and detailed documentation, visit:" -ForegroundColor Gray
Write-Host "   http://localhost:5244/swagger" -ForegroundColor Gray
