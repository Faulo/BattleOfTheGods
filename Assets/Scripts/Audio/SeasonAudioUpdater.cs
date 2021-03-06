using FMOD.Studio;
using UnityEngine;

namespace Runtime.Audio {
    public class SeasonAudioUpdater : MonoBehaviour {

        PARAMETER_DESCRIPTION seasonsDescription;
        PARAMETER_ID seasonsID;
        // Start is called before the first frame update
        void Start() {
            FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName("Seasons", out seasonsDescription);
            seasonsID = seasonsDescription.id;
            FMODUnity.RuntimeManager.StudioSystem.setParameterByID(seasonsID, ((int)World.instance.season) + 1);

        }
        protected void OnSeasonChange() {

            if (World.instance.season == Season.Spring) {
                FMODUnity.RuntimeManager.StudioSystem.setParameterByID(seasonsID, 0);
            }
            FMODUnity.RuntimeManager.StudioSystem.setParameterByID(seasonsID, ((int)World.instance.season) + 1);
            // Debug.Log(World.instance.season);
        }
    }
}