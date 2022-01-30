#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Runtime;
using System;
using Runtime.Cards;

namespace Editor {
    public class WaveEditorWindow : EditorWindow {

        Wave wave;
        int currIndex;
        Vector2 scrollPos;
        [MenuItem("WaveEditing/Wave Editor")]
        public static void Open() {
            WaveEditorWindow w = (WaveEditorWindow)EditorWindow.GetWindow(typeof(WaveEditorWindow));
            w.currIndex = -1;
        }

        private void OnEnable() {
            SceneView.duringSceneGui += DrawPositions;

        }
        private void OnDisable() {
            SceneView.duringSceneGui -= DrawPositions;

        }

        private void DrawPositions(SceneView obj) {
            if (wave != null) {
                foreach (var e in wave.cardsWithTarget) {
                    if (e.card == default)
                        continue;
                    
                    Vector3 pos = World.instance.GridToWorld(e.target);
                    
                    Handles.DrawWireCube(pos, new Vector3(.5f, .5f, .5f));
                    GUIStyle style = new GUIStyle();
                    style.fixedWidth = 30;
                    style.fixedHeight = 30;
                    style.alignment = TextAnchor.MiddleCenter;
                    if (e.card.sprite != null)
                        Handles.Label(pos, new GUIContent(e.card.sprite.texture),style);
                }
            }
        }
        
        void OnGUI() {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            wave = EditorGUILayout.ObjectField(wave, typeof(Wave), false) as Wave;

            if (wave != default) {
                if (GUILayout.Button("Add")) {
                    wave.cardsWithTarget.Add(new CardTargetTuple());
                }
                EditorGUILayout.Separator();
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                for (int i = 0; i < wave.cardsWithTarget.Count; i++) 
                {
                    Rect curr = EditorGUILayout.GetControlRect();
                    if (i % 3 == 0) {
                        curr = EditorGUILayout.BeginHorizontal();
                    }

                    var e = wave.cardsWithTarget[i];

                    Rect imageTarget = EditorGUILayout.BeginVertical();
                    e.card = EditorGUILayout.ObjectField(e.card, typeof(CardData)) as CardData;

                    imageTarget = EditorGUILayout.GetControlRect();
                    imageTarget.width = 120;
                    imageTarget.height = 90;
                    if (e.card != null &&
                        e.card.sprite != null) {
                        //EditorGUI.DrawPreviewTexture(imageTarget, e.card.sprite.texture);
                        GUILayout.Box(
                            e.card.sprite.texture,
                            GUILayout.Width(imageTarget.width),
                            GUILayout.Height(imageTarget.height));

                    } else {
                        EditorGUILayout.LabelField("No card or card image");
                    }

                    e.target = EditorGUILayout.Vector3IntField("Position", e.target);
                    if (GUILayout.Button("Select")) {
                        currIndex = i;
                        SceneView.duringSceneGui += SetPos;
                    }
                    if (GUILayout.Button("Remove")) {
                        wave.cardsWithTarget.RemoveAt(i);
                    }
                    EditorGUILayout.EndVertical();

                    if (i % 3 == 0) {
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }

        private void SetPos(SceneView obj) {

            if (Event.current.clickCount > 0 &&
                currIndex >= 0 &&
                wave != default) {
                SceneView.duringSceneGui -= SetPos;
                Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                if (Physics.Raycast(r, out RaycastHit hit, LayerMask.GetMask("Floor"))) {
                    wave.cardsWithTarget[currIndex].target = World.instance.WorldToGrid(hit.point);
                }
                currIndex = -1;
            }


        }


     
    }
}
#endif