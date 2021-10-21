﻿namespace Sudoku.Data;

partial record struct ChainLink
{
	/// <summary>
	/// Defines a JSON converter that allows the current instance being serialized.
	/// </summary>
	[JsonConverter(typeof(ChainLink))]
	public sealed unsafe class JsonConverter : JsonConverter<ChainLink>
	{
		/// <inheritdoc/>
		public override bool HandleNull => false;


		/// <inheritdoc/>
		/// <exception cref="InvalidOperationException">Throws when the specified data is invalid.</exception>
		public override ChainLink Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var (start, end, linkType, propName) = default((int, int, ChainLinkType, string?));
			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.PropertyName:
					{
						propName = reader.GetString();
						break;
					}
					case JsonTokenType.Number:
					{
						switch (propName)
						{
							case nameof(StartCandidate):
							{
								start = reader.GetInt32();
								break;
							}
							case nameof(EndCandidate):
							{
								end = reader.GetInt32();
								break;
							}
							case nameof(LinkType):
							{
								linkType = (ChainLinkType)reader.GetByte();
								break;
							}
							default:
							{
								throw new InvalidOperationException("Throws when the specified data is invalid.");
							}
						}

						break;
					}
				}
			}

			return new(start, end, linkType);
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, ChainLink value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber(nameof(StartCandidate), value.StartCandidate);
			writer.WriteNumber(nameof(EndCandidate), value.EndCandidate);
			writer.WriteNumber(nameof(LinkType), (byte)value.LinkType);
			writer.WriteEndObject();
		}
	}
}