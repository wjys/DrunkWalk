using UnityEngine;
using UnityEditor;
 
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/GlitchEffect")]


public class glitchEffect : ImageEffectBase {

	public Texture2D displacementMap;

	//glitches and flickers
	float glitchup, glitchdown, flicker,
			glitchupTime = 0.00f, glitchdownTime = 0.00f, flickerTime = 0.9f, famount = 0.0f;
	
	public float intensity;
	
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		
		material.SetFloat("_Intensity", intensity);
		material.SetTexture("_DispTex", displacementMap);
		
		glitchup += Time.deltaTime * intensity;
		glitchdown += Time.deltaTime * intensity;
		flicker += Time.deltaTime * intensity;
		
		if(flicker > flickerTime){
			material.SetFloat("filterRadius", (famount += Random.Range(-0.1f, 0.1f))  * intensity);
			flicker = 0;
			flickerTime = Random.value;
		}
		
		if(glitchup > glitchupTime){
			if(Random.value < 0.1f * intensity)
				material.SetFloat("flip_up", Random.Range(-0.1f, 0.1f) * intensity);
			else
				material.SetFloat("flip_up", 0);
			
			glitchup = 0;
			glitchupTime = Random.value/2f;
		}
		
		if(glitchdown > glitchdownTime){
			if(Random.value < 0.1f * intensity)
				material.SetFloat("flip_down", 1-Random.Range(-0.1f, 0.1f) * intensity);
			else
				material.SetFloat("flip_down", 1);
			
			glitchdown = 0;
			glitchdownTime = Random.value/2f;
		}
		
		// if(Random.value < 0.05 * intensity){
		// 	material.SetFloat("displace", Random.value * intensity);
		// 	material.SetFloat("scale", 1-Random.value * intensity);
		// }else
		// 	material.SetFloat("displace", 0);
		
		Graphics.Blit (source, destination, material);
	}
}