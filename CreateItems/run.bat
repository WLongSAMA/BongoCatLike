cd %~dp0
call .venv\Scripts\activate

rem python -m pip install --upgrade pip
rem pip install steam[client]

rem python inventory.py
python createItems.py

