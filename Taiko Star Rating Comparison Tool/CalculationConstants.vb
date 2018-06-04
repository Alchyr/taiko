Module CalculationConstants
    'new constants
    Public Const STAR_SCALING_FACTOR As Double = 0.06

    'decay weight closer to one, make sure to lower strain gap to prevent too much of effect on shorter maps
    'the closer to 1 decay weight is, the less effect bursts have
    'After adjusting these, star scaling factor must also be adjusted appropriately
    Public Const DECAY_WEIGHT As Double = 0.9
    Public Const STRAIN_GAP As Double = 200.0 '200 on either side '400.0

    Public Const EFFECTIVE_OBJECT_DECAY_SCALE As Double = 2

    'base strain
    Public Const BASE_SPEED_VALUE As Double = 0.8
    Public Const SPEED_DECAY_ONE As Double = 700 '670
    Public Const SPEED_DECAY_TWO As Double = 700 '670 
    Public Const SPEED_DECAY_OFFSET As Double = 0 '0.025

    Public Const SPEED_DESCALE As Double = -0.125 'affects how much value very high bpm (<50 ms gap) notes have
    'must be adjusted along with SPEED_DESCALE to ensure value starts from 1 at 50 ms
    Public Const SPEED_DESCALE_OFFSET_1 As Double = 60
    Public Const SPEED_DESCALE_OFFSET_2 As Double = 1.125

    'These numbers mostly affect streams, but affect every map to some degree
    Public Const STREAM_BONUS As Double = 0.15
    Public Const STREAM_BONUS_DECAY_BASE As Double = 0.95
    Public Const STREAM_BONUS_DECAY_SCALE As Double = 4 'higher number means more strict on note consistency for stream bonus

    'color
    Public Const MAX_SWAP_BONUS As Double = 0.7 'cap on bonus from changing color
    Public Const BASE_SWAP_BONUS As Double = 0.6

    'Public Const SWAP_LOG_BASE As Double = 32
    'Public Const SWAP_LOG_OFFSET As Double = 0.75

    'Public Const BONUS_SCALE As Double = 1.2 '0.9
    'Public Const WEAK_BONUS_SCALE As Double = 0.9 '0.75

    Public Const REPEAT_LOSS As Double = 0.85
    Public Const EVEN_LOSS As Double = 0.8





    'consistency
    Public Const STAMINA_GROWTH As Double = 0.1
    Public Const CONSISTENCY_SCALE As Double = 2
    Public Const STAMINA_SCALING_FACTOR As Double = 1.5
    Public Const STAMINA_DECAY_BASE As Double = 0.85


    'technicality (rhythm)
    Public Const TECHNICALITY_DECAY_BASE As Double = 0.9
    Public Const TECHNICALITY_BONUS_CAP As Double = 1.5 'maximum multiplier
    'Public Const TECHNICALITY_SCALING_FACTOR As Double = 0.25

    Public Const SPEEDUP_TINY_BONUS As Double = 0.35 '.9
    Public Const SPEEDUP_SMALL_BONUS As Double = 0.8 'This number increases value of 1/6 and other weird rhythms.
    Public Const SPEEDUP_MEDIUM_BONUS As Double = 0.45 'speed doubling - this one affects pretty much every map other than stream maps
    Public Const SPEEDUP_BIG_BONUS As Double = 0.5 'This number increases value of very extreme speed changes. Affects doubles.

    Public Const SLOWDOWN_TINY_BONUS As Double = 0.35 '.9
    'Increasing this small slowdown bonus, which mainly affects 1/6 -> 1/4 transitions, increases value of sustained 1/6 but not really buffing short sudden bursts
    'at end of stream
    Public Const SLOWDOWN_SMALL_BONUS As Double = 0.4 'in between - this affects (mostly) 1/6
    Public Const SLOWDOWN_MEDIUM_BONUS As Double = 0.25 'half speed - this affects pretty much every map
    Public Const SLOWDOWN_BIG_BONUS As Double = 0.1 'extreme slowdown, affects doubles.


    'for old calculation
    Public Const old_star_scaling_factor As Double = 0.04125
    Public Const old_strain_step As Double = 400.0
    Public Const old_decay_weight As Double = 0.9

    Public Const decay_base As Double = 0.3

    Public Const type_change_bonus As Double = 0.75
    Public Const rhythm_change_bonus As Double = 1.0
    Public Const rhythm_change_base_threshold As Double = 0.2
    Public Const rhythm_change_base As Double = 2.0

End Module
