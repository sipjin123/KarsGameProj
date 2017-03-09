using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumManager : MonoBehaviour {


}


public static class OPCODE_CLASS
{
    public static int MeshOpcode { get { return 114; } }
    public static int HealthOpcode { get { return 118; } }

    public static int StatusOpcode { get { return 113; } }
    public static int MissleOpcode { get { return 115; } }

    public static int TrailOpcode { get { return 116; } }
    public static int MovementOpcode { get { return 111; } }

    public static int ResetOpcode { get { return 066; } }
    public static int ResultOpcode { get { return 067; } }
}

public enum MENUSTATE
{
    HOME,
    CHARACTER_SELECT,
    CHARACTER_STATS_VIEW,
    QUEST,
    SHOP,
    SOCIAL,

    MATCH_FOUND,
    RESTART_GAME,
    MATCH_FIND,
    START_GAME,
    RESULT,
    RETURN_TO_MAIN_MENU
}

public enum NetworkPlayerStatus
{
    NONE,
    ACTIVATE_SHIELD,
    ACTIVATE_TRAIL,
    ACTIVATE_NITRO,
    ACTIVATE_STUN,
    ACTIVATE_BLIND,
    ACTIVATE_CONFUSE,
    ACTIVATE_SLOW,
    ACTIVATE_SILENCE,
    ACTIVATE_GHOST,
    ACTIVATE_FLY,
    ACTIVATE_EXPAND,
    SET_READY,
    SET_START
}

public enum NetworkPlayerVariableList
{
    NONE,
    HEALTH,
    TRAIL,
}
