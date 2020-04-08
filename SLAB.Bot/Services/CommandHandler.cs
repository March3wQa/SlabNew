using Discord.Commands;
using Discord.WebSocket;
using SLAB.Bot.TypeReaders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SLAB.Bot.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;

        // DiscordSocketClient, CommandService, IConfigurationRoot, and IServiceProvider are
        // injected automatically from the IServiceProvider
        public CommandHandler(
            DiscordSocketClient discord,
            CommandService commands,
            IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;

            _discord.MessageReceived += OnMessageReceivedAsync;

            _commands.AddTypeReader(typeof(object), new ObjectTypeReader());
        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            // Ensure the message is from a user/bot
            if (!(s is SocketUserMessage msg)) return;
            if (msg.Author.Id == _discord.CurrentUser.Id) return;     // Ignore self when checking commands

            var context = new SocketCommandContext(_discord, msg);     // Create the command context

            int argPos = 0;     // Check if the message has a valid command prefix
            if (msg.HasStringPrefix("s!", ref argPos) || msg.HasStringPrefix("S!", ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _provider);     // Execute the command

                if (!result.IsSuccess)
                {
                    switch (result.Error)
                    {
                        case CommandError.UnknownCommand:
                            await context.Channel.SendMessageAsync("I don't have a command you wanted.");
                            break;

                        case CommandError.BadArgCount:
                            await context.Channel.SendMessageAsync(result.ErrorReason);
                            break;

                        case CommandError.UnmetPrecondition:
                            await context.Channel.SendMessageAsync(result.ErrorReason);
                            break;

                        default:
                            await context.Channel.SendMessageAsync("I had some problems with that command. Sorry!");
                            break;
                    }
                }
            }
        }
    }
}
