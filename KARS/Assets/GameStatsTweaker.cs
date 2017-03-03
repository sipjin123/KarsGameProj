using GameSparks.RT;
using UnityEngine;
using UnityEngine.UI;

public class GameStatsTweaker : MonoBehaviour {

    protected GameSparksRTUnity GetRTSession;
    public GameObject[] PlayerObjects;

    #region DEFAULT VALUES
    public static float DefaultMovement = 1;
    public static float DefaultRotation = 40;

    public static float DefaulttrailDistanceTotal = 5;
    public static float DefaulttrailDistanceChild = DefaulttrailDistanceTotal / 3;

    public static float DefaultStun = 5;
    public static float DefaultBlind = 5;
    public static float DefaultConfuse = 5;

    public static float DefaultMissleCooldown = 5;
    public static float DefaultShieldCooldown = 5;

    public static float DefaultNitroCooldown = 5;
    public static float DefaultNitroSpeed = 50;
    public static float DefaultNitroDuration = 5;

    public static float DefaultAccelerationSpeedMax = 10;
    public static float DefaultAccelerationTimerMax = 5;
    #endregion
    
    #region PLAYER PREF KEYS
    public static string PrefKey_Movement = "MovementKey";
    public static string PrefKey_Rotation = "RotationKey";

    public static string PrefKey_TrailTotal = "TrailTotalKey";
    public static string PrefKey_TrailCap = "TrailCapKey";

    public static string PrefKey_Stun = "StunKey";
    public static string PrefKey_Blind = "BlindKey";
    public static string PrefKey_Confuse = "ConfuseKey";

    public static string PrefKey_MissleCooldown = "MissleCooldownKey";
    public static string PrefKey_ShieldCooldown = "ShieldCooldownKey";

    public static string PrefKey_NitroCooldown = "NitroCooldownKey";
    public static string PrefKey_NitroSpeed = "NitroSpeedKey";
    public static string PrefKey_NitroDuration = "NitroDurationKey";

    public static string PrefKey_AccelerationSpeedMax = "AccelerationSpeedMaxKey";
    public static string PrefKey_AccelerationTimerMax = "AccelerationTimerMaxKey";
    #endregion

    #region TWEAKABLE VARIABLES

    //MOVEMENT
    public Text Text_MovementSpeed;
    public float MovementSpeed;

    public Text Text_rotationSpeed;
    public float rotationSpeed;

    //ACCELERATION
    public Text Text_accelerationSpeedMax;
    public float accelerationSpeedMax;

    public Text Text_accelerationTimerMax;
    public float accelerationTimerMax;

    //TRAIL
    public Text Text_trailDistanceTotal;
    public float trailDistanceTotal;
    

    //POWERUPS
    public Text Text_missleCooldown;
    public float missleCooldown;

    public Text Text_shieldCooldown;
    public float shieldCooldown;

    public Text Text_nitroCooldown;
    public float nitroCooldown;

    public Text Text_nitroSpeed;
    public float nitroSpeed;

    public Text Text_nitroDuration;
    public float nitroDuration;

    //DISABLES
    public Text Text_const_StunDuration;
    public float const_StunDuration;

    public Text Text_BlindDuration;
    public float BlindDuration;

    public Text Text_ConfuseDuration;
    public float ConfuseDuration;




    public float Base_Value_Speed = 10,
        Base_Value_Acceleration = 5,
        Base_Value_Rotation = 40,
        Base_Value_Trail = 5;

    public float Increment_Value_Speed = .25f,
        Increment_Value_Acceleration = -.1f,
        Increment_Value_Rotation = 2.5f,
        Increment_Value_Trail = .5f;

    public float Speed_Stat, Acceleration_Stat, Rotation_Stat, Trail_Stat;
    public Text Speed_Text, Acceleration_Text, Rotation_Text, Trail_Text;
    #endregion

    public virtual void Initer()
    {

        MovementSpeed = PlayerPrefs.GetFloat(PrefKey_Movement, DefaultMovement);
        rotationSpeed = PlayerPrefs.GetFloat(PrefKey_Rotation, DefaultRotation);

        trailDistanceTotal = PlayerPrefs.GetFloat(PrefKey_TrailTotal, DefaulttrailDistanceTotal);

        const_StunDuration = PlayerPrefs.GetFloat(PrefKey_Stun, DefaultStun);
        BlindDuration = PlayerPrefs.GetFloat(PrefKey_Blind, DefaultBlind);
        ConfuseDuration = PlayerPrefs.GetFloat(PrefKey_Confuse, DefaultConfuse);

        missleCooldown = PlayerPrefs.GetFloat(PrefKey_MissleCooldown, DefaultMissleCooldown);
        shieldCooldown = PlayerPrefs.GetFloat(PrefKey_ShieldCooldown, DefaultShieldCooldown);

        nitroCooldown = PlayerPrefs.GetFloat(PrefKey_NitroCooldown, DefaultNitroCooldown);
        nitroSpeed = PlayerPrefs.GetFloat(PrefKey_NitroSpeed, DefaultNitroSpeed);
        nitroDuration = PlayerPrefs.GetFloat(PrefKey_NitroDuration, DefaultNitroDuration);

        accelerationSpeedMax = PlayerPrefs.GetFloat(PrefKey_AccelerationSpeedMax, DefaultAccelerationSpeedMax);
        accelerationTimerMax = PlayerPrefs.GetFloat(PrefKey_AccelerationTimerMax, DefaultAccelerationTimerMax);

        Speed_Stat = 1;
        Acceleration_Stat = 1;
        Rotation_Stat = 1;
        Rotation_Stat = 1;

        IncrementValue = 1;
        DivisibleTrailValue = 10;

        Drag_Value = 1f;
        AngularDrag_Value = 10f;

        Mass_Value = 2;
        Force_Value = 50;
        Torque_Value = 2;
    }



    public  Text IncrementText;
    public float IncrementValue;
    public void Tweak_IncrementValue(float _val)
    {
        IncrementValue += _val;
    }

    public GameObject Walls;
    public void EnableDisalbeWalls()
    {
        Walls.SetActive(!Walls.activeInHierarchy);
    }


    public Text DivisibleTrailText;
    public float DivisibleTrailValue;
    public void Tweak_DivisibleTrailValue(float _val)
    {
        DivisibleTrailValue += _val;
        SendTrailData();
    }


    public void SendTrailData()
    {

        if (GameSparkPacketReceiver.Instance.PeerID == 1)
            PlayerObjects[0].GetComponent<Car_DataReceiver>().ReceiveTrailVAlue(trailDistanceTotal, DivisibleTrailValue);
        if (GameSparkPacketReceiver.Instance.PeerID == 2)
            PlayerObjects[1].GetComponent<Car_DataReceiver>().ReceiveTrailVAlue(trailDistanceTotal, DivisibleTrailValue);

        try
        {
            GetRTSession = GameSparkPacketReceiver.Instance.GetRTSession();
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, GameSparkPacketReceiver.Instance.PeerID);
                data.SetFloat(2, trailDistanceTotal);
                data.SetFloat(3, DivisibleTrailValue);
                GetRTSession.SendData(116, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }
        catch { }
    }
    //--------------------------------------------------------------------------------------
    #region TWEAK RAW STATS
    //=========================MOVEMENT
    public void TweakMoveSpeed(float _var)
    {
        MovementSpeed += _var;
    }
    public void TweakrotationSpeed(float _var)
    {
        rotationSpeed += _var;
    }

    public void Tweak_accelerationSpeedMax(float _var)
    {
        accelerationSpeedMax += _var;
    }
    public void Tweak_accelerationTimerMax(float _var)
    {
        accelerationTimerMax += _var;
    }
    //=========================TRAIL
    public void TweaktrailDistanceTotal(float _var)
    {
        trailDistanceTotal += _var;

    }

    #endregion
    //--------------------------------------------------------------------------------------
    #region DISABLES AND PWOERUPS
    //=========================POWERUPS
    public void Tweak_missleCooldown(float _var)
    {
        missleCooldown += _var;
    }

    public void Tweak_shieldCooldown(float _var)
    {
        shieldCooldown += _var;
    }

    public void Tweak_nitroCooldown(float _var)
    {
        nitroCooldown += _var;
    }

    public void Tweak_nitroSpeed(float _var)
    {
        nitroSpeed += _var;
    }

    public void Tweak_nitroDuration(float _var)
    {
        nitroDuration += _var;
    }

    //=========================DISABLES 
    public void Tweakconst_StunDuration(float _var)
    {
        const_StunDuration += _var;
    }

    public void Tweakconst_BlindDuration(float _var)
    {
        BlindDuration += _var;
    }

    public void Tweakconst_ConfuseDuration(float _var)
    {
        ConfuseDuration += _var;
    }
    #endregion
    //--------------------------------------------------------------------------------------
    #region STAT BASE
    public Text BaseText_Speed, BaseText_Acceleration, BaseText_Rotation, BaseText_Trail;
    public void Tweak_Base_Speed(float _val)
    {
        if (_val < 0)
            Base_Value_Speed -= IncrementValue;
        else
            Base_Value_Speed += IncrementValue;
    }
    public void Tweak_Base_Acceleration(float _val)
    {
        if (_val < 0)
            Base_Value_Acceleration -= IncrementValue;
        else
            Base_Value_Acceleration += IncrementValue;
    }
    public void Tweak_Base_Rotation(float _val)
    {
        if (_val < 0)
            Base_Value_Rotation -= IncrementValue;
        else
            Base_Value_Rotation += IncrementValue;
    }
    public void Tweak_Base_Trail(float _val)
    {
        if (_val < 0)
            Base_Value_Trail -= IncrementValue;
        else
            Base_Value_Trail += IncrementValue;
    }
    #endregion
    //--------------------------------------------------------------------------------------
    #region STAT INCREMENTS
    public Text IncrementText_Speed, IncrementText_Acceleration, IncrementText_Rotation, IncrementText_Trail;
    public void Tweak_Increment_Speed(float _val)
    {
        if (_val < 0)
            Increment_Value_Speed -= IncrementValue;
        else
            Increment_Value_Speed += IncrementValue;
    }
    public void Tweak_Increment_Acceleration(float _val)
    {
        if (_val < 0)
            Increment_Value_Acceleration -= IncrementValue;
        else
            Increment_Value_Acceleration += IncrementValue;
    }
    public void Tweak_Increment_Rotation(float _val)
    {
        if (_val < 0)
            Increment_Value_Rotation -= IncrementValue;
        else
            Increment_Value_Rotation += IncrementValue;
    }
    public void Tweak_Increment_Trail(float _val)
    {
        if (_val < 0)
            Increment_Value_Trail -= IncrementValue;
        else
            Increment_Value_Trail += IncrementValue;
    }
    #endregion
    //--------------------------------------------------------------------------------------
    #region PHYSICS
    public float Force_Value, Torque_Value , Drag_Value, AngularDrag_Value,Mass_Value;
    public Text Force_Text, Torque_Text, Drag_Text, AngularDrag_Text,Mass_Text;

    public void Tweak_Force(float _val)
    {
        if (_val < 0)
            Force_Value -= IncrementValue;
        else
            Force_Value += IncrementValue;
    }

    public void Tweak_Torque(float _val)
    {
        if (_val < 0)
            Torque_Value-= IncrementValue;
        else
            Torque_Value += IncrementValue;
    }
    public void Tweak_Drag(float _val)
    {
        if (_val < 0)
            Drag_Value -= IncrementValue;
        else
            Drag_Value += IncrementValue;
    }
    public void Tweak_AngularDrag(float _val)
    {
        if (_val < 0)
            AngularDrag_Value -= IncrementValue;
        else
            AngularDrag_Value += IncrementValue;
    }
    public void Tweak_Mass(float _val)
    {
        if (_val < 0)
            Mass_Value -= IncrementValue;
        else
            Mass_Value += IncrementValue;
    }
    #endregion
    //--------------------------------------------------------------------------------------
    #region DESIGNER PHYSICS

    public float turningRate = 5f;
    public float turningForce = 50;
    public float turningStraightDamping = 0.9f;

    public Text TextturningRate ;
    public Text TextturningForce ;
    public Text TextturningStraightDamping;

    public void Adjust_TurningRate(float _val)
    {
        if (_val < 0)
            turningRate -= IncrementValue;
        else
            turningRate += IncrementValue;
    }
    public void Adjust_turningForce(float _val)
    {
        if (_val < 0)
            turningForce -= IncrementValue;
        else
            turningForce += IncrementValue;
    }
    public void Adjust_turningStraightDamping(float _val)
    {
        if (_val < 0)
            turningStraightDamping -= IncrementValue;
        else
            turningStraightDamping += IncrementValue;
    }

    #endregion
}
