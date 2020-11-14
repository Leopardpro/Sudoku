﻿using System.Collections.Generic;

namespace Sudoku.Bot
{
	/// <summary>
	/// The resource dictionary.
	/// </summary>
	public static class Resources
	{
		/// <summary>
		/// The inner dictionary.
		/// </summary>
		private static readonly IDictionary<string, string> InnerDictionary = new Dictionary<string, string>
		{
			["AnalysisCommand"] = "-分析",
			["DrawingCommand"] = "-生成图片",
			["GenerateEmptyGridCommand"] = "-生成空盘",
			["CleaningGridCommand"] = "-清盘",
			["IntroducingCommand"] = "小蛋蛋，介绍一下你吧",
			["HelpCommand"] = "-帮助",

			["Help1"] = "帮助：",
			["Help2"] = "下面提示一些文字。",
			["Help3"] = "注：指令前面减号也是命令的一部分；",
			["Help4"] = "另外，中间的空格需要严格遵守，缺少一个都会导致命令不能识别。",
			["HelpHelp"] = "-帮助：显示此帮助信息。",
			["HelpAnalyze"] = "-分析 <盘面>：显示题目的分析结果。",
			["HelpGeneratePicture"] = "-生成图片 <盘面>：将题目文本转为图片显示。",
			["HelpClean"] = "-清盘 <盘面>：将盘面前期的排除、唯一余数、区块和数组技巧全部应用。",
			["HelpEmpty"] = "-生成空盘：给一个空盘的图片。",
			["HelpStartDrawing"] = "-开始绘图 边长 <图片边长>：开始从空盘画盘面图，随后可以添加其它的操作，例如添加候选数涂色等。",
			["HelpFillGiven"] = "-填入 提示数 <数字> 到 <单元格>：填入提示数（初盘固定的数）到指定的单元格里。",
			["HelpFillModifiable"] = "-填入 填入数 <数字> 到 <单元格>：填入数字到指定单元格里。",
			["HelpFillCandidate"] = "-填入 候选数 <单元格> <数字>：填入候选数到指定单元格里。",
			["HelpDrawCell"] = "-画 单元格 <单元格> 颜色 <a> <r> <g> <b>：给单元格添加指定的颜色。",
			["HelpDrawCandidate"] = "-画 候选数 <单元格> <数字> 颜色 <a> <r> <g> <b>：给候选数添加指定的颜色。",
			["HelpDrawRegion"] = "-画 区域 <区域> 颜色 <a> <r> <g> <b>：给指定区域添加指定的颜色。",
			["HelpDrawRow"] = "-画 行 <行> 颜色 <a> <r> <g> <b>：给指定的行添加指定的颜色。",
			["HelpDrawColumn"] = "-画 列 <列> 颜色 <a> <r> <g> <b>：给指定的列添加指定的颜色。",
			["HelpDrawBlock"] = "-画 宫 <宫> 颜色 <a> <r> <g> <b>：给指定的宫添加指定的颜色。",
			["HelpDrawChain"] = "-画 链 从 <单元格> <数字> 到 <单元格> <数字>：画从哪里到哪里的链。",
			["HelpDrawCross"] = "-画 叉叉 <单元格>：在单元格上标注叉叉。",
			["HelpDrawCircle"] = "-画 圆圈 <单元格>：在单元格上标注圆圈。",
			["HelpClose"] = "-结束绘图：指定画图过程结束，清除画板。",
			["HelpIntroduceMyself"] = "小蛋蛋，介绍一下你吧：我 介 绍 我 自 己",

			["Introduce1"] = "Hello 大家好！我是一个机器人，叫小蛋蛋，是女孩子哦 (✿◡‿◡)",
			["Introduce2"] = "我爸还在给我加别的功能，所以我现在还需要大家的鼓励和支持哦，蟹蟹~",

			["Analysis1"] = "答案盘：",
		};


		/// <summary>
		/// Get the resource key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>The value.</returns>
		/// <exception cref="KeyNotFoundException">Throws when the specified key can't be found.</exception>
		public static string GetValue(string key) => InnerDictionary[key];
	}
}
