using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Ru1t3rl.Planets.Atmos
{
    [RequireComponent(typeof(PlanetGenerator))]
    [ExecuteInEditMode, ImageEffectAllowedInSceneView]
    public class Atmosphere : MonoBehaviour
    {
        [SerializeField] GameObject backupCam;

        public Shader atmosphereShader;
        public AtmosphereSettings settings;
        AtmosphereSettings previousASettings;

        Material _Material;
        public Material material
        {
            private set
            {
                _Material = value;
            }
            get
            {
                if (_Material == null)
                {
                    InitMaterial();
                }

                return _Material;
            }
        }

        ShapeSettings _shapeSettings;
        ShapeSettings shapeSettings
        {
            get
            {
                if (_shapeSettings == null)
                    _shapeSettings = GetComponent<PlanetGenerator>().shapeSettings;
                return _shapeSettings;
            }
        }

        void InitMaterial()
        {
            _Material = new Material(atmosphereShader ? atmosphereShader : Shader.Find("Custom/AtmospherePostFX"));
            settings.SetProperties(ref _Material, shapeSettings.radius);
            _Material.SetVector("planetCentre", transform.position);
            _Material.name = $"{gameObject.name}_Atmosphere";
        }

        public void UpdateEffect()
        {
            if (atmosphereShader == null || settings == null)
                return;

            material.SetVector("planetCentre", transform.position);
            settings.SetProperties(ref _Material, shapeSettings.radius);

            if (Camera.main != null)
                Camera.main.depthTextureMode = DepthTextureMode.DepthNormals | DepthTextureMode.Depth;

            previousASettings = settings;
        }

        public void UpdateSun(Vector3 sunDirection)
        {
            material.SetVector("dirToSun", sunDirection);
        }

        async void OnEnable()
        {
            await Enable();
        }

        async Task Enable()
        {
            await Task.Delay(1);
            try
            {
                Camera.main.GetComponent<AtmosphereManager>()?.AddAtmosphere(this);
            }
            catch (System.NullReferenceException)
            {
                backupCam.GetComponent<AtmosphereManager>()?.AddAtmosphere(this);
            }
        }

        async void OnDisable()
        {
            await Disable();
        }

        async Task Disable()
        {
            await Task.Delay(1);
            try
            {
                Camera.main.GetComponent<AtmosphereManager>()?.RemoveAtmosphere(this);
            }
            catch (System.NullReferenceException) { backupCam.gameObject.GetComponent<AtmosphereManager>()?.RemoveAtmosphere(this); }
        }

    }
}