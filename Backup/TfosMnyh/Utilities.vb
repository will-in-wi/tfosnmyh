Module Utilities
    Public Function CheckHSDir(ByVal Directory As String) As Boolean
        Return My.Computer.FileSystem.DirectoryExists(Directory & "\FNDNDX")
    End Function

    Public Function AutoDetectHS() As String
        If CheckHSDir("C:\Program Files\HymnSoft") = True Then
            Return "C:\Program Files\HymnSoft"
        ElseIf CheckHSDir("C:\Program Files (x86)\HymnSoft") Then
            Return "C:\Program Files (x86)\HymnSoft"
        Else
            Dim path As String
            path = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Classes\HSoft_File\DefaultIcon", "", "")
            path = path.Substring(0, path.IndexOf("cw.ico"))
            If CheckHSDir(path) = True Then
                Return path
            Else
                Return ""
            End If
        End If
    End Function

    Public Class Hymn
        Protected intNumber As Integer
        Protected strTitle As String
        Protected strVerses As String
        Protected strMelody As String
        Protected strTheme As String
        Protected strTune As String
        Protected strScripture As String
        Protected intTuneID As String

        Public Property Number() As Integer
            Get
                Return intNumber
            End Get
            Set(ByVal value As Integer)
                intNumber = value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return strTitle
            End Get
            Set(ByVal value As String)
                strTitle = value
            End Set
        End Property

        Public Property Verses() As String
            Get
                Return strVerses
            End Get
            Set(ByVal value As String)
                strVerses = value
            End Set
        End Property

        Public Property Melody() As String
            Get
                Return strMelody
            End Get
            Set(ByVal value As String)
                strMelody = value
            End Set
        End Property

        Public Property Theme() As String
            Get
                Return strTheme
            End Get
            Set(ByVal value As String)
                strTheme = value
            End Set
        End Property

        Public Property Tune() As String
            Get
                Return strTune
            End Get
            Set(ByVal value As String)
                strTune = value
            End Set
        End Property

        Public Property Scripture() As String
            Get
                Return strScripture
            End Get
            Set(ByVal value As String)
                strScripture = value
            End Set
        End Property

        Public Property TuneID() As String
            Get
                Return intTuneID
            End Get
            Set(ByVal value As String)
                intTuneID = value
            End Set
        End Property
    End Class

    Private Function GetTuneID(ByVal Tunes() As HymnTune, ByVal TuneName As String) As Integer
        For Each tune In Tunes
            If tune.Name = TuneName Then
                Return tune.ID
            End If
        Next
    End Function

    Public Function LoadTitles(ByVal path As String) As Hymn()
        Dim btNumbers() As Byte = My.Computer.FileSystem.ReadAllBytes(path & "\Number.SBF")

        Dim Hymns As New List(Of Hymn)

        ' pointer for where in the file we are
        Dim i As Integer = 0

        While i + 1 < btNumbers.Length

            Dim NewHymn As New Hymn

            ' Find beginning of hymn number
            ' Look for three consecutive bytes that contain 128 255 and 2. The next two bytes are the hymn number.
            Dim x As Integer
            x = i
            While x < btNumbers.Length
                If btNumbers(x) = 128 Then
                    If btNumbers(x + 1) = 255 And btNumbers(x + 2) = 2 Then
                        i = x + 3
                        Exit While
                    End If
                End If
                x = x + 1
            End While

            ' Pull and convert the hymn number
            Dim btNumber(2) As Byte
            btNumber(0) = btNumbers(i)
            btNumber(1) = btNumbers(i + 1)
            NewHymn.Number = BitConverter.ToInt16(btNumber, 0)

            i = i + 2

            ' Get title
            Dim btTitle As New List(Of Byte)

            x = i
            While x < btNumbers.Length
                If btNumbers(x) = 0 Then
                    Exit While
                Else
                    btTitle.Add(btNumbers(x))
                End If
                x = x + 1
            End While
            NewHymn.Title = System.Text.Encoding.ASCII.GetString(btTitle.ToArray())

            Hymns.Add(NewHymn)

            i = x
        End While

        Return Hymns.ToArray

    End Function

    Public Function LoadEverything(ByVal path As String) As Hymn()
        Dim btNumbers() As Byte = My.Computer.FileSystem.ReadAllBytes(path & "\Test.SBF")

        Dim Hymns As New List(Of Hymn)

        ' pointer for where in the file we are
        Dim i As Integer = 5

        ' Fix weird chars in the text
        ' There are places where there will be 4 bytes of nonsense. Every one is one higher than the last one.
        ' Exceptions, there will be four bytes of 0s sometimes. Sometimes a number will be skipped.
        ' A pass through the database will remove these.
        Dim intOne As Integer = 3
        Dim intTwo As Integer = 0
        Dim tmpNumbers As New List(Of Byte)
        ' convert array to a list for easier processing.
        For Each num As Byte In btNumbers
            tmpNumbers.Add(num)
        Next

        i = 0

        ' remove junk bytes
        Dim lastI As Integer = 0
        Dim reachedEnd As Integer = 0
        While 1 = 1

            If reachedEnd = 5 Then
                Exit While
            End If

            If i = lastI + 1000 Or i + 5 >= tmpNumbers.Count Then
                i = lastI
                reachedEnd = reachedEnd + 1
                If intOne = 255 Then
                    intOne = 0
                    intTwo = intTwo + 1
                Else
                    intOne = intOne + 1
                End If
            End If

            If tmpNumbers(i) = intOne And tmpNumbers(i + 1) = intTwo And tmpNumbers(i + 2) = 0 And tmpNumbers(i + 3) = 0 Then
                tmpNumbers.RemoveAt(i + 3)
                tmpNumbers.RemoveAt(i + 2)
                tmpNumbers.RemoveAt(i + 1)
                tmpNumbers.RemoveAt(i)
                If intOne = 255 Then
                    intOne = 0
                    intTwo = intTwo + 1
                ElseIf intOne = 12 And intTwo = 0 Then ' 13 is the carrige return char and messes it up. It isn't there anyway.
                    intOne = 14
                Else
                    intOne = intOne + 1
                End If
                lastI = i
                reachedEnd = 0
            Else

            End If
            i = i + 1
        End While
        btNumbers = tmpNumbers.ToArray

        ' remove 0000 where it is a problem
        While i + 5 < tmpNumbers.Count
            If tmpNumbers(i) = 0 And tmpNumbers(i - 1) <> 0 And tmpNumbers(i + 1) = 0 And tmpNumbers(i + 2) = 0 And tmpNumbers(i + 3) = 0 And tmpNumbers(i + 4) <> 0 Then
                tmpNumbers.RemoveAt(i + 3)
                tmpNumbers.RemoveAt(i + 2)
                tmpNumbers.RemoveAt(i + 1)
                tmpNumbers.RemoveAt(i)
            End If
            i = i + 1
        End While

        i = 0

        While i + 1 < btNumbers.Length

            Dim NewHymn As New Hymn

            ' Find beginning of hymn number
            ' Look for three consecutive bytes that contain 128 255 and 2. The next two bytes are the hymn number.
            Dim x As Integer
            x = i
            While x < btNumbers.Length
                If btNumbers(x) = 128 Then
                    If btNumbers(x + 1) = 255 And btNumbers(x + 2) = 2 Then
                        i = x + 3
                        Exit While
                    End If
                End If
                x = x + 1

                If x >= btNumbers.Length Then
                    Return Hymns.ToArray
                End If
            End While

            ' Pull and convert the hymn number
            Dim btNumber(2) As Byte
            btNumber(0) = btNumbers(i)
            btNumber(1) = btNumbers(i + 1)
            NewHymn.Number = BitConverter.ToInt16(btNumber, 0)

            i = i + 2

            ' Get Title
            Dim btTitle As New List(Of Byte)
            While i < btNumbers.Length
                If btNumbers(i) <> 0 Then
                    btTitle.Add(btNumbers(i))
                    i = i + 1
                Else
                    i = i + 1
                    NewHymn.Title = System.Text.Encoding.ASCII.GetString(btTitle.ToArray)
                    Exit While
                End If
            End While

            ' Get Verses
            ' Something is weird with this where there will be a number or two followed by two or three 0s in the middle of the text.
            ' Attempt to fix above
            Dim btVerses As New List(Of Byte)
            While i < btNumbers.Length

                If NewHymn.Verses <> "" Then
                    Exit While
                End If

                If btNumbers(i) <> 0 Then
                    btVerses.Add(btNumbers(i))
                    i = i + 1
                Else
                    ' The end of the text will be marked by 0s followed by a 127, 90, and 76

                    x = i
                    While i + 1 < btNumbers.Length
                        x = x + 1
                        If btNumbers(x) = 0 Then
                            Continue While
                        ElseIf btNumbers(x) = 127 And btNumbers(x + 1) = 90 And btNumbers(x + 2) = 76 Then
                            NewHymn.Verses = System.Text.Encoding.ASCII.GetString(btVerses.ToArray)
                            i = x
                            Exit While
                        Else
                            ' we have hit corruption in the middle of the verses. Remove 4 bytes. Shouldn't hit now.
                            If x - i = 3 Then
                                btVerses.RemoveAt(btVerses.Count - 1)
                                i = x
                                Exit While
                            ElseIf x - i = 2 Then
                                btVerses.RemoveAt(btVerses.Count - 1)
                                btVerses.RemoveAt(btVerses.Count - 2)
                                i = x
                                Exit While
                            ElseIf x - i = 1 Then
                                btVerses.RemoveAt(btVerses.Count - 1)
                                btVerses.RemoveAt(btVerses.Count - 2)
                                btVerses.RemoveAt(btVerses.Count - 3)
                                i = x
                                Exit While
                            End If
                        End If
                    End While

                End If
            End While

            ' Get Tune ID
            While i + 1 < btNumbers.Length
                If btNumbers(i) = 127 And btNumbers(i + 1) = 90 And btNumbers(i + 2) = 76 Then
                    i = i + 3
                    Exit While
                Else
                    i = i + 1
                End If
            End While

            While i + 1 < btNumbers.Length
                If btNumbers(i) = 6 Then ' Done with the string. Break and move on.
                    i = i + 1
                    Exit While
                ElseIf btNumbers(i) = 24 Then
                    NewHymn.TuneID = NewHymn.TuneID & "0"
                ElseIf btNumbers(i) = 25 Then
                    NewHymn.TuneID = NewHymn.TuneID & "1"
                ElseIf btNumbers(i) = 26 Then
                    NewHymn.TuneID = NewHymn.TuneID & "2"
                ElseIf btNumbers(i) = 27 Then
                    NewHymn.TuneID = NewHymn.TuneID & "3"
                ElseIf btNumbers(i) = 28 Then
                    NewHymn.TuneID = NewHymn.TuneID & "4"
                ElseIf btNumbers(i) = 29 Then
                    NewHymn.TuneID = NewHymn.TuneID & "5"
                ElseIf btNumbers(i) = 30 Then
                    NewHymn.TuneID = NewHymn.TuneID & "6"
                ElseIf btNumbers(i) = 31 Then
                    NewHymn.TuneID = NewHymn.TuneID & "7"
                ElseIf btNumbers(i) = 16 Then
                    NewHymn.TuneID = NewHymn.TuneID & "8"
                ElseIf btNumbers(i) = 17 Then
                    NewHymn.TuneID = NewHymn.TuneID & "9"
                ElseIf btNumbers(i) = 73 Then
                    NewHymn.TuneID = NewHymn.TuneID & "A"
                ElseIf btNumbers(i) = 74 Then
                    NewHymn.TuneID = NewHymn.TuneID & "B"
                ElseIf btNumbers(i) = 75 Then
                    NewHymn.TuneID = NewHymn.TuneID & "C"
                ElseIf btNumbers(i) = 76 Then
                    NewHymn.TuneID = NewHymn.TuneID & "D"
                ElseIf btNumbers(i) = 77 Then
                    NewHymn.TuneID = NewHymn.TuneID & "E"
                ElseIf btNumbers(i) = 78 Then
                    NewHymn.TuneID = NewHymn.TuneID & "F"
                End If
                i = i + 1
            End While

            ' Get theme
            x = i
            While x < btNumbers.Length
                If btNumbers(x) = 80 Then
                    If btNumbers(x + 1) = 0 Then
                        i = x + 2
                        Exit While
                    End If
                End If
                x = x + 1
            End While

            Dim btTheme As New List(Of Byte)
            While i < btNumbers.Length
                If btNumbers(i) <> 0 Then
                    btTheme.Add(btNumbers(i))
                    i = i + 1
                Else
                    i = i + 1
                    NewHymn.Theme = System.Text.Encoding.ASCII.GetString(btTheme.ToArray)
                    Exit While
                End If
            End While

            ' Stupid hack to avoid fixing other code for one hymn.
            ' Hymn 276 does not have a tune name or a scripture reference. So just skip the rest and continue.
            ' It also happens to be the only hymn that has four 0s following the hymn. An earlier loop would have removed that.
            If NewHymn.Number = 276 Then
                NewHymn.Theme = "Hymns Of The Liturgy"
                Hymns.Add(NewHymn)
                Continue While
            End If

            ' Get Tune Name
            Dim btTune As New List(Of Byte)
            While i < btNumbers.Length
                If btNumbers(i) <> 0 Then
                    btTune.Add(btNumbers(i))
                    i = i + 1
                Else
                    i = i + 1
                    NewHymn.Tune = System.Text.Encoding.ASCII.GetString(btTune.ToArray)
                    Exit While
                End If
            End While

            ' Get scripture
            Dim btScripture As New List(Of Byte)
            While i < btNumbers.Length
                If btNumbers(i) <> 0 Then
                    btScripture.Add(btNumbers(i))
                    i = i + 1
                Else
                    i = i + 1
                    NewHymn.Scripture = System.Text.Encoding.ASCII.GetString(btScripture.ToArray)
                    Exit While
                End If
            End While

            ' Add Tune ID
            'Dim arrTunes() As HymnTune = GetTunes(path)
            'NewHymn.TuneID = GetTuneID(arrTunes, NewHymn.Tune)

            Hymns.Add(NewHymn)
        End While

        Return Hymns.ToArray
    End Function

    Public Class HymnTune
        Protected strTuneName As String
        Protected intID As Integer

        Property Name() As String
            Get
                Return strTuneName
            End Get
            Set(ByVal value As String)
                strTuneName = value
            End Set
        End Property

        Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
            End Set
        End Property
    End Class

    Public Function GetTunes(ByVal path As String) As HymnTune()

        Dim btNumbers() As Byte = My.Computer.FileSystem.ReadAllBytes(path & "\Organ.SBF")

        Dim Tunes As New List(Of HymnTune)

        ' pointer for where in the file we are
        Dim i As Integer = 0

        While i + 1 < btNumbers.Length

            Dim NewTune As New HymnTune

            ' Find beginning of tune name
            ' Look for 128
            Dim x As Integer
            x = i
            While x + 1 < btNumbers.Length
                If btNumbers(x) = 128 Then
                    i = x + 1
                    Exit While
                Else
                    x = x + 1
                End If
            End While

            ' Get Tune
            Dim btTune As New List(Of Byte)
            While i + 1 < btNumbers.Length
                If btNumbers(i) = 0 Then
                    i = i + 3
                    NewTune.Name = System.Text.Encoding.ASCII.GetString(btTune.ToArray)
                    Exit While
                Else
                    btTune.Add(btNumbers(i))
                    i = i + 1
                End If
            End While

            ' Get Tune ID for retrieving midi
            Dim btID(1) As Byte
            btID(0) = btNumbers(i)
            btID(1) = btNumbers(i + 1)

            ' Fix stupid numbers
            Dim tuneID As Integer = BitConverter.ToInt16(btID, 0)
            If tuneID < 303 Then
                NewTune.ID = tuneID
            Else
                NewTune.ID = tuneID + 1
            End If

            i = i + 3

            Tunes.Add(NewTune)

        End While

        Return Tunes.ToArray

    End Function
End Module
