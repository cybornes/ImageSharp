// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
/// Packed pixel type containing four 8-bit unsigned integer values, ranging from 0 to 255.
/// <para>
/// Ranges from [0, 0, 0, 0] to [255, 255, 255, 255] in vector form.
/// </para>
/// </summary>
public partial struct Byte4 : IPixel<Byte4>, IPackedVector<uint>
{
    /// <summary>
    /// The maximum byte value.
    /// </summary>
    private static readonly Vector4 MaxBytes = Vector128.Create(255f).AsVector4();

    /// <summary>
    /// Initializes a new instance of the <see cref="Byte4"/> struct.
    /// </summary>
    /// <param name="vector">
    /// A vector containing the initial values for the components of the Byte4 structure.
    /// </param>
    public Byte4(Vector4 vector) => this.PackedValue = Pack(vector);

    /// <summary>
    /// Initializes a new instance of the <see cref="Byte4"/> struct.
    /// </summary>
    /// <param name="x">The x-component</param>
    /// <param name="y">The y-component</param>
    /// <param name="z">The z-component</param>
    /// <param name="w">The w-component</param>
    public Byte4(float x, float y, float z, float w)
    {
        Vector4 vector = new(x, y, z, w);
        this.PackedValue = Pack(vector);
    }

    /// <inheritdoc/>
    public uint PackedValue { get; set; }

    /// <summary>
    /// Compares two <see cref="Byte4"/> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Byte4"/> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Byte4"/> on the right side of the operand.</param>
    /// <returns>
    /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Byte4 left, Byte4 right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="Byte4"/> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Byte4"/> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Byte4"/> on the right side of the operand.</param>
    /// <returns>
    /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Byte4 left, Byte4 right) => !left.Equals(right);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Rgba32 ToRgba32() => new() { PackedValue = this.PackedValue };

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector4 ToScaledVector4() => this.ToVector4() / 255f;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector4 ToVector4() => new(
            this.PackedValue & 0xFF,
            (this.PackedValue >> 8) & 0xFF,
            (this.PackedValue >> 16) & 0xFF,
            (this.PackedValue >> 24) & 0xFF);

    /// <inheritdoc />
    public static PixelTypeInfo GetPixelTypeInfo()
        => PixelTypeInfo.Create<Byte4>(
            PixelComponentInfo.Create<Byte4>(4, 8, 8, 8, 8),
            PixelColorType.RGB | PixelColorType.Alpha,
            PixelAlphaRepresentation.Unassociated);

    /// <inheritdoc />
    public readonly PixelOperations<Byte4> CreatePixelOperations() => new PixelOperations();

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Byte4 FromScaledVector4(Vector4 source) => FromVector4(source * 255f);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Byte4 FromVector4(Vector4 source) => new() { PackedValue = Pack(source) };

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Byte4 FromRgba32(Rgba32 source) => new() { PackedValue = source.PackedValue };

    /// <inheritdoc />
    public override readonly bool Equals(object? obj) => obj is Byte4 byte4 && this.Equals(byte4);

    /// <inheritdoc />
    public readonly bool Equals(Byte4 other) => this.PackedValue.Equals(other.PackedValue);

    /// <inheritdoc />
    public override readonly int GetHashCode() => this.PackedValue.GetHashCode();

    /// <inheritdoc />
    public override readonly string ToString()
    {
        Vector4 vector = this.ToVector4();
        return FormattableString.Invariant($"Byte4({vector.X:#0.##}, {vector.Y:#0.##}, {vector.Z:#0.##}, {vector.W:#0.##})");
    }

    /// <summary>
    /// Packs a vector into a uint.
    /// </summary>
    /// <param name="vector">The vector containing the values to pack.</param>
    /// <returns>The <see cref="uint"/> containing the packed values.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Pack(Vector4 vector)
    {
        vector = Numerics.Clamp(vector, Vector4.Zero, MaxBytes);

        Vector128<uint> result = Vector128.ConvertToUInt32(vector.AsVector128());

        uint byte4 = result.GetElement(0) & 0xFF;
        uint byte3 = result.GetElement(1) << 8;
        uint byte2 = result.GetElement(2) << 16;
        uint byte1 = result.GetElement(3) << 24;

        return byte4 | byte3 | byte2 | byte1;
    }
}
