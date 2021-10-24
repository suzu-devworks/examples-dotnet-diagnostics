# Analyze Stack Overflow

## Run Crash Program

```console
PS > .\bin\Debug\net5.0\Examples.Stackoverflow.exe 

Hello World!
Stack overflow.
Repeat 32102 times:
--------------------------------
   at StackOverflowSimple.Program.CallStackOver()
--------------------------------
   at StackOverflowSimple.Program.Main(System.String[])

```

It will take some time because the size of the output crash dump will be large.


## Analyze Crash Dump

### Start WinDbg

* Setting WinDbg Symbol file Path
  * (File > Symbol File Path...)
  * ```srv*```

* Open dump
  * (File > Open Crash Dump...) or Drag-Drop

### Analyze using SOS


```console
# スタック バックトレースの表示 (~*で全スレッド）"coreclr"を確認するのみ。
0:000> ~*k

.  0  Id: 2eb4.490c Suspend: 0 Teb: 00000011`f69cb000 Unfrozen
 # Child-SP          RetAddr           Call Site
00 00000011`f6655ff0 00007ffb`61b7b03e 0x00007ffb`61b7b039
01 00000011`f6656020 00007ffb`61b7b03e 0x00007ffb`61b7b03e
02 00000011`f6656050 00007ffb`61b7b03e 0x00007ffb`61b7b03e
...　
★すごいのがいる

fd 00000011`f6658f60 00007ffb`61b7b03e 0x00007ffb`61b7b03e
fe 00000011`f6658f90 00007ffb`61b7b03e 0x00007ffb`61b7b03e
ff 00000011`f6658fc0 00007ffb`61b7b03e 0x00007ffb`61b7b03e

   1  Id: 2eb4.284 Suspend: 1 Teb: 00000011`f69cd000 Unfrozen
 # Child-SP          RetAddr           Call Site
00 00000011`f6b7f778 00007ffc`17a42dc7 ntdll!NtWaitForWorkViaWorkerFactory+0x14
...


# SOS拡張のロード
0:000> .load C:\Users\akira\.dotnet\sos\sos.dll

# コールスタックを表示(CLRStack)
0:000> !clrstack
*** WARNING: Unable to verify checksum for StackOverflowSimple.dll
*** WARNING: Unable to verify checksum for StackOverflowSimple.exe
OS Thread Id: 0x490c (0)
        Child SP               IP Call Site
00000011F6655FF0 00007ffb61b7b039 StackOverflowSimple.Program.CallStackOver() [C:\Users\akira\Sources\StackOverflowSimple\Program.cs @ 9]
00000011F6656020 00007ffb61b7b03e StackOverflowSimple.Program.CallStackOver() [C:\Users\akira\Sources\StackOverflowSimple\Program.cs @ 9]
00000011F6656050 00007ffb61b7b03e StackOverflowSimple.Program.CallStackOver() [C:\Users\akira\Sources\StackOverflowSimple\Program.cs @ 9]
00000011F6656080 00007ffb61b7b03e StackOverflowSimple.Program.CallStackOver() [C:\Users\akira\Sources\StackOverflowSimple\Program.cs @ 9]
...
<failed>
Stack Walk failed. Reported stack incomplete.
★すごく長いので [CTRL] + [Break] ... //［Fn］＋［Ctrl］＋［B］

# メソッドポインタから該当のものを表示。（こいつが犯人）
0:000> !U /d 00007ffb61b7b03e
Normal JIT generated code
StackOverflowSimple.Program.CallStackOver()
ilAddr is 000001EA512B2050 pImport is 0000013A5CA734F0
Begin 00007FFB61B7B020, size 26

C:\Users\akira\Sources\StackOverflowSimple\Program.cs @ 8:
00007ffb`61b7b020 55              push    rbp
00007ffb`61b7b021 4883ec20        sub     rsp,20h
00007ffb`61b7b025 488d6c2420      lea     rbp,[rsp+20h]
00007ffb`61b7b02a 833d3f7a090000  cmp     dword ptr [00007ffb`61c12a70],0
00007ffb`61b7b031 7405            je      00007ffb`61b7b038
00007ffb`61b7b033 e808c8ca5f      call    coreclr!JIT_DbgIsJustMyCode (00007ffb`c1827840)
00007ffb`61b7b038 90              nop

C:\Users\akira\Sources\StackOverflowSimple\Program.cs @ 9:
00007ffb`61b7b039 e82217ffff      call    00007ffb`61b6c760 (StackOverflowSimple.Program.CallStackOver(), mdToken: 0000000006000001)
>>> 00007ffb`61b7b03e 90              nop

C:\Users\akira\Sources\StackOverflowSimple\Program.cs @ 10:
00007ffb`61b7b03f 90              nop
00007ffb`61b7b040 488d6500        lea     rsp,[rbp]
00007ffb`61b7b044 5d              pop     rbp
00007ffb`61b7b045 c3              ret

```
