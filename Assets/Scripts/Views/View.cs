
namespace TestProject.Gameplay.Views {

    public class View : UnityEngine.MonoBehaviour {

        public int poolId;
        public float health;
        
        public void SetPosition(UnityEngine.Vector3 position) {

            this.transform.position = position;

        }

    }

}