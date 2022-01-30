using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMOD.Studio;

namespace Runtime.Audio {
    public class SeasonAudioUpdater : MonoBehaviour {

        PARAMETER_DESCRIPTION seasonsDescription;
        PARAMETER_ID seasonsID;
        // Start is called before the first frame update
        void Start() {
            FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName("Seasons", out seasonsDescription);
            seasonsID = seasonsDescription.id;
            FMODUnity.RuntimeManager.StudioSystem.setParameterByID(seasonsID, ((int)World.instance.season)+1);

        }
        void OnSeasonChange() {

            if (World.instance.season == Season.Spring) {
                FMODUnity.RuntimeManager.StudioSystem.setParameterByID(seasonsID, 0);
            }
            FMODUnity.RuntimeManager.StudioSystem.setParameterByID(seasonsID, ((int)World.instance.season)+1);
            UnityEngine.Debug.Log(World.instance.season);
        }

    }
}