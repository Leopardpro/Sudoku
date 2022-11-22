﻿namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Defines a command.
/// </summary>
internal abstract class Command
{
	/// <summary>
	/// Indicates the command name that will be used as comparison.
	/// </summary>
	public abstract string CommandName { get; }

	/// <summary>
	/// Indicates the environment command that the current command relies on.
	/// </summary>
	public virtual string? EnvironmentCommand { get; } = null;

	/// <summary>
	/// Indicates the prefix.
	/// </summary>
	public virtual string[] Prefixes { get; } = { "!", "\uff01" };

	/// <summary>
	/// Indicates the comparison mode that will be used as check commands.
	/// </summary>
	public abstract CommandComparison ComparisonMode { get; }


	/// <summary>
	/// Execute the command.
	/// </summary>
	/// <param name="args">The arguments.</param>
	/// <param name="e">The event arguments.</param>
	/// <returns>
	/// Returns a task instance that returns a <see cref="bool"/> value indicating whether the operation executed successfully.
	/// </returns>
	public async Task<bool> ExecuteAsync(string args, GroupMessageReceiver e)
	{
		var a = Prefixes.FirstOrDefault(args.StartsWith) switch
		{
			{ } prefix when args.IndexOf(prefix) is var i && i < args.Length => args[(i + 1)..],
			_ => null
		};

		if (a is null)
		{
			return false;
		}

		if (RunningContexts.TryGetValue(e.GroupId, out var context) && context.ExecutingCommand != EnvironmentCommand)
		{
			return false;
		}

#pragma warning disable format
		if (ComparisonMode switch
			{
				CommandComparison.Strict => CommandName != a,
				CommandComparison.Prefix => !a.StartsWith(CommandName),
				_ => throw new ArgumentOutOfRangeException(nameof(ComparisonMode))
			})
#pragma warning restore format
		{
			return false;
		}

		return await ExecuteCoreAsync(
			ComparisonMode switch { CommandComparison.Strict => a, CommandComparison.Prefix => a[CommandName.Length..].Trim() },
			e
		);
	}

	/// <summary>
	/// The internal method to handle with arguments <paramref name="args"/>, using <paramref name="e"/> to send messages.
	/// </summary>
	/// <param name="args">
	/// The arguments that is passed from QQ client. This argument will be pre-handled by the core method
	/// <see cref="ExecuteAsync(string, GroupMessageReceiver)"/>. The target value of this argument will be different
	/// with different settings of property <see cref="ComparisonMode"/>, if the property is:
	/// <list type="table">
	/// <item>
	/// <term><see cref="CommandComparison.Strict"/></term>
	/// <description>The argument <paramref name="args"/> will be strictly equal to command name <see cref="CommandName"/>.</description>
	/// </item>
	/// <item>
	/// <term><see cref="CommandComparison.Prefix"/></term>
	/// <description>
	/// <para>The message received from QQ client will start with the command name <see cref="CommandName"/>.</para>
	/// <para>
	/// For example, if the command name is <c>a</c>, the message can be "<c>!a arg1 arg2 arg3</c>".
	/// Then the core method will cut the command name, keeping arguments <c>arg1</c>, <c>arg2</c> and <c>arg3</c>
	/// in the target arguments, passing into <paramref name="args"/> like "<c>arg1 arg2 arg3</c>", then you can use method
	/// <see cref="string.Split(char[])"/> to get all values like:
	/// <code><![CDATA[
	/// var arguments = args.Split(' ');
	/// ]]></code>
	/// </para>
	/// </description>
	/// </item>
	/// </list>
	/// Other kinds of values are not supported.
	/// </param>
	/// <param name="e">
	/// The group message receiver instance. This instance can send message to the target user (i.e. mention somebody <c>@sb</c>),
	/// or just send a normal message to the target QQ group without mentioning anybody. You can send message using extension method
	/// <see cref="MiraiScaffold.SendMessageAsync(GroupMessageReceiver, MessageChain)"/>:
	/// <code><![CDATA[
	/// await e.SendMessageAsync(targetMessage);
	/// ]]></code>
	/// </param>
	/// <returns></returns>
	/// <seealso cref="ExecuteAsync(string, GroupMessageReceiver)"/>
	/// <seealso cref="ComparisonMode"/>
	/// <seealso cref="CommandName"/>
	/// <seealso cref="string.Split(char[])"/>
	/// <seealso cref="MiraiScaffold.SendMessageAsync(GroupMessageReceiver, MessageChain)"/>
	protected abstract Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e);
}