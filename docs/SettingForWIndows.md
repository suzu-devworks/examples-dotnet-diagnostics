# Setting to output a crash dump for Windows.

If you want to output a dump file when an application crashes, use the WER (Windows Error Reporing) to output it.

* [WER Settings - Win32 apps | Microsoft Docs](https://docs.microsoft.com/en-us/windows/win32/wer/wer-settings)

## WER Settings

"one of the following registry subkeys"<br/>
I tried HKEY_CURRENT_USER, but the dump was not output.

* HKEY_CURRENT_USER\Software\Microsoft\Windows\Windows Error Reporting
* HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\Windows Error Reporting

> LocalDumps\DumpFolder or LocalDumps\[Application Name]\DumpFolder

```powershell
# Run as Administrator.

reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps" /v DumpFolder /t REG_EXPAND_SZ /d "C:\Dumps" /f
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps" /v DumpType /t REG_DWORD /d "2" /f
```

|Entry |Type |Description |
|--|--|--|
|DumpFolder |REG_SZ |Output folder |
|DumpType |REG_DWARD |0 - Custom dump<br/>1 - Minidump (default)<br/>2 - Full dump|

* [Collecting User-Mode Dumps | Microsoft Docs](https://docs.microsoft.com/ja-jp/windows/win32/wer/collecting-user-mode-dumps)
