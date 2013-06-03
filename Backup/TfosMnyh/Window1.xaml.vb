Class Window1 

    Private Declare Function mciSendString Lib "winmm.dll" Alias "mciSendStringA" (ByVal lpstrCommand As String, ByVal lpstrReturnString As String, ByVal uReturnLength As Long, ByVal hwndCallback As Long) As Long

    Dim dsHymns As New System.Data.DataSet

    Private Sub lblConfigure_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles lblConfigure.MouseEnter
        Mouse.OverrideCursor = Cursors.Hand
    End Sub

    Private Sub lblConfigure_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles lblConfigure.MouseLeave
        Mouse.OverrideCursor = Cursors.Arrow
    End Sub


    Private Sub lblConfigure_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblConfigure.MouseLeftButtonDown
        Dim winCfg As New winConfigure
        Me.IsEnabled = False
        winCfg.ShowDialog()
        Me.IsEnabled = True
    End Sub


    Private Sub lblHelp_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles lblHelp.MouseEnter
        Mouse.OverrideCursor = Cursors.Hand
    End Sub

    Private Sub lblHelp_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles lblHelp.MouseLeave
        Mouse.OverrideCursor = Cursors.Arrow
    End Sub

    Private Sub lblHelp_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lblHelp.MouseLeftButtonDown
        MsgBox("TODO: Add Help Dialog")
    End Sub

    Private Sub Window1_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded

        ' Set up Hymns
        Dim gv As New GridView
        Dim table As New System.Data.DataTable
        Dim column As System.Data.DataColumn

        'dsHymns.ReadXml("test.xml")

        column = New System.Data.DataColumn
        column.DataType = System.Type.GetType("System.Int32")
        column.ColumnName = "Number"
        column.AutoIncrement = False
        column.Unique = True
        table.Columns.Add(column)

        column = New System.Data.DataColumn
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "Title"
        column.AutoIncrement = False
        column.Unique = False
        column.ReadOnly = False
        column.Caption = "Title"
        table.Columns.Add(column)

        column = New System.Data.DataColumn
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "Theme"
        column.AutoIncrement = False
        column.Unique = False
        column.ReadOnly = False
        column.Caption = "Theme"
        table.Columns.Add(column)

        column = New System.Data.DataColumn
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "Tune"
        column.AutoIncrement = False
        column.Unique = False
        column.ReadOnly = False
        column.Caption = "Tune"
        table.Columns.Add(column)

        column = New System.Data.DataColumn
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "Scripture"
        column.AutoIncrement = False
        column.Unique = False
        column.ReadOnly = False
        column.Caption = "Scripture"
        table.Columns.Add(column)

        dsHymns = New System.Data.DataSet
        dsHymns.Tables.Add(table)

        For Each item As System.Data.DataColumn In table.Columns
            Dim gvc As New GridViewColumn
            gvc.DisplayMemberBinding = New Binding(item.ColumnName)
            gvc.Header = item.ColumnName
            gvc.Width = [Double].NaN
            gv.Columns.Add(gvc)
        Next

        column = New System.Data.DataColumn
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "Verses"
        column.AutoIncrement = False
        column.Unique = False
        column.ReadOnly = False
        column.Caption = "Verses"
        table.Columns.Add(column)

        column = New System.Data.DataColumn
        column.DataType = System.Type.GetType("System.String")
        column.ColumnName = "TuneID"
        column.AutoIncrement = False
        column.Unique = False
        column.ReadOnly = False
        column.Caption = "TuneID"
        table.Columns.Add(column)

        lstvwHymns.View = gv

        lstvwHymns.DataContext = Table
        Dim Bind As New Binding
        lstvwHymns.SetBinding(ListView.ItemsSourceProperty, Bind)

        table.DefaultView.Sort = "Number"

        For Each x As Hymn In Utilities.LoadEverything(My.Settings.HSDataDir)
            'MsgBox(x.Number & " - " & x.Title)
            Dim row As System.Data.DataRow

            row = dsHymns.Tables(0).NewRow()
            row("Number") = x.Number
            row("Title") = x.Title
            row("Theme") = x.Theme
            row("Tune") = x.Tune
            row("Scripture") = x.Scripture
            row("Verses") = x.Verses
            row("TuneID") = x.TuneID

            ' This line has to be at end for some reason
            table.Rows.Add(row)

        Next
    End Sub

    Private Sub btnFilterHymns_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnFilterHymns.Click
        Dim table As New System.Data.DataTable
        table = lstvwHymns.DataContext
        table.DefaultView.Sort = "Number"

    End Sub

    Private Sub GridViewColumnHeaderClickedHandler(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim headerClicked As GridViewColumnHeader = TryCast(e.OriginalSource, GridViewColumnHeader)

        If headerClicked IsNot Nothing Then
            Dim table As New System.Data.DataTable
            table = lstvwHymns.DataContext
            table.DefaultView.Sort = headerClicked.Content
        End If
        

    End Sub

    Private Sub lstvwHymns_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lstvwHymns.SelectionChanged
        Dim row As System.Data.DataRowView

        row = lstvwHymns.SelectedItem
        txtVerses.Text = row.Item("Verses")
    End Sub

    Private Sub btnPlayPause_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnPlayPause.Click

        Dim currentRow As System.Data.DataRowView
        currentRow = lstvwHymns.SelectedItem

        If lstvwHymns.SelectedIndex = -1 Then
            Exit Sub
        End If

        Dim ret As Integer

        Dim strTuneID As String
        strTuneID = currentRow.Item("TuneID")

        Dim path As String = My.Settings.HSDataDir

        ' Decide whether to use the melody or four-part
        If rdoFourPart.IsChecked = True Then
            path = path & "\SRCHNDX\"
        Else
            path = path & "\FNDNDX\"
        End If

        Dim filename As String

        If My.Computer.FileSystem.FileExists(path & "WRD" & strTuneID & ".NDX") Then
            filename = "WRD" & strTuneID & ".NDX"
        Else
            filename = "WRD" & strTuneID & "A.NDX"
        End If

        My.Computer.FileSystem.CurrentDirectory = path
        ret = mciSendString("close wrd001", 0&, 0, 0)
        ret = mciSendString("open " & filename & " type sequencer alias wrd001", 0&, 0, 0)
        ret = mciSendString("play wrd001", 0&, 0, 0)

    End Sub

    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnStop.Click
        Dim ret As Integer
        ret = mciSendString("close wrd001", 0&, 0, 0)
    End Sub


End Class
