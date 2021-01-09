﻿using System;
using System.Drawing;
using System.Extensions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace System.Text.Json.Converters
{
	/// <summary>
	/// Indicates a color JSON converter.
	/// </summary>
	[JsonConverter(typeof(Color))]
	public sealed class ColorJsonConverter : JsonConverter<Color>
	{
		/// <inheritdoc/>
		public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Color);


		/// <inheritdoc/>
		[SkipLocalsInit]
		public override unsafe Color Read(
			ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			const int length = 4;
			byte* span = stackalloc byte[length];
			for (int index = -1, i = 0; reader.Read() && i < length << 1; i++)
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.PropertyName:
					case JsonTokenType.String:
					{
						if (reader.GetString() is "A" or "R" or "G" or "B")
						{
							index++;
						}

						break;
					}
					case JsonTokenType.Number:
					{
						span[index] = reader.GetByte();

						break;
					}
				}
			}

			return Color.FromArgb(span[0], span[1], span[2], span[3]);
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
		{
			var (a, r, g, b) = value;
			writer.WriteStartObject();
			writer.WriteNumber("A", a);
			writer.WriteNumber("R", r);
			writer.WriteNumber("G", g);
			writer.WriteNumber("B", b);
			writer.WriteEndObject();
		}
	}
}
