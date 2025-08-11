cd %~dp0
call .venv\Scripts\activate

python createItems.py
python offset.py

