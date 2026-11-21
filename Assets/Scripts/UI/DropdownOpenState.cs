using UnityEngine;

namespace StudentSurvival.UI
{
    // UGUI Dropdown has no public IsExpanded property. Track its template/list
    // lifecycle through a small marker, without reflecting into private fields.
    public sealed class DropdownOpenState : MonoBehaviour
    {
        private int activeLists;
        public bool IsOpen { get { return activeLists > 0; } }
        public void Register(bool active) { activeLists = Mathf.Max(0, activeLists + (active ? 1 : -1)); }
    }
}
