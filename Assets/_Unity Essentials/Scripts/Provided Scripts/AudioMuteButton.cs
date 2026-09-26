using UnityEngine;
using UnityEngine.UI;

namespace _Unity_Essentials.Scripts.Provided_Scripts
{
    [RequireComponent(typeof(Button))]
    public class AudioMuteButton : MonoBehaviour
    {
        [Header("Audio")] [SerializeField] private AudioSource musicSource;

        [Header("UI")] [SerializeField] private Image icon;
        [SerializeField] private Sprite soundOnIcon;
        [SerializeField] private Sprite soundOffIcon;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(ToggleMute);
        }

        private void Start()
        {
            UpdateIcon();
        }

        private void ToggleMute()
        {
            if (musicSource == null)
            {
                Debug.LogWarning("Music Source не призначений.", this);
                return;
            }

            musicSource.mute = !musicSource.mute;
            UpdateIcon();
        }

        private void UpdateIcon()
        {
            if (musicSource == null || icon == null)
                return;

            icon.sprite = musicSource.mute
                ? soundOffIcon
                : soundOnIcon;
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(ToggleMute);
        }
    }
}