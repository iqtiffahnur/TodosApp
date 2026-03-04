========================================
Todos - Single-file App (README)
========================================

What is this?
-------------
This folder contains a self-contained build of the Todos app:
- Backend: ASP.NET Core Web API
- Frontend: React (built and placed in wwwroot)
- Database: SQLite (todos.db will be created on first write)
- No .NET or Node.js required to run this .exe

How to run
----------
1) Unzip this folder somewhere you have write access (e.g., Desktop or Documents).
2) Double-click: Todos.Api.exe
   - Or run from terminal for a clear URL:
     Todos.Api.exe --urls http://localhost:5000
3) Open your browser:
   - UI:   http://localhost:5000/
   - API:  http://localhost:5000/api/todos
   - Docs: http://localhost:5000/swagger

Notes
-----
- On first write (adding/updating a todo), SQLite file "todos.db" is created in this same folder.
- If Windows Firewall prompts, click "Allow access" (Private networks is enough).
- If a browser tab shows a blank page, ensure the "wwwroot" folder is present and not empty.
- If API returns 500 on first run, close the app and run it again (the app auto-applies EF migrations at startup).

Common issues & quick fixes
---------------------------
A) "The process cannot access the file ... Todos.Api.exe is being used"
   - Close any running instance of Todos.Api.exe, then run again.

B) "Failed to fetch" or the UI can't load data
   - Make sure you launched the .exe with a URL (e.g., http://localhost:5000)
   - Open http://localhost:5000/swagger and try GET /api/todos
   - If Swagger works, refresh the UI page
   - If it still fails, close the app and reopen from a writable location (e.g., Desktop)

C) Want HTTPS?
   - If this package includes https-dev.pfx and appsettings.Production.json points to it,
     the app will also listen on https://localhost:5001.
   - Browsers may warn for self-signed certificates. You can proceed or run on HTTP.

Folder contents (what you should see)
-------------------------------------
- Todos.Api.exe               <-- double-click this to run
- wwwroot\                    <-- React build (static files)
- appsettings.json / appsettings.Production.json
- *.dll, *.json, *.dat, etc.  <-- runtime files
- (optional) https-dev.pfx    <-- only if HTTPS is configured

Troubleshooting deeper
----------------------
- Open http://localhost:5000/swagger to test API endpoints directly.
- If /api/todos fails with 500, the console window shows the error message.
- The app applies EF Core migrations automatically at startup to create/update the SQLite schema.

Stop the app
------------
- Close the console window OR press Ctrl + C in the console.

Enjoy!