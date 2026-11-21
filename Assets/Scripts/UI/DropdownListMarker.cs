using UnityEngine;

namespace StudentSurvival.UI
{
    public sealed class DropdownListMarker : MonoBehaviour
    {
        public DropdownOpenState Owner;
        private void OnEnable() { if (Owner != null) Owner.Register(true); }
        private void OnDisable() { if (Owner != null) Owner.Register(false); }
    }
}
