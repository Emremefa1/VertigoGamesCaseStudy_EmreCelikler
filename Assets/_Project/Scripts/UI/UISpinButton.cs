using UnityEngine;
using UnityEngine.UI;

namespace WheelGame.UI
{
    [RequireComponent(typeof(Button))]
    public class UISpinButton : MonoBehaviour
    {
        private Button btn;
        private void Awake()
        {
            btn = GetComponent<Button>();
        }
        public void Setup(System.Action onClick)
        {
            btn.onClick.RemoveAllListeners();
            if (onClick != null) btn.onClick.AddListener(() => onClick());
        }
    }
}
