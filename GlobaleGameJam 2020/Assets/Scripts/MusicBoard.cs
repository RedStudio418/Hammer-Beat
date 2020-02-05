using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBoard : MonoBehaviour
{
    [SerializeField, BoxGroup("Manager")] ScoreManager _score;
    [SerializeField, BoxGroup("Prefab Instance")] GameObject[] _prefabs;
    [SerializeField, BoxGroup("Game Conf")] float _movementDuration = 2f;
    [SerializeField, BoxGroup("Game Conf")] Transform _validationObject;

    List<InGameNote> _createdGameObjects;

    public float MovementDuration => _movementDuration;
    public float ValidationXPosition => _validationObject.transform.position.x;

    private void Awake()
    {
        _createdGameObjects = new List<InGameNote>();
    }

    public void CreateNote(Transform line)
    {
        var note = Instantiate(_prefabs.PickRandom(), line).GetComponent<InGameNote>().Initialize(this);
        note.OnDeadNote += (n) => { };
        note.OnValidate += (n) => { _score.ScoreUp(n); };
        note.OnDestroyedNote += (n) => { _createdGameObjects.Remove(n); };
        _createdGameObjects.Add(note);
    }

    public void NextUpdate()
    {
        _createdGameObjects.ForEach(i => i.UpdateByMusicBoard());
    }



}
