﻿namespace DaLion.Stardew.Professions.Framework.Utility;

#region using directives

using System.Threading.Tasks;

#endregion using directives

public static class Multiplayer
{
    public static TaskCompletionSource<string> ResponseReceived;

    public static async Task<string> SendRequestAsync(string message, string messageType, long playerId)
    {
        ModEntry.ModHelper.Multiplayer.SendMessage(message, messageType, new[] {ModEntry.Manifest.UniqueID},
            new[] {playerId});

        ResponseReceived = new();
        return await ResponseReceived.Task;
    }
}