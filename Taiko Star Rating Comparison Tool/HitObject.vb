Public Class HitObject
    Implements IComparable(Of HitObject)

    Private Const OBJECT_TYPE_COUNT As Integer = 8

    Public Property ObjectPos As Long
    Private type As Integer '1 circle, 2 slider, 8 spinner
    Public Property IsKat As Boolean 'true red, false blue

    Public Property nextObject As HitObject = Nothing
    Public Property previousObject As HitObject = Nothing

    'for new
    Public Property Strain As Double = 0
    Public Property Speed As Double = 0
    Public Property Consistency As Double = 0
    Public Property Rhythm As Double = 0
    Public Property Color As Double = 0

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

        Dim decay As Double = Math.Min(1, SPEED_DECAY_ONE / (timeElapsed + SPEED_DECAY_TWO)) + SPEED_DECAY_OFFSET
        Dim addition As Double = BASE_SPEED_VALUE
        Dim colorAddition As Double = 0
        Dim additionFactor As Double = 1

        'color
        colorAddition = TypeChangeAddition(previous)
        Color = colorAddition

        If (timeElapsed > 1000) Then
            additionFactor = 0
        End If

        If (timeElapsed < 50) Then
            'additionFactor = SPEED_DESCALE * Math.Log10(-(timeElapsed - SPEED_DESCALE_OFFSET_1)) + SPEED_DESCALE_OFFSET_2
            additionFactor = 0.5 + (0.5 * timeElapsed / 50.0) 'scale from 0.5 to 1
        End If

        Speed = previous.Speed * decay + ((addition + colorAddition) * additionFactor)

        'consistency
        addition = (1 - previous.Consistency) * STAMINA_GROWTH
        If (previous.timeElapsed > 0) Then
            decay = Math.Pow(STAMINA_DECAY_BASE, timeElapsed / (previous.timeElapsed * CONSISTENCY_SCALE))
        Else
            decay = 0
        End If

        Consistency = (previous.Consistency * decay) + addition

        'technicality
        'rhythm
        decay = TECHNICALITY_DECAY_BASE
        If (timeElapsed > 600) Then
            decay = 0
        ElseIf (timeElapsed > 100) Then
            decay *= 1 - (timeElapsed - 100) / 500
        End If
        addition = 0

        addition += RhythmChangeAddition(previous)

        Rhythm = previous.Rhythm * decay + addition

        'stream bonus
        Dim change As Double = (previous.timeElapsed / timeElapsed)
        decay = change - 1
        decay = Math.Max(0, (decay * -decay) * STREAM_BONUS_DECAY_SCALE + STREAM_BONUS_DECAY_BASE)

        streamBonus = previous.streamBonus * decay

        If (change > 0.8 And change < 1.2) Then
            streamBonus += STREAM_BONUS
        End If

        'totalStrain
        'Speed *= Math.Min((1 - (3 / (Rhythm - 6))) - 0.5, TECHNICALITY_BONUS_CAP)
        Strain = (Speed + streamBonus) * Math.Min((1 - (2.5 / (Rhythm - 5))) - 0.5, TECHNICALITY_BONUS_CAP) * Consistency * STAMINA_SCALING_FACTOR

        Return True
    End Function

    'New Functions
    Private Function TypeChangeAddition(ByRef previous As HitObject) As Double
        Dim returnVal As Double = 0
        Dim returnMultipler As Double = 1
        previousDonLength = previous.previousDonLength
        previousKatLength = previous.previousKatLength
        If (IsKat() Xor previous.IsKat()) Then 'color is different from previous

            If (previous.IsKat()) Then 'previous is kat mono
                'returnVal = BONUS_SCALE * Math.Log(previous.SameTypeCount + SWAP_LOG_OFFSET, SWAP_LOG_BASE)
                returnVal = -(1 / (previous.SameTypeCount + 2)) + BASE_SWAP_BONUS

                'If (previous.SameTypeCount Mod 2 = 0 And previousKatLength(0) Mod 2 = 0) Then
                '    returnMultipler *= EVEN_LOSS
                'End If

                If (previous.SameTypeCount Mod 2 = previousKatLength(0) Mod 2) Then
                    returnMultipler *= EVEN_LOSS
                End If


                If (previousKatLength(0) = previous.SameTypeCount) Then
                    returnMultipler *= REPEAT_LOSS
                End If
                If (previousKatLength(1) = previous.SameTypeCount) Then
                    returnMultipler *= REPEAT_LOSS
                End If
            Else 'previous is don mono
                'returnVal = BONUS_SCALE * Math.Log(previous.SameTypeCount + SWAP_LOG_OFFSET, SWAP_LOG_BASE)
                returnVal = -(1 / (previous.SameTypeCount + 2)) + BASE_SWAP_BONUS

                'If (previous.SameTypeCount Mod 2 = 0 And previousDonLength(0) Mod 2 = 0) Then
                '    returnMultipler *= EVEN_LOSS
                'End If

                If (previous.SameTypeCount Mod 2 = previousDonLength(0) Mod 2) Then
                    returnMultipler *= EVEN_LOSS
                End If

                If (previousDonLength(0) = previous.SameTypeCount) Then
                    returnMultipler *= REPEAT_LOSS
                End If
                If (previousDonLength(1) = previous.SameTypeCount) Then
                    returnMultipler *= REPEAT_LOSS
                End If
            End If

            If (previous.IsKat()) Then 'if the object is a kat, the last chain was kay
                previousKatLength(1) = previousKatLength(0)
                previousKatLength(0) = previous.SameTypeCount
            Else 'otherwise don
                previousDonLength(1) = previousDonLength(0)
                previousDonLength(0) = previous.SameTypeCount
            End If
        Else
            SameTypeCount = previous.SameTypeCount + 1
        End If
        Return Math.Min(returnVal, MAX_SWAP_BONUS) * returnMultipler
    End Function
    'Private Function TypeChangeAdditionTest(ByRef previous As HitObject) As Double
    '    Dim returnVal As Double = 0
    '    previousDonLength = previous.previousDonLength
    '    previousKatLength = previous.previousKatLength
    '    If (IsKat() Xor previous.IsKat()) Then 'color is different from previous

    '        If (previous.IsKat()) Then 'previous is kat mono
    '            If (previousKatLength.Contains(previous.SameTypeCount)) Then
    '                returnVal = WEAK_BONUS_SCALE * Math.Log(previous.SameTypeCount + SWAP_LOG_OFFSET, SWAP_LOG_BASE)
    '            Else
    '                returnVal = BIG_BONUS_SCALE * Math.Log(previous.SameTypeCount + SWAP_LOG_OFFSET, SWAP_LOG_BASE)
    '            End If
    '        Else 'previous is don mono
    '            If (previousDonLength.Contains(previous.SameTypeCount)) Then
    '                returnVal = WEAK_BONUS_SCALE * Math.Log(previous.SameTypeCount + SWAP_LOG_OFFSET, SWAP_LOG_BASE)
    '            Else
    '                returnVal = BIG_BONUS_SCALE * Math.Log(previous.SameTypeCount + SWAP_LOG_OFFSET, SWAP_LOG_BASE)
    '            End If
    '        End If

    '        If (previous.IsKat()) Then 'if the object is a kat, the last chain was kay
    '            previousKatLength(1) = previousKatLength(0)
    '            previousKatLength(0) = previous.SameTypeCount
    '        Else 'otherwise don
    '            previousDonLength(1) = previousDonLength(0)
    '            previousDonLength(0) = previous.SameTypeCount
    '        End If
    '    Else
    '        SameTypeCount = previous.SameTypeCount + 1
    '    End If
    '    Return Math.Min(returnVal, MAX_SWAP_BONUS)
    'End Function
    Private Function RhythmChangeAddition(ByRef previous As HitObject) As Double
        If previous.timeElapsed > 0 Then
            If (previous.timeElapsed <> timeElapsed) Then
                Dim speedup As Double = (timeElapsed / previous.timeElapsed)  'if this is <1, it has gotten faster
                Dim slowdown As Double = (previous.timeElapsed / timeElapsed) 'if < 1, it got slower

                If (speedup < 0.98) Then 'sometimes gaps are slightly different due to position rounding
                    If (speedup > 0.9) Then
                        Return SPEEDUP_TINY_BONUS
                    ElseIf (speedup >= 0.52) Then
                        Return SPEEDUP_SMALL_BONUS
                    ElseIf (speedup >= 0.48) Then
                        Return SPEEDUP_MEDIUM_BONUS
                    Else
                        Return SPEEDUP_BIG_BONUS
                    End If
                ElseIf (slowdown < 0.98) Then
                    If (slowdown > 0.9) Then
                        Return SLOWDOWN_TINY_BONUS
                    ElseIf (slowdown >= 0.52) Then
                        Return SLOWDOWN_SMALL_BONUS
                    ElseIf (slowdown >= 0.48) Then
                        Return SLOWDOWN_MEDIUM_BONUS
                    Else
                        Return SLOWDOWN_BIG_BONUS
                    End If
                End If
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
