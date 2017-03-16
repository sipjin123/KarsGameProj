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
    
    public static int MovementOpcode { get { return 111; } }

    public static int MenuStateOpcode { get { return 066; } }

}

public enum MENUSTATE
{
    HOME,
    CHARACTER_SELECT,
    CHARACTER_STATS_VIEW,
    QUEST,
    SHOP,
    SOCIAL,

    CANCEL_MATCH_FIND,
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
    ACTIVATE_EXPLOSION,
    SET_READY,
    SET_START
}

public enum NetworkPlayerVariableList
{
    NONE,
    HEALTH,
    TRAIL,
    CHILD_TRAIL,
}

public enum TYPE_OF_DATA_TO_SEND
{
    MOVEMENT,
    STATUS,
    REDUCE_HP
}

public enum SKILL_LIST
{
    Shield,
    Stun,
    Blind,
    Confuse,
    Slow,
    Silence,
    Fly,
    Nitro,
    Expand
}

public enum MethodUsed
{
    LINEAR,
    CUBIC

}