// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
/// Packed pixel type containing unsigned normalized values, ranging from 0 to 1, using 4 bits each for x, y, z, and w.
/// <para>
/// Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
/// </para>
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Bgra4444"/> struct.
/// </remarks>
/// <param name="vector">The vector containing the components for the packed vector.</param>
public partial struct Bgra4444(Vector4 vector) : IPixel<Bgra4444>, IPackedVector<ushort>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Bgra4444"/> struct.
    /// </summary>
    /// <param name="x">The x-component</param>
    /// <param name="y">The y-component</param>
    /// <param name="z">The z-component</param>
    /// <param name="w">The w-component</param>
    public Bgra4444(float x, float y, float z, float w)
        : this(new Vector4(x, y, z, w))
    {
    }

    /// <inheritdoc/>
    public ushort PackedValue { get; set; } = Pack(vector);

    /// <summary>
    /// Compares two <see cref="Bgra4444"/> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Bgra4444"/> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Bgra4444"/> on the right side of the operand.</param>
    /// <returns>
    /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Bgra4444 left, Bgra4444 right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="Bgra4444"/> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Bgra4444"/> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Bgra4444"/> on the right side of the operand.</param>
    /// <returns>
    /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Bgra4444 left, Bgra4444 right) => !left.Equals(right);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector4 ToScaledVector4() => this.ToVector4();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector4 ToVector4()
    {
        const float max = 1 / 15f;

        return new Vector4(
            (this.PackedValue >> 8) & 0x0F,
            (this.PackedValue >> 4) & 0x0F,
            this.PackedValue & 0x0F,
            (this.PackedValue >> 12) & 0x0F) * max;
    }

    /// <inheritdoc />
    public static PixelTypeInfo GetPixelTypeInfo()
        => PixelTypeInfo.Create<Bgra4444>(
            PixelComponentInfo.Create<Bgra4444>(4, 4, 4, 4, 4),
            PixelColorType.BGR | PixelColorType.Alpha,
            PixelAlphaRepresentation.Unassociated);

    /// <inheritdoc />
    public readonly PixelOperations<Bgra4444> CreatePixelOperations() => new PixelOperations();

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bgra4444 FromScaledVector4(Vector4 source) => FromVector4(source);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bgra4444 FromVector4(Vector4 source) => new() { PackedValue = Pack(source) };

    /// <inheritdoc />
    public override readonly bool Equals(object? obj) => obj is Bgra4444 other && this.Equals(other);

    /// <inheritdoc />
    public readonly bool Equals(Bgra4444 other) => this.PackedValue.Equals(other.PackedValue);

    /// <inheritdoc />
    public override readonly string ToString()
    {
        Vector4 vector = this.ToVector4();
        return FormattableString.Invariant($"Bgra4444({vector.Z:#0.##}, {vector.Y:#0.##}, {vector.X:#0.##}, {vector.W:#0.##})");
    }

    /// <inheritdoc />
    public override readonly int GetHashCode() => this.PackedValue.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ushort Pack(Vector4 vector)
    {
        vector = Numerics.Clamp(vector, Vector4.Zero, Vector4.One);
        return (ushort)((((int)Math.Round(vector.W * 15F) & 0x0F) << 12)
                      | (((int)Math.Round(vector.X * 15F) & 0x0F) << 8)
                      | (((int)Math.Round(vector.Y * 15F) & 0x0F) << 4)
                      | ((int)Math.Round(vector.Z * 15F) & 0x0F));
    }
}
