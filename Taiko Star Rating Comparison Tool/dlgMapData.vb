Imports System.Windows.Forms
Imports System.Windows.Forms.DataVisualization.Charting

Public Class dlgMapData
    Public Overloads Function ShowDialog(ByVal mapTitle As String, ByVal maximum As Long, ByRef data() As Series) As DialogResult
        Me.Text = mapTitle
        chtConsistency.Series.Clear()

        For Each area As ChartArea In chtConsistency.ChartAreas
            area.AxisX.Maximum = maximum
            area.AxisX.Minimum = 0
            'area.AxisX.CustomLabels.Add(New CustomLabel(-0.5, 0.5, "0:00:00", 0, LabelMarkStyle.None))
            'area.AxisX.CustomLabels.Add(New CustomLabel(maximum - 1.5, maximum - 0.5, TimeConvert(maximum), 0, LabelMarkStyle.None))
        Next

        chtConsistency.ChartAreas.Item("Rhythm").AxisY.Maximum = 5

        data(0).ChartArea = "Speed"
        data(1).ChartArea = "Consistency"
        data(2).ChartArea = "Rhythm"
        data(3).ChartArea = "Final Strain"

        For Each s As Series In data
            chtConsistency.Series.Add(s)
        Next


        Return MyBase.ShowDialog()
    End Function

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Function TimeConvert(ByVal ms As Long) As String
        Dim returnString As String = ""

        'first hours
        returnString += (ms \ 3600000).ToString()
        returnString += ":"
        returnString += ((ms Mod 3600000) \ 60000).ToString()
        returnString += ":"
        returnString += ((ms Mod 60000) \ 1000).ToString()

        Return returnString
    End Function

    Private Sub chtConsistency_Click(sender As Object, e As EventArgs) Handles chtConsistency.Click
        If (chtConsistency.ChartAreas(0).Visible) Then
            chtConsistency.ChartAreas(0).Visible = False
            chtConsistency.ChartAreas(1).Visible = False
            chtConsistency.ChartAreas(2).Visible = True
            chtConsistency.ChartAreas(3).Visible = True
        Else
            chtConsistency.ChartAreas(0).Visible = True
            chtConsistency.ChartAreas(1).Visible = True
            chtConsistency.ChartAreas(2).Visible = False
            chtConsistency.ChartAreas(3).Visible = False
        End If
    End Sub
End Class
