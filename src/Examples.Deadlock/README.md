# Analyze Deadlock (Hang)

If a program hangs due to a deadlock, the program simply hangs if the OS cannot detect the deadlock. In this case, you can force a dump to analyze the situation.

## Analyze Crash Dump

### Collection of dump

Run the problem code.

```console
%PS > .\bun\Deug\net5.0\Examples.Deadlock.exe

Hello World!
Inside Method B
Inside Method A
Method A: Inside LockA and Trying to enter LockB
Method B: Inside LockB and Trying to enter LockA
wait enter...

#Deadlocked...

```

Procdump on another terminal..

```
# Find out the PID of the deadlocked process.
%PS C:\Dumps> tlist | findstr ConsoleApp1
10680 Code.exe          ? Untitled-1 - ConsoleApp1 - Visual Studio Code
16064 ConsoleApp1.exe

# Output Full dump.
%PS C:\Dumps> .\Procdump\procdump -ma 10680

ProcDump v10.0 - Sysinternals process dump utility
Copyright (C) 2009-2020 Mark Russinovich and Andrew Richards
Sysinternals - www.sysinternals.com

[22:42:54] Dump 1 initiated: C:\Dumps\ConsoleApp1.exe_210522_224254.dmp
[22:42:54] Dump 1 writing: Estimated dump file size is 83 MB.
[22:42:54] Dump 1 complete: 83 MB written in 0.3 seconds
[22:42:54] Dump count reached.

```

Kill the process after getting the dump.

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

.  0  Id: 3ec0.2bd8 Suspend: 0 Teb: 0000004c`204e2000 Unfrozen
 # Child-SP          RetAddr           Call Site
00 0000004c`2077dfb8 00007ffc`1574aad3 ntdll!NtReadFile+0x14
01 0000004c`2077dfc0 00007ffb`60148272 KERNELBASE!ReadFile+0x73
02 0000004c`2077e040 00007ffb`fcec86e2 0x00007ffb`60148272
03 0000004c`2077e110 00007ffb`fcec852c System_Console!System.ConsolePal+WindowsConsoleStream.ReadFileNative(IntPtr, Byte[], Int32, Int32, Boolean, Int32 ByRef, Boolean)$##60000FB+0x72
04 0000004c`2077e170 00007ffb`bf854efe System_Console!System.ConsolePal+WindowsConsoleStream.Read(Byte[], Int32, Int32)$##60000F8+0x5c
05 0000004c`2077e1e0 00007ffb`bf85527d System_Private_CoreLib!System.IO.StreamReader.ReadBuffer()$##6005674+0xae
06 0000004c`2077e230 00007ffb`fcecd043 System_Private_CoreLib!System.IO.StreamReader.ReadLine()$##6005676+0x3d
07 0000004c`2077e280 00007ffb`fcec47ac System_Console!System.IO.SyncTextReader.ReadLine()$##6000191+0x43
08 0000004c`2077e2d0 00007ffb`601467c4 System_Console!System.Console.ReadLine()$##6000075+0x1c
09 0000004c`2077e300 00007ffb`bfc99373 0x00007ffb`601467c4
0a 0000004c`2077e390 00007ffb`bfb7d0fa coreclr!CallDescrWorkerInternal+0x83 [D:\workspace\_work\1\s\src\coreclr\src\vm\amd64\CallDescrWorkerAMD64.asm @ 100]
0b 0000004c`2077e3d0 00007ffb`bfc007df coreclr!MethodDescCallSite::CallTargetWorker+0x3d2 [D:\workspace\_work\1\s\src\coreclr\src\vm\callhelpers.cpp @ 552]
0c (Inline Function) --------`-------- coreclr!MethodDescCallSite::Call+0xb [D:\workspace\_work\1\s\src\coreclr\src\vm\callhelpers.h @ 458]
...

# Loading SOS extensions.
% 0:000> .load C:\Users\akira\.dotnet\sos\sos.dll

# Display the call stack (!CLRStack) to find the thread that output the exception. (~*e is run in all threads)
% 0:000> ~*e!clrstack

*** WARNING: Unable to verify checksum for ConsoleApp1.dll
OS Thread Id: 0x2bd8 (0) →　Main()’s thread.
        Child SP               IP Call Site
0000004C2077E078 00007ffc17a8cee4 [InlinedCallFrame: 0000004c2077e078] Interop+Kernel32.ReadFile(IntPtr, Byte*, Int32, Int32 ByRef, IntPtr)
...
0000004C2077E2D0 00007ffbfcec47ac System.Console.ReadLine() [/_/src/libraries/System.Console/src/System/Console.cs @ 629]
0000004C2077E300 00007ffb601467c4 ConsoleApp1.Program.Main(System.String[]) [C:\Users\akira\Sources\ConsoleApp1\Program.cs @ 56]
...

OS Thread Id: 0xc84 (5) → MethodB()’s thread.
        Child SP               IP Call Site
0000004C2123F428 00007ffc17a8d974 [HelperMethodFrame_1OBJ: 0000004c2123f428] System.Threading.Monitor.ReliableEnter(System.Object, Boolean ByRef)
0000004C2123F580 00007ffbbf75af9f System.Threading.Monitor.Enter(System.Object, Boolean ByRef) [/_/src/coreclr/src/System.Private.CoreLib/src/System/Threading/Monitor.cs @ 46]
0000004C2123F5B0 00007ffb6014c84c ConsoleApp1.Program.MethodB() [C:\Users\akira\Sources\ConsoleApp1\Program.cs @ 36]
0000004C2123F610 00007ffb6014c761 ConsoleApp1.Program+c.b__4_1() [C:\Users\akira\Sources\ConsoleApp1\Program.cs @ 52]
...

OS Thread Id: 0x6b0 (6) → MethodA()’s thread.
        Child SP               IP Call Site
0000004C213FEFA8 00007ffc17a8d974 [HelperMethodFrame_1OBJ: 0000004c213fefa8] System.Threading.Monitor.ReliableEnter(System.Object, Boolean ByRef)
0000004C213FF100 00007ffbbf75af9f System.Threading.Monitor.Enter(System.Object, Boolean ByRef) [/_/src/coreclr/src/System.Private.CoreLib/src/System/Threading/Monitor.cs @ 46]
0000004C213FF130 00007ffb6014ca0c ConsoleApp1.Program.MethodA() [C:\Users\akira\Sources\ConsoleApp1\Program.cs @ 19]
0000004C213FF190 00007ffb6014c721 ConsoleApp1.Program+c.b__4_0() [C:\Users\akira\Sources\ConsoleApp1\Program.cs @ 51]
...


# スレッドが所有しているオブジェクトに対応する SyncBlock 構造体を表示（SyncBlk）
# Owning Thread にThread Id がついているものを
% 0:000> !syncblc -all

No export syncblc found
% 0:000> !syncblk -all
Index         SyncBlock MonitorHeld Recursion Owning Thread Info          SyncBlock Owner
    1 000001C07D425FC8            0         0 0000000000000000     none    000001c00000aef0 Interop+Advapi32+EtwEnableCallback
...
   10 000001C07D426298            3         1 000001C07EF42020 6b0   6   000001c000011680 System.Object
   11 000001C07D4262E8            3         1 000001C07EF39C50 c84   5   000001c000011698 System.Object
-----------------------------
Total           11
CCW             0
RCW             0
ComClassFactory 0
Free            0

# スレッドを切り替える(Switch)
% 0:000> ~~[6b0]s

ntdll!NtWaitForMultipleObjects+0x14:
00007ffc`17a8d974 c3              ret

# 切り替えたスレッドはコールスタックで確認する。
0:006> !clrstack

OS Thread Id: 0x6b0 (6)
        Child SP               IP Call Site
0000004C213FEFA8 00007ffc17a8d974 [HelperMethodFrame_1OBJ: 0000004c213fefa8] System.Threading.Monitor.ReliableEnter(System.Object, Boolean ByRef)
0000004C213FF100 00007ffbbf75af9f System.Threading.Monitor.Enter(System.Object, Boolean ByRef) [/_/src/coreclr/src/System.Private.CoreLib/src/System/Threading/Monitor.cs @ 46]
0000004C213FF130 00007ffb6014ca0c ConsoleApp1.Program.MethodA() [C:\Users\akira\Sources\ConsoleApp1\Program.cs @ 19]
...

# 現在のスタックの範囲内で見つかったすべてのマネージド オブジェクトを表示します。(DumpStackObjects)
0:006> !dso

OS Thread Id: 0x6b0 (6)
RSP/REG          Object           Name
0000004C213FEDF8 000001c000012058 System.Threading.ThreadPoolWorkQueueThreadLocals
0000004C213FEE40 000001c000011698 System.Object
0000004C213FEE70 000001c000012090 System.Threading.Thread
...
0000004C213FF168 000001c000011698 System.Object
0000004C213FF178 000001c000011680 System.Object
0000004C213FF190 000001c00000e918 consoleApp1.Program+<>c
...

# 指定したアドレスにある（ロックしている）オブジェクトに関する情報を表示します。(DumpObj)
0:006> !do 000001c000011680

Name:        System.Object
MethodTable: 00007ffb60100c68
EEClass:     00007ffb6012db20
Size:        24(0x18) bytes
File:        C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.6\System.Private.CoreLib.dll
Object
Fields:
None

0:006> !do 000001c000011698
Name:        System.Object
MethodTable: 00007ffb60100c68
EEClass:     00007ffb6012db20
Size:        24(0x18) bytes
File:        C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.6\System.Private.CoreLib.dll
Object
Fields:
None

...

```
