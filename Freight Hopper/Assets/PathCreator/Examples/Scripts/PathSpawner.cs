using UnityEngine;

namespace PathCreation.Examples {

    public class PathSpawner : MonoBehaviour {

        public PathCreator pathPrefab;
        public PathFollower followerPrefab;
        public Transform[] spawnPoints;

        private void Start () {
            foreach (Transform t in spawnPoints) {
                var path = Instantiate (pathPrefab, t.position, t.rotation);
                var follower = Instantiate (followerPrefab);
                follower.pathCreator = path;
            }
        }
    }

}