using GameSparks.RT;
using UnityEngine;
using UnityEngine.UI;

public class GameStatsTweaker : MonoBehaviour {

    protected GameSparksRTUnity GetRTSession;
    public GameObject[] PlayerObjects;

    public GameObject testPanelView;
    public void FlipTest_Panel()
    {
        testPanelView.SetActive(!testPanelView.activeInHierarchy);
    }

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



    //STATS
    public float Base_Value_Speed,
        Base_Value_Acceleration,
        Base_Value_Rotation,
        Base_Value_Trail;

    public float Increment_Value_Speed,
        Increment_Value_Acceleration,
        Increment_Value_Rotation,
        Increment_Value_Trail;

    public float Speed_Stat, Acceleration_Stat, Rotation_Stat, Trail_Stat;
    public Text Speed_Text, Acceleration_Text, Rotation_Text, Trail_Text;
    public Text BaseText_Speed, BaseText_Acceleration, BaseText_Rotation, BaseText_Trail;
    public Text IncrementText_Speed, IncrementText_Acceleration, IncrementText_Rotation, IncrementText_Trail;


    public Text DivisibleTrailText;
    public float DivisibleTrailValue;

    #endregion

    public void UpdateTexts()
    {
        Text_MovementSpeed.text = MovementSpeed.ToString("F2");
        Text_rotationSpeed.text = rotationSpeed.ToString("F2");

        Text_accelerationSpeedMax.text = accelerationSpeedMax.ToString("F2");
        Text_accelerationTimerMax.text = accelerationTimerMax.ToString("F2");

        Text_trailDistanceTotal.text = trailDistanceTotal.ToString("F2");

        Text_const_StunDuration.text = const_StunDuration.ToString("F2");
        Text_BlindDuration.text = BlindDuration.ToString("F2");
        Text_ConfuseDuration.text = ConfuseDuration.ToString("F2");

        Text_missleCooldown.text = missleCooldown.ToString("F2");
        Text_shieldCooldown.text = shieldCooldown.ToString("F2");
        Text_nitroCooldown.text = nitroCooldown.ToString("F2");
        Text_nitroSpeed.text = nitroSpeed.ToString("F2");
        Text_nitroDuration.text = nitroDuration.ToString("F2");


        IncrementText.text = IncrementValue.ToString("F2");
        DivisibleTrailText.text = DivisibleTrailValue.ToString("F2");

        BaseText_Speed.text = Base_Value_Speed.ToString("F2");
        BaseText_Acceleration.text = Base_Value_Acceleration.ToString("F2");
        BaseText_Rotation.text = Base_Value_Rotation.ToString("F2");
        BaseText_Trail.text = Base_Value_Trail.ToString("F2");

        IncrementText_Speed.text = Increment_Value_Speed.ToString("F2");
        IncrementText_Acceleration.text = Increment_Value_Acceleration.ToString("F2");
        IncrementText_Rotation.text = Increment_Value_Rotation.ToString("F2");
        IncrementText_Trail.text = Increment_Value_Trail.ToString("F2");


        Force_Text.text = Force_Value.ToString("F2");
        Torque_Text.text = Torque_Value.ToString("F2");
        Drag_Text.text = Drag_Value.ToString("F2");
        AngularDrag_Text.text = AngularDrag_Value.ToString("F2");
        Mass_Text.text = Mass_Value.ToString("F2");

        TextturningRate.text = turningRate.ToString("F2");
        TextturningForce.text = turningForce.ToString("F2");
        TextturningStraightDamping.text = turningStraightDamping.ToString("F2");

        // StatTextPanel.text = _statPanel.ToString();

        

        Speed_Text.text = Speed_Stat.ToString();
        Acceleration_Text.text = Acceleration_Stat.ToString();
        Rotation_Text.text = Rotation_Stat.ToString();
        Trail_Text.text = Trail_Stat.ToString();
    }

    public void ReturnStatsToDefault()
    {
        //Stats
        Speed_Stat = 1;
        Acceleration_Stat = 1;
        Rotation_Stat = 1;
        Rotation_Stat = 1;

        //Base STats
        Base_Value_Speed = 10;
        Base_Value_Acceleration = 5;
        Base_Value_Rotation = 40;
        Base_Value_Trail = 50;

        //IncrementStats
        Increment_Value_Speed = .25f;
        Increment_Value_Acceleration = -.1f;
        Increment_Value_Rotation = 2.5f;
        Increment_Value_Trail = 5f;

        //PHYSICS
        Drag_Value = 1f;
        AngularDrag_Value = 10f;

        Mass_Value = 2;
        Force_Value = 50;
        Torque_Value = 2;

        //RAW
        MovementSpeed = (DefaultMovement);
        rotationSpeed = (DefaultRotation);
        trailDistanceTotal = (DefaulttrailDistanceTotal);
        DivisibleTrailValue = 5;

        //POWERUPS
        const_StunDuration = (DefaultStun);
        BlindDuration = (DefaultBlind);
        ConfuseDuration = (DefaultConfuse);

        missleCooldown = (DefaultMissleCooldown);
        shieldCooldown = (DefaultShieldCooldown);

        nitroCooldown = (DefaultNitroCooldown);
        nitroSpeed = (DefaultNitroSpeed);
        nitroDuration = (DefaultNitroDuration);

        IncrementValue = 1;




        TweakMoveSpeed(0);
        TweakrotationSpeed(0);

        TweaktrailDistanceTotal(0);

        Tweakconst_StunDuration(0);
        Tweakconst_BlindDuration(0);
        Tweakconst_ConfuseDuration(0);

        Tweak_missleCooldown(0);
        Tweak_shieldCooldown(0);

        Tweak_nitroCooldown(0);
        Tweak_nitroSpeed(0);
        Tweak_nitroDuration(0);

        Tweak_accelerationSpeedMax(0);
        Tweak_accelerationTimerMax(0);

        Add_Stat_Speed(1);
        Add_Stat_Acceleration(1);
        Add_Stat_Rotation(1);
        Add_Stat_Trail(1);


        UpdateTexts();
    }
    public virtual void Initer()
    {
        ReturnStatsToDefault();
    }




    //DEBUGS
    //--------------------------------------------------------------------------------------
    #region STATS PANEL
    public enum STAT_PANEL
    {
        STATS,
        BASE_STATS,
        INC_STATS,
        RAW,
        BUFFS,
        FORCE
    }
    public STAT_PANEL _statPanel;
    public Text StatTextPanel;
    public void SelectPanel(int _panel)
    {
        _statPanel = (STAT_PANEL)_panel;


        Stat_Panel.SetActive(false);
        BaseStat_Panel.SetActive(false);
        IncSTat_Panel.SetActive(false);
        Force_Panel.SetActive(false);
        Buff_Panel.SetActive(false);
        Raw_Panel.SetActive(false);

        switch (_statPanel)
        {
            case STAT_PANEL.STATS:
                Stat_Panel.SetActive(true);
                break;
            case STAT_PANEL.BASE_STATS:
                BaseStat_Panel.SetActive(true);
                break;
            case STAT_PANEL.INC_STATS:
                IncSTat_Panel.SetActive(true);
                break;
            case STAT_PANEL.RAW:
                Raw_Panel.SetActive(true);
                break;
            case STAT_PANEL.BUFFS:
                Buff_Panel.SetActive(true);
                break;
            case STAT_PANEL.FORCE:
                Force_Panel.SetActive(true);
                break;
        }
    }

    public GameObject Stat_Panel, BaseStat_Panel, IncSTat_Panel, Force_Panel, Buff_Panel, Raw_Panel;

    public float ValueOfIncrement;

    public void SelectValueOfIncrement(float _val)
    {
        ValueOfIncrement = _val;
        Tweak_IncrementValue(ValueOfIncrement);
    }

    public Text IncrementText;
    public float IncrementValue;
    public void Tweak_IncrementValue(float _val)
    {
        IncrementValue = ValueOfIncrement;
    }

    public GameObject Walls;
    public void EnableDisalbeWalls()
    {
        Walls.SetActive(!Walls.activeInHierarchy);
    }
    #endregion
    //--------------------------------------------------------------------------------------
    #region LOGS PANEL
    public void ClearLogs()
    {
        UIManager.Instance.GameUpdateText.text = "";
    }
    public GameObject Logss;
    public void EnebleDisableLogs()
    {
        Logss.SetActive(!Logss.activeInHierarchy);
    }
    #endregion
    //--------------------------------------------------------------------------------------

    //TWEAKERS
    //--------------------------------------------------------------------------------------
    #region GAME STATS TWEAKER
    public void Add_Stat_Speed(int _stat)
    {
        if (_stat < 0)
            Speed_Stat -= IncrementValue;
        else
            Speed_Stat += IncrementValue;

        accelerationSpeedMax = Base_Value_Speed + ((Speed_Stat - 1) * Increment_Value_Speed);
    }
    public void Add_Stat_Acceleration(int _stat)
    {
        if (_stat < 0)
            Acceleration_Stat -= IncrementValue;
        else
            Acceleration_Stat += IncrementValue;

        accelerationTimerMax = Base_Value_Acceleration + ((Acceleration_Stat - 1) * Increment_Value_Acceleration);
    }
    public void Add_Stat_Rotation(int _stat)
    {
        if (_stat < 0)
            Rotation_Stat -= IncrementValue;
        else
            Rotation_Stat += IncrementValue;

        rotationSpeed = Base_Value_Rotation + ((Rotation_Stat - 1) * Increment_Value_Rotation);
    }
    public void Add_Stat_Trail(int _stat)
    {
        if (_stat < 0)
            Trail_Stat -= IncrementValue;
        else
            Trail_Stat += IncrementValue;

        trailDistanceTotal = Base_Value_Trail + ((Trail_Stat - 1) * Increment_Value_Trail);

        SendTrailData();
    }
    #endregion
    //--------------------------------------------------------------------------------------
    #region STAT BASE TWEAKER
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
    #region TWEAK DISABLES AND PWOERUPS
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
        SendTrailData();
    }

    public void Tweak_DivisibleTrailValue(float _val)
    {
        DivisibleTrailValue += _val;
        SendTrailData();
    }
    #endregion
    //--------------------------------------------------------------------------------------
    #region TWEAK PHYSICS
    public float Force_Value, Torque_Value, Drag_Value, AngularDrag_Value, Mass_Value;
    public Text Force_Text, Torque_Text, Drag_Text, AngularDrag_Text, Mass_Text;

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
            Torque_Value -= IncrementValue;
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
    #region TWEAK DESIGNER PHYSICS

    public float turningRate = 5f;
    public float turningForce = 50;
    public float turningStraightDamping = 0.9f;

    public Text TextturningRate;
    public Text TextturningForce;
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
    //--------------------------------------------------------------------------------------



    public void SendTrailData()
    {
        if (GameSparkPacketHandler.Instance.GetPeerID() == 0)
            return;
        PlayerObjects[GameSparkPacketHandler.Instance.GetPeerID() - 1].GetComponent<Car_DataReceiver>().ReceiveTrailVAlue(trailDistanceTotal);
        PlayerObjects[GameSparkPacketHandler.Instance.GetPeerID() - 1].GetComponent<Car_DataReceiver>().ReceiveTrailChildVAlue(DivisibleTrailValue);

        try
        {
            GetRTSession = GameSparkPacketHandler.Instance.GetRTSession();
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, GameSparkPacketHandler.Instance.GetPeerID());
                data.SetInt(2, (int)NetworkPlayerVariableList.TRAIL);
                data.SetFloat(3, trailDistanceTotal);
                GetRTSession.SendData(OPCODE_CLASS.HealthOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, GameSparkPacketHandler.Instance.GetPeerID());
                data.SetInt(2, (int)NetworkPlayerVariableList.CHILD_TRAIL);
                data.SetFloat(3, DivisibleTrailValue);
                GetRTSession.SendData(OPCODE_CLASS.HealthOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }
        catch { }
    }

}
