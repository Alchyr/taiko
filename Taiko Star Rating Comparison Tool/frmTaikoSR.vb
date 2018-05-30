Public Class frmTaikoSR
    Private ReadOnly UserName As String = My.User.Name.Substring(My.User.Name.IndexOf("\") + 1)

    Private loadMaps As System.ComponentModel.BackgroundWorker

    Private loadState As Integer
    Private difficultyCount As Integer
    Private displayedCount As Integer
    Private calcDT As Boolean

    Private osuFolder As String

    Private data As List(Of Difficulty)

    Private mapFolders As ObjectModel.ReadOnlyCollection(Of String)

    Private Sub FormLoad(sender As Object, e As EventArgs) Handles MyBase.Load
        data = New List(Of Difficulty)
        loadState = 2
        SetupDataGridView()
        lblReport.Text = ""
        If My.Computer.FileSystem.DirectoryExists("C:\Users\" + UserName + "\AppData\Local\osu!") Then
            txtOsuFolder.Text = "C:\Users\" + UserName + "\AppData\Local\osu!"
        ElseIf My.Computer.FileSystem.DirectoryExists("C:\Program Files\osu!") Then
            txtOsuFolder.Text = "C:\Program Files\osu!"
        ElseIf My.Computer.FileSystem.DirectoryExists("C:\Program Files(x86)\osu!") Then
            txtOsuFolder.Text = "C:\Program Files(x86)\osu!"
        Else
            txtOsuFolder.Text = "osu! was not installed in a default location"
        End If

        TestOsuFolder()
    End Sub
    Private Sub SetupDataGridView()
        dgvSR.ColumnCount = 8
        With dgvSR.ColumnHeadersDefaultCellStyle
            .BackColor = Color.White
            .ForeColor = Color.Black
            .Font = New Font(dgvSR.Font, FontStyle.Bold)
        End With

        With dgvSR
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
            .CellBorderStyle = DataGridViewCellBorderStyle.Single
            .GridColor = Color.Black
            .RowHeadersVisible = False
            .EditMode = DataGridViewEditMode.EditProgrammatically

            .Columns(0).Name = "Map"
            .Columns(1).Name = "Object Count"
            .Columns(2).Name = "Old Star Rating"
            .Columns(3).Name = "New Star Rating"
            .Columns(4).Name = "Rating Difference"
            .Columns(5).Name = "Old Star Rating (DT)"
            .Columns(6).Name = "New Star Rating (DT)"
            .Columns(7).Name = "Rating Difference (DT)"
            .Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader

            .SelectionMode = DataGridViewSelectionMode.CellSelect
            .MultiSelect = False
        End With
    End Sub

    Private Sub TestOsuFolder()
        osuFolder = txtOsuFolder.Text

        If (osuFolder.EndsWith("\")) Then
            osuFolder = osuFolder.Substring(0, osuFolder.Length - 1)
        End If

        If My.Computer.FileSystem.DirectoryExists(osuFolder) Then
            If My.Computer.FileSystem.DirectoryExists(osuFolder + "\Songs") Then
                btnLoad.Enabled = True
            Else
                btnLoad.Enabled = False
            End If
        Else
            btnLoad.Enabled = False
        End If
    End Sub
    Private Sub txtOsuFolder_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtOsuFolder.TextChanged
        TestOsuFolder()
    End Sub

    Private Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        If (loadState = 2) Then
            dgvSR.Rows.Clear()
            data.Clear()

            barLoad.Value = 0
            loadState = 0
            calcDT = chkDT.Checked
            difficultyCount = 0
            displayedCount = 0
            Try
                'check map folder
                mapFolders = My.Computer.FileSystem.GetDirectories(osuFolder + "\Songs\")

                loadMaps = New ComponentModel.BackgroundWorker With {
                    .WorkerSupportsCancellation = False,
                    .WorkerReportsProgress = True
                }

                AddHandler loadMaps.DoWork, AddressOf MapTask
                AddHandler loadMaps.RunWorkerCompleted, AddressOf mapFinalize
                AddHandler loadMaps.ProgressChanged, AddressOf progressUpdate

                loadMaps.RunWorkerAsync()
            Catch ex As Exception
                MsgBox("Error loading beatmaps.", , "Error")
                Close()
            End Try
        Else
            MsgBox("Already loading maps!", , "Hey there what you doin")
        End If
    End Sub

    Private Sub progressUpdate(ByVal sender As Object, ByVal e As ComponentModel.ProgressChangedEventArgs)
        Try
            If (e.ProgressPercentage >= 0 And e.ProgressPercentage <= 100) Then
                barLoad.Value = e.ProgressPercentage
            End If
            Select Case loadState
                Case 0
                    lblReport.Text = mapFolders.Count.ToString() + " map folders found"
                    loadState = 1
                Case 1
                    lblReport.Text = difficultyCount.ToString() + " taiko maps found"
                    While (displayedCount < difficultyCount)
                        Dim diff As Difficulty = data(displayedCount)
                        'Dim newRow() As Object = {diff.ToString(), diff.ObjectCount, Math.Round(diff.OldStarRating, 2), Math.Round(diff.NewStarRating, 2), Math.Round(diff.NewStarRating - diff.OldStarRating, 2), Math.Round(diff.OldStarRatingDT, 2), Math.Round(diff.NewStarRatingDT, 2), Math.Round(diff.NewStarRatingDT - diff.OldStarRatingDT, 2)}
                        Dim newRow() As Object = {diff.ToString(), diff.ObjectCount, Math.Round(diff.OldStarRating, 2), Math.Round(diff.NewStarRating, 2), Math.Round(diff.NewStarRating - diff.OldStarRating, 2), Math.Round(diff.OldStarRatingDT, 2), Math.Round(diff.NewStarRatingDT, 2), Math.Round(diff.NewStarRatingDT - diff.OldStarRatingDT, 2)}
                        dgvSR.Rows.Add(newRow)
                        displayedCount += 1
                    End While
            End Select
        Catch ex As Exception
            'just to avoid errors
        End Try
    End Sub


    Private Sub MapTask()
        loadMaps.ReportProgress(0)
        Try
            Dim mapLoadSkip As Integer = 0
            For x As Integer = 0 To mapFolders.Count - 1
                ReadMapFolder(x, mapLoadSkip)
                loadMaps.ReportProgress((x * 100) / (mapFolders.Count - 1))
                x += mapLoadSkip
                mapLoadSkip = 0
            Next
        Catch ex As ConstraintException
            'done, but there was an error loading last map which caused it to go beyond max.
        End Try
    End Sub
    Private Sub ReadMapFolder(ByVal mapFolderIndex As Integer, ByRef mapLoadSkip As Integer)
        If (mapFolderIndex > mapFolders.Count - 1) Then
            'we have gone too far, and it is too late to turn back.
            Throw New ConstraintException
        End If
        Try
            Dim mapFiles As ObjectModel.ReadOnlyCollection(Of String) = My.Computer.FileSystem.GetFiles(mapFolders(mapFolderIndex), FileIO.SearchOption.SearchAllSubDirectories, {"*.osu"})

            If (mapFiles.Count > 0) Then
                'read the .osu files
                For Each file As String In mapFiles
                    Try
                        Dim diff As Difficulty = ReadDifficulty(file)
                        If (diff.IsTaiko()) Then
                            'add this difficulty to chart
                            data.Add(diff)
                            difficultyCount += 1 'one taiko map found
                        End If
                    Catch ex As Exception
                        'error reading that map
                    End Try
                Next
            Else
                'This folder doesn't even have any .osu files
                mapLoadSkip += 1
                ReadMapFolder(mapFolderIndex + 1, mapLoadSkip) 'attempt next beatmap
            End If

            'Dim fullFile As String = mapReader.ReadToEnd()
        Catch ex As Exception
            'MsgBox("Error opening song.", , "Error")
            mapLoadSkip += 1
            ReadMapFolder(mapFolderIndex + 1, mapLoadSkip) 'attempt next beatmap
        End Try
    End Sub

    Private Sub mapFinalize()
        barLoad.Value = 0
        dgvSR.Columns().Item(0).MinimumWidth = dgvSR.Columns().Item(0).Width
        dgvSR.Columns().Item(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        loadState = 2
    End Sub

    Private Function ReadDifficulty(ByVal file As String) As Difficulty
        ReadDifficulty = New Difficulty(file)

        ReadDifficulty.DifficultyName = file.Substring(file.LastIndexOf("\") + 1)
        ReadDifficulty.DifficultyName = ReadDifficulty.DifficultyName.Substring(0, ReadDifficulty.DifficultyName.LastIndexOf("."))

        Dim difficultyReader As New IO.StreamReader(file)

        Dim currentLine As String = difficultyReader.ReadLine()

        While Not (currentLine.StartsWith("Mode:") Or difficultyReader.EndOfStream)
            currentLine = difficultyReader.ReadLine()
        End While
        currentLine = currentLine.Substring(currentLine.IndexOf(":") + 1).Trim("     ".ToCharArray())
        Dim mode As Integer = Integer.Parse(currentLine) 'ms position
        If (mode = 1) Then 'if not taiko no need to read rest of file
            ReadDifficulty.isTaiko = True
            Dim hitobjects As List(Of HitObject) = ReadHitObjects(ReadDifficulty, difficultyReader)
            ReadDifficulty.CalcSR(hitobjects)
            If (calcDT) Then
                ReadDifficulty.CalcDT(hitobjects)
            End If
        End If

        difficultyReader.Close()
    End Function

    Private Function ReadHitObjects(ByRef diff As Difficulty, ByRef r As IO.StreamReader) As List(Of HitObject)
        Dim currentLine As String = r.ReadLine()
        Dim hitobjects As List(Of HitObject) = New List(Of HitObject)

        While Not (currentLine.Equals("[HitObjects]") Or r.EndOfStream)
            currentLine = r.ReadLine()
        End While

        'current line - [HitObjects]
        currentLine = r.ReadLine().Trim("     ".ToCharArray())
        While Not (r.EndOfStream Or currentLine.Length = 0)
            'hitobjects - x,y,ms,a,b,hitsound:hitsound:hitsound:hitsound:
            hitobjects.Add(New HitObject(currentLine))

            currentLine = r.ReadLine().Trim("     ".ToCharArray())
        End While
        If (currentLine.Length > 0) Then
            hitobjects.Add(New HitObject(currentLine))
        End If

        diff.ObjectCount = hitobjects.Count

        hitobjects.Sort()

        Return hitobjects
    End Function
    Private Function ReadHitObjects(ByRef diff As Difficulty) As List(Of HitObject)
        Dim r As IO.StreamReader = New IO.StreamReader(diff.File())
        Dim currentLine As String = r.ReadLine()
        Dim hitobjects As List(Of HitObject) = New List(Of HitObject)

        While Not (currentLine.Equals("[HitObjects]") Or r.EndOfStream)
            currentLine = r.ReadLine()
        End While

        'current line - [HitObjects]
        currentLine = r.ReadLine().Trim("     ".ToCharArray())
        While Not (r.EndOfStream Or currentLine.Length = 0)
            'hitobjects - x,y,ms,a,b,hitsound:hitsound:hitsound:hitsound:
            hitobjects.Add(New HitObject(currentLine))

            currentLine = r.ReadLine().Trim("     ".ToCharArray())
        End While
        If (currentLine.Length > 0) Then
            hitobjects.Add(New HitObject(currentLine))
        End If

        r.Close()

        diff.ObjectCount = hitobjects.Count

        hitobjects.Sort()

        Return hitobjects
    End Function

    Private Sub dgvSR_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvSR.CellDoubleClick
        If (e.RowIndex >= 0) Then
            If (data.Count > 0) Then
                ViewDetail(data.Find(Function(i) i.ToString().Equals(dgvSR.Rows.Item(e.RowIndex).Cells().Item(0).Value.ToString())))
            End If
        End If
    End Sub
    Private Sub ViewDetail(ByRef diff As Difficulty)
        'as hitobjects aren't saved because there are WAAAAAY too many of them and doing so would be inefficient, must recalculate
        'If (loadState = 2) Then
        Dim hitobjects As List(Of HitObject) = ReadHitObjects(diff)
            diff.CalcSR(hitobjects)

            Dim speed As DataVisualization.Charting.Series = New DataVisualization.Charting.Series("Speed") With {
                .ChartType = DataVisualization.Charting.SeriesChartType.Line,
                .XValueType = DataVisualization.Charting.ChartValueType.Double
            }
            Dim consistency As DataVisualization.Charting.Series = New DataVisualization.Charting.Series("Consistency") With {
                .ChartType = DataVisualization.Charting.SeriesChartType.Line,
                .XValueType = DataVisualization.Charting.ChartValueType.Double
            }
            Dim rhythm As DataVisualization.Charting.Series = New DataVisualization.Charting.Series("Rhythm") With {
                .ChartType = DataVisualization.Charting.SeriesChartType.Line,
                .XValueType = DataVisualization.Charting.ChartValueType.Double
            }
            Dim color As DataVisualization.Charting.Series = New DataVisualization.Charting.Series("Color") With {
                .ChartType = DataVisualization.Charting.SeriesChartType.Line,
                .XValueType = DataVisualization.Charting.ChartValueType.Double
            }

            speed.Points.AddXY(0, 0)
            consistency.Points.AddXY(0, 0)
            rhythm.Points.AddXY(0, 0)
            color.Points.AddXY(0, 0)
            For Each hit As HitObject In hitobjects
                speed.Points.AddXY(hit.ObjectPos, hit.Speed)
                consistency.Points.AddXY(hit.ObjectPos, hit.Consistency)
                rhythm.Points.AddXY(hit.ObjectPos, hit.Rhythm)
                color.Points.AddXY(hit.ObjectPos, hit.Color)
            Next


            dlgMapData.ShowDialog(diff.DifficultyName, hitobjects.Last.ObjectPos, {speed, consistency, rhythm, color})
        'Else
        'MsgBox("You can double-click to view map details once all maps have been loaded.", , "hold up there pardner")
        'End If
    End Sub
    Private Sub Search(ByVal searchFilter As String)
        Dim terms() As String = searchFilter.ToLower().Split(" ")
        Dim searchText As String = ""
        Dim add As Boolean

        dgvSR.Rows.Clear()

        For Each item As Difficulty In data
            add = True
            searchText = item.ToString().ToLower()
            For Each term As String In terms
                If Not (searchText.Contains(term)) Then
                    add = False
                End If
            Next
            If add Then
                Dim newRow() As Object = {item.ToString(), item.ObjectCount, Math.Round(item.OldStarRating, 2), Math.Round(item.NewStarRating, 2), Math.Round(item.NewStarRating - item.OldStarRating, 2), Math.Round(item.OldStarRatingDT, 2), Math.Round(item.NewStarRatingDT, 2), Math.Round(item.NewStarRatingDT - item.OldStarRatingDT, 2)}
                dgvSR.Rows.Add(newRow)
            End If
        Next
    End Sub
    Private Sub ResetSearch()
        If (dgvSR.Rows.Count < displayedCount) Then
            dgvSR.Rows.Clear()
            For Each item As Difficulty In data
                Dim newRow() As Object = {item.ToString(), item.ObjectCount, Math.Round(item.OldStarRating, 2), Math.Round(item.NewStarRating, 2), Math.Round(item.NewStarRating - item.OldStarRating, 2), Math.Round(item.OldStarRatingDT, 2), Math.Round(item.NewStarRatingDT, 2), Math.Round(item.NewStarRatingDT - item.OldStarRatingDT, 2)}
                dgvSR.Rows.Add(newRow)
            Next
        End If
    End Sub
    Private Sub txtSearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearch.KeyDown
        If (e.KeyValue = Keys.Return) Then
            If (loadState = 2) Then
                If (data.Count > 0) Then
                    If (txtSearch.Text.Length > 0) Then
                        Search(txtSearch.Text)
                    Else
                        ResetSearch()
                    End If
                Else
                    MsgBox("there's not even anything to search yet what are yo u doING", , "why tho")
                End If
            ElseIf (loadState = 1) Then
                MsgBox("Please wait until all maps are loaded before searching.", , "DO THE PATIENCE")
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtSearch_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtSearch.KeyPress
        If (e.KeyChar = vbCr) Then
            e.Handled = True
        End If
    End Sub
End Class
