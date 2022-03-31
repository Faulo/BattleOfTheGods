using UnityEngine;
using UnityEngine.UI;
namespace Runtime {
    public class ActionPreviewController : MonoBehaviour {
        [SerializeField] LineRenderer lr;
        [SerializeField] Transform lineTarget, lineOrigin, canvasPos;
        [SerializeField] Image image;
        Camera cam;
        void Awake() {
            cam = FindObjectOfType<Camera>();
        }
        void Update() {
            lr.SetPositions(new Vector3[] { lineOrigin.position, lineTarget.position });

            var lookPos = cam.transform.position;
            lookPos.y = transform.position.y;

            transform.LookAt(lookPos);
        }


        public void Init(CardTargetTuple tp) {
            image.sprite = tp.card.sprite;
            var pos = World.instance.GridToWorld(tp.target);
            transform.position = pos;
            lineTarget.position = pos;
            //canvasPos.position = pos;
        }

    }
}