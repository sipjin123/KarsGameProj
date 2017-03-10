using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SendDataBuilder
{
    #region MOVEMENT
    private int _playerID;
    private Vector3 _position;
    private Vector3 _rotation;
    public Vector3 Position
    {
        get
        {
            return _position;
        }

        set
        {
            _position = value;
        }
    }
    public int PlayerID
    {
        get
        {
            return _playerID;
        }

        set
        {
            _playerID = value;
        }
    }
    public Vector3 Rotation
    {
        get
        {
            return _rotation;
        }

        set
        {
            _rotation = value;
        }
    }
    #endregion
    #region STATUS
    private bool _switch;
    private NetworkPlayerStatus _networkPlayerStatus;
    public bool Switch
    {
        get
        {
            return _switch;
        }

        set
        {
            _switch = value;
        }
    }
    public NetworkPlayerStatus NetworkPlayerStatus
    {
        get
        {
            return _networkPlayerStatus;
        }

        set
        {
            _networkPlayerStatus = value;
        }
    }
    #endregion

    public void SetMovement(int _id, Vector3 _pos, Vector3 _rot)
    {
        PlayerID = _id;
        Position = _pos;
        Rotation = _rot;
    }

    public void SetStatus(int _id, bool _switch, NetworkPlayerStatus _netStatus)
    {
        PlayerID = _id;
        this.Switch = _switch;
        NetworkPlayerStatus = _netStatus;
    }
}
