﻿#define ENHANCED_DRAWING_APIS

namespace Sudoku.Gdip;

/// <summary>
/// Defines and encapsulates a data structure that provides the operations to draw a sudoku puzzle.
/// </summary>
public sealed partial class GridImageGenerator
{
	/// <summary>
	/// Indicates the default string format.
	/// </summary>
	private static readonly StringFormat DefaultStringFormat = new()
	{
		Alignment = StringAlignment.Center,
		LineAlignment = StringAlignment.Center
	};


	/// <inheritdoc cref="GridImageGenerator(PointCalculator, DrawingConfigurations)"/>
	/// <summary>
	/// <inheritdoc path="/summary"/>
	/// </summary>
	/// <param name="canvasSize">The size of the drawing canvas.</param>
	/// <param name="outsideOffset">The outside offset.</param>
	[SetsRequiredMembers]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public GridImageGenerator(float canvasSize, float outsideOffset) : this(new(canvasSize, outsideOffset))
	{
	}

	/// <inheritdoc cref="GridImageGenerator(PointCalculator, DrawingConfigurations)"/>
	[SetsRequiredMembers]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public GridImageGenerator(PointCalculator calculator) : this(calculator, new())
	{
	}

	/// <summary>
	/// Initializes a <see cref="GridImageGenerator"/> instance via the specified values.
	/// </summary>
	/// <param name="calculator">The point calculator that is used for conversion of drawing pixels.</param>
	/// <param name="preferences">The user-defined preferences.</param>
	[SetsRequiredMembers]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public GridImageGenerator(PointCalculator calculator, DrawingConfigurations preferences)
		=> (Calculator, Preferences) = (calculator, preferences);


	/// <summary>
	/// Indicates the drawing width.
	/// </summary>
	public float Width => Calculator.Width;

	/// <summary>
	/// Indicates the drawing height.
	/// </summary>
	public float Height => Calculator.Height;

	/// <summary>
	/// Indicate the footer text. This property is optional, and you can keep this with <see langword="null"/> value
	/// if you don't want to make any footers on a picture.
	/// </summary>
	public string? FooterText { get; set; }

	/// <summary>
	/// Indicates the footer text alignment.
	/// </summary>
	public StringAlignment FooterTextAlignment { get; set; }

	/// <summary>
	/// Indicates the puzzle.
	/// </summary>
	public Grid Puzzle { get; set; } = Grid.Empty;

	/// <summary>
	/// Indicates the view.
	/// </summary>
	public View? View { get; set; }

	/// <summary>
	/// Indicates all conclusions.
	/// </summary>
	public IEnumerable<Conclusion>? Conclusions { get; set; }

	/// <summary>
	/// Indicates the <see cref="PointCalculator"/> instance that calculates the pixels to help the inner
	/// methods to handle and draw the picture used for displaying onto the UI projects.
	/// </summary>
	public required PointCalculator Calculator { get; set; }

	/// <summary>
	/// Indicates the <see cref="DrawingConfigurations"/> instance that stores the default preferences
	/// that decides the drawing behavior.
	/// </summary>
	public required DrawingConfigurations Preferences { get; set; }


	/// <summary>
	/// To render the image onto the canvas specified as parameter <paramref name="g"/> of type <see cref="Graphics"/>.
	/// </summary>
	/// <param name="g">The graphics instance as base canvas, offering APIs allowing you doing drawing operations.</param>
	/// <seealso cref="Graphics"/>
	public void RenderTo(Graphics g)
	{
		DrawBackground(g);
		DrawGridAndBlockLines(g);

		g.SmoothingMode = SmoothingMode.HighQuality;
		g.TextRenderingHint = TextRenderingHint.AntiAlias;
		g.InterpolationMode = InterpolationMode.HighQualityBicubic;
		g.CompositingQuality = CompositingQuality.HighQuality;

		DrawView(g);
		DrawEliminations(g);
		DrawValue(g);
		DrawFooterText(g);
	}

	/// <summary>
	/// Render the image, with automatically calculation to get the target <see cref="Image"/> instance, and then return it.
	/// </summary>
	/// <returns>The default-generated <see cref="Image"/> instance.</returns>
	public Image RenderTo()
	{
		using var data = GetFooterTextRenderingData();
		var (font, extraHeight, alignment) = data;

		// There is a little bug that this method ignores the case when the text is too long.
		// However, I don't want to handle on this case. If the text is too long, it will be overflown, as default case to be kept;
		// otherwise, the picture drawn will be aligned as left, which is not the expected result.
		var bitmap = new Bitmap((int)Width, (int)(FooterText is not null ? Height + extraHeight : Height));

		using var g = Graphics.FromImage(bitmap);
		RenderTo(g);

		return bitmap;
	}

	/// <summary>
	/// Gets the rendering data.
	/// </summary>
	/// <returns>Rendering data.</returns>
	internal TextRenderingData GetFooterTextRenderingData()
	{
		if (this is not
			{
				FooterTextAlignment: var footerAlignment,
				FooterText: var footer,
				Preferences.FooterTextFont: var (fontName, fontSize, fontStyle)
			})
		{
			throw new();
		}

		using var tempBitmap = new Bitmap((int)Width, (int)Height);
		using var tempGraphics = Graphics.FromImage(tempBitmap);
		var footerFont = new Font(fontName, fontSize, fontStyle);
		var (_, footerHeight) = footer is not null ? tempGraphics.MeasureString(footer, footerFont) : default;
		return new(footerFont, footerHeight, new() { Alignment = footerAlignment });
	}

	/// <summary>
	/// Try to get the result color value.
	/// </summary>
	/// <param name="identifier">The color identifier.</param>
	/// <param name="result">The result color got.</param>
	/// <returns>The <see cref="bool"/> result.</returns>
	/// <exception cref="InvalidOperationException">Throws when the ID is invalid.</exception>
	private bool GetValueById(Identifier identifier, out Color result)
	{
		if ((Preferences, identifier) is ({ ColorPalette: var palette }, { Mode: IdentifierColorMode.Id, Id: var id }))
		{
			return (result = palette.Length > id ? palette[id] : Color.Transparent) != Color.Transparent;
		}
		else
		{
			result = Color.Transparent;
			return false;
		}
	}

	/// <summary>
	/// Gets the color value.
	/// </summary>
	/// <param name="id">The identifier instance.</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Throws when the specified value is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Color GetColor(Identifier id)
		=> id switch
		{
			{ Mode: IdentifierColorMode.Raw, A: var a, R: var r, G: var g, B: var b } => Color.FromArgb(a, r, g, b),
			{ Mode: IdentifierColorMode.Id } when GetValueById(id, out var color) => Color.FromArgb(64, color),
			{ Mode: IdentifierColorMode.Named, NamedKind: var namedKind } => namedKind switch
			{
				DisplayColorKind.Normal => Preferences.ColorPalette[0],
				DisplayColorKind.Assignment => Preferences.ColorPalette[0],
				DisplayColorKind.Elimination => Preferences.EliminationColor,
				DisplayColorKind.Exofin => Preferences.ColorPalette[1],
				DisplayColorKind.Endofin => Preferences.ColorPalette[2],
				DisplayColorKind.Cannibalism => Preferences.CannibalismColor,
				DisplayColorKind.Link => Preferences.ChainColor,
				_ => throw new InvalidOperationException("Such displaying color kind is invalid.")
			},
			_ => throw new InvalidOperationException("Such identifier instance contains invalid value.")
		};


	/// <summary>
	/// Get the font via the specified name, size and the scale.
	/// </summary>
	/// <param name="fontName">The font name that decides the font to use and presentation.</param>
	/// <param name="size">The size that decides the default font size.</param>
	/// <param name="scale">The scale that decides the result font size.</param>
	/// <param name="style">The style that decides the font style of the text in the picture.</param>
	/// <returns>The font.</returns>
	/// <exception cref="ArgumentNullException">Throws when <paramref name="fontName"/> is <see langword="null"/>.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Font GetFont(string? fontName, float size, decimal scale, FontStyle style)
		=> new(fontName ?? throw new ArgumentNullException(nameof(size)), size * (float)scale, style);


	partial void DrawGridAndBlockLines(Graphics g);
	partial void DrawBackground(Graphics g);
	partial void DrawValue(Graphics g);
	partial void DrawEliminations(Graphics g);
	partial void DrawFooterText(Graphics g);
	partial void DrawView(Graphics g);
	partial void DrawCells(Graphics g);
	partial void DrawCandidates(Graphics g);
	partial void DrawHouses(Graphics g);
	partial void DrawLinks(Graphics g);
	partial void DrawUnknownValue(Graphics g);
	partial void DrawBorderBar(Graphics g);
	partial void DrawKropkiDot(Graphics g);
	partial void DrawGreaterThanSigns(Graphics g);
	partial void DrawXvSigns(Graphics g);
	partial void DrawNumberLabels(Graphics g);
	partial void DrawBattenburg(Graphics g);
	partial void DrawQuadrupleHint(Graphics g);
	partial void DrawClockfaceDot(Graphics g);
	partial void DrawNeighborSigns(Graphics g);
	partial void DrawWheel(Graphics g);
}

partial class GridImageGenerator
{
	/// <summary>
	/// Draw footer text.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	partial void DrawFooterText(Graphics g)
	{
		if (this is not { Width: var w, FooterText: { } text, Preferences.FooterTextColor: var color })
		{
			return;
		}

		using var brush = new SolidBrush(color);
		using var data = GetFooterTextRenderingData();
		var (font, extraHeight, alignment) = data;
		g.DrawString(text, font, brush, new RectangleF(0, w, w, extraHeight), alignment);
	}

	/// <summary>
	/// Draw givens, modifiables and candidates, where the values are specified as a grid.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	partial void DrawValue(Graphics g)
	{
		if (this is not
			{
				Puzzle: { IsUndefined: false } puzzle,
				Calculator: { CellSize.Width: var cellWidth, CandidateSize.Width: var candidateWidth } calc,
				Preferences:
				{
					GivenColor: var gColor,
					ModifiableColor: var mColor,
					CandidateColor: var cColor,
					GivenFontName: var gFontName,
					ModifiableFontName: var mFontName,
					CandidateFontName: var cFontName,
					ValueScale: var vScale,
					CandidateScale: var cScale,
					GivenFontStyle: var gFontStyle,
					ModifiableFontStyle: var mFontStyle,
					CandidateFontStyle: var cFontStyle,
					ShowCandidates: var showCandidates
				}
			})
		{
			return;
		}

		var vOffsetValue = cellWidth / (AnchorsCount / 3); // The vertical offset of rendering each value.
		var vOffsetCandidate = candidateWidth / (AnchorsCount / 3); // The vertical offset of rendering each candidate.
		var halfWidth = cellWidth / 2F;

		using var bGiven = new SolidBrush(gColor);
		using var bModifiable = new SolidBrush(mColor);
		using var bCandidate = new SolidBrush(cColor);
		using var bCandidateLighter = new SolidBrush(cColor.QuarterAlpha());
		using var fGiven = GetFont(gFontName, halfWidth, vScale, gFontStyle);
		using var fModifiable = GetFont(mFontName, halfWidth, vScale, mFontStyle);
		using var fCandidate = GetFont(cFontName, halfWidth, cScale, cFontStyle);

		for (var cell = 0; cell < 81; cell++)
		{
			var mask = puzzle.GetMask(cell);
			switch (MaskToStatus(mask))
			{
				case CellStatus.Empty when showCandidates:
				{
					// Draw candidates.
					var overlaps = View.UnknownOverlaps(cell);
					foreach (var digit in (short)(mask & Grid.MaxCandidatesMask))
					{
						var originalPoint = calc.GetMousePointInCenter(cell, digit);
						var point = originalPoint with { Y = originalPoint.Y + vOffsetCandidate };
						g.DrawValue(digit + 1, fCandidate, overlaps ? bCandidateLighter : bCandidate, point, DefaultStringFormat);
					}

					break;
				}
				case var status and (CellStatus.Modifiable or CellStatus.Given):
				{
					// Draw values.
					var originalPoint = calc.GetMousePointInCenter(cell);
					var point = originalPoint with { Y = originalPoint.Y + vOffsetValue };
					g.DrawValue(puzzle[cell] + 1, f(status, fGiven, fModifiable), f(status, bGiven, bModifiable), point, DefaultStringFormat);

					break;


					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					static T f<T>(CellStatus status, T given, T modifiable) => status == CellStatus.Given ? given : modifiable;
				}
			}
		}
	}

	/// <summary>
	/// Draw custom view if <see cref="View"/> is not <see langword="null"/>.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	/// <seealso cref="View"/>
	partial void DrawView(Graphics g)
	{
		if (View is null)
		{
			return;
		}

		// Normal nodes
		DrawHouses(g);
		DrawCells(g);
		DrawCandidates(g);
		DrawLinks(g);
		DrawUnknownValue(g);

#if ENHANCED_DRAWING_APIS
		// Shapes
		DrawBorderBar(g);
		DrawKropkiDot(g);
		DrawGreaterThanSigns(g);
		DrawXvSigns(g);
		DrawNumberLabels(g);
		DrawBattenburg(g);
		DrawQuadrupleHint(g);
		DrawClockfaceDot(g);
		DrawNeighborSigns(g);
		DrawWheel(g);
#endif
	}

	/// <summary>
	/// Draw the background, where the color is specified in <see cref="DrawingConfigurations.BackgroundColor"/>.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	/// <seealso cref="DrawingConfigurations.BackgroundColor"/>
	partial void DrawBackground(Graphics g) => g.Clear(Preferences.BackgroundColor);

	/// <summary>
	/// Draw grid lines and block lines.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	partial void DrawGridAndBlockLines(Graphics g)
	{
		if (this is not
			{
				Calculator.GridPoints: var pts,
				Preferences:
				{
					GridLineColor: var gridLineColor,
					BlockLineColor: var blockLineColor,
					GridLineWidth: var gridLineWidth,
					BlockLineWidth: var blockLineWidth
				}
			})
		{
			return;
		}

		using var pg = new Pen(gridLineColor, gridLineWidth);
		using var pb = new Pen(blockLineColor, blockLineWidth);

		const int length = AnchorsCount + 1;
		for (var i = 0; i < length; i += AnchorsCount / 9)
		{
			g.DrawLine(pg, pts[i, 0], pts[i, AnchorsCount]);
			g.DrawLine(pg, pts[0, i], pts[AnchorsCount, i]);
		}

		for (var i = 0; i < length; i += AnchorsCount / 3)
		{
			g.DrawLine(pb, pts[i, 0], pts[i, AnchorsCount]);
			g.DrawLine(pb, pts[0, i], pts[AnchorsCount, i]);
		}
	}

	/// <summary>
	/// Draw eliminations.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	partial void DrawEliminations(Graphics g)
	{
		if (this is not
			{
				Conclusions: { } conclusions,
				Preferences: { EliminationColor: var eColor, CannibalismColor: var cColor },
				View: var view
			} || !conclusions.Any())
		{
			return;
		}

		using var elimBrush = new SolidBrush(eColor);
		using var cannibalBrush = new SolidBrush(cColor);
		using var elimBrushLighter = new SolidBrush(eColor.QuarterAlpha());
		using var canniBrushLighter = new SolidBrush(cColor.QuarterAlpha());
		foreach (var (t, c, d) in conclusions)
		{
			if (t != ConclusionType.Elimination)
			{
				continue;
			}

			var cannibalism = false;
			if (view is not { CandidateNodes: var candidateNodes })
			{
				goto Drawing;
			}

			foreach (var candidateNode in candidateNodes)
			{
				var value = candidateNode.Candidate;
				if (value == c * 9 + d)
				{
					cannibalism = true;
					break;
				}
			}

		Drawing:
			g.FillEllipse(
				(cannibalism, view.UnknownOverlaps(c)) switch
				{
					(true, true) => canniBrushLighter,
					(true, false) => cannibalBrush,
					(false, true) => elimBrushLighter,
					_ => elimBrush
				},
				Calculator.GetMouseRectangle(c, d)
			);
		}
	}

	/// <summary>
	/// Draw cells.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	partial void DrawCells(Graphics g)
	{
		if (View is not { CellNodes: var cellNodes })
		{
			return;
		}

		foreach (var cellNode in cellNodes)
		{
			var cell = cellNode.Cell;
			var id = cellNode.Identifier;
			using var brush = new SolidBrush(GetColor(id));

			g.FillRectangle(brush, Calculator.GetMouseRectangleViaCell(cell));
		}
	}

	/// <summary>
	/// Draw candidates.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	partial void DrawCandidates(Graphics g)
	{
		if (this is not
			{
				View: { CandidateNodes: var candidateNodes } view,
				Calculator: { CellSize.Width: var cellWidth, CandidateSize.Width: var candidateWidth } calc,
				Conclusions: var conclusions,
				Preferences:
				{
					CandidateColor: var cColor,
					CandidateFontName: var cFontName,
					CandidateScale: var cScale,
					CandidateFontStyle: var cFontStyle,
					ShowCandidates: var showCandidates
				}
			})
		{
			return;
		}

		var vOffsetCandidate = candidateWidth / 9; // The vertical offset of rendering each candidate.

		using var bCandidate = new SolidBrush(cColor);
		using var bCandidateLighter = new SolidBrush(cColor.QuarterAlpha());
		using var fCandidate = GetFont(cFontName, cellWidth / 2F, cScale, cFontStyle);

		foreach (var candidateNode in candidateNodes)
		{
			var candidate = candidateNode.Candidate;
			var id = candidateNode.Identifier;

			var isOverlapped = false;
			if (conclusions is null)
			{
				goto IsOverlapped;
			}

			foreach (var (concType, concCandidate) in conclusions)
			{
				if (concType == ConclusionType.Elimination && concCandidate == candidate)
				{
					isOverlapped = true;
					break;
				}
			}

		IsOverlapped:
			if (!isOverlapped)
			{
				var cell = candidate / 9;
				var digit = candidate % 9;
				var overlaps = view.UnknownOverlaps(cell);

				switch (id)
				{
					case { Mode: IdentifierColorMode.Raw, A: var alpha, R: var red, G: var green, B: var blue }:
					{
						using var brush = new SolidBrush(Color.FromArgb(overlaps ? alpha : alpha >> 2, red, green, blue));
						g.FillEllipse(brush, calc.GetMouseRectangle(cell, digit));

						// In direct view, candidates should be drawn also.
						if (!showCandidates)
						{
							d(cell, digit, vOffsetCandidate, overlaps ? bCandidateLighter : bCandidate);
						}

						break;
					}
					case { Mode: var mode and (IdentifierColorMode.Id or IdentifierColorMode.Named) }:
					{
						var color = mode switch
						{
							IdentifierColorMode.Id when GetValueById(id, out var c) => c,
							IdentifierColorMode.Named => GetColor(id),
							_ => throw new InvalidOperationException()
						};

						// In the normal case, I'll draw these circles.
						using var brush = new SolidBrush(overlaps ? color.QuarterAlpha() : color);
						g.FillEllipse(brush, calc.GetMouseRectangle(cell, digit));

						// In direct view, candidates should be drawn also.
						if (!showCandidates)
						{
							d(cell, digit, vOffsetCandidate, overlaps ? bCandidateLighter : bCandidate);
						}

						break;
					}
				}
			}
		}

		if (!showCandidates && conclusions is not null)
		{
			foreach (var (type, cell, digit) in conclusions)
			{
				var overlaps = view.UnknownOverlaps(cell);
				if (type == ConclusionType.Elimination)
				{
					d(cell, digit, vOffsetCandidate, overlaps ? bCandidateLighter : bCandidate);
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void d(int cell, int digit, float vOffsetCandidate, Brush brush)
		{
			var originalPoint = calc.GetMousePointInCenter(cell, digit);
			var point = originalPoint with { Y = originalPoint.Y + vOffsetCandidate };
			g.DrawValue(digit + 1, fCandidate, brush, point, DefaultStringFormat);
		}
	}

	/// <summary>
	/// Draw houses.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	partial void DrawHouses(Graphics g)
	{
		if (this is not { Calculator: { CellSize: var (w, h) } calc, View.HouseNodes: var houseNodes, Preferences.ShowLightHouse: var showLightHouse })
		{
			return;
		}

		foreach (var houseNode in houseNodes)
		{
			var house = houseNode.House;
			var id = houseNode.Identifier;

			Color? tempColor;
			try
			{
				tempColor = GetColor(id);
			}
			catch (InvalidOperationException)
			{
				tempColor = null;
			}
			if (tempColor is not { } color)
			{
				continue;
			}

			if (showLightHouse)
			{
				using var pen = new Pen(color, 4F);
				switch (house)
				{
					case >= 0 and < 9:
					{
						g.DrawRoundedRectangle(pen, calc.GetMouseRectangleViaHouse(house), 6);

						break;
					}
					case >= 9 and < 27:
					{
						var (l, r) = calc.GetAnchorsViaHouse(house);
						w /= 2;
						h /= 2;
						l = l with { X = l.X + w, Y = l.Y + h };
						r = r with { X = r.X - w, Y = r.Y - h };

						g.DrawLine(pen, l, r);

						break;
					}
				}
			}
			else
			{
				using var brush = new SolidBrush(Color.FromArgb(64, color));
				g.FillRectangle(brush, calc.GetMouseRectangleViaHouse(house));
			}
		}
	}

	/// <summary>
	/// Draw links.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	partial void DrawLinks(Graphics g)
	{
		if (this is not
			{
				View: { LinkNodes: var links } view,
				Conclusions: var conclusions,
				Calculator: { CandidateSize: var (cw, ch) } calc,
				Preferences.ChainColor: var chainColor
			})
		{
			return;
		}

		// Gather all points used.
		var points = new HashSet<PointF>();
		var linkArray = links.CastToArray();
		foreach (var linkNode in linkArray)
		{
			points.Add(calc.GetMouseCenter(linkNode.Start));
			points.Add(calc.GetMouseCenter(linkNode.End));
		}

		if (conclusions is not null)
		{
			foreach (var conclusion in conclusions)
			{
				points.Add(calc.GetMousePointInCenter(conclusion.Cell, conclusion.Digit));
			}
		}

		// Iterate on each inference to draw the links and grouped nodes (if so).
		using var linePen = new Pen(chainColor, 2F);
		using var arrowPen = new Pen(chainColor, 2F) { CustomEndCap = new AdjustableArrowCap(cw / 4F, ch / 3F) };

		foreach (var linkNode in linkArray)
		{
			if (linkNode is not { Start: var start, End: var end, Inference: var inference })
			{
				continue;
			}

			arrowPen.DashStyle = inference switch { Inference.Strong => DashStyle.Solid, Inference.Weak => DashStyle.Dot, _ => DashStyle.Dash };

			_ = (calc.GetMouseCenter(start), calc.GetMouseCenter(end)) is (var pt1 and var (pt1x, pt1y), var pt2 and var (pt2x, pt2y));

			var penToDraw = inference != Inference.Default ? arrowPen : linePen;
			if (inference == Inference.Default)
			{
				// Draw the link.
				g.DrawLine(penToDraw, pt1, pt2);
			}
			else
			{
				// If the distance of two points is lower than the one of two adjacent candidates,
				// the link will be emitted to draw because of too narrow.
				var distance = Sqrt((pt1x - pt2x) * (pt1x - pt2x) + (pt1y - pt2y) * (pt1y - pt2y));
				if (distance <= cw * SqrtOf2 || distance <= ch * SqrtOf2)
				{
					continue;
				}

				// Check if another candidate lies in the direct line.
				var deltaX = pt2x - pt1x;
				var deltaY = pt2y - pt1y;
				var alpha = Atan2(deltaY, deltaX);
				var dx1 = deltaX;
				var dy1 = deltaY;
				var through = false;
				adjust(pt1, pt2, out var p1, out _, alpha, cw);
				foreach (var point in points)
				{
					if (point == pt1 || point == pt2)
					{
						// The point is itself.
						continue;
					}

					var dx2 = point.X - p1.X;
					var dy2 = point.Y - p1.Y;
					if (Sign(dx1) == Sign(dx2) && Sign(dy1) == Sign(dy2)
						&& Abs(dx2) <= Abs(dx1) && Abs(dy2) <= Abs(dy1)
						&& (dx1 == 0 || dy1 == 0 || (dx1 / dy1).NearlyEquals(dx2 / dy2, epsilon: 1E-1F)))
					{
						through = true;
						break;
					}
				}

				// Now cut the link.
				cut(ref pt1, ref pt2, cw, ch, pt1x, pt1y, pt2x, pt2y);

				if (through)
				{
					var bezierLength = 20F;

					// The end points are rotated 45 degrees
					// (counterclockwise for the start point, clockwise for the end point).
					rotate(new(pt1x, pt1y), ref pt1, -RotateAngle);
					rotate(new(pt2x, pt2y), ref pt2, RotateAngle);

					var aAlpha = alpha - RotateAngle;
					var bx1 = pt1.X + bezierLength * Cos(aAlpha);
					var by1 = pt1.Y + bezierLength * Sin(aAlpha);

					aAlpha = alpha + RotateAngle;
					var bx2 = pt2.X - bezierLength * Cos(aAlpha);
					var by2 = pt2.Y - bezierLength * Sin(aAlpha);

					g.DrawBezier(penToDraw, pt1.X, pt1.Y, bx1, by1, bx2, by2, pt2.X, pt2.Y);
				}
				else
				{
					// Draw the link.
					g.DrawLine(penToDraw, pt1, pt2);
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void rotate(PointF pt1, scoped ref PointF pt2, float angle)
		{
			// Translate 'pt2' to (0, 0).
			pt2.X -= pt1.X;
			pt2.Y -= pt1.Y;

			// Rotate.
			var sinAngle = Sin(angle);
			var cosAngle = Cos(angle);
			var xAct = pt2.X;
			var yAct = pt2.Y;
			pt2.X = (float)(xAct * cosAngle - yAct * sinAngle);
			pt2.Y = (float)(xAct * sinAngle + yAct * cosAngle);

			pt2.X += pt1.X;
			pt2.Y += pt1.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void adjust(PointF pt1, PointF pt2, out PointF p1, out PointF p2, float alpha, float candidateSize)
		{
			p1 = pt1;
			p2 = pt2;
			var tempDelta = candidateSize / 2;
			var px = (int)(tempDelta * Cos(alpha));
			var py = (int)(tempDelta * Sin(alpha));

			p1.X += px;
			p1.Y += py;
			p2.X -= px;
			p2.Y -= py;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void cut(scoped ref PointF pt1, scoped ref PointF pt2, float cw, float ch, float pt1x, float pt1y, float pt2x, float pt2y)
		{
			var slope = Abs((pt2y - pt1y) / (pt2x - pt1x));
			var x = cw / (float)Sqrt(1 + slope * slope);
			var y = ch * (float)Sqrt(slope * slope / (1 + slope * slope));

			if (pt1y > pt2y && pt1x.NearlyEquals(pt2x)) { pt1.Y -= ch / 2; pt2.Y += ch / 2; }
			else if (pt1y < pt2y && pt1x.NearlyEquals(pt2x)) { pt1.Y += ch / 2; pt2.Y -= ch / 2; }
			else if (pt1y.NearlyEquals(pt2y) && pt1x > pt2x) { pt1.X -= cw / 2; pt2.X += cw / 2; }
			else if (pt1y.NearlyEquals(pt2y) && pt1x < pt2x) { pt1.X += cw / 2; pt2.X -= cw / 2; }
			else if (pt1y > pt2y && pt1x > pt2x) { pt1.X -= x / 2; pt1.Y -= y / 2; pt2.X += x / 2; pt2.Y += y / 2; }
			else if (pt1y > pt2y && pt1x < pt2x) { pt1.X += x / 2; pt1.Y -= y / 2; pt2.X -= x / 2; pt2.Y += y / 2; }
			else if (pt1y < pt2y && pt1x > pt2x) { pt1.X -= x / 2; pt1.Y += y / 2; pt2.X += x / 2; pt2.Y -= y / 2; }
			else if (pt1y < pt2y && pt1x < pt2x) { pt1.X += x / 2; pt1.Y += y / 2; pt2.X -= x / 2; pt2.Y -= y / 2; }
		}
	}

	/// <summary>
	/// Draw unknown values.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	partial void DrawUnknownValue(Graphics g)
	{
		if (this is not
			{
				View.UnknownNodes: var unknownNodes,
				Calculator: { CellSize.Width: var cellWidth } calc,
				Preferences:
				{
					UnknownIdentifierColor: var uColor,
					ValueScale: var vScale,
					UnknownFontStyle: var uFontStyle,
					UnknownFontName: var uFontName
				}
			})
		{
			return;
		}

		var vOffsetValue = cellWidth / (AnchorsCount / 3); // The vertical offset of rendering each value.
		var halfWidth = cellWidth / 2F;

		using var brush = new SolidBrush(uColor);
		using var font = GetFont(uFontName, halfWidth, vScale, uFontStyle);

		foreach (var unknownNode in unknownNodes)
		{
			var cell = unknownNode.Cell;
			var character = unknownNode.UnknownValueChar;

			// Draw values.
			var orginalPoint = calc.GetMousePointInCenter(cell);
			var point = orginalPoint with { Y = orginalPoint.Y + vOffsetValue };
			g.DrawValue(character, font, brush, point, DefaultStringFormat);
		}
	}

	/// <summary>
	/// Draw border bars.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	[Conditional("ENHANCED_DRAWING_APIS")]
	partial void DrawBorderBar(Graphics g)
	{
		if (this is not
			{
				View.BorderBarNodes: var barNodes,
				Calculator: var calc,
				Preferences: { BorderBarWidth: var barWidth, BorderBarFullyOverlapsGridLine: var fullyOverlapping }
			})
		{
			return;
		}

		foreach (var barNode in barNodes)
		{
			if (barNode is not (var c1, var c2) { Identifier: var identifier })
			{
				continue;
			}

			using var brush = new SolidBrush(GetColor(identifier));
			using var pen = new Pen(brush, barWidth);

			// Draw bars.
			var (start, end) = calc.GetSharedLinePosition(c1, c2, fullyOverlapping);
			g.DrawLine(pen, start, end);
		}
	}

	/// <summary>
	/// Draw Kropki dots.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	[Conditional("ENHANCED_DRAWING_APIS")]
	partial void DrawKropkiDot(Graphics g)
	{
		if (this is not
			{
				View.KropkiDotNodes: var kropkiNodes,
				Calculator: var calc,
				Preferences: { KropkiDotBorderWidth: var borderWidth, KropkiDotSize: var dotSize, BackgroundColor: var backColor }
			})
		{
			return;
		}

		foreach (var kropkiNode in kropkiNodes)
		{
			if (kropkiNode is not (var c1, var c2) { Identifier: var identifier, IsSolid: var isSolid })
			{
				continue;
			}

			using var solidBrush = new SolidBrush(GetColor(identifier));
			using var hollowBrush = new SolidBrush(backColor);
			using var pen = new Pen(solidBrush, borderWidth);

			var ((x1, y1), (x2, y2)) = calc.GetSharedLinePosition(c1, c2);
			var rect = new RectangleF((x1 + x2) / 2 - dotSize / 2, (y1 + y2) / 2 - dotSize / 2, dotSize, dotSize);

			// Draw Kropki dots.
			// Please note that method 'DrawEllipse' and 'FillEllipse' starts with the point at top-left position, rather than the center.
			g.DrawEllipse(pen, rect);
			g.FillEllipse(isSolid ? solidBrush : hollowBrush, rect);
		}
	}

	/// <summary>
	/// Draw greater-than signs.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	[Conditional("ENHANCED_DRAWING_APIS")]
	partial void DrawGreaterThanSigns(Graphics g)
	{
		if (this is not
			{
				View.GreaterThanNodes: var greaterThanNodes,
				Calculator: var calc,
				Preferences: { GreaterThanSignFont: var (fontName, fontSize, fontStyle), BackgroundColor: var backColor }
			})
		{
			return;
		}

		using var font = new Font(fontName, fontSize, fontStyle);
		using var backBrush = new SolidBrush(backColor);
		foreach (var greaterThanNode in greaterThanNodes)
		{
			if (greaterThanNode is not (var c1, var c2, var isRow) { Identifier: var identifier, IsGreaterThan: var isGreaterThan })
			{
				continue;
			}

			using var brush = new SolidBrush(GetColor(identifier));
			var text = isGreaterThan ? ">" : "<";
			var ((x1, y1), (x2, y2)) = calc.GetSharedLinePosition(c1, c2);
			var centerPoint = new PointF((x1 + x2) / 2, (y1 + y2) / 2);

			// Draw sign.
			if (isRow)
			{
				var textSize = g.MeasureString(text, font);
				var (tw, th) = textSize;
				var (pointX, pointY) = centerPoint - new SizeF(tw / 2, th / 2);

				g.FillRectangle(backBrush, pointX, pointY, tw, th);
				g.DrawString(text, font, brush, centerPoint, DefaultStringFormat);
			}
			else
			{
				// If column-ish, we need rotate the text.
				// By using 'g.Transform', we can rotate a text shape on rendering.
				// Please note that 'Matrix' type is a reference type: be careful to assign and replace.

				var matrixOriginal = g.Transform;
				using var matrixRotating = g.Transform.Clone();
				matrixRotating.RotateAt(90, centerPoint);
				g.Transform = matrixRotating;

				var textSize = g.MeasureString(text, font);
				var (tw, th) = textSize;
				var (pointX, pointY) = centerPoint - new SizeF(tw / 2, th / 2);

				g.FillRectangle(backBrush, pointX, pointY, tw, th);
				g.DrawString(text, font, brush, centerPoint, DefaultStringFormat);

				g.Transform = matrixOriginal;
			}
		}
	}

	/// <summary>
	/// Draw XV signs.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	[Conditional("ENHANCED_DRAWING_APIS")]
	partial void DrawXvSigns(Graphics g)
	{
		if (this is not
			{
				View.XvNodes: var xvNodes,
				Calculator: var calc,
				Preferences: { XvSignFont: var (fontName, fontSize, fontStyle), BackgroundColor: var backColor }
			})
		{
			return;
		}

		using var font = new Font(fontName, fontSize, fontStyle);
		using var backBrush = new SolidBrush(backColor);
		foreach (var xvNode in xvNodes)
		{
			if (xvNode is not (var c1, var c2) { Identifier: var identifier, IsX: var isX })
			{
				continue;
			}

			using var brush = new SolidBrush(GetColor(identifier));
			var text = isX ? "X" : "V";
			var ((x1, y1), (x2, y2)) = calc.GetSharedLinePosition(c1, c2);
			var centerPoint = new PointF((x1 + x2) / 2, (y1 + y2) / 2);
			var textSize = g.MeasureString(text, font);
			var (tw, th) = textSize;
			var (pointX, pointY) = centerPoint - new SizeF(tw / 2, th / 2);

			g.FillRectangle(backBrush, pointX, pointY, tw, th);
			g.DrawString(text, font, brush, centerPoint, DefaultStringFormat);
		}
	}

	/// <summary>
	/// Draw number labels.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	[Conditional("ENHANCED_DRAWING_APIS")]
	partial void DrawNumberLabels(Graphics g)
	{
		if (this is not
			{
				View.NumberLabelNodes: var numberLabelNodes,
				Calculator: var calc,
				Preferences: { NumberLabelFont: var (fontName, fontSize, fontStyle), BackgroundColor: var backColor }
			})
		{
			return;
		}

		using var font = new Font(fontName, fontSize, fontStyle);
		using var backBrush = new SolidBrush(backColor);
		foreach (var numberLabelNode in numberLabelNodes)
		{
			if (numberLabelNode is not (var c1, var c2) { Identifier: var identifier, Label: var label })
			{
				continue;
			}

			using var brush = new SolidBrush(GetColor(identifier));
			var ((x1, y1), (x2, y2)) = calc.GetSharedLinePosition(c1, c2);
			var centerPoint = new PointF((x1 + x2) / 2, (y1 + y2) / 2);
			var textSize = g.MeasureString(label, font);
			var (tw, th) = textSize;
			var (pointX, pointY) = centerPoint - new SizeF(tw / 2, th / 2);

			g.FillRectangle(backBrush, pointX, pointY, tw, th);
			g.DrawString(label, font, brush, centerPoint, DefaultStringFormat);
		}
	}

	/// <summary>
	/// Draw Batternburg.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	[Conditional("ENHANCED_DRAWING_APIS")]
	partial void DrawBattenburg(Graphics g)
	{
		if (this is not
			{
				View.BattenburgNodes: var battenburgNodes,
				Calculator: { CellSize: var (cw, ch) } calc,
				Preferences.BattenburgSize: var battenburgSize
			})
		{
			return;
		}

		foreach (var battenburgNode in battenburgNodes)
		{
			if (battenburgNode is not { Identifier: var identifier, Cells: [.., var lastCell] })
			{
				continue;
			}

			var (tempX, tempY) = calc.GetMousePointInCenter(lastCell) - new SizeF(cw / 2, ch / 2);

			var p1 = new PointF(tempX - battenburgSize / 2, tempY - battenburgSize / 2);
			var p2 = new PointF(tempX, tempY - battenburgSize / 2);
			var p3 = new PointF(tempX - battenburgSize / 2, tempY);
			var p4 = new PointF(tempX, tempY);

			using var brush = new SolidBrush(GetColor(identifier));
			using var pen = new Pen(Brushes.Black);

			scoped var points = (stackalloc[] { p1, p2, p3, p4 });
			for (var i = 0; i < points.Length; i++)
			{
				var (x, y) = points[i];
				var shouldBeFilled = i is 0 or 3;
				if (shouldBeFilled)
				{
					g.DrawRectangle(pen, x, y, battenburgSize / 2, battenburgSize / 2);
					g.FillRectangle(brush, x, y, battenburgSize / 2, battenburgSize / 2);
				}
				else
				{
					g.DrawRectangle(pen, x, y, battenburgSize / 2, battenburgSize / 2);
				}
			}
		}
	}

	/// <summary>
	/// Draw quadruple hint.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	[Conditional("ENHANCED_DRAWING_APIS")]
	partial void DrawQuadrupleHint(Graphics g)
	{
		if (this is not
			{
				View.QuadrupleHintNodes: var quadrupleHintNodes,
				Calculator: { CellSize: var (cw, ch) } calc,
				Preferences: { QuadrupleHintFont: var (fontName, fontSize, fontStyle), BackgroundColor: var backColor }
			})
		{
			return;
		}

		using var font = new Font(fontName, fontSize, fontStyle);
		foreach (var quadrupleHintNode in quadrupleHintNodes)
		{
			if (quadrupleHintNode is not { Identifier: var identifier, Cells: [.., var lastCell], Hint: var hint })
			{
				continue;
			}

			using var brush = new SolidBrush(backColor);
			using var textColor = new SolidBrush(GetColor(identifier));
			var (tw, th) = g.MeasureString(hint, font);
			var (x, y) = calc.GetMousePointInCenter(lastCell);

			g.FillRectangle(brush, x - cw / 2 - tw / 2, y - ch / 2 - th / 2, tw, th);
			g.DrawString(hint, font, textColor, x - cw / 2, y - ch / 2, DefaultStringFormat);
		}
	}

	/// <summary>
	/// Draw clockface dots.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	[Conditional("ENHANCED_DRAWING_APIS")]
	partial void DrawClockfaceDot(Graphics g)
	{
		if (this is not
			{
				View.ClockfaceDotNodes: var clockfaceDotNodes,
				Calculator: { CellSize: var (cw, ch) } calc,
				Preferences:
				{
					ClockfaceDotSize: var dotSize,
					ClockfaceDotBorderWidth: var borderWidth,
					BackgroundColor: var backColor
				}
			})
		{
			return;
		}

		foreach (var clockfaceDotNode in clockfaceDotNodes)
		{
			if (clockfaceDotNode is not { Identifier: var identifier, Cells: [.., var lastCell], IsClockwise: var isClockwise })
			{
				continue;
			}

			using var brush = new SolidBrush(GetColor(identifier));
			using var pen = new Pen(brush, borderWidth);
			using var backBrush = new SolidBrush(backColor);

			var (x, y) = calc.GetMousePointInCenter(lastCell);
			if (isClockwise)
			{
				g.DrawEllipse(pen, x - cw / 2 - dotSize / 2, y - ch / 2 - dotSize / 2, dotSize, dotSize);
				g.FillEllipse(backBrush, x - cw / 2 - dotSize / 2, y - ch / 2 - dotSize / 2, dotSize, dotSize);
			}
			else
			{
				g.DrawEllipse(pen, x - cw / 2 - dotSize / 2, y - ch / 2 - dotSize / 2, dotSize, dotSize);
				g.FillEllipse(brush, x - cw / 2 - dotSize / 2, y - ch / 2 - dotSize / 2, dotSize, dotSize);
			}
		}
	}

	/// <summary>
	/// Draw neighbor signs.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	[Conditional("ENHANCED_DRAWING_APIS")]
	partial void DrawNeighborSigns(Graphics g)
	{
		if (this is not
			{
				View.NeighborNodes: var neighborNodes,
				Calculator: { CellSize: var (cw, ch) } calc,
				Preferences: { NeighborSignsWidth: var width, NeighborSignCellPadding: var padding }
			})
		{
			return;
		}

		foreach (var neighborNode in neighborNodes)
		{
			if (neighborNode is not (var cell, _) { Identifier: var identifier, IsFourDirections: var isFourDirections })
			{
				continue;
			}

			using var brush = new SolidBrush(GetColor(identifier));
			using var pen = new Pen(brush, width);

			var (x, y) = calc.GetMousePointInCenter(cell);
			var topLeft = new PointF(x - cw / 2 + padding, y - ch / 2 + padding);
			var bottomRight = new PointF(x + cw / 2 - padding, y + ch / 2 - padding);

			if (isFourDirections)
			{
				// Draw cross sign.
				var topRight = new PointF(x + cw / 2 - padding, y - ch / 2 + padding);
				var bottomLeft = new PointF(x - cw / 2 + padding, y + ch / 2 - padding);

				g.DrawLine(pen, topLeft, bottomRight);
				g.DrawLine(pen, topRight, bottomLeft);
			}
			else
			{
				// Draw circle.
				var rect = RectangleMarshal.CreateInstance(topLeft, bottomRight);
				g.DrawEllipse(pen, rect);
			}
		}
	}

	/// <summary>
	/// Draw wheels.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	[Conditional("ENHANCED_DRAWING_APIS")]
	partial void DrawWheel(Graphics g)
	{
		if (this is not
			{
				View.WheelNodes: var wheelNodes,
				Calculator: { CellSize: var (cw, ch) } calc,
				Preferences:
				{
					WheelFont: var (fontName, fontSize, fontStyle),
					WheelWidth: var width,
					WheelTextColor: var textColor,
					BackgroundColor: var backColor
				}
			})
		{
			return;
		}

		using var backBrush = new SolidBrush(backColor);
		using var font = new Font(fontName, fontSize, fontStyle);
		using var textBrush = new SolidBrush(textColor);

		var positions = (stackalloc PointF[4]);
		foreach (var wheelNode in wheelNodes)
		{
			if (wheelNode is not (var cell, _) { Identifier: var identifier, DigitString: var digitString })
			{
				continue;
			}

			using var pen = new Pen(GetColor(identifier), width);

			var (x, y) = calc.GetMousePointInCenter(cell);
			var topLeft = new PointF(x - cw * SqrtOf2 / 2, y - ch * SqrtOf2 / 2);
			var bottomRight = new PointF(x + cw * SqrtOf2 / 2, y + ch * SqrtOf2 / 2);
			var rect = RectangleMarshal.CreateInstance(topLeft, bottomRight);

			// Draw wheel main circle.
			g.DrawEllipse(pen, rect);

			// Draw strings.
			positions[0] = new(x, y - ch * SqrtOf2 / 2/* - fontSize / 4*/); // fontSize / 4: a little offset correction.
			positions[1] = new(x + ch * SqrtOf2 / 2, y);
			positions[2] = new(x, y + ch * SqrtOf2 / 2/* + fontSize / 4*/);
			positions[3] = new(x - ch * SqrtOf2 / 2, y);
			for (var i = 0; i < 4; i++)
			{
				var renderingText = digitString[i].ToString();
				var position = positions[i];
				var (px, py) = position;
				var renderingSize = g.MeasureString(renderingText, font);
				var (tw, th) = renderingSize;

				g.FillRectangle(backBrush, new RectangleF(new(px - tw / 2, py - th / 2), renderingSize));
				g.DrawString(renderingText, font, textBrush, position, DefaultStringFormat);
			}
		}
	}
}
