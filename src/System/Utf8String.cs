﻿namespace System;

/// <summary>
/// Represents text as a sequence of UTF-8 code units.
/// </summary>
public sealed unsafe class Utf8String :
	IComparable<Utf8String>,
	IComparisonOperators<Utf8String, Utf8String>,
	IDefaultable<Utf8String>,
	IEqualityOperators<Utf8String, Utf8String>,
	IEquatable<Utf8String>
{
	/// <summary>
	/// Indicates the empty <see cref="Utf8String"/> instance.
	/// </summary>
	public static readonly Utf8String Empty = new(Array.Empty<byte>());


	/// <summary>
	/// Indicates the inner value.
	/// </summary>
	private readonly byte[]? _value;


	/// <summary>
	/// Initializes a <see cref="Utf8String"/> instance via the specified array of <see cref="byte"/>s
	/// as the underlying array.
	/// </summary>
	/// <param name="underlyingArray">The underlying array.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Utf8String(byte[]? underlyingArray) => _value = underlyingArray;


	/// <inheritdoc/>
	public int Length => _value?.Length ?? 0;

	/// <summary>
	/// Indicates the underlying array.
	/// </summary>
	public ReadOnlySpan<byte> UnderlyingArray => _value;

	/// <inheritdoc/>
	bool IDefaultable<Utf8String>.IsDefault => CompareTo(Empty) == 0;

	/// <inheritdoc/>
	static Utf8String IDefaultable<Utf8String>.Default => Empty;


	/// <summary>
	/// Gets the reference of a character at the specified index in the current string.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The reference of the character.</returns>
	public ref byte this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (_value is null)
			{
				throw new NullReferenceException();
			}

			return ref _value[index];
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) => Equals(obj as Utf8String);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] Utf8String? other)
	{
		if (other is null)
		{
			return false;
		}

		if (_value is null)
		{
			throw new NullReferenceException();
		}

		int length = _value.Length;
		if (length != other.Length)
		{
			return false;
		}

		if (_value == other._value)
		{
			return true;
		}

		fixed (byte* ap = _value, bp = other._value)
		{
			byte* a = ap, b = bp;
			while (length >= 4)
			{
				if (*(int*)a != *(int*)b)
				{
					return false;
				}

				a += 4;
				b += 4;
				length -= 4;
			}

			if (length >= 2)
			{
				if (*(short*)a != *(short*)b)
				{
					return false;
				}

				a += 2;
				b += 2;
				length -= 2;
			}

			if (length > 0 && *a != *b)
			{
				return false;
			}

			return true;
		}
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		if (_value is null)
		{
			return 0;
		}

		int length = _value.Length;
		uint hash = (uint)length;
		fixed (byte* ap = _value)
		{
			byte* a = ap;

			while (length >= 4)
			{
				hash = (hash + RotateLeft(hash, 5)) ^ *(uint*)a;
				a += 4; length -= 4;
			}
			if (length >= 2)
			{
				hash = (hash + RotateLeft(hash, 5)) ^ *(ushort*)a;
				a += 2; length -= 2;
			}
			if (length > 0)
			{
				hash = (hash + RotateLeft(hash, 5)) ^ *a;
			}

			hash += RotateLeft(hash, 7);
			hash += RotateLeft(hash, 15);

			return (int)hash;
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo([NotNull] Utf8String? other!!) => Compare(this, other);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() =>
		_value is null ? throw new NullReferenceException() : Encoding.UTF8.GetString(_value);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	int IComparable.CompareTo([NotNullWhen(true)] object? obj) =>
		obj is Utf8String comparer
			? CompareTo(comparer)
			: throw new ArgumentException($"The target value must be of type '{nameof(Utf8String)}'.");


	/// <summary>
	/// Compares two values.
	/// </summary>
	/// <param name="strA">The first string to be compared.</param>
	/// <param name="strB">The second string to be compared.</param>
	/// <returns>An <see cref="int"/> value indicating which one is greater.</returns>
	private static int Compare(Utf8String strA, Utf8String strB)
	{
		int length = Min(strA.Length, strB.Length);

		fixed (byte* ap = strA._value, bp = strB._value)
		{
			byte* a = ap, b = bp;
			while (length > 0)
			{
				if (*a != *b)
				{
					return *a - *b;
				}

				a++;
				b++;
				length--;
			}

			// At this point, we have compared all the characters in at least one string.
			// The longer string will be larger.
			// We could optimize and compare lengths before iterating strings, but we want
			// Foo and Foo1 to be sorted adjacent to eachother.
			return strA.Length - strB.Length;
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Utf8String? left, Utf8String? right) =>
		(left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false };

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Utf8String? left, Utf8String? right) => !(left == right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >(Utf8String left, Utf8String right) => left.CompareTo(right) > 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >=(Utf8String left, Utf8String right) => left.CompareTo(right) >= 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <(Utf8String left, Utf8String right) => left.CompareTo(right) < 0;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <=(Utf8String left, Utf8String right) => left.CompareTo(right) <= 0;


	/// <summary>
	/// Explicitly cast from <see cref="Utf8String"/> to <see cref="string"/>.
	/// </summary>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator string(Utf8String s) =>
		Encoding.UTF8.GetString(s._value ?? throw new NullReferenceException());

	/// <summary>
	/// Explicitly cast from <see cref="Utf8String"/> to <see cref="byte"/>[].
	/// </summary>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator byte[](Utf8String s) => s._value ?? throw new NullReferenceException();

	/// <summary>
	/// Implicitly cast from <see cref="byte"/>[] to <see cref="Utf8String"/>.
	/// </summary>
	/// <param name="underlyingArray">The underlying array.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Utf8String(byte[] underlyingArray) => new(underlyingArray);
}