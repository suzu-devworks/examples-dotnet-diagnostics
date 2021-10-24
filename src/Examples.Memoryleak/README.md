# Analyze Memory Leak

## Run Leak Program

```console
PS > .\bin\Debug\net5.0\Examples.Memoryleak.exe

Hello World!
wait enter...

#Leak objects...
```

## VMMap


## Analyze Crash Dump

### Collection of dump force

別端末で procdump します。

```console
# デッドロックしているプロセスのPIDを調べます。
PS C:\Dumps> tlist | findstr Memoryleak
2664 xamples.Memoryleak.exe

# フルダンプ出力
PS C:\Dumps> .\Procdump\procdump.exe -ma 2664

ProcDump v10.0 - Sysinternals process dump utility
Copyright (C) 2009-2020 Mark Russinovich and Andrew Richards
Sysinternals - www.sysinternals.com

[22:23:07] Dump 1 initiated: C:\Dumps\ManagedMemoryLeak.exe_210523_222307.dmp
[22:23:07] Dump 1 writing: Estimated dump file size is 1192 MB.
[22:23:14] Dump 1 complete: 1192 MB written in 7.8 seconds
[22:23:15] Dump count reached.

```

結構おっきいです。

### Start WinDbg

* Setting WinDbg Symbol file Path
  * (File > Symbol File Path...)
  * ```srv*```

* Open dump
  * (File > Open Crash Dump...) or Drag-Drop

### Analize using SOS

```console
# SOS拡張のロード
0:000> .load C:\Users\akira\.dotnet\sos\sos.dll

# 最初にマネージド ヒープの全体的な状態（概略）を確認します。
0:000> !dumpheap -stat

Statistics:
              MT    Count    TotalSize Class Name
00007ffb6482d9e0        1           24 System.IO.SyncTextReader
...
00007ffb6482a718        3        33356 System.Char[]
00007ffb647e6490        2      8388656 ManagedMemoryLeak.LeakObject[]
00000236ca26cca0   123893     11242848      Free
00007ffb64797a90  1000185     33815384 System.String
00007ffb647e4a78  1000000     40000000 ManagedMemoryLeak.LeakObject
00007ffb647dbf30  1000006   1024007885 System.Byte[]
Total 3124273 objects

# 一番大きい System.Byte[] をダンプ
0:000> !DumpHeap /d -mt 00007ffb647dbf30

         Address               MT     Size
00000236cbc4ab58 00007ffb647dbf30       41
00000236cbc4b1b8 00007ffb647dbf30       58
...
00000236cc3e8e68 00007ffb647dbf30     1024
00000236cc3e92b0 00007ffb647dbf30     1024
Command canceled at the user's request.
★すごく長いので [CTRL] + [Break] ... //［Fn］＋［Ctrl］＋［B］

# 適当にオブジェクトダンプ
0:000> !DumpObj /d 00000236cc3e92b0

Name:        System.Byte[]
MethodTable: 00007ffb647dbf30
EEClass:     00007ffb647dbeb0
Size:        1024(0x400) bytes
Array:       Rank 1, Number of elements 1000, Type Byte (Print Array)
Content:     ................................................................................................................................
Fields:
None

# 配列要素なので(Print Array)をクリック(!DumpArray)
0:000> !DumpArray /d 00000236cc3e8e68

Name:        System.Byte[]
MethodTable: 00007ffb647dbf30
EEClass:     00007ffb647dbeb0
Size:        1024(0x400) bytes
Array:       Rank 1, Number of elements 1000, Type Byte
Element Methodtable: 00007ffb646d94b8
[0] 00000236cc3e8e78
[1] 00000236cc3e8e79
[2] 00000236cc3e8e7a
...
[997] 00000236cc3e925d
[998] 00000236cc3e925e
[999] 00000236cc3e925f


#　Byte要素をダンプ（!DumpVC）
0:000> !DumpVC /d 00007ffb646d94b8 00000236cc3e925f

Name:        System.Byte
MethodTable: 00007ffb646d94b8
EEClass:     00007ffb64782178
Size:        24(0x18) bytes
File:        C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.6\System.Private.CoreLib.dll
Fields:
              MT    Field   Offset                 Type VT     Attr            Value Name
00007ffb646d94b8  40002a6        0          System.Byte  1 instance                0 m_value

# そのByte要素はどこから参照されているか確認する。
0:000> !gcroot 00000236cc3e92b0

Thread 1a68:
    0000004B34BDE3E0 00007FFB64715F7D ManagedMemoryLeak.Program.Main(System.String[]) [C:\Users\akira\Sources\ManagedMemoryLeak\Program.cs @ 34]
        rbp-18: 0000004b34bde408
            ->  00000236CBC4B540 System.Collections.Generic.List`1[[ManagedMemoryLeak.LeakObject, ManagedMemoryLeak]]
            ->  00000236DC421188 ManagedMemoryLeak.LeakObject[]
            ->  00000236CC3E9288 ManagedMemoryLeak.LeakObject
            ->  00000236CC3E92B0 System.Byte[]

Found 1 unique roots (run '!gcroot -all' to see all roots).

# スレッドをswitch。
0:000> ~~[1a68]s

ntdll!NtReadFile+0x14:
00007ffc`17a8cee4 c3              ret

# スタックを表示し、オブジェクトへの参照を確認します。
0:000> !dso

OS Thread Id: 0x1a68 (0)
RSP/REG          Object           Name
r8               00000236a235e728 System.Object
0000004B34BDE130 00000236a2361d50 System.Byte[]
...
0000004B34BDE408 00000236cbc4b540 System.Collections.Generic.List`1[[ManagedMemoryLeak.LeakObject, ManagedMemoryLeak]]
...
0000004B34BDEB98 00000236cbc4a4e0 System.String[]

# List~1を表示
0:000> !DumpObj /d 00000236cbc4b540

Name:        System.Collections.Generic.List`1[[ManagedMemoryLeak.LeakObject, ManagedMemoryLeak]]
MethodTable: 00007ffb647e4b08
EEClass:     00007ffb647cf088
Size:        32(0x20) bytes
File:        C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.6\System.Private.CoreLib.dll
Fields:
              MT    Field   Offset                 Type VT     Attr            Value Name
00007ffb647e97b8  4001d3c        8     System.__Canon[]  0 instance 00000236dc421188 _items
00007ffb646db258  4001d3d       10         System.Int32  1 instance          1000000 _size
00007ffb646db258  4001d3e       14         System.Int32  1 instance          1000000 _version
00007ffb647e97b8  4001d3f        8     System.__Canon[]  0   static dynamic statics NYI                 s_emptyArray

# List~1の参照
0:000> !gcroot 00000236cbc4b540

Thread 1a68:
    0000004B34BDE3E0 00007FFB64715F7D ManagedMemoryLeak.Program.Main(System.String[]) [C:\Users\akira\Sources\ManagedMemoryLeak\Program.cs @ 34]
        rbp-18: 0000004b34bde408
            ->  00000236CBC4B540 System.Collections.Generic.List`1[[ManagedMemoryLeak.LeakObject, ManagedMemoryLeak]]

HandleTable:
    00000236CA1F13E8 (strong handle)
    -> 00000236E3C41018 System.Object[]
    -> 00000236CBC4B540 System.Collections.Generic.List`1[[ManagedMemoryLeak.LeakObject, ManagedMemoryLeak]]

Found 2 unique roots (run '!gcroot -all' to see all roots).


# メモリ概要を表示します
0:000> !address -summary

--- Usage Summary ---------------- RgnCount ----------- Total Size -------- %ofBusy %ofTotal
Free                                     63     7dfe`953cf000 ( 125.994 TB)           98.43%
<unknown>                               152      201`6726b000 (   2.005 TB) 100.00%    1.57%
Image                                   197        0`02d28000 (  45.156 MB)   0.00%    0.00%
Stack                                    15        0`00640000 (   6.250 MB)   0.00%    0.00%
Heap                                     18        0`0046e000 (   4.430 MB)   0.00%    0.00%
Other                                     8        0`001d5000 (   1.832 MB)   0.00%    0.00%
TEB                                       5        0`0000a000 (  40.000 kB)   0.00%    0.00%
PEB                                       1        0`00001000 (   4.000 kB)   0.00%    0.00%

--- Type Summary (for busy) ------ RgnCount ----------- Total Size -------- %ofBusy %ofTotal
MEM_MAPPED                               80      200`033ce000 (   2.000 TB)  99.73%    1.56%
MEM_PRIVATE                             119        1`64b2b000 (   5.573 GB)   0.27%    0.00%
MEM_IMAGE                               197        0`02d28000 (  45.156 MB)   0.00%    0.00%

--- State Summary ---------------- RgnCount ----------- Total Size -------- %ofBusy %ofTotal
MEM_FREE                                 63     7dfe`953cf000 ( 125.994 TB)           98.43%
MEM_RESERVE                              66      201`1fa0d000 (   2.004 TB)  99.94%    1.57%
MEM_COMMIT                              330        0`4b214000 (   1.174 GB)   0.06%    0.00%
★このへんを見るらしいことを言っていた。

--- Protect Summary (for commit) - RgnCount ----------- Total Size -------- %ofBusy %ofTotal
PAGE_READWRITE                           98        0`440dc000 (   1.063 GB)   0.05%    0.00%
PAGE_READONLY                           138        0`029d5000 (  41.832 MB)   0.00%    0.00%
PAGE_NOACCESS                            23        0`0261b000 (  38.105 MB)   0.00%    0.00%
PAGE_EXECUTE_READ                        33        0`020e6000 (  32.898 MB)   0.00%    0.00%
PAGE_WRITECOPY                           23        0`00029000 ( 164.000 kB)   0.00%    0.00%
PAGE_EXECUTE_READWRITE                   10        0`00028000 ( 160.000 kB)   0.00%    0.00%
PAGE_READWRITE|PAGE_GUARD                 5        0`00011000 (  68.000 kB)   0.00%    0.00%

--- Largest Region by Usage ----------- Base Address -------- Region Size ----------
Free                                    236`fe910000     7bbd`6ba00000 ( 123.740 TB)
<unknown>                              7dfe`47771000      1f6`fda67000 (   1.965 TB)
Image                                  7ffb`c29a4000        0`008ca000 (   8.789 MB)
Stack                                    4b`35280000        0`0017c000 (   1.484 MB)
Heap                                    236`ec232000        0`001bd000 (   1.738 MB)
Other                                   236`ca540000        0`00181000 (   1.504 MB)
TEB                                      4b`34c7b000        0`00002000 (   8.000 kB)
PEB                                      4b`34c7a000        0`00001000 (   4.000 kB)

```
