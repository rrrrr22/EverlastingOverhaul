using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria;
using System.Diagnostics;
using System;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using EverlastingOverhaul.Common.ItemOverhaul;


namespace EverlastingOverhaul.Common.Graphics;
public struct TrailShaderSettings {

	public string shaderType;
	public Color Color;
	public Vector2[] oldPos;
	public float[] oldRot;
	public Asset<Texture2D> image1;
	public Asset<Texture2D> image2;
	public Asset<Texture2D> image3;
	public Vector4 shaderData;
	public Vector2 offset;
}

public struct ShaderSettings {

	public Color Color;
	public Asset<Texture2D> image1;
	public Asset<Texture2D> image2;
	public Asset<Texture2D> image3;
	public Vector4 shaderData;
}
/// <summary>
/// Keep in mind that:
/// Spritebatch automatically Sets Main.Instance.GraphicsDevice.Textures[0] to the texture its currently drawing in the batch (when calling Draw() for immediate mode and End() for other modes),
/// and if you want to modify Main.Instance.GraphicsDevice.Textures, for things like vertex buffers, you would do it while spritebatch is not active (before Begin() or after End()),
/// </summary>
public class ModdedShaderHandler : ILoadable {
	static GraphicsDevice GraphicsDevice => Main.instance.GraphicsDevice;
	Asset<Effect> _effect;
	Color _color = new Color(0, 0, 0);
	Texture2D _texutre1 = null;
	Texture2D _texutre2 = null;
	Texture2D _texutre3 = null;
	Vector4 _shaderData = new Vector4(0, 0, 0, 0);
	bool usePasses = false;
	string currentPass = "";
	public bool enabled = false;
	bool repeat = true;
	Color? secondColor = null;
    public ModdedShaderHandler(Asset<Effect> effect) {

        _effect = effect;

	}
	public void setProperties(Color color, Texture2D texutre1 = null, Texture2D texutre2 = null, Texture2D texutre3 = null, Vector4 shaderData = default, bool usesPasses = false, bool repeat = true, Vector2? rect = null, Color? secondColor = null) {
        _color = color;
        _texutre1 = texutre1;
        _texutre2 = texutre2;
        _texutre3 = texutre3;
        _shaderData = shaderData;
		this.usePasses = usesPasses;
		this.repeat = repeat;
		this.secondColor = secondColor;
    }
	public void setProperties(ShaderSettings shaderSettings) {
        _color = shaderSettings.Color;
        _texutre1 = shaderSettings.image1?.Value;
        _texutre2 = shaderSettings.image2?.Value;
        _texutre3 = shaderSettings.image3?.Value;
        _shaderData = shaderSettings.shaderData;
	}
	/// <summary>
	/// call this before Begin() or after End() 
	/// </summary>
	public void setupTextures() 
	{
        if (_texutre1 != null) {
			GraphicsDevice.SamplerStates[1] = repeat ? SamplerState.LinearWrap : SamplerState.LinearClamp;
			GraphicsDevice.Textures[1] = _texutre1;
		}
		if (_texutre2 != null) {
			GraphicsDevice.SamplerStates[2] = repeat ? SamplerState.LinearWrap : SamplerState.LinearClamp;
			GraphicsDevice.Textures[2] = _texutre2;
		}
		if (_texutre3 != null) {
			GraphicsDevice.SamplerStates[3] = repeat ? SamplerState.LinearWrap : SamplerState.LinearClamp;
            GraphicsDevice.Textures[3] = _texutre3;
		}
	}
	public void apply() {
		var viewport = GraphicsDevice.Viewport;
		Effect effect = _effect.Value;
		setupTextures();
		effect.Parameters["viewWorldProjection"].SetValue(Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix * Matrix.CreateOrthographicOffCenter(left: 0, right: viewport.Width, bottom: viewport.Height, top: 0, zNearPlane: -1, zFarPlane: 10));
		effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
		effect.Parameters["color"].SetValue(_color.ToVector3());
		effect.Parameters["shaderData"].SetValue(_shaderData);
		//new optional stuff
		effect.Parameters["ScreenRes"]?.SetValue(Main.ScreenSize.ToVector2());
		effect.Parameters["Image1Size"]?.SetValue(_texutre1.Size());
		effect.Parameters["Image2Size"]?.SetValue(_texutre2.Size());
		effect.Parameters["Image3Size"]?.SetValue(_texutre3.Size());
		effect.Parameters["SecondColor"]?.SetValue(secondColor.HasValue ? secondColor.Value.ToVector3() : Color.White.ToVector3());
		effect.Parameters["CameraPositionMovement"]?.SetValue(Main.Camera.Center / Main.ScreenSize.ToVector2());
		if (usePasses) 
		{
			for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
				effect.CurrentTechnique.Passes[i].Apply();

		}
		else
		{
            if (currentPass == string.Empty)
                effect.CurrentTechnique.Passes[0].Apply();
            else
                effect.CurrentTechnique.Passes[currentPass].Apply();
        }
    }

    public void Load(Mod mod) {

	}

	public void Unload() {
		Main.RunOnMainThread(() => {

			_effect.Dispose();

		}).Wait();
	}
}
