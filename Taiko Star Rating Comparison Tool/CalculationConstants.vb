Module CalculationConstants
    'new constants
    Public Const STAR_SCALING_FACTOR As Double = 0.04075

    'decay weight closer to one, make sure to lower strain gap to prevent too much of effect on shorter maps
    'the closer to 1 decay weight is, the less effect bursts have
    'After adjusting these, star scaling factor must also be adjusted appropriately
    Public Const DECAY_WEIGHT As Double = 0.9
    Public Const STRAIN_GAP As Double = 200.0 '200 on either side, so 400.0

    Public Const EFFECTIVE_OBJECT_DECAY_SCALE As Double = 2

    'base strain
    Public Const BASE_SPEED_VALUE As Double = 1.0

    'color
    Public Const BASE_SWAP_BONUS As Double = 1.5 '0.8
    Public Const SWAP_SCALE As Double = 1.75 '1
    Public Const COLOR_BONUS_CAP As Double = 1.25

    Public Const SAME_POLARITY_LOSS As Double = 0.8
    Public Const close_repeat_loss As Double = 0.525
    Public Const late_repeat_loss As Double = 0.75

    'rhythm
    Public Const SPEEDUP_TINY_BONUS As Double = 0.25 '.9
    Public Const SPEEDUP_SMALL_BONUS As Double = 1.0 'This number increases value of 1/4 -> 1/6 and other weird rhythms.
    Public Const SPEEDUP_MEDIUM_BONUS As Double = 0.5 'speed doubling - this one affects pretty much every map other than stream maps
    Public Const SPEEDUP_BIG_BONUS As Double = 0.65 'This number increases value of anything that more than doubles speed. Affects doubles.

    Public Const SLOWDOWN_TINY_BONUS As Double = 0.15 '.9, small speed changes
    Public Const SLOWDOWN_SMALL_BONUS As Double = 0.425 'in between - this affects (mostly) 1/6 -> 1/4
    Public Const SLOWDOWN_MEDIUM_BONUS As Double = 0.3 'half speed or more - this affects pretty much every map
    'Public Const SLOWDOWN_BIG_BONUS As Double = 0.3 'any slowdown that more than halves speed, affects doubles


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
