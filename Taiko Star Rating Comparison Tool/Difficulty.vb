﻿Public Class Difficulty
    Private p_difficulty As String 'file name of difficulty

    Public Property DifficultyName As String 'returned by toString

    Public Property IsTaiko As Boolean

    Public Property ObjectCount As Integer = 0
    Public Property OldStarRating As Double = 0
    Public Property NewStarRating As Double = 0
    Public Property OldStarRatingDT As Double = 0
    Public Property NewStarRatingDT As Double = 0

    Public Sub New(ByVal fileName As String)
        p_difficulty = fileName
        IsTaiko = False
        DifficultyName = Nothing
    End Sub

    Public Function File() As String
        Return p_difficulty
    End Function

    Public Overrides Function ToString() As String
        Return DifficultyName
    End Function



    Public Sub CalcSR(ByRef hitobjects As List(Of HitObject))
        If (hitobjects.Count > 0) Then
            CalculateStrains(hitobjects, 1)
            OldStarRating = OldCalculateDifficulty(hitobjects, 1) * old_star_scaling_factor
            NewStarRating = CalculateDifficulty(hitobjects, 1) * STAR_SCALING_FACTOR
        End If
    End Sub
    Public Sub CalcDT(ByRef hitobjects As List(Of HitObject))
        If (hitobjects.Count > 0) Then
            CalculateStrains(hitobjects, 1.5)
            OldStarRatingDT = OldCalculateDifficulty(hitobjects, 1.5) * old_star_scaling_factor
            NewStarRatingDT = CalculateDifficulty(hitobjects, 1.5) * STAR_SCALING_FACTOR
        End If
    End Sub

    Private Sub CalculateStrains(ByRef hitobjects As List(Of HitObject), ByVal timerate As Double)
        Dim current, previous As HitObject
        previous = hitobjects(0)
        For x As Integer = 1 To hitobjects.Count - 1
            current = hitobjects(x)
            If (current.calculateStrain(previous, timerate)) Then
                previous = current
            End If 'if returned false, gap was 0; do not update previous object
        Next
    End Sub

    Private Function CalculateDifficulty(ByRef hitobjects As List(Of HitObject), ByVal timerate As Double) As Double
        Dim actualStrainStep As Double = STRAIN_GAP / timerate

        Dim highestStrains As List(Of Double) = New List(Of Double)
        Dim sortedObjects As List(Of HitObject) = New List(Of HitObject)(hitobjects)

        Try
            sortedObjects.Sort(Function(a As HitObject, b As HitObject) b.Strain - a.Strain)
        Catch ex As Exception

        End Try

        For Each hit As HitObject In sortedObjects 'should be sorted from highest strain to lowest strain
            If (hit.Valid) Then
                hit.Valid = False
                highestStrains.Add(hit.Strain)
                ExcludeNear(hit, actualStrainStep)
            End If
        Next

        'now build weighted sum from these highest points

        Dim Difficulty As Double = 0
        Dim weight As Double = 1.0
        highestStrains.Sort()
        highestStrains.Reverse()

        For Each strain As Double In highestStrains
            Difficulty += weight * strain
            weight *= DECAY_WEIGHT
        Next

        Return Difficulty
    End Function
    Private Sub ExcludeNear(ByRef hit As HitObject, ByVal gap As Double)
        Dim nextObj As HitObject = hit
        While (Not IsNothing(nextObj.previousObject))
            If (nextObj.previousObject.Pos > hit.Pos - gap) Then
                nextObj.previousObject.Valid = False
                nextObj = nextObj.previousObject
            Else
                nextObj.previousObject = Nothing
            End If
        End While
        nextObj = hit
        While (Not IsNothing(nextObj.nextObject))
            If (nextObj.nextObject.Pos < hit.Pos + gap) Then
                nextObj.nextObject.Valid = False
                nextObj = nextObj.nextObject
            Else
                nextObj.nextObject = Nothing
            End If
        End While
    End Sub
    Private Function OldCalculateDifficulty(ByRef hitobjects As List(Of HitObject), ByVal timerate As Double) As Double
        Dim actualStrainStep As Double = old_strain_step * timerate

        Dim highestStrains As List(Of Double) = New List(Of Double)
        Dim intervalEndTime As Double = actualStrainStep
        Dim maximumStrain As Double = 0 'keep track of highest strain in interval

        Dim previous As HitObject = Nothing

        For Each hit As HitObject In hitobjects
            While (hit.Pos > intervalEndTime)
                highestStrains.Add(maximumStrain)

                If IsNothing(previous) Then
                    maximumStrain = 0
                Else
                    Dim decay As Double = Math.Pow(decay_base, (intervalEndTime - previous.Pos) / 1000)
                    maximumStrain = previous.OldStrain * decay
                End If
                'increase interval end time until it is past current position
                intervalEndTime += actualStrainStep
            End While

            maximumStrain = Math.Max(hit.OldStrain, maximumStrain)

            previous = hit
        Next

        'now build weighted sum from these highest points

        Dim Difficulty As Double = 0
        Dim weight As Double = 1.0
        highestStrains.Sort()
        highestStrains.Reverse()

        For Each strain As Double In highestStrains
            Difficulty += weight * strain
            weight *= old_decay_weight
        Next

        Return Difficulty
    End Function
End Class
