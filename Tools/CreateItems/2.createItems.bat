@echo off

cd %~dp0
rem  call .venv\Scripts\activate

for /f "delims=" %%a in ('python checktags.py') do (
    echo %%a
    pause
    exit /b
)

python createItems.py
python offset.py

