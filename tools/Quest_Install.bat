@echo off
setlocal
pushd "%~dp0.." || exit /b 1
where py >nul 2>&1
if not errorlevel 1 (
    py -3 tools\quest_device.py install %*
) else (
    where python >nul 2>&1
    if errorlevel 1 (
        echo This optional helper needs Python 3. Install Python, or use Unity's Android Build And Run.
        popd
        pause
        exit /b 1
    )
    python tools\quest_device.py install %*
)
set "fxr_exit=%errorlevel%"
popd
pause
exit /b %fxr_exit%
