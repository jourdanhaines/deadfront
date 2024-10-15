using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Deadfront.Server
{
    public class Network : NetworkManager
    {
        [Header("MultiScene Setup")]
        public int instances = 3;

        [Scene]
        public string gameScene;

        private readonly Dictionary<NetworkConnectionToClient, Scene> _playerScenes = new();

        public static Network Singleton =>
            (Network)singleton;

        #region Scene Management

        public Scene GetPlayerScene(NetworkConnectionToClient conn)
        {
            return _playerScenes.TryGetValue(conn, out var scene) ? scene : SceneManager.GetActiveScene();
        }

        #endregion

        #region Server System Callbacks

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            StartCoroutine(OnServerAddPlayerDelayed(conn));
        }

        private IEnumerator OnServerAddPlayerDelayed(NetworkConnectionToClient conn)
        {
            conn.Send(new SceneMessage { sceneName = gameScene, sceneOperation = SceneOperation.LoadAdditive });

            yield return new WaitForEndOfFrame();

            var loadScene = SceneManager.LoadSceneAsync(gameScene,
                new LoadSceneParameters
                {
                    loadSceneMode = LoadSceneMode.Additive,
                    localPhysicsMode = LocalPhysicsMode.Physics3D
                });

            yield return loadScene;

            // This might cause incorrect scenes if players are loading at the same time
            var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            if (!scene.isLoaded) throw new Exception("Failed to load scene");

            _playerScenes[conn] = scene;

            base.OnServerAddPlayer(conn);

            SceneManager.MoveGameObjectToScene(conn.identity.gameObject, scene);
        }

        #endregion

        #region Start & Stop Callbacks

        public override void OnStopServer()
        {
            NetworkServer.SendToAll(
                new SceneMessage
                {
                    sceneName = gameScene,
                    sceneOperation = SceneOperation.UnloadAdditive
                });

            StartCoroutine(ServerUnloadPlayerScenes());
        }

        private IEnumerator ServerUnloadPlayerScenes()
        {
            var operations = _playerScenes.Values
                .Where(scene => scene.isLoaded)
                .Select(SceneManager.UnloadSceneAsync)
                .ToList();

            yield return new WaitUntil(() => operations.TrueForAll(op => op.isDone));
            yield return Resources.UnloadUnusedAssets();

            _playerScenes.Clear();
        }

        #endregion
    }
}