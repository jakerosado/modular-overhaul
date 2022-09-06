﻿namespace DaLion.Common.Harmony;

#region using directives

using Exceptions;
using Extensions.Collections;
using Extensions.Reflection;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

#endregion using directives

/// <summary>Provides an API for abstracting common transpiler operations.</summary>
public class ILHelper
{
    private readonly Stack<int> _indexStack = new();

    /// <summary>Construct an instance.</summary>
    /// <param name="original">A <see cref="MethodBase"/> representation of the original method.</param>
    /// <param name="instructions">The <see cref="CodeInstruction"/>s to be modified.</param>
    public ILHelper(MethodBase original, IEnumerable<CodeInstruction> instructions)
    {
        Original = original;
        Instructions = instructions.ToList();
        Locals = Instructions.Where(insn => (insn.IsLdloc() || insn.IsStloc()) && insn.operand is not null)
            .Select(insn => (LocalBuilder)insn.operand)
            .ToHashSet()
            .ToDictionary(lb => lb.LocalIndex, lb => lb);

        _indexStack.Push(0);
    }

    /// <summary>Metadata about the original target method.</summary>
    public MethodBase Original { get; }

    /// <summary>The current list of <see cref="CodeInstruction"/>s that will eventually replace the target method.</summary>
    public List<CodeInstruction> Instructions { get; }

    /// <summary>A look-up table for easy indexing of <see cref="LocalBuilder"/> objects by their corresponding local index.</summary>
    public Dictionary<int, LocalBuilder> Locals { get; }

    /// <summary>The index currently at the top of the index stack.</summary>
    public int CurrentIndex
    {
        get
        {
            if (_indexStack.Count <= 0)
                ThrowHelper.ThrowInvalidOperationException("Tried to access the index stack while it was null or empty.");

            return _indexStack.Peek();
        }
    }

    /// <summary>The index of the last <see cref="CodeInstruction" /> in the current instruction list.</summary>
    public int LastIndex
    {
        get
        {
            if (Instructions.Count <= 0)
                ThrowHelper.ThrowInvalidOperationException("Tried to access the instruction list while it was null or empty.");

            return Instructions.Count - 1;
        }
    }

    /// <summary>Find the first occurrence of a pattern in the active code instruction list and move the index pointer to it.</summary>
    /// <param name="pattern">A sequence of <see cref="CodeInstruction"/>s to match.</param>
    public ILHelper FindFirst(params CodeInstruction[] pattern)
    {
        var index = Instructions.IndexOf(pattern);
        if (index < 0) ThrowHelperExtensions.ThrowPatternNotFoundException(pattern, Original, Snitch);

        _indexStack.Push(index);
        return this;
    }

    /// <summary>Find the last occurrence of a pattern in the active code instruction list and move the index pointer to it.</summary>
    /// <param name="pattern">A sequence of <see cref="CodeInstruction"/>s to match.</param>
    public ILHelper FindLast(params CodeInstruction[] pattern)
    {
        var reversedInstructions = Instructions.Clone();
        reversedInstructions.Reverse();

        var index = Instructions.Count - reversedInstructions.IndexOf(pattern.Reverse().ToArray()) - pattern.Length;
        if (index < 0) ThrowHelperExtensions.ThrowPatternNotFoundException(pattern, Original, Snitch);

        _indexStack.Push(index);
        return this;
    }

    /// <summary>Find the next occurrence of a pattern in the active code instruction list and move the index pointer to it.</summary>
    /// <param name="pattern">A sequence of <see cref="CodeInstruction"/>s to match.</param>
    public ILHelper FindNext(params CodeInstruction[] pattern)
    {
        var index = Instructions.IndexOf(pattern, CurrentIndex + 1);
        if (index < 0) ThrowHelperExtensions.ThrowPatternNotFoundException(pattern, Original, Snitch);

        _indexStack.Push(index);
        return this;
    }

    /// <summary>Find the previous occurrence of a pattern in the active code instruction list and move the index pointer to it.</summary>
    /// <param name="pattern">A sequence of <see cref="CodeInstruction"/>s to match.</param>
    public ILHelper FindPrevious(params CodeInstruction[] pattern)
    {
        var reversedInstructions = Instructions.Clone();
        reversedInstructions.Reverse();

        var index = Instructions.Count -
                    reversedInstructions.IndexOf(pattern.Reverse().ToArray(), Instructions.Count - CurrentIndex) -
                    pattern.Length;
        if (index >= Instructions.Count) ThrowHelperExtensions.ThrowPatternNotFoundException(pattern, Original, Snitch);

        _indexStack.Push(index);
        return this;
    }

    /// <summary>Find a specific label in the active code instruction list and move the index pointer to it.</summary>
    /// <param name="label">The <see cref="Label" /> object to match.</param>
    /// <param name="fromCurrentIndex">Whether to begin search from the currently pointed index.</param>
    public ILHelper FindLabel(Label label, bool fromCurrentIndex = false)
    {
        var index = Instructions.IndexOf(label, fromCurrentIndex ? CurrentIndex + 1 : 0);
        if (index < 0) ThrowHelperExtensions.ThrowLabelNotFoundException(label, Original, Snitch);

        _indexStack.Push(index);
        return this;
    }

    /// <summary>Move the index pointer forward an integer number of steps.</summary>
    /// <param name="steps">Number of steps by which to move the index pointer.</param>
    public ILHelper Advance(int steps = 1)
    {
        if (CurrentIndex + steps < 0 || CurrentIndex + steps > LastIndex)
            ThrowHelperExtensions.ThrowIndexOutOfRangeException("New index is out of range.");

        _indexStack.Push(CurrentIndex + steps);
        return this;
    }

    /// <summary>Alias for <see cref="FindNext(CodeInstruction[])" />.</summary>
    /// <param name="pattern">A sequence of <see cref="CodeInstruction"/>s to match.</param>
    public ILHelper AdvanceUntil(params CodeInstruction[] pattern) => FindNext(pattern);

    /// <summary>Move the index pointer backward an integer number of steps.</summary>
    /// <param name="steps">Number of steps by which to move the index pointer.</param>
    public ILHelper Retreat(int steps = 1) => Advance(-steps);

    /// <summary>Alias for <see cref="FindPrevious(CodeInstruction[])" />.</summary>
    /// <param name="pattern">A sequence of <see cref="CodeInstruction"/>s to match.</param>
    public ILHelper RetreatUntil(params CodeInstruction[] pattern) => FindPrevious(pattern);

    /// <summary>Insert a sequence of code instructions at the currently pointed index.</summary>
    /// <param name="instructions">Sequence of <see cref="CodeInstruction" /> objects to insert.</param>
    /// <remarks>The instruction at the current address is pushed forward, such that the index pointer continues to point to the same instruction after insertion.</remarks>
    public ILHelper InsertInstructions(params CodeInstruction[] instructions)
    {
        Instructions.InsertRange(CurrentIndex, instructions);
        _indexStack.Push(CurrentIndex + instructions.Length);
        return this;
    }

    /// <summary>Insert a sequence of code instructions at the currently pointed index and add the specified labels to the first instruction in the sequence.</summary>
    /// <param name="labels">Any labels to add at the start of the insertion.</param>
    /// <param name="instructions">Sequence of <see cref="CodeInstruction" /> objects to insert.</param>
    /// <remarks>The instruction at the current address is pushed forward, such that the index pointer continues to point to the same instruction after insertion.</remarks>
    public ILHelper InsertWithLabels(Label[] labels, params CodeInstruction[] instructions)
    {
        instructions[0].labels.AddRange(labels);
        Instructions.InsertRange(CurrentIndex, instructions);
        _indexStack.Push(CurrentIndex + instructions.Length);
        return this;
    }

    /// <summary>Add a sequence of code instructions to the end of the instructions list.</summary>
    /// <param name="instructions">Sequence of <see cref="CodeInstruction" /> objects to add.</param>
    /// <remarks>The index pointer is moved to the first instruction in the added sequence.</remarks>
    public ILHelper AddInstructions(params CodeInstruction[] instructions)
    {
        Instructions.AddRange(instructions);
        _indexStack.Push(LastIndex - instructions.Length);
        return this;
    }

    /// <summary>Add a sequence of code instructions to the end of the instructions list and add the specified labels to the first instruction in the sequence.</summary>
    /// <param name="instructions">Sequence of <see cref="CodeInstruction" /> objects to add.</param>
    /// <remarks>The index pointer is moved to the first instruction in the added sequence.</remarks>
    public ILHelper AddWithLabels(Label[] labels, params CodeInstruction[] instructions)
    {
        instructions[0].labels.AddRange(labels);
        Instructions.AddRange(instructions);
        _indexStack.Push(LastIndex - instructions.Length);
        return this;
    }

    /// <summary>Get code instructions starting from the currently pointed index. </summary>
    /// <param name="instructions">The got code instructions.</param>
    /// <param name="count">Number of code instructions to get.</param>
    /// <param name="advance">Whether to advance the index pointer.</param>
    public ILHelper GetInstructions(out CodeInstruction[] instructions, int count = 1, bool removeLabels = false, bool advance = false)
    {
        instructions = Instructions.GetRange(CurrentIndex, count).Clone().ToArray();
        if (removeLabels)
            foreach (var insn in instructions)
                insn.labels.Clear();
        if (advance) _indexStack.Push(_indexStack.Peek() + count);
        return this;
    }

    /// <summary>Get code instructions starting from the currently pointed index up to and including the first instruction in the specified pattern.</summary>
    /// <param name="instructions">The got code instructions.</param>
    /// <param name="advance">Whether to advance the index pointer.</param>
    /// <param name="pattern">A sequence of <see cref="CodeInstruction"/>s to match.</param>
    public ILHelper GetInstructionsUntil(out CodeInstruction[] instructions, bool removeLabels = false, bool advance = false, params CodeInstruction[] pattern)
    {
        AdvanceUntil(pattern);

        var endIndex = _indexStack.Pop() + 1;
        var count = endIndex - CurrentIndex;
        instructions = Instructions.GetRange(CurrentIndex, count).Clone().ToArray();
        if (removeLabels)
            foreach (var insn in instructions)
                insn.labels.Clear();
        if (advance) _indexStack.Push(_indexStack.Peek() + count);
        return this;
    }

    /// <summary>Remove code instructions starting from the currently pointed index.</summary>
    /// <param name="count">Number of code instructions to remove.</param>
    public ILHelper RemoveInstructions(int count = 1)
    {
        if (CurrentIndex + count > LastIndex) ThrowHelperExtensions.ThrowIndexOutOfRangeException("Can't remove item out of range.");

        Instructions.RemoveRange(CurrentIndex, count);
        return this;
    }

    /// <summary>Remove code instructions starting from the currently pointed index up to and including the first instruction in the specified pattern.</summary>
    /// <param name="pattern">A sequence of <see cref="CodeInstruction"/>s to match.</param>
    public ILHelper RemoveInstructionsUntil(params CodeInstruction[] pattern)
    {
        AdvanceUntil(pattern);
        var endIndex = _indexStack.Pop() + 1;
        var count = endIndex - CurrentIndex;
        Instructions.RemoveRange(CurrentIndex, count);
        return this;
    }

    /// <summary>Replace the code instruction at the currently pointed index.</summary>
    /// <param name="instruction">The <see cref="CodeInstruction" /> object to replace with.</param>
    public ILHelper ReplaceInstructionWith(CodeInstruction instruction, bool preserveLabels = false)
    {
        if (preserveLabels)
            instruction.labels = Instructions[CurrentIndex].labels;

        Instructions[CurrentIndex] = instruction;
        return this;
    }

    /// <summary>Add one or more labels to the code instruction at the currently pointed index.</summary>
    /// <param name="labels">A sequence of <see cref="Label" /> objects to add.</param>
    public ILHelper AddLabels(params Label[] labels)
    {
        Instructions[CurrentIndex].labels.AddRange(labels);
        return this;
    }

    /// <summary>Remove labels from the code instruction at the currently pointed index.</summary>
    public ILHelper RemoveLabels()
    {
        Instructions[CurrentIndex].labels.Clear();
        return this;
    }

    /// <summary>Remove labels from the code instruction at the currently pointed index.</summary>
    public ILHelper RemoveLabels(params Label[] labels)
    {
        labels.ForEach(l => Instructions[CurrentIndex].labels.Remove(l));
        return this;
    }

    /// <summary>Replace the labels of the code instruction at the currently pointed index.</summary>
    public ILHelper SetLabels(params Label[] labels)
    {
        Instructions[CurrentIndex].labels = labels.ToList();
        return this;
    }

    /// <summary>Get the labels from the code instruction at the currently pointed index.</summary>
    /// <param name="labels">The returned list of <see cref="Label" /> objects.</param>
    public ILHelper GetLabels(out Label[] labels)
    {
        labels = Instructions[CurrentIndex].labels.ToArray();
        return this;
    }

    /// <summary>Remove labels from the code instruction at the currently pointed index.</summary>
    public ILHelper StripLabels(out Label[] labels)
    {
        GetLabels(out labels);
        return RemoveLabels();

    }

    /// <summary>Return the opcode of the code instruction at the currently pointed index.</summary>
    /// <param name="opcode">The returned <see cref="OpCode" /> object.</param>
    public ILHelper GetOpCode(out OpCode opcode)
    {
        opcode = Instructions[CurrentIndex].opcode;
        return this;
    }

    /// <summary>Change the opcode of the code instruction at the currently pointed index.</summary>
    /// <param name="opcode">The new <see cref="OpCode" /> object.</param>
    public ILHelper SetOpCode(OpCode opcode)
    {
        Instructions[CurrentIndex].opcode = opcode;
        return this;
    }

    /// <summary>Return the operand of the code instruction at the currently pointed index.</summary>
    /// <param name="operand">The returned operand <see cref="object" />.</param>
    public ILHelper GetOperand(out object operand)
    {
        operand = Instructions[CurrentIndex].operand;
        return this;
    }

    /// <summary>Change the operand of the code instruction at the currently pointed index.</summary>
    /// <param name="operand">The new <see cref="object" /> operand.</param>
    public ILHelper SetOperand(object operand)
    {
        Instructions[CurrentIndex].operand = operand;
        return this;
    }

    /// <summary>Return the index pointer to a previous state.</summary>
    /// <param name="count">Number of index changes to discard.</param>
    public ILHelper Return(int count = 1)
    {
        for (var i = 0; i < count; ++i) _indexStack.Pop();
        return this;
    }

    /// <summary>Move the index pointer to a specific index.</summary>
    /// <param name="index">The index to move to.</param>
    public ILHelper GoTo(int index)
    {
        if (index < 0) ThrowHelperExtensions.ThrowIndexOutOfRangeException("Can't go to a negative index.");

        if (index > LastIndex) ThrowHelperExtensions.ThrowIndexOutOfRangeException("New index is out of range.");

        _indexStack.Push(index);
        return this;

    }

    /// <summary>Applied the specified action to all occurrences of the specified pattern within the instrutions list.</summary>
    /// <param name="pattern">A sequence of <see cref="CodeInstruction"/>s to match.</param>
    /// <param name="action">The action to be applied.</param>
    public ILHelper ForEach(CodeInstruction[] pattern, Action action)
    {
        while (TryMoveNext(pattern)) action.Invoke();
        return this;
    }

    /// <summary>Reset the current instance.</summary>
    public ILHelper Clear()
    {
        _indexStack.Clear();
        Instructions.Clear();
        return this;
    }

    /// <summary>Reset the instance and return the active code instruction list as enumerable.</summary>
    public IEnumerable<CodeInstruction> Flush()
    {
        var result = Instructions.Clone();
        Clear();
        return result.AsEnumerable();
    }

    /// <summary>Snitch on other transpilers applied to the target method.</summary>
    /// <returns>A formatted string listing all the transpilers applied to the target method.</returns>
    /// <remarks>Inspired by <see href="https://github.com/atravita-mods/StardewMods/blob/f450bd2fe72a884e89ca6a06c187605bdb79fa3d/AtraShared/Utils/Extensions/HarmonyExtensions.cs#L46">Atravita</see>.</remarks>
    private string Snitch()
    {
        var sb = new StringBuilder();
        sb.Append("Applied transpilers:");
        var count = 0;
        foreach (var transpiler in Original.GetAppliedTranspilers())
        {
            sb.AppendLine().Append($"\t{transpiler.PatchMethod.GetFullName()}");
            ++count;
        }

        return count > 0 ? sb.ToString() : string.Empty;
    }

    /// <summary>Attempt to move the stack pointer to the next occurrence of the specified pattern.</summary>
    /// <param name="pattern">A sequence of <see cref="CodeInstruction"/>s to match.</param>
    /// <returns></returns>
    private bool TryMoveNext(params CodeInstruction[] pattern)
    {
        var index = Instructions.IndexOf(pattern, CurrentIndex + 1);
        if (index < 0) return false;

        _indexStack.Push(index);
        return true;
    }
}