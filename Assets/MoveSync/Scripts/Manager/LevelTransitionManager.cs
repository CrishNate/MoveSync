using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MoveSync
{
    public class LevelTransitionManager : MonoBehaviour
    {
        enum TransitionType
        {
            None,
            ToGame,
            ToMenu
        }
        
        [SerializeField] private UnityEvent onToTransition;
        [SerializeField] private UnityEvent onFromTransition;
        
        public static SongInfo songInfo;
        private static TransitionType _transitionType;
        
        public static readonly string LevelName = $"Level";
        public static readonly string MenuName = $"Menu";
        public static readonly string InitialName = $"InitialScene";

        public void StartGame(SongInfo inSongInfo)
        {
            if (_transitionType != TransitionType.None) 
                return;
            
            songInfo = inSongInfo;
            onToTransition.Invoke();
            _transitionType = TransitionType.ToGame;
        }

        public void ToMenu()
        {
            if (_transitionType != TransitionType.None) 
                return;
            
            onToTransition.Invoke();
            _transitionType = TransitionType.ToMenu;
        }
        
        public void TransitToLevel()
        {
            SceneManager.LoadSceneAsync(LevelName);
            SceneManager.UnloadSceneAsync(MenuName);
        }

        public void TransitToMenu()
        {
            SceneManager.LoadSceneAsync(MenuName);
            SceneManager.UnloadSceneAsync(LevelName);
        }
        
        void Start()
        {
            LevelSequencer.instance.onLevelFinished.AddListener(ToMenu);
            
            if (_transitionType != TransitionType.None)
            {
                onFromTransition.Invoke();
            }
            
            if (_transitionType == TransitionType.ToGame)
            {
                LevelDataManager.instance.onLoadedSong.AddListener(LevelSequencer.instance.StartSong);
                LevelDataManager.instance.LoadFile(songInfo.filePath);
            }
            
            _transitionType = TransitionType.None;
        }

        private void OnDestroy()
        {
            if (!LevelSequencer.isShutDown)
            {
                LevelSequencer.instance.onLevelFinished.RemoveListener(ToMenu);
            }
        }
    }
}