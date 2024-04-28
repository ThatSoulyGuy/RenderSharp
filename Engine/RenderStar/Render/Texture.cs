using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Runtime.InteropServices;
using RenderStar.Core;
using RenderStar.ECS;

using Image = SixLabors.ImageSharp.Image;

namespace RenderStar.Render
{
    public enum TextureFilter
    {
        MinMagMipPoint = Filter.MinMagMipPoint,
        MinMagPointMipLinear = Filter.MinMagPointMipLinear,
        MinPointMagLinearMipPoint = Filter.MinPointMagLinearMipPoint,
        MinPointMagMipLinear = Filter.MinPointMagMipLinear,
        MinLinearMagMipPoint = Filter.MinLinearMagMipPoint,
        MinLinearMagPointMipLinear = Filter.MinLinearMagPointMipLinear,
        MinMagLinearMipPoint = Filter.MinMagLinearMipPoint,
        MinMagMipLinear = Filter.MinMagMipLinear,
        Anisotropic = Filter.Anisotropic,
    }
    
    public class Texture : Component, IManageable
    {
        public string Name { get; private set; } = "";

        public string LocalPath { get; private set; } = "";

        public string Domain { get; private set; } = "";

        private string Path => $"Assets/{Domain}/{LocalPath}";

        public ShaderResourceView TextureResourceView { get; private set; } = null!;

        public bool Generate()
        {
            try
            {
                using Image<Rgba32> image = Image.Load<Rgba32>(Path);

                byte[] pixels = new byte[image.Width * image.Height * 4];
                image.CopyPixelDataTo(pixels);

                GCHandle handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
                try
                {
                    nint dataPointer = handle.AddrOfPinnedObject();
                    DataRectangle dataRectangle = new(dataPointer, image.Width * 4);

                    Texture2DDescription textureDescription = new()
                    {
                        Width = image.Width,
                        Height = image.Height,
                        ArraySize = 1,
                        BindFlags = BindFlags.ShaderResource,
                        Usage = ResourceUsage.Default,
                        CpuAccessFlags = CpuAccessFlags.None,
                        Format = Format.R8G8B8A8_UNorm,
                        MipLevels = 1,
                        OptionFlags = ResourceOptionFlags.None,
                        SampleDescription = new(1, 0)
                    };

                    using Texture2D texture2D = new(Renderer.Device, textureDescription, dataRectangle);
                    TextureResourceView = new(Renderer.Device, texture2D);
                }
                finally
                {
                    handle.Free();
                }

                return true;
            }
            catch (Exception exception)
            {
                Logger.ThrowError("Exception", $"Error loading texture from file {Path}: {exception.Message}");
                return false;
            }
        }

        public override void CleanUp()
        {
            TextureResourceView?.Dispose();
        }

        public static Texture Create(string name, string localPath, string domain = Settings.DefaultDomain)
        {
            Texture texture = new()
            {
                Name = name,
                LocalPath = localPath,
                Domain = domain
            };

            if (!texture.Generate())
                return null!;

            return texture;
        }
    }
}
