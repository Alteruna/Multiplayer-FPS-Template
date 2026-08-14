using System;
using System.Collections.Generic;
using System.Linq;
using Alteruna.Multiplayer.Core;
using Alteruna.Multiplayer.Core.MethodArguments;
using Alteruna.Multiplayer.Core.PacketProcessing.Reader;
using Alteruna.Multiplayer.Core.PacketProcessing.Writer;
using Alteruna.Multiplayer.Unity;
using Alteruna.Multiplayer.Unity.EventArgument;
using TMPro;
using UnityEngine;


public class ScoreBoard : Synchronizable
{
    private static ScoreBoard _instance;
    public static ScoreBoard Instance => _instance;


    [SerializeField]
    private ScoreSetup[] _scoreSetups;

    [SerializeField, Tooltip("The prefab to spawn as a new row in the score board")]
    private GameObject _scoreRowPrefab;
    [SerializeField, Tooltip("The parent to spawn new rows under")]
    private Transform _scoreRowParent;

    private List<ScoreBoardRow> _rows = new List<ScoreBoardRow>();
    private List<IScoreObject> _scoreObjects = new List<IScoreObject>();

    private List<KeyValuePair<int, ushort>> _serialationQueue = new List<KeyValuePair<int, ushort>>();

    private int _scoreIndexScore;
    private int _scoreIndexKills;
    private int _scoreIndexDeaths;

    private bool _forceSync;


    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;

        _scoreObjects.Capacity = _scoreSetups.Length;
        for (var i = 0; i < _scoreSetups.Length; i++)
        {
            _scoreObjects.Add(_scoreSetups[i].Type.TypeToScoreObject(_scoreSetups[i].Name));
            _scoreObjects[i].OnChanged += ScoreObjectChanged;
        }

        _scoreIndexScore = _scoreObjects.GetScoreID<int>("Score");
        _scoreIndexKills = _scoreObjects.GetScoreID<int>("Kills");
        _scoreIndexDeaths = _scoreObjects.GetScoreID<int>("Deaths");
    }

    private void Start()
    {
        Multiplayer.OnRoomJoined.AddListener(OnRoomJoined);
        Multiplayer.OnOtherUserJoined.AddListener(OtherUserJoined);
        Multiplayer.OnOtherUserLeft.AddListener(OtherUserLeft);

        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (_serialationQueue.Count > 0)
        {
            Multiplayer.Sync(this, Reliability.Reliable);
        }
    }

    private void OnRoomJoined(RoomJoinedEvent @event)
    {
        AddRow(@event.User.Index, @event.User.Name);
    }

    private void OtherUserJoined(OtherUserJoinedEvent @event)
    {
        AddRow(@event.User.Index,  @event.User.Name);
    }

    private void OtherUserLeft(OtherUserLeftEvent @event)
    {
        RemoveRow(@event.User.Index);
    }

    private void ScoreObjectChanged(int userID, IScoreObject scoreObj)
    {
        if (userID >= 0)
        {
            var row = _rows.FirstOrDefault(r => r.ID == userID);
            if (row == null)
            {
                Debug.LogError("ROW IS NULL! NO ROW WITH MATCHING USERID FOUND!");
                return;
            }

            row.UpdateScore(scoreObj);
        }
        else // userID == -1 means all values have been changed.
        {
            for (int i = 0; i < scoreObj.Size; i++)
            {
                var row = _rows.FirstOrDefault(r => r.ID == (ushort)i);
                if (row == null)
                    row = AddRow((ushort)i, "");
                row.UpdateScore(scoreObj);
            }
        }
        SortRows();
    }

    public ScoreBoardRow AddRow(ushort userID, string name)
    {
        if (_rows.FirstOrDefault(r => r.ID == userID) is ScoreBoardRow row)
        {
            row.gameObject.SetActive(true);
            bool resetStats = Multiplayer.GetUser(userID) == null;
            row.Initialize(userID, name, resetStats);
            return row; // return row if it already exists.
        }

        var newRow = Instantiate(_scoreRowPrefab, _scoreRowParent).GetComponent<ScoreBoardRow>();
        newRow.gameObject.SetActive(true);
        newRow.Initialize(userID, name);

        _scoreSetups[_scoreIndexScore].Texts.Add(newRow.scoreText);
        _scoreSetups[_scoreIndexKills].Texts.Add(newRow.killsText);
        _scoreSetups[_scoreIndexDeaths].Texts.Add(newRow.deathsText);

        _rows.Add(newRow);

        return newRow;
    }

    public void RemoveRow(ushort userID)
    {
        // Resets all values in _scoreObjects for userID;
        _scoreObjects.AddUser(userID);

        ScoreBoardRow row = _rows.FirstOrDefault(r => r.ID == userID);
        if (row != null)
            row.gameObject.SetActive(false);
    }

    public void SortRows()
    {
        var sortedRows = _rows.OrderByDescending(r => r.Score).ThenByDescending(r => r.Kills).ThenBy(r => r.Deaths).ToList();
        for (int i = 0; i < sortedRows.Count; i++)
        {
            sortedRows[i].transform.SetSiblingIndex(i);
        }
    }

    #region Update Stats

    public void AddScore(ushort userID, int amount)
    {
        _scoreObjects.AppendScore("Score", userID, amount);
        _serialationQueue.Add(new KeyValuePair<int, ushort>(_scoreIndexScore, userID));
    }

    public void AddKills(ushort userID, int amount = 1)
    {
        _scoreObjects.AppendScore("Kills", userID, amount);
        _serialationQueue.Add(new KeyValuePair<int, ushort>(_scoreIndexKills, userID));
    }

    public void AddDeaths(ushort userID, int amount = 1)
    {
        _scoreObjects.AppendScore("Deaths", userID, amount);
        _serialationQueue.Add(new KeyValuePair<int, ushort>(_scoreIndexDeaths, userID));
    }
    
    #endregion

	public override void Serialize(ITransportStreamWriter processor, SerializeInfo info)
	{
        _forceSync = info.ForceSync;
        base.Serialize(processor, info);
    }

	public override void AssembleData(Writer writer, SerializeInfo info)
	{
        if (_forceSync)
        {
            writer.Write(true);
            _scoreObjects.SerializeValues(writer);

            return;
        }

        writer.Write(false);

        int l = _serialationQueue.Count;
        writer.Write(l);

        for (int i = 0; i < l; i++)
        {
            writer.Write(_serialationQueue[i].Key);
            writer.Write(_serialationQueue[i].Value);
            _scoreObjects.SerializeValue(writer, _serialationQueue[i].Key, _serialationQueue[i].Value);
        }

        _serialationQueue.Clear();
    }

	public override void DisassembleData(Reader reader, UnserializeInfo info)
	{
        try
        {
            bool forceSync = reader.ReadBool();
            if (forceSync)
            {
                _scoreObjects.DeserializeValues(reader);

                foreach (var scoreObj in _scoreObjects)
                {
                    for (int i = 0; i < scoreObj.Size; i++)
                    {
                        var row = _rows.FirstOrDefault(r => r.ID == (ushort)i);
                        if (row == null)
                            row = AddRow((ushort)i, "");
                        row.UpdateScore(scoreObj);
                    }
                }

                return;
            }

            int l = reader.ReadInt();
            for (int i = 0; i < l; i++)
            {
                int scoreID = reader.ReadInt();
                ushort userID = reader.ReadUshort();
                _scoreObjects.DeserializeValue(reader, scoreID, userID);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Something went wrong with DisassembleData! Error: {e.Message}");
        }
    }


    [Serializable]
    public class ScoreSetup
    {
        public string Name;
        public ScoreType Type;
        [HideInInspector]
        public List<TMP_Text> Texts;
    }
}
