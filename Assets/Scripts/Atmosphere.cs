using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ru1t3rl.Planets
{
    [RequireComponent(typeof(PlanetGenerator))]
    [ExecuteInEditMode, ImageEffectAllowedInSceneView]
    public class Atmosphere : MonoBehaviour
    {
        public Shader atmosphereShader;
        public AtmosphereSettings settings;
        AtmosphereSettings previousASettings;

        Material _material;
        public Material material
        {
            private set
            {
                _material = value;
            }
            get
            {
                if (_material == null)
                {
                    InitMaterial();
                }

                return _material;
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
            _material = new Material(atmosphereShader ? atmosphereShader : Shader.Find("Custom/AtmospherePostFX"));
            settings.SetProperties(ref _material, shapeSettings.radius);
            _material.SetVector("planetCentre", transform.position);
            _material.name = $"{gameObject.name}_Atmosphere";
        }


        private void Awake()
        {
            Camera.main.transform.GetComponent<AtmosphereManager>()?.AddAtmosphere(this);
        }

        public void UpdateEffect()
        {
            if (atmosphereShader == null || settings == null)
                return;

            material.SetVector("planetCentre", transform.position);
            settings.SetProperties(ref _material, shapeSettings.radius);

            if (Camera.main != null)
                Camera.main.depthTextureMode = DepthTextureMode.DepthNormals | DepthTextureMode.Depth;

            previousASettings = settings;
        }

        void OnDestroy()
        {
            Camera.main.transform.GetComponent<AtmosphereManager>()?.RemoveAtmosphere(this);
        }
    }
}