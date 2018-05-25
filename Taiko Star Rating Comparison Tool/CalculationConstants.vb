Module CalculationConstants
    'new constants
    Public Const STAR_SCALING_FACTOR As Double = 0.66

    Public Const SPEED_DECAY_ONE As Double = 690
    Public Const SPEED_DECAY_TWO As Double = 700
    Public Const SPEED_SCALING_FACTOR As Double = 0.1
    'Public Const SPEED_DECAY_WEIGHT As Double = 0.91 'for final scoring

    Public Const STAMINA_GROWTH As Double = 0.1
    Public Const STAMINA_SCALING_FACTOR As Double = 1.5
    Public Const STAMINA_DECAY_BASE As Double = 0.85

    Public Const NORMAL_STRAIN_DECAY As Double = 0.99

    Public Const TECHNICALITY_DECAY_BASE As Double = 0.9
    Public Const TECHNICALITY_SCALING_FACTOR As Double = 0.2

    'These numbers mostly affect streams, but affect every map to some degree
    Public Const MAX_SWAP_BONUS_MULT As Double = 4

    Public Const BIG_SWAP_BONUS As Double = 0.4
    Public Const WEAK_SWAP_BONUS As Double = 0.15
    Public Const TINY_SWAP_BONUS As Double = 0.1

    Public Const SPEEDUP_TINY_BONUS As Double = 0.2 '.9
    Public Const SPEEDUP_SMALL_BONUS As Double = 1 'This number increases value of 1/6 and other weird rhythms.
    Public Const SPEEDUP_MEDIUM_BONUS As Double = 0.4 'speed doubling - this one affects pretty much every map other than very continuous long stream maps
    Public Const SPEEDUP_BIG_BONUS As Double = 0.7 'This number increases value of very extreme speed changes.

    Public Const SLOWDOWN_TINY_BONUS As Double = 0.2 '.9
    Public Const SLOWDOWN_SMALL_BONUS As Double = 0.4 'in between - this affects 1/6
    Public Const SLOWDOWN_MEDIUM_BONUS As Double = 0.2 'half speed - this affects every map
    Public Const SLOWDOWN_BIG_BONUS As Double = 0.1 'extreme slowdown


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
