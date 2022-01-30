using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Runtime {
    public class ActionPreviewController : MonoBehaviour {
        [SerializeField] LineRenderer lr;
        [SerializeField] Transform lineTarget, lineOrigin, canvasPos;
        [SerializeField] Image image;
        private void Update() {
            lr.SetPositions(new Vector3[] { lineOrigin.position, lineTarget.position });
        }

      
        public void Init(CardTargetTuple tp) {
            image.sprite = tp.card.sprite;
            Vector3 pos = World.instance.GridToWorld(tp.target);
            transform.position = pos;
            lineTarget.position = pos;
            //canvasPos.position = pos;
        }

    }
}