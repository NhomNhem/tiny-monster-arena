using UnityEngine;
using UnityEngine.UIElements;

namespace TinyMonsterArena.Presentation.UI {
    public class MainMenuView : MonoBehaviour{
        [SerializeField] private UIDocument _uiDocument;
        public VisualElement Root => _uiDocument.rootVisualElement;
    }
}