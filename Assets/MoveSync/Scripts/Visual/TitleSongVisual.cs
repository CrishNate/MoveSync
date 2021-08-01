using UnityEngine;

namespace MoveSync
{
    public class TitleSongVisual : MonoBehaviour
    {
        [SerializeField] private TextMesh _songNameMesh;
        [SerializeField] private TextMesh _songAuthorMesh;
        [SerializeField] private Animator _animator;
        private static readonly int Show = Animator.StringToHash("Show");

        private void Start()
        {
            EventManager.instance.BindEvent(MoveSyncEvent.TitleAnimation, TitleAnimation);
            _songNameMesh.gameObject.SetActive(false);
            _songAuthorMesh.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (!EventManager.isShutDown)
                EventManager.instance.UnbindEvent(MoveSyncEvent.TitleAnimation, TitleAnimation);
        }

        void TitleAnimation(float time)
        {
            _songNameMesh.text = LevelDataManager.instance.levelInfo.songInfo.songName;
            _songAuthorMesh.text = LevelDataManager.instance.levelInfo.songInfo.songAuthor;
            _songNameMesh.gameObject.SetActive(true);
            _songAuthorMesh.gameObject.SetActive(true);
            _animator.SetTrigger(Show);
        }
    }
}