using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Ru1t3rl.Planets.Atmos
{
    [ExecuteAlways, ImageEffectAllowedInSceneView]
    public class AtmosphereManager : MonoBehaviour
    {
        public Transform sun;
        List<Atmosphere> atmospheres = new List<Atmosphere>();
        Dictionary<Atmosphere, PostProcessingEffect> effects = new Dictionary<Atmosphere, PostProcessingEffect>();

        Vector3 GetSunDirection(Transform sun, Transform gobj) => (sun.position - gobj.position).normalized;


        public void AddAtmosphere(Atmosphere atmos)
        {
            if (!atmospheres.Contains(atmos))
            {
                atmospheres.Add(atmos);
                bool hasEffect = false;

                PostProcessingEffect[] pEffects = GetComponentsInChildren<PostProcessingEffect>();
                for (int i = 0; i < pEffects.Length; i++)
                {
                    if (pEffects[i].atmos == atmos)
                    {
                        hasEffect = true;
                        effects.Add(atmos, pEffects[i]);
                        break;
                    }
                }

                if (!hasEffect)
                    CreateNewEffect(atmos);
            }
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
                    atmospheres[i].UpdateSun(GetSunDirection(sun, atmospheres[i].transform));
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
            catch (KeyNotFoundException)
            {
                PostProcessingEffect[] pEffects = GetComponentsInChildren<PostProcessingEffect>();
                for (int i = 0; i < pEffects.Length; i++)
                {
                    if (pEffects[i].atmos == atmos)
                    {
                        DestroyImmediate(atmos);
                        break;
                    }
                }
            }
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