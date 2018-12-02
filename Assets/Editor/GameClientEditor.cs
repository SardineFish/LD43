using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [CustomEditor(typeof(GameClient))]
    [CanEditMultipleObjects]
    class GameClientEditor: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var client = target as GameClient;
            if (GUILayout.Button("Join"))
            {
                client.JoinGame(client.PlayerName);
            }
        }
    }
}
