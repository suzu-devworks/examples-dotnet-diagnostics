# Tools

## WinDbg

* [Windows 10 SDK - Windows app development](https://developer.microsoft.com/ja-jp/windows/downloads/windows-10-sdk/)
* [WinDbg Commands | Microsoft Docs](https://docs.microsoft.com/ja-jp/windows-hardware/drivers/debugger/commands)


## SOS debugging extension

* [SOS installer (dotnet-sos)](https://docs.microsoft.com/ja-jp/dotnet/core/diagnostics/dotnet-sos)
* [SOS.dll (SOS Debugging Extension) | Microsoft Docs](https://docs.microsoft.com/ja-jp/dotnet/framework/tools/sos-dll-sos-debugging-extension)

### install

```powershell
dotnet tool install --global dotnet-sos
```

```console
> dotnet sos install
Installing SOS to C:\Users\akira\.dotnet\sos
Creating installation directory...
Copying files from C:\Users\akira\.dotnet\tools\.store\dotnet-sos\5.0.221401\dotnet-sos\5.0.221401\tools\netcoreapp2.1\any\win-x64
Copying files from C:\Users\akira\.dotnet\tools\.store\dotnet-sos\5.0.221401\dotnet-sos\5.0.221401\tools\netcoreapp2.1\any\lib
Execute '.load C:\Users\akira\.dotnet\sos\sos.dll' to load SOS in your Windows debugger.
SOS install succeeded
```

## Others

* [ProcDump - Windows Sysinternals](https://docs.microsoft.com/en-us/sysinternals/downloads/procdump)
* [VMMap - Windows Sysinternals](https://docs.microsoft.com/ja-jp/sysinternals/downloads/vmmap)
* [icsharpcode/ILSpy · GitHub](https://github.com/icsharpcode/ILSpy)
* [General Extensions | Microsoft Docs](https://docs.microsoft.com/ja-jp/windows-hardware/drivers/debugger/general-extensions)
