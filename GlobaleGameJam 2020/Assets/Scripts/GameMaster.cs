using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    static Coroutine _gameRoutine;
    [SerializeField, BoxGroup("Managers"), Required] MusicBoard _musicBoard;
    [SerializeField, BoxGroup("Managers"), Required] MusicProvider _musicProvider;
    [SerializeField, BoxGroup("Managers"), Required] MusicInputManager _musicInput;

    [SerializeField, BoxGroup("StartMenu"), Required] StartMenu _startMenu;

    [SerializeField, BoxGroup("EndMenu"), Required] Button _endButton;
    [SerializeField, BoxGroup("EndMenu"), Required] Transform _endCanvas;

    public UnityEvent OnSongStart;
    public UnityEvent OnSongEnd;


    private void Awake()
    {
        if (_gameRoutine != null) return;
        DontDestroyOnLoad(gameObject);
        _gameRoutine = StartCoroutine(GameMasterRoutine());
    }

    IEnumerator GameMasterRoutine()
    {
        _endCanvas.gameObject.SetActive(false);
        _startMenu.gameObject.SetActive(false);

        while (true)
        {
            // Show Menu
            _startMenu.gameObject.SetActive(true);
            yield return null;

            // Trigg Go To Song
            Partition musicToLaunch = null;
            void TriggPartition(Partition p) => musicToLaunch = p;
            _startMenu.OnLaunchPartition += TriggPartition;
            yield return new WaitWhile(() => musicToLaunch==null);
            _startMenu.OnLaunchPartition -= TriggPartition;

            // DisableMenu
            _startMenu.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);

            // Start Music
            _musicProvider.LaunchMusic(musicToLaunch);
            OnSongStart?.Invoke();

            // Prepare Ending
            bool endGame = false;
            void TriggEndGame() => endGame = true;
            _musicProvider.OnFinishMusic += TriggEndGame;

            // GameProcess
            while (!endGame)
            {
#if UNITY_EDITOR
                if(Input.GetKeyDown(KeyCode.Space)) break;
#endif

                var direction = _musicProvider.NextNote();
                if(direction != Partition.VirtualNote.None)
                {
                    _musicBoard.CreateNote(_musicInput.MusicLines.First(i=> i.Note == direction).Line);
                }

                _musicBoard.NextUpdate();
                _musicInput.InputPass();

                yield return null;
            }

            // Finish the song
            SpecialCountDown scd = new SpecialCountDown(4);
            while (!scd.isDone)
            {
                _musicBoard.NextUpdate();
                _musicInput.InputPass();
                yield return null;
            }
            
            OnSongEnd?.Invoke();

            // Show EndGame Menu & wait button
            _endCanvas.gameObject.SetActive(true);
            bool closeEndMenu = false;
            void TriggEndMenuButton() => closeEndMenu = true;
            _endButton.onClick.AddListener(TriggEndMenuButton);
            yield return new WaitWhile(() => !closeEndMenu && !Input.GetButtonDown("Fire_A"));
            _endButton.onClick.RemoveListener(TriggEndMenuButton);
            _endCanvas.gameObject.SetActive(false);

        }

    }


}
