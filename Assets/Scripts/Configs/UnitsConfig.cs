using UnityEngine;
using views = TestProject.Gameplay.Views;

namespace TestProject.Database {

    [CreateAssetMenu()]
    public class UnitsConfig : ScriptableObject {

        public views::SoldierView[] units;

        public int GetCount() {

            return this.units.Length;

        }

        public views::SoldierView GetViewByIndex(int index) {

            return this.units[index];

        }

    }

}