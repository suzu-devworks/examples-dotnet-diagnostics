# Basic Crash dump Analyze

## Basic Analyze using WinDbg

### Collection of dump

Run the problem code.

```console
> .\ConsoleApp1.exe
Hello World!
Unhandled exception. System.Exception: Test
   at ConsoleApp1.Program.Main(String[] args) in C:\Users\akira\Sources\ConsoleApp1\Program.cs:line 10
```

Output to WER's DumpFolder.

### Start WinDbg

* Setting WinDbg Symbol file Path
  * (File > Symbol File Path...)
  * ```srv*```

* Open dump
  * (File > Open Crash Dump...) or Drag-Drop

### Analize using SOS

```console
# Check by displaying "coreclr" in the stack backtrace (~* is all threads).
% 0:000> ~*k

.  0  Id: 137c.4cc Suspend: 0 Teb: 0000000c`2861f000 Unfrozen
 # Child-SP          RetAddr           Call Site
00 0000000c`2897cea8 00007ffc`1576c5a0 ntdll!NtWaitForMultipleObjects+0x14
01 0000000c`2897ceb0 00007ffc`1576c49e KERNELBASE!WaitForMultipleObjectsEx+0xf0
02 0000000c`2897d1a0 00007ffc`1795f6aa KERNELBASE!WaitForMultipleObjects+0xe
...
0c 0000000c`2897e5a0 00007ffb`c7e09649 coreclr!RaiseTheExceptionInternalOnly+0x29a [D:\workspace\_work\1\s\src\coreclr\src\vm\excep.cpp @ 2806]
0d 0000000c`2897e6b0 00007ffb`682f5f45 coreclr!IL_Throw+0xd9 [D:\workspace\_work\1\s\src\coreclr\src\vm\jithelpers.cpp @ 4185]
...

# Loading SOS extensions. but .loadby doesn't work.
% 0:000> .load C:\Users\akira\.dotnet\sos\sos.dll

# Display the call stack (!CLRStack) to find the thread that output the exception. (~*e is run in all threads)
% 0:000> ~*e!clrstack

*** WARNING: Unable to verify checksum for consoleApp1.dll
OS Thread Id: 0x4cc (0)
        Child SP               IP Call Site
0000000C2897E718 00007ffc17a8d974 [HelperMethodFrame: 0000000c2897e718]
0000000C2897E810 00007ffb682f5f45 ConsoleApp1.Program.Main(System.String[]) [C:\Users\akira\Sources\ConsoleApp1\Program.cs @ 10]
OS Thread Id: 0x347c (1)
Unable to walk the managed stack. The current thread is likely not a
managed thread. You can run !threads to get a list of managed threads in
the process
Failed to start stack walk: 80070057
...

# 現在のスタックの範囲内で見つかったすべてのマネージド オブジェクトを表示します。(DumpStackObjects)
% 0:000> !dso

OS Thread Id: 0x4cc (0)
RSP/REG          Object           Name
0000000C2897DD20 0000019f0000e8f0 System.Exception
0000000C2897E470 0000019f0000e8f0 System.Exception
0000000C2897E4C0 0000019f0000e8f0 System.Exception
...

# 指定したアドレスにある Exception クラスから派生したすべてのオブジェクトのフィールドが表示および書式設定されます。(PrintException)
% 0:000> !pe 0000019f0000e8f0

Exception object: 0000019f0000e8f0
Exception type:   System.Exception
Message:          Test
InnerException:   <none>
StackTrace (generated):
    SP               IP               Function
    0000000C2897E810 00007FFB682F5F45 ConsoleApp1!ConsoleApp1.Program.Main(System.String[])+0x85

StackTraceString: <none>
HResult: 80131500

# -nested は入れ子になった例外(InnerException)オブジェクトに関する詳細を表示します。
% 0:000> !pe -nested 0000019f0000e8f0

Exception object: 0000019f0000e8f0
Exception type:   System.Exception
Message:          Test
InnerException:   <none>
StackTrace (generated):
    SP               IP               Function
    0000000C2897E810 00007FFB682F5F45 ConsoleApp1!ConsoleApp1.Program.Main(System.String[])+0x85

StackTraceString: <none>
HResult: 80131500

# アドレスを省略すると、現在のスレッドで最後にスローされた例外が表示されます。
% 0:000> !pe

Exception object: 0000019f0000e8f0
Exception type:   System.Exception
Message:          Test
InnerException:   <none>
StackTrace (generated):
    SP               IP               Function
    0000000C2897E810 00007FFB682F5F45 ConsoleApp1!ConsoleApp1.Program.Main(System.String[])+0x85

StackTraceString: <none>
HResult: 80131500

# 指定したアドレスにあるオブジェクトに関する情報を表示します。(DumpObj)
% 0:000> !do 0000019f0000e8f0

Name:        System.Exception
MethodTable: 00007ffb68379410
EEClass:     00007ffb68366428
Size:        128(0x80) bytes
File:        C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.6\System.Private.CoreLib.dll
Fields:
              MT    Field   Offset                 Type VT     Attr            Value Name
00007ffb683f83a0  400014f        8 ...ection.MethodBase  0 instance 0000000000000000 _exceptionMethod
00007ffb68377a90  4000150       10        System.String  0 instance 0000019f0000e970 _message
00007ffb6837b840  4000151       18 ...tions.IDictionary  0 instance 0000000000000000 _data
00007ffb68379410  4000152       20     System.Exception  0 instance 0000000000000000 _innerException
00007ffb68377a90  4000153       28        System.String  0 instance 0000000000000000 _helpURL
00007ffb683bb718  4000154       30        System.Byte[]  0 instance 0000019f0000e990 _stackTrace
...

```
