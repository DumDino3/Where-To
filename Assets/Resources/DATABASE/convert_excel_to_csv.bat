@echo off
REM Batch file to convert Excel sheets to CSV files
REM This script runs the excel_to_csv.py Python script

echo ========================================
echo Excel to CSV Converter
echo ========================================
echo.

REM Change to the Database directory
cd /d "%~dp0"

REM Check if Python is installed
python --version >nul 2>&1
if errorlevel 1 (
    echo Error: Python is not installed or not in PATH
    echo Please install Python 3.10+ from https://www.python.org/downloads/
    pause
    exit /b 1
)

REM Check if openpyxl is installed
python -c "import openpyxl" >nul 2>&1
if errorlevel 1 (
    echo Warning: openpyxl is not installed
    echo Installing openpyxl...
    python -m pip install openpyxl
    if errorlevel 1 (
        echo Error: Failed to install openpyxl
        pause
        exit /b 1
    )
)

echo Converting Excel sheets to CSV...
echo.

REM Run the Python script
python excel_to_csv.py Database.xlsx --out ../Resources/CsvDatabase

echo.
echo ========================================
echo Conversion complete!
echo ========================================
echo.
pause
