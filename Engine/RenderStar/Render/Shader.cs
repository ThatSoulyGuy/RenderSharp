using RenderStar.Core;
using RenderStar.ECS;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

using Buffer = SharpDX.Direct3D11.Buffer;

namespace RenderStar.Render
{
    public enum ShaderStage
    {
        Vertex,
        Pixel
    }

    public class Shader : Component, IManageable
    {
        public string Name { get; private set; } = "";
        public string LocalPath { get; private set; } = "";
        public string Domain { get; private set; } = "";

        public byte[] VertexShaderByteCode { get; private set; } = null!;
        public byte[] PixelShaderByteCode { get; private set; } = null!;

        public bool UsesConstantBuffer { get; set; }
        public bool UsesSampler { get; set; }

        private string VertexShaderPath => $"Assets/{Domain}/{LocalPath}Vertex.hlsl";
        private string PixelShaderPath => $"Assets/{Domain}/{LocalPath}Pixel.hlsl";

        private VertexShader VertexShader { get; set; } = null!;
        private PixelShader PixelShader { get; set; } = null!;
        private ShaderSignature InputSignature { get; set; } = null!;

        private Dictionary<string, Buffer> ConstantBuffers { get; } = [];
        private Dictionary<string, ShaderResourceView> ShaderResourceViews { get; } = [];
        private Dictionary<string, SamplerState> SamplerStates { get; } = [];

        public bool Generate()
        {
            try
            {
                CompilationResult vertexShaderByteCode = ShaderBytecode.CompileFromFile(VertexShaderPath, "Main", "vs_5_0", ShaderFlags.Debug);

                VertexShaderByteCode = vertexShaderByteCode.Bytecode;
                VertexShader = new(Renderer.Device, vertexShaderByteCode.Bytecode);
                InputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);

                CompilationResult pixelShaderByteCode = ShaderBytecode.CompileFromFile(PixelShaderPath, "Main", "ps_5_0", ShaderFlags.Debug);

                PixelShaderByteCode = pixelShaderByteCode.Bytecode;
                PixelShader = new(Renderer.Device, pixelShaderByteCode.Bytecode);

                return true;
            }
            catch (Exception exception)
            {
                Logger.ThrowError("Exception", $"Error loading shaders: {exception.Message}");
                return false;
            }
        }

        public void CreateConstantBuffer<T>(string name, int size = 0) where T : struct
        {
            size = size == 0 ? Utilities.SizeOf<T>() : size;
            int roundedSize = (size + 15) / 16 * 16;

            BufferDescription bufferDescription = new()
            {
                Usage = ResourceUsage.Default,
                SizeInBytes = roundedSize,
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            Buffer buffer = new(Renderer.Device, bufferDescription);
            ConstantBuffers[name] = buffer;
        }

        public void UpdateConstantBuffer<T>(string name, T data) where T : struct
        {
            if (ConstantBuffers.TryGetValue(name, out Buffer? buffer))
                Renderer.Context.UpdateSubresource(ref data, buffer);
        }

        public void SetConstantBuffer(string name, ShaderStage stage, int slot)
        {
            if (ConstantBuffers.TryGetValue(name, out Buffer? buffer))
            {
                if (stage == ShaderStage.Vertex)
                    Renderer.Context.VertexShader.SetConstantBuffer(slot, buffer);
                else if (stage == ShaderStage.Pixel)
                    Renderer.Context.PixelShader.SetConstantBuffer(slot, buffer);
            }
        }

        public void SetTexture(string name, ShaderResourceView texture, int slot)
        {
            ShaderResourceViews[name] = texture;
            Renderer.Context.PixelShader.SetShaderResource(slot, texture);
        }

        public void CreateSamplerState(string name, SamplerStateDescription samplerDescription)
        {
            SamplerState samplerState = new(Renderer.Device, samplerDescription);
            SamplerStates[name] = samplerState;
        }

        public void SetSamplerState(string name, int slot)
        {
            if (SamplerStates.TryGetValue(name, out SamplerState? samplerState))
                Renderer.Context.PixelShader.SetSampler(slot, samplerState);
        }

        public void Use()
        {
            Renderer.Context.VertexShader.Set(VertexShader);
            Renderer.Context.PixelShader.Set(PixelShader);
        }

        public override void CleanUp()
        {
            InputSignature?.Dispose();
            VertexShader?.Dispose();
            PixelShader?.Dispose();

            foreach (Buffer buffer in ConstantBuffers.Values)
                buffer.Dispose();
            foreach (SamplerState sampler in SamplerStates.Values)
                sampler.Dispose();
        }

        public static Shader Create(string name, string localPath, string domain = Settings.DefaultDomain)
        {
            Shader shader = new()
            {
                Name = name,
                LocalPath = localPath,
                Domain = domain
            };

            if (!shader.Generate())
                return null!;

            return shader;
        }
    }
}
