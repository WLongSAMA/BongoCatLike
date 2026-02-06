@echo off

cd %~dp0
rem  call .venv\Scripts\activate

rem python -m pip install --upgrade pip
rem pip install steam[client]
rem pip install requests

set HTTP_PROXY=http://127.0.0.1:7890
set HTTPS_PROXY=http://127.0.0.1:7890
python inventory.py

