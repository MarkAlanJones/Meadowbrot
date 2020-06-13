Imports System.Runtime.CompilerServices
Imports System.Threading

' The Computer Language Benchmarks Game
' https://salsa.debian.org/benchmarksgame-team/benchmarksgame/

'      started with Java #2 program (Krause/Whipkey/Bennet/AhnTran/Enotus/Stalcup)
'      adapted for C# by Jan de Vaan
'      converted to VB using Telerik online converter - with hand editing by Mark Jones since it crashed
'          https://converter.telerik.com/

Namespace Meadowbrot
    Public Class MandelbrotVB
        Private Shared n As Integer
        Private Shared data As Byte()()
        Private Shared lineCount As Integer
        Private Shared Crb As Double()
        Private Shared Cib As Double()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function getByte(ByVal x As Integer, ByVal y As Integer) As Integer
            Dim res As Integer = 0

            For i As Integer = 0 To 8 - 1 Step 2
                Dim Zr1 As Double = Crb(x + i)
                Dim Zi1 As Double = Cib(y)
                Dim Zr2 As Double = Crb(x + i + 1)
                Dim Zi2 As Double = Cib(y)
                Dim b As Integer = 0
                Dim j As Integer = 49

                Do
                    Dim nZr1 As Double = Zr1 * Zr1 - Zi1 * Zi1 + Crb(x + i)
                    Dim nZi1 As Double = Zr1 * Zi1 + Zr1 * Zi1 + Cib(y)
                    Zr1 = nZr1
                    Zi1 = nZi1
                    Dim nZr2 As Double = Zr2 * Zr2 - Zi2 * Zi2 + Crb(x + i + 1)
                    Dim nZi2 As Double = Zr2 * Zi2 + Zr2 * Zi2 + Cib(y)
                    Zr2 = nZr2
                    Zi2 = nZi2

                    If Zr1 * Zr1 + Zi1 * Zi1 > 4 Then
                        b = b Or 2
                        If b = 3 Then Exit Do
                    End If

                    If Zr2 * Zr2 + Zi2 * Zi2 > 4 Then
                        b = b Or 1
                        If b = 3 Then Exit Do
                    End If
                Loop While Interlocked.Decrement(j) > 0
                res = (res << 2) + b
            Next

            Return res Xor -1
        End Function

        Public Shared Function Generate(ByVal dimensions As Integer) As Byte()()
            n = dimensions

            Dim sw As Stopwatch = New Stopwatch()
            sw.Start()

            Dim lineLen As Integer = (n - 1) / 8 + 1
            data = New Byte(n - 1)() {}
            Crb = New Double(n + 7) {}
            Cib = New Double(n + 7) {}
            Dim invN As Double = 2.0 / n

            For i As Integer = 0 To n - 1
                Cib(i) = i * invN - 1.0
                Crb(i) = i * invN - 1.5
            Next

            lineCount = -1
            Console.WriteLine($"{Environment.ProcessorCount} threads")
            Dim threads = New Thread(Environment.ProcessorCount - 1) {}

            For i As Integer = 0 To threads.Length - 1
                threads(i) = New Thread(Sub()
                                            Dim y As Integer

                                            Do
                                                y = Interlocked.Increment(lineCount)
                                                If (y < data.Length) Then
                                                    Dim buffer = New Byte(lineLen - 1) {}

                                                    For x As Integer = 0 To lineLen - 1
                                                        buffer(x) = CByte(getByte(x * 8, y) And &HFF&)
                                                    Next

                                                    data(y) = buffer
                                                End If
                                            Loop While y <= (n - 1)
                                        End Sub)
                threads(i).Start()
            Next

            For Each t In threads
                t.Join()
            Next

            sw.Stop()
            Console.WriteLine($"VB Compute {sw.ElapsedMilliseconds / 1000.0}s")

            Return data
        End Function

    End Class
End Namespace
