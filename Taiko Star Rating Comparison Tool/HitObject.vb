﻿Public Class HitObject
    Implements IComparable(Of HitObject)

    Private Const OBJECT_TYPE_COUNT As Integer = 8

    Public Property ObjectPos As Long
    Private type As Integer '1 circle, 2 slider, 8 spinner
    Public Property IsKat As Boolean 'true red, false blue

    Public Property nextObject As HitObject = Nothing
    Public Property previousObject As HitObject = Nothing

    'for new
    Public Property Strain As Double = 1

    Public Property Valid As Boolean = True 'For calculating difficulty.

    Private streamBonus As Double = 0

    Private previousKatLength() As Integer = {0, 0}
    Private previousDonLength() As Integer = {0, 0}

    'for old
    Public Property OldStrain As Double = 1
    Private lastTypeSwitchEven As Integer = -1

    'for both
    Private timeElapsed As Double = 0
    Private OldSameTypeCount As Integer = 1
    Private SameTypeCount As Integer = 1

    Public Sub New(ByVal pos As Long)
        ObjectPos = pos
        'USE ONLY FOR SEARCHING POS WITH COMPARISON
    End Sub
    Public Property Pos As Long
        Get
            Return ObjectPos
        End Get
        Set(ByVal value As Long)
            If (value > 0) Then
                ObjectPos = value
            Else
                ObjectPos = 0
            End If
        End Set
    End Property

    Public Sub New(ByRef description As String)
        description = description.Substring(description.IndexOf(",") + 1)
        description = description.Substring(description.IndexOf(",") + 1)

        Dim temp As Integer = description.IndexOf(",")

        ObjectPos = Long.Parse(description.Substring(0, temp)) 'just in case? Some people map some pretty long things.
        description = description.Substring(temp + 1)

        temp = description.IndexOf(",")
        type = Integer.Parse(description.Substring(0, temp))
        description = description.Substring(temp + 1)

        temp = description.IndexOf(",")
        If (temp = -1) Then
            temp = description.Length
        End If
        Dim hitsound As Integer = Integer.Parse(description.Substring(0, temp))

        IsKat = False 'default to don

        temp = hitsound
        If (temp >= 8) Then 'clap
            IsKat = True
            temp -= 8
        End If
        If (temp >= 4) Then 'finish
            'finish = True
            temp -= 4
        End If
        If (temp >= 2) Then 'whistle
            IsKat = True
        End If

        If (type = 5 Or type = 6 Or type = 12) Then 'these are new combo
            type -= 4
            'newcombo = True
        End If
    End Sub

    Public Function isHit() As Boolean
        Return (type = 1)
    End Function

    'needed for sorting
    Public Function CompareTo(ByVal other As HitObject) As Integer Implements System.IComparable(Of HitObject).CompareTo
        If (IsNothing(other)) Then
            Return 1
        End If
        Return Me.ObjectPos - other.ObjectPos
    End Function

    Public Function calculateStrain(ByRef previous As HitObject, ByVal timerate As Double) As Boolean
        timeElapsed = (ObjectPos - previous.ObjectPos) / timerate

        Valid = True

        If (timeElapsed = 0) Then
            Return False
        End If
        If Not isHit() Then
            Return False
        End If
        If Not previous.isHit() Then 'this must be the first circle in the map
            Return True 'strain "successfully calculated" as 1, so make this the previous object
        End If

        'form a linkedlist of hitobjects for ease of exclusion later
        previousObject = previous
        previous.nextObject = Me

        'OLD STRAIN__________________________________________________________
        Dim oldaddition As Double = 1
        Dim oldadditionfactor As Double = 1
        Dim olddecay As Double = Math.Pow(decay_base, timeElapsed / 1000)

        If (timeElapsed < 1000) Then
            oldaddition += OldTypeChangeAddition(previous)
            oldaddition += OldRhythmChangeAddition(previous)
        End If

        If (timeElapsed < 50) Then
            oldadditionfactor = 0.4 + (0.6 * timeElapsed / 50.0)
        End If

        OldStrain = previous.OldStrain * olddecay + oldaddition * oldadditionfactor

        'NEW STRAIN______________________________________________________________________

        'Dim change As Double = (previous.timeElapsed / timeElapsed)
        Dim decay As Double
        Dim additionFactor As Double = 1

        'Speed
        'decay = Math.Min(1, SPEED_DECAY_ONE / (timeElapsed + SPEED_DECAY_TWO)) + SPEED_DECAY_OFFSET

        'decay = Math.Max(0, 0.99 - timeElapsed / 1000)

        decay = Math.Pow(decay_base, timeElapsed / 1000)

        Dim addition As Double = BASE_SPEED_VALUE

        Dim rhythmBonus = RhythmChangeAddition(previous)

        'color
        Dim colorAddition As Double = TypeChangeAddition(previous)

        If (timeElapsed > 1000) Then
            additionFactor = 0
        End If

        If (timeElapsed < 65) Then
            addition *= 0.6 + (0.4 * timeElapsed / 65.0) 'scale from 0.5 to 1
            'colorAddition *= 0.85 + (0.15 * timeElapsed / 65.0) 'slight devalue as speed increases
            rhythmBonus *= (timeElapsed / 65.0) 'extreme devalue as speed increases
        End If

        addition += Math.Sqrt(Math.Pow(colorAddition, 2) + Math.Pow(rhythmBonus, 2))

        Strain = previous.Strain * decay + (addition * additionFactor)

        'consistency
        'addition = (1 - previous.Consistency) * STAMINA_GROWTH
        'If (previous.timeElapsed > 0) Then
        '    decay = Math.Pow(STAMINA_DECAY_BASE, timeElapsed / (previous.timeElapsed * CONSISTENCY_SCALE))
        'Else
        '    decay = 0
        'End If

        'Consistency = (previous.Consistency * decay) + addition

        'technicality
        'rhythm
        'decay = TECHNICALITY_DECAY_BASE

        'If (timeElapsed > 1000) Then
        '    decay = 0
        'End If
        'ElseIf (timeElapsed > 100) Then
        '    decay *= 1 - (timeElapsed - 100) / 500
        'End If


        'Rhythm = previous.Rhythm * decay + addition

        'totalStrain
        'This formula prevents technicality from becoming exponentially more effective as speed increases.
        'Strain = Speed * Math.Min(0.5 - (3 / (Rhythm - 6)), TECHNICALITY_BONUS_CAP) * (Math.Pow(0.9, Speed) + 0.75)

        Return True
    End Function

    'New Functions
    Private Function TypeChangeAddition(ByRef previous As HitObject) As Double
        Dim returnVal As Double = 0
        Dim returnMultipler As Double = 1
        previousDonLength = previous.previousDonLength
        previousKatLength = previous.previousKatLength
        If (IsKat() Xor previous.IsKat()) Then 'color is different from previous
            returnVal = BASE_SWAP_BONUS - (SWAP_SCALE / (previous.SameTypeCount + 0.65))

            If (previous.IsKat()) Then 'previous is kat mono
                If (previous.SameTypeCount Mod 2 = previousDonLength(0) Mod 2) Then
                    returnMultipler *= SAME_POLARITY_LOSS
                End If

                If (previousKatLength(0) = previous.SameTypeCount) Then
                    returnMultipler *= close_repeat_loss
                End If
                If (previousKatLength(1) = previous.SameTypeCount) Then
                    returnMultipler *= late_repeat_loss
                End If

                previousKatLength(1) = previousKatLength(0)
                previousKatLength(0) = previous.SameTypeCount
            Else 'previous is don mono
                If (previous.SameTypeCount Mod 2 = previousKatLength(0) Mod 2) Then
                    returnMultipler *= SAME_POLARITY_LOSS
                End If

                If (previousDonLength(0) = previous.SameTypeCount) Then
                    returnMultipler *= close_repeat_loss
                End If
                If (previousDonLength(1) = previous.SameTypeCount) Then
                    returnMultipler *= late_repeat_loss
                End If

                previousDonLength(1) = previousDonLength(0)
                previousDonLength(0) = previous.SameTypeCount
            End If
        Else
            SameTypeCount = previous.SameTypeCount + 1
        End If
        Return Math.Min(COLOR_BONUS_CAP, returnVal) * returnMultipler
    End Function

    Private Function RhythmChangeAddition(ByRef previous As HitObject) As Double
        If previous.timeElapsed > 0 Then
            Dim change As Double = timeElapsed / previous.timeElapsed

            If (change < 0.48) Then 'sometimes gaps are slightly different due to position rounding
                Return SPEEDUP_BIG_BONUS
            ElseIf (change < 0.52) Then
                Return SPEEDUP_MEDIUM_BONUS
            ElseIf change <= 0.9 Then
                Return SPEEDUP_SMALL_BONUS
            ElseIf change < 0.95 Then
                Return SPEEDUP_TINY_BONUS
            ElseIf change > 1.95 Then
                Return SLOWDOWN_MEDIUM_BONUS
            ElseIf change > 1.15 Then
                Return SLOWDOWN_SMALL_BONUS
            ElseIf change > 1.05 Then
                Return SLOWDOWN_TINY_BONUS
            End If
        End If

        Return 0
    End Function

    'Old functions
    Private Function OldTypeChangeAddition(ByRef previous As HitObject) As Double
        ' If we don't have the same hit type, trigger a type change!
        If (previous.IsKat() Xor IsKat()) Then

            If (previous.OldSameTypeCount Mod 2 = 0) Then
                lastTypeSwitchEven = 1
            Else
                lastTypeSwitchEven = 0
            End If

            ' We only want a bonus if the parity of the type switch changes!
            Select Case (previous.lastTypeSwitchEven)
                Case 1 'previous swap was even, if this one is odd give bonus
                    If lastTypeSwitchEven = 0 Then Return type_change_bonus
                Case 0 'previous swap was odd, if this one is even give bonus
                    If lastTypeSwitchEven = 1 Then Return type_change_bonus
            End Select
        Else ' No type change? Increment counter And keep track of last type switch
            lastTypeSwitchEven = previous.lastTypeSwitchEven
            OldSameTypeCount = previous.OldSameTypeCount + 1
        End If

        Return 0
    End Function
    Private Function OldRhythmChangeAddition(ByRef previous As HitObject) As Double
        Dim timeElapsedRatio As Double = Math.Max(previous.timeElapsed / timeElapsed, timeElapsed / previous.timeElapsed)

        If (timeElapsedRatio >= 8) Then Return 0

        Dim difference As Double = Math.Log(timeElapsedRatio, rhythm_change_base) Mod 1.0

        If (isWithinChangeThreshold(difference)) Then Return rhythm_change_bonus

        Return 0
    End Function
    Private Function isWithinChangeThreshold(ByVal value As Double) As Boolean
        Return (value > rhythm_change_base_threshold) And (value < 1 - rhythm_change_base_threshold)
    End Function
End Class
