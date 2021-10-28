using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ru1t3rl.Planets
{
    [ExecuteAlways, ImageEffectAllowedInSceneView]
    public class AtmosphereManager : MonoBehaviour
    {
        public Transform sun;
        List<Atmosphere> atmospheres = new List<Atmosphere>();
        Dictionary<Atmosphere, PostProcessingEffect> effects = new Dictionary<Atmosphere, PostProcessingEffect>();

        Vector3 GetSunDirection(Transform sun, Transform gobj) => (gobj.position - sun.position).normalized;


        public void AddAtmosphere(Atmosphere atmos)
        {
            atmospheres.Add(atmos);
            CreateNewEffect(atmos);
        }

        void CreateNewEffect(Atmosphere atmos)
        {
            effects.Add(atmos, gameObject.AddComponent<PostProcessingEffect>());
            effects[atmos].atmos = atmos;
            effects[atmos].OnRender += UpdateEffect;
        }

        void UpdateEffect(PostProcessingEffect effect)
        {
            int i = 0;
            foreach (Atmosphere atmos in effects.Keys)
            {
                if (effect == effects[atmos])
                {
                    atmospheres[i].UpdateEffect();
                    effect.atmos = atmospheres[i];
                    break;
                }
                i++;
            }
        }

        public void RemoveAtmosphere(Atmosphere atmos)
        {
            // Remove the atmosphere reference
            atmospheres.Remove(atmos);

            // Destroy the effect
            try
            {
                DestroyImmediate(effects[atmos]);
                effects.Remove(atmos);
            }
            catch (KeyNotFoundException) { }
        }

        public void RemoveAtmosphereAt(int i)
        {
            // Remove the atmosphere reference
            atmospheres.RemoveAt(i);

            int j = 0;
            foreach (Atmosphere atmos in effects.Keys)
            {
                if (j == i)
                {
                    try
                    {
                        // Destroy the effect
                        effects[atmos].OnRender -= UpdateEffect;
                        DestroyImmediate(effects[atmos]);
                        effects.Remove(atmos);
                    }
                    catch (KeyNotFoundException) { }
                    break;
                }

                j++;
            }
        }
    }
}