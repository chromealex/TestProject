using UnityEngine;
using views = TestProject.Gameplay.Views;

namespace TestProject.Database {

    [CreateAssetMenu()]
    public class BombsConfig : ScriptableObject {

        public views::BombView[] bombs;
        public float bombTime = 3f;

        public int GetCount() {

            return this.bombs.Length;

        }

        public views::BombView GetViewByIndex(int index) {

            return this.bombs[index];

        }

    }

}