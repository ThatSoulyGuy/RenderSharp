using RenderStar.ECS;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Runtime.InteropServices;

using Buffer = SharpDX.Direct3D11.Buffer;

namespace RenderStar.Render
{
    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct Vertex
    {
        public Vector3 Position { get; set; }
        public Vector3 Color { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2 TextureCoordinates { get; set; }

        private Vertex(byte _) { }

        public static InputLayout GetInputLayout(byte[] shaderBytecode)
        {
            return new(Renderer.Device, ShaderSignature.GetInputSignature(shaderBytecode),
            [
                new("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
                new("COLOR", 0, Format.R32G32B32_Float, 12, 0, InputClassification.PerVertexData, 0),
                new("NORMAL", 0, Format.R32G32B32_Float, 24, 0, InputClassification.PerVertexData, 0),
                new("TEXTURECOORDINATES", 0, Format.R32G32_Float, 36, 0, InputClassification.PerVertexData, 0)
            ]);
        } 

        public static Vertex Create(Vector3 position, Vector2 textureCoordinates)
        {
            return new(0)
            {
                Position = position,
                Normal = new(0.0f, 0.0f, 0.0f),
                Color = new(1.0f, 1.0f, 1.0f),
                TextureCoordinates = textureCoordinates
            };
        }

        public static Vertex Create(Vector3 position, Vector3 normal, Vector2 textureCoordinates)
        {
            return new(0)
            {
                Position = position,
                Normal = normal,
                Color = new(1.0f, 1.0f, 1.0f),
                TextureCoordinates = textureCoordinates
            };
        }

        public static Vertex Create(Vector3 position, Vector3 color, Vector3 normal, Vector2 textureCoordinates)
        {
            return new(0)
            {
                Position = position,
                Normal = normal,
                Color = color,
                TextureCoordinates = textureCoordinates
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public struct DefaultBufferType
    {
        public Matrix ModelMatrix;
    }

    [RequireComponent(typeof(Shader))]
    public class Mesh : Component
    {
        public List<Vertex> Vertices { get; set; } = [];
        public List<uint> Indices { get; set; } = [];

        private Buffer VertexBuffer { get; set; } = null!;
        private VertexBufferBinding VertexBufferBinding { get; set; }

        private Buffer IndexBuffer { get; set; } = null!;
        
        private InputLayout InputLayout { get; set; } = null!;

        private Dictionary<string, int> SamplerStates { get; } = [];
        private Dictionary<string, int> ConstantBuffers { get; } = [];

        public void GenerateSquare()
        {
            Vertices =
            [
                Vertex.Create(new(-0.5f, -0.5f, 0.0f), new(0.0f, 1.0f)),
                Vertex.Create(new(-0.5f,  0.5f, 0.0f), new(0.0f, 0.0f)),
                Vertex.Create(new( 0.5f,  0.5f, 0.0f), new(1.0f, 0.0f)),
                Vertex.Create(new( 0.5f, -0.5f, 0.0f), new(1.0f, 1.0f))
            ];

            Indices =
            [
                0, 1, 2,
                0, 2, 3
            ];
        }

        public void Generate(TextureFilter textureFilter)
        {
            VertexBuffer?.Dispose();
            IndexBuffer?.Dispose();

            VertexBuffer = Buffer.Create(Renderer.Device, BindFlags.VertexBuffer, Vertices.ToArray());
            VertexBufferBinding = new(VertexBuffer, Utilities.SizeOf<Vertex>(), 0);

            IndexBuffer = Buffer.Create(Renderer.Device, BindFlags.IndexBuffer, Indices.ToArray());

            InputLayout = Vertex.GetInputLayout(GameObject.GetComponent<Shader>().VertexShaderByteCode);

            if (GameObject.HasComponent<Texture>())
            {
                if (!SamplerStates.ContainsKey("samplerState"))
                {
                    SamplerStateDescription samplerDescription = new()
                    {
                        Filter = (Filter)textureFilter,
                        AddressU = TextureAddressMode.Wrap,
                        AddressV = TextureAddressMode.Wrap,
                        AddressW = TextureAddressMode.Wrap,
                        ComparisonFunction = Comparison.Never,
                        MinimumLod = 0,
                        MaximumLod = float.MaxValue
                    };

                    AddSampler("samplerState", 0, samplerDescription);
                }
            }

            if (!ConstantBuffers.ContainsKey("MatrixBuffer"))
                AddConstantBuffer<DefaultBufferType>("MatrixBuffer", 0, Utilities.SizeOf<DefaultBufferType>());
        }

        public virtual void AddSampler(string name, int slot, SamplerStateDescription samplerDescription)
        {
            GameObject.GetComponent<Shader>().CreateSamplerState(name, samplerDescription);

            SamplerStates[name] = slot;
        }

        public virtual void AddConstantBuffer<T>(string name, int slot, int size = 0) where T : struct
        {
            if (!ConstantBuffers.ContainsKey(name))
            {
                GameObject.GetComponent<Shader>().CreateConstantBuffer<T>(name, size);
                ConstantBuffers[name] = slot;
            }
        }

        public virtual void UpdateConstantBuffer<T>(string name, T data) where T : struct
        {
            GameObject.GetComponent<Shader>().UpdateConstantBuffer(name, data);
        }

        public override void Render()
        {
            GameObject.GetComponent<Shader>().Use();

            Renderer.Context.InputAssembler.InputLayout = InputLayout;

            Renderer.Context.InputAssembler.SetVertexBuffers(0, VertexBufferBinding);

            Renderer.Context.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);

            UpdateConstantBuffer<DefaultBufferType>("MatrixBuffer", new()
            {
                ModelMatrix = Matrix.Transpose(Transform.WorldMatrix)
            });

            foreach (string name in ConstantBuffers.Keys)
                GameObject.GetComponent<Shader>().SetConstantBuffer(name, ShaderStage.Vertex, ConstantBuffers[name]);

            if(GameObject.HasComponent<Texture>())
                GameObject.GetComponent<Shader>().SetTexture("diffuse", GameObject.GetComponent<Texture>().TextureResourceView, 0);

            foreach (string name in SamplerStates.Keys)
                GameObject.GetComponent<Shader>().SetSamplerState(name, SamplerStates[name]);

            Renderer.Context.DrawIndexed(Indices.Count, 0, 0);
        }

        public override void CleanUp()
        {
            VertexBuffer?.Dispose();
            IndexBuffer?.Dispose();
            InputLayout?.Dispose();
        }

        private Mesh() { }

        public static Mesh Create(List<Vertex> vertices, List<uint> indices)
        {
            return new()
            {
                Vertices = vertices,
                Indices = indices
            };
        }
    }
}
